using EShop.Web.Utility;
using Microsoft.AspNetCore.Mvc;

namespace EShop.Web.Controllers;

[SessionValidation]
public class CartController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
