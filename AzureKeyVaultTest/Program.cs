// See https://aka.ms/new-console-template for more information

using Azure.Core;
using Azure.Identity;
using Azure.Security.KeyVault.Keys;
using Azure.Security.KeyVault.Secrets;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Azure.Security.KeyVault.Keys.Cryptography;
using System.Text;

namespace AzureKeyVaultTest;

public static class AzureKeyVaultTest
{
    public static async Task Main(string[] args)
    {
        var settings = Setup();
        var token = GetToken(settings);
        var key = GetKeyVaultKey(settings, token);

        EncryptionTests(token, key);
    }

    #region VaultKey Methods

    private static KeyVaultKey GetKeyVaultKey(
        CertInfo settings,
        TokenCredential token)
    {
        try
        {
            //---------------------------------------------
            var client = new KeyClient(
                new Uri(settings.KeyVaultUrl),
                token);

            var key = client.GetKey("givonne-key-01");

            //---------------------------------------------
            //var client = new SecretClient(
            //    new Uri(settings.KeyVaultUrl),
            //    token);

            //var secret = client.GetSecret("givonne-02");

            return key.Value;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    private static TokenCredential GetToken(CertInfo settings)
    {
        //-------------------------------------------------
        // fails
        //var tokenCredential = new ManagedIdentityCredential(
        //    "8b0538b4-ed32-4e8e-a67e-a1d9e4d50723");

        //-------------------------------------------------
        // fails
        //var token = new DefaultAzureCredential(
        //    new DefaultAzureCredentialOptions());

        //-------------------------------------------------
        // fails
        //var token = new ClientCertificateCredential(
        //    settings.ActiveDirectoryId,
        //    settings.AppId,
        //    @"c:\temp\local-key-vault.pfx");

        //-------------------------------------------------
        // works
        //var cert = GetCert(settings);

        //var token = new ClientCertificateCredential(
        //    settings.ActiveDirectoryId,
        //    settings.AppId,
        //    cert);

        //-------------------------------------------------
        // works
        //var certificate = new X509Certificate2(
        //    @"c:\temp\local-key-vault.pfx",
        //    "LeastMango2");

        //var token = new ClientCertificateCredential(
        //    settings.ActiveDirectoryId,
        //    settings.AppId,
        //    certificate);

        //-------------------------------------------------
        // works but requires you to sign in to azure using visual studio
        //var token = new VisualStudioCredential();

        //-------------------------------------------------
        // works
        var token = new ClientSecretCredential(
           settings.ActiveDirectoryId,
           settings.AppId,
           "Os58Q~XHZZ5yVnzcOu5bqs6wnTQRJoRbm9rErckt");


        return token;
    }

    private static X509Certificate2 GetCert(CertInfo settings)
    {
        var certStore = new X509Store(StoreLocation.LocalMachine);

        certStore.Open(OpenFlags.ReadOnly);

        var cert = certStore.Certificates
            .Find(X509FindType.FindByThumbprint, settings.CertThumbPrint, false)
            .OfType<X509Certificate2>()
            .FirstOrDefault();

        if (cert == null)
        {
            throw new Exception("Cert Not found");
        }

        return cert;
    }

    #endregion

    #region Encryption Async Methods

    //private static async Task EncryptionTestsAsync(
    //   TokenCredential token,
    //   KeyVaultKey key)
    //{
    //    var cryptoClient = new CryptographyClient(key.Id, token);

    //    var file = new FileInfo(@"Files/pexels-photo-1054666.jpeg");
    //    using var sr = new StreamReader(file.FullName);

    //    var resultStream = await EncryptFileStreamAsync(cryptoClient, sr.BaseStream);
    //    using var sw = new StreamWriter(file.FullName + ".encrypted");
    //    sw.Write(resultStream);
    //}

    //private static Task<Stream> EncryptFileStreamAsync(
    //    CryptographyClient cryptoClient,
    //    Stream fileStream)
    //{
    //    var encryptResult = await cryptoClient.EncryptAsync(
    //        EncryptionAlgorithm.Rsa15,
    //        fileStream);

    //    return Convert.ToBase64String(encryptResult.Ciphertext);
    //}

    #endregion

    #region Encryption Methods

    private static void EncryptionTests(
       TokenCredential token,
       KeyVaultKey key)
    {
        var cryptoClient = new CryptographyClient(key.Id, token);

        //var value = "test";
        //var encryptValue = EncryptString(cryptoClient, value);
        //var decryptedValue = DecryptString(cryptoClient, encryptValue);
    }

    private static string EncryptString(
        CryptographyClient cryptoClient,
        string input)
    {
        var inputAsByteArray = Encoding.UTF8.GetBytes(input);

        var encryptResult = cryptoClient.Encrypt(
            EncryptionAlgorithm.Rsa15,
            inputAsByteArray);

        return Convert.ToBase64String(encryptResult.Ciphertext);
    }

    private static string DecryptString(
        CryptographyClient cryptoClient,
        string input)
    {
        var inputAsByteArray = Convert.FromBase64String(input);

        var decryptResult = cryptoClient.Decrypt(
            EncryptionAlgorithm.Rsa15,
            inputAsByteArray);

        return Encoding.UTF8.GetString(decryptResult.Plaintext);
    }

    #endregion

    #region Helper Methods

    private static CertInfo Setup()
    {
        var builder = new ConfigurationBuilder();

        IConfiguration config = builder
            .AddJsonFile("appsettings.json")
            .AddEnvironmentVariables()
            .Build();

        var settings = config
            .GetRequiredSection("AzureInfo")
            .Get<CertInfo>();

        return settings;
    }

    #endregion
}
