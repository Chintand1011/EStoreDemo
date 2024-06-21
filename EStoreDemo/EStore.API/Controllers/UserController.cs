using EShop.API.Data.Models;
using EShop.API.DataUtils.Interface;
using Microsoft.AspNetCore.Mvc;

namespace EShop.API.Controllers;

[Route("api/user")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly IUserData _user;

    public UserController(IUserData user) => _user = user;

    [HttpPost]
    [Route("Register")]
    public async Task<ServiceResponse> Register([FromBody] UserModel model) => await _user.RegisterUser(model);

    [HttpPost]
    [Route("Login")]
    public async Task<ServiceResponse<UserModel>> LoginUser([FromBody] LoginModel model) => await _user.Login(model);
}
