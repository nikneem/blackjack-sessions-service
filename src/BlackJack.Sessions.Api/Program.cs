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

var allowedCorsOrigins = builder.Configuration.GetRequiredValue("AllowedCorsOrigins");
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: defaultCorsPolicyName,
        bldr =>
        {
            bldr.WithOrigins(allowedCorsOrigins.Split(';'))
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
app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = HealthCheckExtensions.WriteResponse
});
app.MapControllers();

app.Run();