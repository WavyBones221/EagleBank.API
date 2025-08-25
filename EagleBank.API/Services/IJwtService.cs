using EagleBank.API.Model;

namespace EagleBank.Api.Services;

public interface IJwtService
{
    string GenerateToken(User user);
}
