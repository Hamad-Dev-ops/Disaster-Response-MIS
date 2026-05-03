using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using DisasterMIS_Frontend.Helpers;
using System.Data;

namespace DisasterMIS_Frontend.Controllers
{
    public class UserController : Controller
    {
        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("Role") != "Administrator")
                return RedirectToAction("Index", "Home");

            string query = "SELECT u.UserID, u.Name, u.Email, r.RoleName, u.Status FROM [User] u JOIN Role r ON u.RoleID = r.RoleID";
            DataTable dt = DBHelper.ExecuteQuery(query);
            return View(dt);
        }
    }
}