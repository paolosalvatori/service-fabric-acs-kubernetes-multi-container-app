#region Copyright
//=======================================================================================
// Microsoft 
//
// This sample is supplemental to the technical guidance published on my personal
// blog at https://github.com/paolosalvatori. 
// 
// Author: Paolo Salvatori
//=======================================================================================
// Copyright (c) Microsoft Corporation. All rights reserved.
// 
// LICENSED UNDER THE APACHE LICENSE, VERSION 2.0 (THE "LICENSE"); YOU MAY NOT USE THESE 
// FILES EXCEPT IN COMPLIANCE WITH THE LICENSE. YOU MAY OBTAIN A COPY OF THE LICENSE AT 
// http://www.apache.org/licenses/LICENSE-2.0
// UNLESS REQUIRED BY APPLICABLE LAW OR AGREED TO IN WRITING, SOFTWARE DISTRIBUTED UNDER THE 
// LICENSE IS DISTRIBUTED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY 
// KIND, EITHER EXPRESS OR IMPLIED. SEE THE LICENSE FOR THE SPECIFIC LANGUAGE GOVERNING 
// PERMISSIONS AND LIMITATIONS UNDER THE LICENSE.
//=======================================================================================
#endregion

#region Using Directives
using System;
using System.IO;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
#endregion

namespace TodoWeb.Helpers
{
    /// <summary>
    /// CertificateFileType
    /// </summary>
    public enum CertificateFileType
    {
        /// <summary>
        /// Certificate
        /// </summary>
        Certificate,
        /// <summary>
        /// PrivateKey
        /// </summary>
        Pkcs8PrivateKey,
        /// <summary>
        /// RSA Private Key
        /// </summary>
        RsaPrivateKey
    }

    /// <summary>
    /// This class can be used to read PEM certificates
    /// </summary>
    public class CertificateHelper
    {
        /// <summary>
        /// Get bytes from PEM file
        /// </summary>
        /// <param name="pemString"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static byte[] GetBytesFromPem(string pemString, CertificateFileType type)
        {
            string header;
            string footer;

            switch (type)
            {
                case CertificateFileType.Certificate:
                    header = "-----BEGIN CERTIFICATE-----";
                    footer = "-----END CERTIFICATE-----";
                    break;
                case CertificateFileType.Pkcs8PrivateKey:
                    header = "-----BEGIN PRIVATE KEY-----";
                    footer = "-----END PRIVATE KEY-----";
                    break;
                case CertificateFileType.RsaPrivateKey:
                    header = "-----BEGIN RSA PRIVATE KEY-----";
                    footer = "-----END RSA PRIVATE KEY-----";
                    break;
                default:
                    return null;
            }

            var start = pemString.IndexOf(header, StringComparison.Ordinal) + header.Length;
            var end = pemString.IndexOf(footer, start, StringComparison.Ordinal) - start;
            var base64 = pemString.Substring(start, end).Trim().Replace("\r\n", "");
            return Convert.FromBase64String(base64);
        }

        /// <summary>
        /// Get PEM certificate
        /// </summary>
        /// <param name="certificateFilePath"></param>
        /// <param name="keyFilePath"></param>
        /// <returns></returns>
        public static async Task<X509Certificate2> GetCertificateAsync(string certificateFilePath, string keyFilePath)
        {
            // Validate parameters
            if (string.IsNullOrEmpty(certificateFilePath))
            {
                throw new ArgumentException($"{nameof(certificateFilePath)} parameter cannot bu null or empty.", nameof(certificateFilePath));
            }

            if (string.IsNullOrEmpty(keyFilePath))
            {
                throw new ArgumentException($"{nameof(keyFilePath)} parameter cannot bu null or empty.", nameof(keyFilePath));
            }

            if (!File.Exists(certificateFilePath))
            {
                throw new FileNotFoundException($"{certificateFilePath} file not found.", certificateFilePath);
            }

            if (!File.Exists(keyFilePath))
            {
                throw new FileNotFoundException($"{keyFilePath} file not found.", keyFilePath);
            }

            if (Environment.OSVersion.Platform.ToString().ToLower().Contains("win"))
            {
                SetReadPermission(certificateFilePath);
                SetReadPermission(keyFilePath);
                var password = File.ReadAllLines(keyFilePath, Encoding.Default)[0];
                password = password.Replace("\0", string.Empty);
                var certificate = new X509Certificate2(certificateFilePath, password);
                return certificate;
            }
            else
            {
                var pemCertificate = await File.ReadAllTextAsync(certificateFilePath);
                var pemKey = await File.ReadAllTextAsync(keyFilePath);

                var certBuffer = GetBytesFromPem(pemCertificate, CertificateFileType.Certificate);
                var keyBuffer = GetBytesFromPem(pemKey, CertificateFileType.Pkcs8PrivateKey);

                var certificate = new X509Certificate2(certBuffer);
                var privateKey = DecodePrivateKeyInfo(keyBuffer);
                certificate = certificate.Copy​With​Private​Key(privateKey);
                return certificate;
            }
        }

