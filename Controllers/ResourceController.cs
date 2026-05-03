using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using DisasterMIS_Frontend.Models;
using DisasterMIS_Frontend.Helpers;
using System.Data.SqlClient;
using System.Data;

namespace DisasterMIS_Frontend.Controllers
{
    public class ResourceController : Controller
    {
        // ========== ALLOCATE RESOURCES ==========
        [HttpGet]
        public IActionResult Allocate()
        {
            if (HttpContext.Session.GetString("Role") != "WarehouseManager" && HttpContext.Session.GetString("Role") != "Administrator")
                return RedirectToAction("Index", "Home");
            return View();
        }

        [HttpPost]
        public IActionResult Allocate(AllocationModel model)
        {
            // Check role
            if (HttpContext.Session.GetString("Role") != "WarehouseManager" && HttpContext.Session.GetString("Role") != "Administrator")
                return RedirectToAction("Index", "Home");

            // Check model validity
            if (!ModelState.IsValid)
                return View(model);

            int userId = int.Parse(HttpContext.Session.GetString("UserID"));

            using (SqlConnection conn = new SqlConnection(DBHelper.GetConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("usp_AllocateResources", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@RequestID", model.RequestID);
                    cmd.Parameters.AddWithValue("@ResourceID", model.ResourceID);
                    cmd.Parameters.AddWithValue("@QuantityAllocated", model.QuantityAllocated);
                    cmd.Parameters.AddWithValue("@WarehouseID", model.WarehouseID);
                    cmd.Parameters.AddWithValue("@AllocatedBy", userId);

                    conn.Open();
                    try
                    {
                        cmd.ExecuteNonQuery();
                        ViewBag.Success = "Resources allocated successfully!";
                    }
                    catch (SqlException ex)
                    {
                        ViewBag.Error = ex.Message;
                    }
                }
            }
            return View(model); // ✅ Always return view after processing
        }

        // ========== ADD RESOURCE TYPE ==========
        [HttpGet]
        public IActionResult Create()
        {
            if (HttpContext.Session.GetString("Role") != "WarehouseManager" && HttpContext.Session.GetString("Role") != "Administrator")
                return RedirectToAction("Index", "Home");
            return View();
        }

        [HttpPost]
        public IActionResult Create(string resourceName, string resourceType)
        {
            if (HttpContext.Session.GetString("Role") != "WarehouseManager" && HttpContext.Session.GetString("Role") != "Administrator")
                return RedirectToAction("Index", "Home");

            string query = "INSERT INTO Resource (ResourceName, ResourceType) VALUES (@Name, @Type)";
            SqlParameter[] parameters = {
                new SqlParameter("@Name", resourceName),
                new SqlParameter("@Type", resourceType)
            };
            DBHelper.ExecuteNonQuery(query, parameters);
            ViewBag.Success = "Resource type added successfully.";
            return View();
        }
    }
}