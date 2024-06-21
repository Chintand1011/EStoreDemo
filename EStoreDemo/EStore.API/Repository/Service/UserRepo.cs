using EShop.API.Data.Entity;
using EShop.API.Data.Models;
using EShop.API.Helper;
using EShop.API.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace EShop.API.Repository.Service;
public class UserRepo : IUserRepo
{
    private readonly EShopDbContext _db;
    private readonly IConfiguration _configuration;

    public UserRepo(EShopDbContext db, IConfiguration configuration)
    {
        _db = db;
        _configuration = configuration;
    }

    public async Task<ServiceResponse> RegisterUser(UserModel model)
    {
        try
        {
            if (model.Email is not null && model.Username is not null)
            {
                var userExists = await _db.Users.Where(x => x.Username == model.Username || x.Email == model.Email).FirstOrDefaultAsync();
                if (userExists is not null)
                    return new ServiceResponse(false, "Username or Email address has already been registered.", StatusCodes.Status302Found);

                Data.Entity.DbSet.User user = new()
                {
                    Email = model.Email,
                    CreatedAt = DateTime.UtcNow,
                    Username = model.Username,
                    Password = AesPasswordEncryption.Encrypt(model.Password),
                    IsAdmin = model.IsAdmin
                };
                await _db.Users.AddAsync(user);
                await _db.SaveChangesAsync();
                return new ServiceResponse(true, "User Created Successfully", StatusCodes.Status201Created);
            }
            else
            {
                return new ServiceResponse(false, "Invalid or partial data entered", StatusCodes.Status400BadRequest);
            }
        }
        catch (Exception ex)
        {
            return new ServiceResponse(false, "Something went wrong! Please try again after sometime.", StatusCodes.Status500InternalServerError);
        }
    }

    public async Task<ServiceResponse<UserModel>> Login(LoginModel model)
    {
        var result = new ServiceResponse<UserModel>();
        if (model != null)
        {
            try
            {
                var user = await _db.Users.Where(x => x.Username == model.Username).FirstOrDefaultAsync();
                if (user != null)
                {
                    var validpassword = await _db.Users.Where(x => x.Username == model.Username && x.Password == AesPasswordEncryption.Encrypt(model.Password)).FirstOrDefaultAsync();
                    if (validpassword is null)
                    {
                        result.Message = "Invalid Password";
                        result.StatusCode = StatusCodes.Status401Unauthorized;
                        result.IsSuccess = false;
                        return result;
                    }
                    else
                    {
                        var authClaims = new List<Claim>
                            {
                                new Claim(ClaimTypes.Name, user.Username),
                                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                            };
                        var tokenExpiryTime = DateTime.Now.AddHours(1);
                        var token = GetToken(authClaims, tokenExpiryTime);
                        
                        result.IsSuccess = true;
                        result.StatusCode = StatusCodes.Status200OK;
                        result.Message = "Login successful.";
                        result.Data = new UserModel()
                        {
                            Token = new JwtSecurityTokenHandler().WriteToken(token),
                            TokenExpiryTime= tokenExpiryTime.ToString(),
                            Email = user.Email,
                            Id = user.Id,
                            Username = user.Username,
                            IsAdmin = user.IsAdmin
                        };
                        return result;
                    }
                }

                result.Message = "Data Not Found";
                result.StatusCode = StatusCodes.Status404NotFound;
                result.IsSuccess = false;
                return result;
            }
            catch (Exception ex)
            {
                result.Message = "Something went wrong! Please try again after sometime.";
                result.StatusCode = StatusCodes.Status500InternalServerError;
                result.IsSuccess = false;
                return result;
            }
        }
        result.Message = "Data Not Found";
        result.StatusCode = StatusCodes.Status404NotFound;
        result.IsSuccess = false;
        return result;
    }


    private JwtSecurityToken GetToken(List<Claim> authClaims, DateTime tokenExpiryTime)
    {
        var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"] ?? string.Empty));

        var token = new JwtSecurityToken(
            issuer: _configuration["JWT:ValidIssuer"],
            audience: _configuration["JWT:ValidAudience"],
            expires: tokenExpiryTime,
            claims: authClaims,
            signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );

        return token;
    }
}
