using EShop.Web.Utility;
using EShop.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace EShop.Web.Controllers;
public class UserController : Controller
{
    private readonly IApiUtility _apiHelper;
    public UserController(IApiUtility apiHelper)
    {
        _apiHelper = apiHelper;
    }
    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    public async Task<ResponseModel<UserModel>> Login([FromBody] LoginModel model)
    {
        var responseMessage = await _apiHelper.MakeApiCallAsync("User/Login", HttpMethod.Post, model);
        var responseAsync = await SharedMethod.ProcessApiResultAsync<UserModel>(responseMessage);
        if (responseAsync.IsSuccess)
        {
            HttpContext.Session.SetString("Token", responseAsync.Data.Token);
            HttpContext.Session.SetString("Username", responseAsync.Data.Username);
            HttpContext.Session.SetInt32("UserId", responseAsync.Data.Id);
            HttpContext.Session.SetInt32("IsAdmin", responseAsync.Data.IsAdmin ? 1 : 0);
        }
        return responseAsync;
    }

    [HttpPost]
    public async Task<ResponseModel<string>> Register([FromBody] RegisterModel model)
    {
        var responseMessage = await _apiHelper.MakeApiCallAsync("User/Register", HttpMethod.Post, model);
        var responseAsync = await SharedMethod.ProcessApiResultAsync<string>(responseMessage);
        return responseAsync;
    }

    [HttpGet]
    public async Task<string> Logout()
    {

        HttpContext.Session.Remove("Token");
        HttpContext.Session.Remove("Username");
        HttpContext.Session.Remove("IsAdmin");
        HttpContext.Session.Remove("UserId");
        HttpContext.Session.Clear();
        var expiredCookie = new CookieOptions
        {
            Expires = DateTime.Now.AddDays(-1),
            IsEssential = true,
            HttpOnly = true,
        };
        Response.Cookies.Append("_EShopCookie", "", expiredCookie);

        return "Logout successfully.";
    }
}
