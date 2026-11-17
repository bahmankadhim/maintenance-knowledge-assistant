using System.ComponentModel.DataAnnotations;

namespace Mka.Api.Dtos;

public record AskRequest(
    [Required, StringLength(1000, MinimumLength = 3)]
    string Question,

    // Optional, only for today's experiments. In production we remove it: users should not tune the model.
    [Range(0.0, 2.0)]
    float? Temperature
);

public record AskResponse(
    string Answer, string FinishReason, string Model,
    int InputTokens, int OutputTokens, long ElapsedMs
);


