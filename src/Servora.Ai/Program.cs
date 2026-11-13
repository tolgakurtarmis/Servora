using Microsoft.Extensions.AI;
using Microsoft.Extensions.Options;
using OllamaSharp;
using Servora.Ai.Options;
using Servora.Ai.Tools;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<OllamaOptions>(builder.Configuration.GetSection(OllamaOptions.SectionName));
builder.Services.Configure<ServoraApiOptions>(builder.Configuration.GetSection(ServoraApiOptions.SectionName));

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<AiCallContext>();
builder.Services.AddHttpClient<SalesSummaryTool>((sp, client) =>
{
    var options = sp.GetRequiredService<IOptions<ServoraApiOptions>>().Value;
    client.BaseAddress = new Uri(options.BaseUrl.TrimEnd('/') + "/");
});

builder.Services.AddSingleton<IChatClient>(sp =>
{
    var options = sp.GetRequiredService<IOptions<OllamaOptions>>().Value;
    return new OllamaApiClient(new Uri(options.BaseUrl), options.ChatModel);
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHealthChecks();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapHealthChecks("/health");
app.MapControllers();

await app.RunAsync();
