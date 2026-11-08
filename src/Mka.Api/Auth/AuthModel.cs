using System.ComponentModel.DataAnnotations;
namespace Mka.Api.Auth;


public record DemoUser(string Username, string Password, string Role);
public record LoginRequest([Required] string Username, [Required] string Password);
public record TokenResponse(string AccessToken, DateTime ExpiresAt);

