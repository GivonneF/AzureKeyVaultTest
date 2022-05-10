// See https://aka.ms/new-console-template for more information

using Azure.Core;
using Azure.Identity;
using Azure.Security.KeyVault.Keys;
using Azure.Security.KeyVault.Secrets;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace AzureKeyVaultTest;

public static class AzureKeyVaultTest
{
    public static void Main(string[] args)
    {
        var builder = new ConfigurationBuilder();

        IConfiguration config = builder
            .AddJsonFile("appsettings.json")
            .AddEnvironmentVariables()
            .Build();

        var certStore = new X509Store(StoreLocation.CurrentUser);

        certStore.Open(OpenFlags.ReadOnly);

        var settings = config.GetRequiredSection("AzureInfo").Get<CertInfo>();
        var cert = certStore.Certificates
            .Find(X509FindType.FindByThumbprint, settings.CertThumbPrint, false)
            .OfType<X509Certificate2>()
            .FirstOrDefault();

        if (cert == null)
        {
            throw new Exception("Cert Not found");
        }

        builder.AddAzureKeyVault(
            new Uri(settings.KeyVaultUrl),
            new ClientCertificateCredential(
                settings.ActiveDirectoryId,
                settings.AppId,
                cert));


        config["SecretName"];
    }

    public static void Main1()
    {
        try
        {
            //keys/givonne-01
            var clientId = "c74755c5-5be2-4b2a-b3cb-357883c4d3fc";
            var tokenCredential = new ManagedIdentityCredential(clientId, new TokenCredentialOptions()
            {
                Retry = { MaxRetries = 1 }
            });
            //var tokenCredential = new DefaultAzureCredential(
            //    new DefaultAzureCredentialOptions { ManagedIdentityClientId = clientId });
            var options = new KeyClientOptions()
            {
                Retry =
                {
                    Delay = TimeSpan.FromSeconds(2),
                    MaxDelay = TimeSpan.FromSeconds(16),
                    MaxRetries = 0,
                    Mode = RetryMode.Exponential
                }
            };

            //var client = new KeyClient(
            //    new Uri("https://caption-kv-dicom-dev.vault.azure.net/"),
            //    tokenCredential);

            //var key = client.GetKey("givonne-01");

            //var a = key;

            var client = new SecretClient(
                new Uri("https://caption-kv-dicom-dev.vault.azure.net"),
                tokenCredential);

            var secret = client.GetSecret("givonne-02");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}

//caption-kv-dicom-dev