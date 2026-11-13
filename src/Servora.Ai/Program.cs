using Microsoft.Extensions.AI;
using Microsoft.Extensions.Options;
using OllamaSharp;
using Servora.Ai.Contracts;
using Servora.Ai.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<OllamaOptions>(builder.Configuration.GetSection(OllamaOptions.SectionName));
builder.Services.AddSingleton<IChatClient>(sp =>
{
    var options = sp.GetRequiredService<IOptions<OllamaOptions>>().Value;
    return new OllamaApiClient(new Uri(options.BaseUrl), options.ChatModel);
});

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

app.MapGet("/ai/status", async (IChatClient chatClient, IOptions<OllamaOptions> options, CancellationToken cancellationToken) =>
{
    try
    {
        _ = await chatClient.GetResponseAsync(
            [new ChatMessage(ChatRole.User, "Reply with OK.")],
            cancellationToken: cancellationToken);
        return Results.Ok(new
        {
            status = "ok",
            provider = "ollama",
            model = options.Value.ChatModel,
            baseUrl = options.Value.BaseUrl
        });
    }
    catch (Exception ex)
    {
        return Results.Json(
            new
            {
                status = "unavailable",
                provider = "ollama",
                model = options.Value.ChatModel,
                baseUrl = options.Value.BaseUrl,
                error = ex.Message
            },
            statusCode: StatusCodes.Status503ServiceUnavailable);
    }
});

app.MapPost("/ai/complete", async (
    CompleteRequest request,
    IChatClient chatClient,
    IOptions<OllamaOptions> options,
    CancellationToken cancellationToken) =>
{
    if (string.IsNullOrWhiteSpace(request.Prompt))
    {
        return Results.BadRequest(new { error = "Prompt is required." });
    }

    var response = await chatClient.GetResponseAsync(
        [new ChatMessage(ChatRole.User, request.Prompt.Trim())],
        cancellationToken: cancellationToken);
    return Results.Ok(new CompleteResponse(response.Text, options.Value.ChatModel));
});

await app.RunAsync();
