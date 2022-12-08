using Azure.Identity;
using BlackJack.Core.Configuration;
using BlackJack.Core.Exceptions;
using BlackJack.Core.ExtensionMethods;
using BlackJack.Core.HealthChecks;
using BlackJack.Events.Configuration;
using BlackJack.Sessions.Core;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

const string defaultCorsPolicyName = "default_cors";

var builder = WebApplication.CreateBuilder(args);


var azureCredential = new ChainedTokenCredential(
    new ManagedIdentityCredential(),
    new EnvironmentCredential(),
    new AzureCliCredential());
try
{
    builder.Configuration.AddAzureAppConfiguration(options =>
    {
        options.Connect(new Uri(builder.Configuration.GetRequiredValue("Azure:AppConfiguration")), azureCredential)
            .ConfigureKeyVault(kv => kv.SetCredential(azureCredential))
            .UseFeatureFlags();
    });
}
catch (Exception ex)
{
    throw new Exception("Configuration failed", ex);
}


builder.Services.AddBlackJackCore(builder.Configuration)
    .AddBlackJackEvents()
    .AddBlackJackSessions();

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: defaultCorsPolicyName,
        bldr =>
        {
            bldr.WithOrigins("http://localhost:4200",
                    "https://pollstar-dev.hexmaster.nl",
                    "https://pollstar-test.hexmaster.nl",
                    "https://pollstar.hexmaster.nl")
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
        });
});

builder.Services.AddApplicationInsightsTelemetry();
builder.Services.AddControllers(options => { options.Filters.Add(typeof(BlackJackExceptionsFilter)); });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(defaultCorsPolicyName);

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();
app.UseEndpoints(ep =>
{
    ep.MapHealthChecks("/health", new HealthCheckOptions
    {
        ResponseWriter = HealthCheckExtensions.WriteResponse
    });
    ep.MapControllers();
});

app.Run();