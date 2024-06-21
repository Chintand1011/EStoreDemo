using EShop.API.Data.Models;
using EShop.API.DataUtils.Interface;
using EShop.API.Repository.Interface;

namespace EShop.API.DataUtils.Service;

public class UserData : IUserData
{
    private readonly IUserRepo _user;
 
    public UserData(IUserRepo userService)
    {
        this._user = userService;
    }

    public Task<ServiceResponse<UserModel>> Login(LoginModel model)
    {
        return _user.Login(model);
    }

    public Task<ServiceResponse> RegisterUser(UserModel model)
    {
        return _user.RegisterUser(model);
    }
}
