using EShop.API.Data.Models;

namespace EShop.API.DataUtils.Interface;

public interface IUserData
{
    public Task<ServiceResponse> RegisterUser(UserModel model);
    public Task<ServiceResponse<UserModel>> Login(LoginModel model);
}