        /// <summary>
        /// DecodePrivateKeyInfo
        /// </summary>
        /// <param name="pkcs8">pkcs8 key</param>
        /// <returns>RSAOpenSsl</returns>
        public static RSAOpenSsl DecodePrivateKeyInfo(byte[] pkcs8)
        {
            // encoded OID sequence for  PKCS #1 rsaEncryption szOID_RSA_RSA = "1.2.840.113549.1.1.1"
            // this byte[] includes the sequence byte and terminal encoded null 
            byte[] seqOid = { 0x30, 0x0D, 0x06, 0x09, 0x2A, 0x86, 0x48, 0x86, 0xF7, 0x0D, 0x01, 0x01, 0x01, 0x05, 0x00 };

            // ---------  Set up stream to read the asn.1 encoded SubjectPublicKeyInfo blob  ------
            var memoryStream = new MemoryStream(pkcs8);
            var streamLength = (int)memoryStream.Length;
            var binaryReader = new BinaryReader(memoryStream);    //wrap Memory Stream with BinaryReader for easy reading

            try
            {

                var twoBytes = binaryReader.ReadUInt16();
                if (twoBytes == 0x8130) //data read as little endian order (actual data order for Sequence is 30 81)
                {
                    binaryReader.ReadByte();    //advance 1 byte
                }
                else if (twoBytes == 0x8230)
                {
                    binaryReader.ReadInt16();   //advance 2 bytes
                }
                else
                {
                    return null;
                }
                var bt = binaryReader.ReadByte();
                if (bt != 0x02)
                {
                    return null;
                }

                twoBytes = binaryReader.ReadUInt16();

                if (twoBytes != 0x0001)
                {
                    return null;
                }
                var seq = binaryReader.ReadBytes(15);
                if (!CompareByteArrays(seq, seqOid))    //make sure Sequence for OID is correct
                {
                    return null;
                }
                bt = binaryReader.ReadByte();
                if (bt != 0x04) //expect an Octet string 
                {
                    return null;
                }
                bt = binaryReader.ReadByte();       //read next byte, or next 2 bytes is  0x81 or 0x82; otherwise bt is the byte count
                if (bt == 0x81)
                {
                    binaryReader.ReadByte();
                }
                else
                {
                    if (bt == 0x82)
                    {
                        binaryReader.ReadUInt16();
                    }
                }
                //------ at this stage, the remaining sequence should be the RSA private key
                byte[] rsaPrivateKey = binaryReader.ReadBytes((int)(streamLength - memoryStream.Position));
                RSAOpenSsl rsaOpenSsl = DecodeRsaPrivateKey(rsaPrivateKey);
                return rsaOpenSsl;
            }

            catch (Exception)
            {
                return null;
            }

            finally
            {
                binaryReader.Close();
            }
        }

