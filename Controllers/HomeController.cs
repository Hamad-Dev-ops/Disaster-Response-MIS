using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using DisasterMIS_Frontend.Helpers;
using System.Data;

namespace DisasterMIS_Frontend.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("UserID") == null)
                return RedirectToAction("Login", "Account");

            // Get statistics for dashboard
            ViewBag.TotalIncidents = DBHelper.ExecuteScalar("SELECT COUNT(*) FROM Incident") ?? 0;
            ViewBag.ActiveIncidents = DBHelper.ExecuteScalar("SELECT COUNT(*) FROM Incident WHERE Status NOT IN ('Resolved','Closed')") ?? 0;
            ViewBag.AvailableTeams = DBHelper.ExecuteScalar("SELECT COUNT(*) FROM RescueTeam WHERE AvailabilityStatus = 'Available'") ?? 0;
            ViewBag.PendingRequests = DBHelper.ExecuteScalar("SELECT COUNT(*) FROM Request WHERE Status = 'Pending'") ?? 0;

            object donations = DBHelper.ExecuteScalar("SELECT ISNULL(SUM(Amount), 0) FROM [Transaction] WHERE TransactionType = 'Donation' AND Status = 'Completed'");
            ViewBag.TotalDonations = donations ?? 0;

            ViewBag.UserName = HttpContext.Session.GetString("UserName");
            ViewBag.Role = HttpContext.Session.GetString("Role");

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
    }
}