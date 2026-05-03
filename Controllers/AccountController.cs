using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using DisasterMIS_Frontend.Helpers;
using System.Data.SqlClient;
using System.Data;

namespace DisasterMIS_Frontend.Controllers
{
    public class AccountController : Controller
    {
        [HttpGet]
        public IActionResult Login()
        {
            if (HttpContext.Session.GetString("UserID") != null)
                return RedirectToAction("Index", "Home");
            return View();
        }

        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            string query = @"
        SELECT u.UserID, u.Name, u.Email, r.RoleName, u.RoleID
        FROM [User] u
        INNER JOIN Role r ON u.RoleID = r.RoleID
        WHERE u.Email = @Email AND u.PasswordHash = @Password AND u.Status = 'Active'";

            SqlParameter[] parameters = {
        new SqlParameter("@Email", email),
        new SqlParameter("@Password", password) // In production, hash password; for demo plain is fine
    };

            DataTable dt = DBHelper.ExecuteQuery(query, parameters);

            if (dt.Rows.Count == 1)
            {
                var row = dt.Rows[0];
                HttpContext.Session.SetString("UserID", row["UserID"].ToString());
                HttpContext.Session.SetString("UserName", row["Name"].ToString());
                HttpContext.Session.SetString("Role", row["RoleName"].ToString());
                HttpContext.Session.SetString("RoleID", row["RoleID"].ToString());

                TempData["Success"] = $"Welcome back, {row["Name"]}!";

                return RedirectToAction("Index", "Home");
            }

            TempData["Error"] = "Invalid email or password.";
            return View();
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        // ========== REGISTER (SIGN UP) ==========
        [HttpGet]
        public IActionResult Register()
        {
            // If already logged in, redirect to home
            if (HttpContext.Session.GetString("UserID") != null)
                return RedirectToAction("Index", "Home");

            // Get list of roles for dropdown
            string roleQuery = "SELECT RoleID, RoleName FROM Role ORDER BY RoleName";
            DataTable roles = DBHelper.ExecuteQuery(roleQuery);
            ViewBag.Roles = roles;
            return View();
        }

        [HttpPost]
        public IActionResult Register(string name, string email, string password, int roleId)
        {
            // Check if email already exists
            string checkQuery = "SELECT COUNT(*) FROM [User] WHERE Email = @Email";
            SqlParameter[] checkParams = { new SqlParameter("@Email", email) };
            int existing = Convert.ToInt32(DBHelper.ExecuteScalar(checkQuery, checkParams));
            if (existing > 0)
            {
                ViewBag.Error = "Email already registered. Please use another email or login.";
                // Reload roles for dropdown
                string roleQuery = "SELECT RoleID, RoleName FROM Role ORDER BY RoleName";
                ViewBag.Roles = DBHelper.ExecuteQuery(roleQuery);
                return View();
            }

            // Insert new user (plain password for demo)
            string insertQuery = @"
        INSERT INTO [User] (Name, Email, PasswordHash, RoleID, Status)
        VALUES (@Name, @Email, @Password, @RoleID, 'Active')";
            SqlParameter[] parameters = {
        new SqlParameter("@Name", name),
        new SqlParameter("@Email", email),
        new SqlParameter("@Password", password),
        new SqlParameter("@RoleID", roleId)
    };
            DBHelper.ExecuteNonQuery(insertQuery, parameters);
            ViewBag.Success = "Registration successful! Please login.";
            return RedirectToAction("Login");
        }
    }

}