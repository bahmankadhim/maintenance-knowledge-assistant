using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mka.Api.Dtos;
using OpenAI.Chat;
namespace Mka.Api.Controllers;

[ApiController]
[Authorize] // any logged-in user (Technician or Supervisor); AI calls cost money
[Route("api/[controller]")]
public class ChatController(ChatClient chat, ILogger<ChatController> logger) : ControllerBase
{
    // The system prompt is code: reviewed in pull requests and versioned in git.
    private const string SystemPrompt = """
        You are the Maintenance Knowledge Assistant for field engineers at an oil and gas company.
        Answer briefly and practically, in the language of the question.
        If you are not sure, say so. Never invent part numbers, pressures, torques or other values.
        Safety comes first: for any hazardous task, remind the user to follow the site permit-to-work procedure.
        """;

    [HttpPost("ask")]
    public async Task<ActionResult<AskResponse>> Ask(AskRequest request, CancellationToken ct)
    {
        List<ChatMessage> messages =
        [
            new SystemChatMessage(SystemPrompt),
            new UserChatMessage(request.Question)
        ];

        var options = new ChatCompletionOptions
        {
            Temperature = request.Temperature ?? 0.2f, // low = consistent, factual answers
            MaxOutputTokenCount = 30                   // hard ceiling on cost and response time
        };

        var timer = Stopwatch.StartNew();
        ChatCompletion completion = await chat.CompleteChatAsync(messages, options, ct);
        timer.Stop();

        logger.LogInformation(
            "LLM call: {InputTokens} in, {OutputTokens} out, {ElapsedMs} ms, finish {FinishReason}",
            completion.Usage.InputTokenCount, completion.Usage.OutputTokenCount,
            timer.ElapsedMilliseconds, completion.FinishReason);

        // Content can be empty when Azure's content filter blocks the answer.
        var answer = completion.Content.Count > 0 ? completion.Content[0].Text : "";

        return Ok(new AskResponse(
            answer,
            completion.FinishReason.ToString(),
            completion.Model,
            completion.Usage.InputTokenCount,
            completion.Usage.OutputTokenCount,
            timer.ElapsedMilliseconds));
    }
}