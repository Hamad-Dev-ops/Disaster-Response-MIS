using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using DisasterMIS_Frontend.Helpers;
using System.Data.SqlClient;

namespace DisasterMIS_Frontend.Controllers
{
    public class WarehouseController : Controller
    {
        [HttpGet]
        public IActionResult Create()
        {
            if (HttpContext.Session.GetString("Role") != "WarehouseManager" && HttpContext.Session.GetString("Role") != "Administrator")
                return RedirectToAction("Index", "Home");
            return View();
        }

        [HttpPost]
        public IActionResult Create(string location, decimal latitude, decimal longitude, int? managerId)
        {
            string query = "INSERT INTO Warehouse (Location, Latitude, Longitude, ManagerID) VALUES (@Loc, @Lat, @Lng, @Mgr)";
            SqlParameter[] parameters = {
                new SqlParameter("@Loc", location),
                new SqlParameter("@Lat", latitude),
                new SqlParameter("@Lng", longitude),
                new SqlParameter("@Mgr", (object?)managerId ?? DBNull.Value)
            };
            DBHelper.ExecuteNonQuery(query, parameters);
            ViewBag.Success = "Warehouse added successfully.";
            return View();
        }
    }
}