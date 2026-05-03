using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using DisasterMIS_Frontend.Models;
using DisasterMIS_Frontend.Helpers;
using System.Data.SqlClient;
using System.Data;

namespace DisasterMIS_Frontend.Controllers
{
    public class IncidentController : Controller
    {
        [HttpGet]
        public IActionResult Report()
        {
            if (HttpContext.Session.GetString("Role") != "EmergencyOperator" && HttpContext.Session.GetString("Role") != "Administrator")
                return RedirectToAction("Index", "Home");
            return View();
        }

        [HttpPost]
        public IActionResult Report(IncidentModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            string query = @"
                INSERT INTO Incident (Location, Latitude, Longitude, DisasterType, SeverityLevel, PriorityLevel, TimeReported, Status, ReporterContact)
                VALUES (@Location, @Latitude, @Longitude, @DisasterType, @SeverityLevel, @PriorityLevel, GETDATE(), 'Reported', @ReporterContact);
                SELECT SCOPE_IDENTITY();";

            SqlParameter[] parameters = {
                new SqlParameter("@Location", model.Location),
                new SqlParameter("@Latitude", model.Latitude),
                new SqlParameter("@Longitude", model.Longitude),
                new SqlParameter("@DisasterType", model.DisasterType),
                new SqlParameter("@SeverityLevel", model.SeverityLevel),
                new SqlParameter("@PriorityLevel", model.PriorityLevel),
                new SqlParameter("@ReporterContact", (object?)model.ReporterContact ?? DBNull.Value)
            };

            var newId = DBHelper.ExecuteScalar(query, parameters);
            ViewBag.Success = $"Incident reported successfully. ID: {newId}";
            ModelState.Clear();
            return View(new IncidentModel());
        }

        // ✅ NEW: Action to list all incidents
        public IActionResult List()
        {
            if (HttpContext.Session.GetString("Role") != "EmergencyOperator" && HttpContext.Session.GetString("Role") != "Administrator")
                return RedirectToAction("Index", "Home");

            string query = "SELECT IncidentID, Location, DisasterType, SeverityLevel, Status, TimeReported FROM Incident ORDER BY TimeReported DESC";
            DataTable dt = DBHelper.ExecuteQuery(query);
            return View(dt);
        }
    }
}