using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using DisasterMIS_Frontend.Helpers;
using System.Data;

namespace DisasterMIS_Frontend.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Dashboard()
        {
            if (HttpContext.Session.GetString("Role") != "Administrator")
                return RedirectToAction("Index", "Home");

            string query = "SELECT * FROM AdminDashboard";
            DataTable dt = DBHelper.ExecuteQuery(query);
            return View(dt);
        }
    }
}