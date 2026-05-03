using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using DisasterMIS_Frontend.Helpers;
using System.Data;
using System.Data.SqlClient;

namespace DisasterMIS_Frontend.Controllers
{
    public class FieldController : Controller
    {
        private bool IsAuthorized()
        {
            var role = HttpContext.Session.GetString("Role");
            return role == "FieldOfficer" || role == "Administrator";
        }

        public IActionResult Dashboard()
        {
            if (!IsAuthorized()) return RedirectToAction("Index", "Home");

            string query = @"
                SELECT IncidentID, Location, DisasterType, SeverityLevel, Status, TimeReported
                FROM Incident
                WHERE Status NOT IN ('Resolved', 'Closed')
                ORDER BY TimeReported DESC";

            DataTable dt = DBHelper.ExecuteQuery(query);
            return View(dt);
        }

        [HttpGet]
        public IActionResult UpdateStatus(int id)
        {
            if (!IsAuthorized()) return RedirectToAction("Index", "Home");

            ViewBag.IncidentID = id;

            // Get current status of the incident
            string query = "SELECT Status FROM Incident WHERE IncidentID = @ID";
            SqlParameter[] parameters = { new SqlParameter("@ID", id) };
            DataTable dt = DBHelper.ExecuteQuery(query, parameters);

            if (dt.Rows.Count > 0)
            {
                ViewBag.CurrentStatus = dt.Rows[0]["Status"].ToString();
            }

            return View();
        }

        [HttpPost]
        public IActionResult UpdateStatus(int incidentId, string status)
        {
            if (!IsAuthorized()) return RedirectToAction("Index", "Home");

            string query = "UPDATE Incident SET Status = @Status WHERE IncidentID = @ID";
            SqlParameter[] parameters = {
                new SqlParameter("@Status", status),
                new SqlParameter("@ID", incidentId)
            };

            DBHelper.ExecuteNonQuery(query, parameters);

            // Also log to IncidentStatusLog for audit trail
            string logQuery = "INSERT INTO IncidentStatusLog (IncidentID, Status, Timestamp) VALUES (@ID, @Status, GETDATE())";
            SqlParameter[] logParams = {
                new SqlParameter("@ID", incidentId),
                new SqlParameter("@Status", status)
            };
            DBHelper.ExecuteNonQuery(logQuery, logParams);

            TempData["Success"] = "Incident status updated successfully.";
            return RedirectToAction("Dashboard");
        }
    }
}