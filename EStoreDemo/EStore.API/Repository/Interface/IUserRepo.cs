using EShop.API.Data.Models;

namespace EShop.API.Repository.Interface;
public interface IUserRepo
{
    public Task<ServiceResponse> RegisterUser(UserModel model);
    public Task<ServiceResponse<UserModel>> Login(LoginModel model);
}
