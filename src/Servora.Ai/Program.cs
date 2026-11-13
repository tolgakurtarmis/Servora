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

builder.Services.AddScoped<AITool>(sp =>
{
    var tool = sp.GetRequiredService<SalesSummaryTool>();
    return AIFunctionFactory.Create(
        tool.GetDailySalesSummaryAsync,
        name: "get_daily_sales_summary",
        description: "Gets one day's sales for the current branch: total revenue, order count, and top products. Date is optional (yyyy-MM-dd); omit it for today. Use for bugün/dün or a specific day.");
});

builder.Services.AddScoped<AITool>(sp =>
{
    var tool = sp.GetRequiredService<SalesSummaryTool>();
    return AIFunctionFactory.Create(
        tool.GetWeeklySalesOverviewAsync,
        name: "get_weekly_sales_overview",
        description: "Gets the last 7 UTC days for the current branch: total revenue, order count, top products, weakest day (lowest revenue), and per-day breakdown. Use for bu hafta, zayıf gün, top ürünler this week.");
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
