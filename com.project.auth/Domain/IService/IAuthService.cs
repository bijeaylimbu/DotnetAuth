using com.project.auth.Application.Request;
using com.project.auth.Application.Response;

namespace com.project.auth.Domain.Inerfaces;

public interface IAuthService
{
    Task<AuthResponse> Registration(RegistrationRequest request);
    Task<AuthResponse> Login(LoginRequest request);

}