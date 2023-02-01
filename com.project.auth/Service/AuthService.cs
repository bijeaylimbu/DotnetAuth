using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using com.project.auth.Application.Request;
using com.project.auth.Application.Response;
using com.project.auth.Domain.Inerfaces;
using com.project.auth.Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace com.project.auth.Service;

public class AuthService: IAuthService
{
    private readonly UserManager<User> _userManager;
    private readonly IConfiguration _configuration;

    public AuthService(UserManager<User> userManager, IConfiguration configuration)
    {
        _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

    }
    public async Task<AuthResponse> Registration(RegistrationRequest request)
    {
        var findEmail = await _userManager.FindByEmailAsync(request.Email);
        var findbyUser = await _userManager.FindByNameAsync(request.Username);
        if (findbyUser is not null && findEmail is not null)
        {
            return new AuthResponse
            {
                Message = "Email and User",
                Status = StatusCodes.Status500InternalServerError.ToString()
            };
        }
        else
        {
            User user = new User()
            {
                Email = request.Email,
                UserName = request.Username,
                
                SecurityStamp = Guid.NewGuid().ToString()
            };
            var result = await _userManager.CreateAsync(user, request.Password);
            if (result.Succeeded)
            {
                return new AuthResponse
                {
                    Message = "User created Successfully",
                    Status = StatusCodes.Status200OK.ToString()
                };
            }

            return new AuthResponse
            {
                Message = "Something Went Wrong",
                Status = StatusCodes.Status500InternalServerError.ToString()
            };
        }
    }

    public async Task<AuthResponse> Login(LoginRequest request)
    {
        var user = await _userManager.FindByNameAsync(request.Username);

        if(user is  null)
        {
            user = await _userManager.FindByEmailAsync(request.Username);
        }

        if (user is null || !await _userManager.CheckPasswordAsync(user, request.Password))
        {
            throw new ArgumentException($"Unable to authenticate user {request.Username}");
        }

        var authClaims = new List<Claim>
        {
            new(ClaimTypes.Name, user.UserName),
            new(ClaimTypes.Email, user.Email),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };

        var token = GetToken(authClaims);
        string jwtToken= new JwtSecurityTokenHandler().WriteToken(token);

        // return new JwtSecurityTokenHandler().WriteToken(token);
        return new AuthResponse
        {
            Message = "Successfully login",
            Status = StatusCodes.Status200OK.ToString(),
            Token = jwtToken
        };
    }

    private JwtSecurityToken GetToken(IEnumerable<Claim> authClaims)
    {
        var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

        var token = new JwtSecurityToken(
            issuer: _configuration["JWT:ValidIssuer"],
            audience: _configuration["JWT:ValidAudience"],
            expires: DateTime.Now.AddHours(3),
            claims: authClaims,
            signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256));

        return token;
    }
}