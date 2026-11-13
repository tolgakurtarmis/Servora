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

// promp buradaki descriptiona göre hani tool:GetDailySalesSummaryAsync gideceğini bulur 
builder.Services.AddScoped<AITool>(sp =>
{
    var tool = sp.GetRequiredService<SalesSummaryTool>();
    return AIFunctionFactory.Create(
        tool.GetDailySalesSummaryAsync,
        name: "get_daily_sales_summary",
        description: "Gets daily sales summary for the current branch from Servora.Api: total revenue, order count, and top products. Date is optional (yyyy-MM-dd); omit it for today.");
});

builder.Services.AddSingleton<IChatClient>(sp =>
{
    var options = sp.GetRequiredService<IOptions<OllamaOptions>>().Value;
    IChatClient client = new OllamaApiClient(new Uri(options.BaseUrl), options.ChatModel);
    return new ChatClientBuilder(client)
        .UseFunctionInvocation()
        .Build();
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
