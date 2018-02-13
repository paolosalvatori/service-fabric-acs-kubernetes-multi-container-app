# Cluster Name
$clusterName = "Whatever"

# Cluster Location
$clusterRegion = "WestEurope"

# Certificate Password
$certificatePassword = 'trustno1'

# Secure Password
$securePassword = ConvertTo-SecureString -String $certificatePassword `
                                         -AsPlainText `
                                         -Force

# The certificate's subject name must match the domain used to access the Service Fabric cluster
$certificateDNSName = $clusterName.ToLower() + "." + $clusterRegion.ToLower() + ".cloudapp.azure.com"

# Path of Service Fabric cluster primary certificate file
$primaryCertificateFileFullPath = $PSScriptRoot + '\' + "KeyVaultCertificate.pfx"

# Service Fabric cluster primary certificate file
$primaryCertificate = New-SelfSignedCertificate -CertStoreLocation Cert:\CurrentUser\My `
                                                -DnsName $certificateDNSName

# Export Service Fabric cluster secondary certificate file
Export-PfxCertificate -FilePath $primaryCertificateFileFullPath `
                      -Password $securePassword `
                      -Cert $primaryCertificate