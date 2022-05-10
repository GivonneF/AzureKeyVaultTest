using System.Security.Cryptography.X509Certificates;
using WebApiKeyVaultTest;
using Azure.Identity;

var builder = WebApplication.CreateBuilder(args);

var certStore = new X509Store(StoreLocation.CurrentUser);

certStore.Open(OpenFlags.ReadOnly);

var settings = builder.Configuration.GetRequiredSection("AzureInfo").Get<CertInfo>();
var cert = certStore.Certificates
    .Find(X509FindType.FindByThumbprint, settings.CertThumbPrint, false)
    .OfType<X509Certificate2>()
    .SingleOrDefault();

if (cert == null)
{
    throw new Exception("Cert Not found");
}

builder.Configuration.AddAzureKeyVault(
    new Uri(settings.KeyVaultUrl),
    new ClientCertificateCredential(
        settings.ActiveDirectoryId,
        settings.AppId,
        cert));

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();