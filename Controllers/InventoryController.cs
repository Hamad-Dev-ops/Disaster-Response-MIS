using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using DisasterMIS_Frontend.Helpers;
using System.Data;

namespace DisasterMIS_Frontend.Controllers
{
    public class InventoryController : Controller
    {
        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("Role") != "WarehouseManager" && HttpContext.Session.GetString("Role") != "Administrator")
                return RedirectToAction("Index", "Home");

            string query = "SELECT * FROM WarehouseManagerView";
            DataTable dt = DBHelper.ExecuteQuery(query);
            return View(dt);
        }
    }
}