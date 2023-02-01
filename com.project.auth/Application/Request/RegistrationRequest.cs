namespace com.project.auth.Application.Request;

public record RegistrationRequest(string Username, string Email, string Password);