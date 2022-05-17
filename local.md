localhost key: FreeMe4

## Mac
create key: `openssl genrsa -des3 -out localhost.key 2048`
create csr: `openssl req -x509 -sha256 -days 365 -newkey rsa:2048 -keyout localhost.key -out localhost.crs`
create crt: `openssl x509 -signkey localhost.key -in localhost.csr -req -days 365 -out localhost.crt`
create pfx: `openssl pkcs12 -export -out localhost.pfx -inkey localhost.key -in localhost.crt`


## Windows

- Step 1

New-SelfSignedCertificate -Subject "localhost" -TextExtension @(DNS=localhost&IPAddress=127.0.0.1&IPAddress=::1") -CertStoreLocation "Cert:\LocalMachine\My\" -KeyExportPolicy Exportable -FriendlyName "localhost-SSC"

- Step 2

$mypwd = ConvertTo-SecureString -String "1234" -Force -AsPlainText


- Step 3

Export-PfxCertificate -Cert Cert:\localmachine\my\26DA60EE9E9376B3B9B31C487A1DF7F593762409 -filepath localhost.pfx -Password $mypwd


- Reference for the two commands above:

https://docs.microsoft.com/en-us/powershell/module/pki/new-selfsignedcertificate?view=windowsserver2022-ps
https://docs.microsoft.com/en-us/powershell/module/pki/export-pfxcertificate?view=windowsserver2022-ps


https://blog.jongallant.com/2021/08/azure-identity-101/
https://docs.microsoft.com/en-us/dotnet/api/azure.identity.defaultazurecredential?view=azure-dotnet


## Windows 2

New-SelfSignedCertificate -KeyExportPolicy Exportable -CertStoreLocation Cert:\LocalMachine\My -Subject "localhost" -DnsName "localhost" -FriendlyName "local-key-vault" -NotAfter (Get-Date).AddYears(10);

DBB8C0F93F9C5D0CE05F4FB452E44CEAB315CF92

$mypwd = ConvertTo-SecureString -String "LeastMango2" -Force -AsPlainText
Export-PfxCertificate -Cert Cert:\localmachine\my\DBB8C0F93F9C5D0CE05F4FB452E44CEAB315CF92 -filepath local-key-vault.pfx -Password $mypwd
