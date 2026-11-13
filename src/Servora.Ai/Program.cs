using Microsoft.Extensions.AI;
using Microsoft.Extensions.Options;
using OllamaSharp;
using Servora.Ai.Options;
using Servora.Ai.Tools;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<OllamaOptions>(builder.Configuration.GetSection(OllamaOptions.SectionName));
builder.Services.AddSingleton<IChatClient>(sp =>
{
    var options = sp.GetRequiredService<IOptions<OllamaOptions>>().Value;
    IChatClient client = new OllamaApiClient(new Uri(options.BaseUrl), options.ChatModel);
    return new ChatClientBuilder(client)
        .UseFunctionInvocation()
        .Build();
});

builder.Services.AddSingleton<AITool>(_ => AIFunctionFactory.Create(
    SalesTools.GetDailySalesSummary,
    name: "get_daily_sales_summary",
    description: "Gets daily sales summary for a branch: total revenue, order count, and top products. Pass date as yyyy-MM-dd."));

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