        /// <summary>
        /// Parses binary ans.1 RSA private key; returns RSAOpenSsl
        /// </summary>
        /// <param name="privateKey"></param>
        /// <returns></returns>
        public static RSAOpenSsl DecodeRsaPrivateKey(byte[] privateKey)
        {
            // ---------  Set up stream to decode the asn.1 encoded RSA private key  ------
            var memoryStream = new MemoryStream(privateKey);
            var binaryReader = new BinaryReader(memoryStream);    //wrap Memory Stream with BinaryReader for easy reading
            try
            {
                var twoBytes = binaryReader.ReadUInt16();
                if (twoBytes == 0x8130) //data read as little endian order (actual data order for Sequence is 30 81)
                {
                    binaryReader.ReadByte();    //advance 1 byte
                }
                else if (twoBytes == 0x8230)
                {
                    binaryReader.ReadInt16();   //advance 2 bytes
                }
                else
                {
                    return null;
                }

                twoBytes = binaryReader.ReadUInt16();
                if (twoBytes != 0x0102) //version number
                {
                    return null;
                }

                var bt = binaryReader.ReadByte();
                if (bt != 0x00)
                {
                    return null;
                }

                //------  all private key components are Integer sequences ----
                var elems = GetIntegerSize(binaryReader);
                var modulus = binaryReader.ReadBytes(elems);

                elems = GetIntegerSize(binaryReader);
                var e = binaryReader.ReadBytes(elems);

                elems = GetIntegerSize(binaryReader);
                var d = binaryReader.ReadBytes(elems);

                elems = GetIntegerSize(binaryReader);
                var p = binaryReader.ReadBytes(elems);

                elems = GetIntegerSize(binaryReader);
                var q = binaryReader.ReadBytes(elems);

                elems = GetIntegerSize(binaryReader);
                var dp = binaryReader.ReadBytes(elems);

                elems = GetIntegerSize(binaryReader);
                var dq = binaryReader.ReadBytes(elems);

                elems = GetIntegerSize(binaryReader);
                var iq = binaryReader.ReadBytes(elems);

                var rsaParameters = new RSAParameters
                {
                    Modulus = modulus,
                    Exponent = e,
                    D = d,
                    P = p,
                    Q = q,
                    DP = dp,
                    DQ = dq,
                    InverseQ = iq
                };

                var rsaOpenSsl = new RSAOpenSsl();
                rsaOpenSsl.ImportParameters(rsaParameters);
                return rsaOpenSsl;
            }
            catch (Exception)
            {
                return null;
            }
            finally
            {
                binaryReader.Close();
            }
        }

        /// <summary>
        /// GetIntegerSize
        /// </summary>
        /// <param name="binaryReader"></param>
        /// <returns></returns>
        private static int GetIntegerSize(BinaryReader binaryReader)
        {
            int count;
            var bt = binaryReader.ReadByte();
            if (bt != 0x02)     //expect integer
                return 0;
            bt = binaryReader.ReadByte();

            if (bt == 0x81)
                count = binaryReader.ReadByte();    // data size in next byte
            else
            if (bt == 0x82)
            {
                var highbyte = binaryReader.ReadByte();
                var lowbyte = binaryReader.ReadByte();
                byte[] modint = { lowbyte, highbyte, 0x00, 0x00 };
                count = BitConverter.ToInt32(modint, 0);
            }
            else
            {
                count = bt;     // we already have the data size
            }

            while (binaryReader.ReadByte() == 0x00)
            {   //remove high order zeros in data
                count -= 1;
            }
            binaryReader.BaseStream.Seek(-1, SeekOrigin.Current);       //last ReadByte wasn't a removed zero, so back up a byte
            return count;
        }

        /// <summary>
        /// CompareByteArrays
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        private static bool CompareByteArrays(byte[] a, byte[] b)
        {
            if (a.Length != b.Length)
            {
                return false;
            }
            var i = 0;
            foreach (var c in a)
            {
                if (c != b[i])
                {
                    return false;
                }
                i++;
            }
            return true;
        }

        private static void SetReadPermission(string filePath)
        {
            // Create a new FileInfo object.
            var fileInfo = new FileInfo(filePath);

            // Get a FileSecurity object that represents the current security settings
            var fileSecurity = new FileSecurity(filePath, AccessControlSections.Access);
            fileSecurity.AddAccessRule(new FileSystemAccessRule("Administrators",
                                                                FileSystemRights.Read,
                                                                AccessControlType.Allow));

            // Set the new access
            fileInfo.SetAccessControl(fileSecurity);
        }
    }
}