REM Create PEM file from PFX file
openssl pkcs12 -in KeyVaultCertificate.pfx -out KeyVaultCertificatePEM.pem -nodes -nokeys

REM Create KEY file from PFX file
openssl pkcs12 -in KeyVaultCertificate.pfx -out KeyVaultCertificatePEM.key -nodes -nocerts

REM Create CRT file from PFX file
openssl pkcs12 -in KeyVaultCertificate.pfx -out KeyVaultCertificate.crt -nokeys -clcerts

REM Create CER file from CRT file
openssl x509 -inform pem -in KeyVaultCertificate.crt -outform der -out KeyVaultCertificate.cer