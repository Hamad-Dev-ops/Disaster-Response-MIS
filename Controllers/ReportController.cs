using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using DisasterMIS_Frontend.Helpers;
using System.Data;
using ClosedXML.Excel;
using System.IO;

namespace DisasterMIS_Frontend.Controllers
{
    public class ReportController : Controller
    {
        private bool IsAuthorized()
        {
            var role = HttpContext.Session.GetString("Role");
            return role == "Administrator" || role == "EmergencyOperator" || role == "WarehouseManager" || role == "FinanceOfficer" || role == "FieldOfficer";
        }

        public IActionResult IncidentsByLocation()
        {
            if (!IsAuthorized()) return RedirectToAction("Index", "Home");
            string query = @"
                SELECT Location, DisasterType, SeverityLevel, COUNT(*) AS IncidentCount
                FROM Incident
                GROUP BY Location, DisasterType, SeverityLevel
                ORDER BY IncidentCount DESC";
            DataTable dt = DBHelper.ExecuteQuery(query);
            return View(dt);
        }

        public IActionResult ResourceUtilization()
        {
            if (!IsAuthorized()) return RedirectToAction("Index", "Home");
            string query = @"
                SELECT r.ResourceName,
                       SUM(d.QuantityDispatched) AS TotalDispatched,
                       ISNULL(SUM(c.QuantityConsumed), 0) AS TotalConsumed,
                       (SUM(d.QuantityDispatched) - ISNULL(SUM(c.QuantityConsumed), 0)) AS UnusedOrLost
                FROM Dispatch d
                JOIN AllocationDetail a ON d.AllocationID = a.AllocationID
                JOIN Resource r ON a.ResourceID = r.ResourceID
                LEFT JOIN ConsumptionRecord c ON d.DispatchID = c.DispatchID
                GROUP BY r.ResourceName
                ORDER BY TotalDispatched DESC";
            DataTable dt = DBHelper.ExecuteQuery(query);
            return View(dt);
        }

        public IActionResult ResponseTime()
        {
            if (!IsAuthorized()) return RedirectToAction("Index", "Home");
            string query = @"
                SELECT TOP 10
                    i.IncidentID,
                    i.TimeReported,
                    ta.AssignedTime,
                    DATEDIFF(MINUTE, i.TimeReported, ta.AssignedTime) AS ResponseMinutes
                FROM Incident i
                JOIN TeamAssignment ta ON i.IncidentID = ta.IncidentID
                WHERE ta.AssignedTime IS NOT NULL
                ORDER BY ResponseMinutes ASC";
            DataTable dt = DBHelper.ExecuteQuery(query);
            return View(dt);
        }

        public IActionResult FinancialSummary()
        {
            if (!IsAuthorized()) return RedirectToAction("Index", "Home");
            string query = @"
                SELECT TransactionType, SUM(Amount) AS TotalAmount, COUNT(*) AS TransactionCount
                FROM [Transaction]
                WHERE Status = 'Completed'
                GROUP BY TransactionType";
            DataTable dt = DBHelper.ExecuteQuery(query);
            return View(dt);
        }

        public IActionResult ApprovalWorkflow()
        {
            if (!IsAuthorized()) return RedirectToAction("Index", "Home");
            string query = @"
                SELECT r.RequestID, r.RequestType, r.Status AS RequestStatus,
                       a.Status AS ApprovalStatus, a.Comments, a.Timestamp AS ApprovalDate
                FROM Request r
                LEFT JOIN Approval a ON r.RequestID = a.RequestID
                ORDER BY r.CreatedAt DESC";
            DataTable dt = DBHelper.ExecuteQuery(query);
            return View(dt);
        }

       

public IActionResult ExportToExcel(string reportType)
    {
        try
        {
            DataTable dt = new DataTable();
            string fileName = "";

            switch (reportType)
            {
                case "Incidents":
                    dt = DBHelper.ExecuteQuery(@"
                    SELECT Location, DisasterType, SeverityLevel, COUNT(*) AS IncidentCount
                    FROM Incident
                    GROUP BY Location, DisasterType, SeverityLevel
                    ORDER BY IncidentCount DESC");
                    fileName = "Incident_Statistics.xlsx";
                    break;
                case "ResourceUtilization":
                    dt = DBHelper.ExecuteQuery(@"
                    SELECT r.ResourceName,
                           ISNULL(SUM(d.QuantityDispatched), 0) AS TotalDispatched,
                           ISNULL(SUM(c.QuantityConsumed), 0) AS TotalConsumed
                    FROM Resource r
                    LEFT JOIN AllocationDetail a ON r.ResourceID = a.ResourceID
                    LEFT JOIN Dispatch d ON a.AllocationID = d.AllocationID
                    LEFT JOIN ConsumptionRecord c ON d.DispatchID = c.DispatchID
                    GROUP BY r.ResourceName");
                    fileName = "Resource_Utilization.xlsx";
                    break;
                case "ResponseTime":
                    dt = DBHelper.ExecuteQuery(@"
                    SELECT TOP 20 i.IncidentID, i.Location, i.TimeReported, ta.AssignedTime,
                           DATEDIFF(MINUTE, i.TimeReported, ta.AssignedTime) AS ResponseMinutes
                    FROM Incident i
                    JOIN TeamAssignment ta ON i.IncidentID = ta.IncidentID
                    WHERE ta.AssignedTime IS NOT NULL
                    ORDER BY ResponseMinutes ASC");
                    fileName = "Response_Time_Report.xlsx";
                    break;
                case "FinancialSummary":
                    dt = DBHelper.ExecuteQuery(@"
                    SELECT TransactionType, SUM(Amount) AS TotalAmount, COUNT(*) AS TransactionCount
                    FROM [Transaction]
                    WHERE Status = 'Completed'
                    GROUP BY TransactionType");
                    fileName = "Financial_Summary.xlsx";
                    break;
                case "ApprovalWorkflow":
                    dt = DBHelper.ExecuteQuery(@"
                    SELECT r.RequestID, r.RequestType, r.Status AS RequestStatus,
                           a.Status AS ApprovalStatus, a.Comments, a.Timestamp AS ApprovalDate
                    FROM Request r
                    LEFT JOIN Approval a ON r.RequestID = a.RequestID
                    ORDER BY r.CreatedAt DESC");
                    fileName = "Approval_Workflow.xlsx";
                    break;
                default:
                    dt = DBHelper.ExecuteQuery("SELECT * FROM Incident");
                    fileName = "Report.xlsx";
                    break;
            }

            if (dt.Rows.Count == 0)
            {
                TempData["Error"] = "No data available to export.";
                return Redirect(Request.Headers["Referer"].ToString());
            }

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Report");
                worksheet.Cell(1, 1).InsertTable(dt, true);
                worksheet.Columns().AdjustToContents();

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    stream.Position = 0;
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
                }
            }
        }
        catch (Exception ex)
        {
            TempData["Error"] = "Export failed: " + ex.Message;
            return Redirect(Request.Headers["Referer"].ToString());
        }
    }
}
}