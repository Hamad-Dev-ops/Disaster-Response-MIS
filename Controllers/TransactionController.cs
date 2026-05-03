using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using DisasterMIS_Frontend.Helpers;
using System.Data.SqlClient;
using System.Data;

namespace DisasterMIS_Frontend.Controllers
{
    public class TransactionController : Controller
    {
        [HttpGet]
        public IActionResult Create()
        {
            // Role check
            var role = HttpContext.Session.GetString("Role");
            if (role != "FinanceOfficer" && role != "Administrator")
                return RedirectToAction("Index", "Home");

            // Fetch incidents for dropdown
            string incidentQuery = "SELECT IncidentID, Location FROM Incident ORDER BY IncidentID DESC";
            DataTable incidents = DBHelper.ExecuteQuery(incidentQuery);
            ViewBag.Incidents = incidents;

            return View();
        }

        [HttpPost]
        public IActionResult Create(string transactionType, decimal amount, string donorName, string purpose, int incidentId)
        {
            var role = HttpContext.Session.GetString("Role");
            if (role != "FinanceOfficer" && role != "Administrator")
                return RedirectToAction("Index", "Home");

            // Validate incident exists (optional)
            if (incidentId <= 0)
            {
                ViewBag.Error = "Please select a valid Incident.";
                // Reload incidents
                string incidentQuery = "SELECT IncidentID, Location FROM Incident ORDER BY IncidentID DESC";
                ViewBag.Incidents = DBHelper.ExecuteQuery(incidentQuery);
                return View();
            }

            // Insert into Transaction table
            string query = @"INSERT INTO [Transaction] (IncidentID, TransactionType, Amount, TransactionDate, Status) 
                             VALUES (@IncidentID, @Type, @Amount, GETDATE(), 'Completed');
                             SELECT SCOPE_IDENTITY();";
            SqlParameter[] parameters = {
                new SqlParameter("@IncidentID", incidentId),
                new SqlParameter("@Type", transactionType),
                new SqlParameter("@Amount", amount)
            };

            try
            {
                int newId = Convert.ToInt32(DBHelper.ExecuteScalar(query, parameters));

                if (transactionType == "Donation")
                {
                    if (string.IsNullOrWhiteSpace(donorName)) donorName = "Anonymous";
                    DBHelper.ExecuteNonQuery("INSERT INTO Donation (TransactionID, DonorName, DonorType) VALUES (@Tid, @Name, 'Individual')",
                        new SqlParameter[] {
                            new SqlParameter("@Tid", newId),
                            new SqlParameter("@Name", donorName)
                        });
                }
                else if (transactionType == "Expense")
                {
                    DBHelper.ExecuteNonQuery("INSERT INTO Expense (TransactionID, Purpose) VALUES (@Tid, @Purpose)",
                        new SqlParameter[] {
                            new SqlParameter("@Tid", newId),
                            new SqlParameter("@Purpose", purpose ?? "")
                        });
                }
                ViewBag.Success = "Transaction added successfully.";
            }
            catch (SqlException ex)
            {
                ViewBag.Error = "Database error: " + ex.Message;
            }

            // Reload incidents for the view (in case of error)
            string reloadQuery = "SELECT IncidentID, Location FROM Incident ORDER BY IncidentID DESC";
            ViewBag.Incidents = DBHelper.ExecuteQuery(reloadQuery);
            return View();
        }
    }
}