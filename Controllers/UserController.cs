using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using HMS.Helpers;
using HMS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Net;
using System.Reflection;
using System.Web;

namespace HMS.Controllers
{
    public class UserController : Controller
    {
        #region Config
        private IConfiguration _configuration;

        public UserController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        #endregion

        #region SelectAllUser

        public IActionResult UserList(string UserName="", string Email="", string MobileNo="")
        {

            //make connection string in appsettings.json file
            //and use it here to connect to the database

            string ConnectionString = this._configuration.GetConnectionString("MyConnectionString");
            SqlConnection sqlConnection = new SqlConnection(ConnectionString);

            //open the connection

            sqlConnection.Open();

            //command to execute 
            SqlCommand command = sqlConnection.CreateCommand();

            //below command type and command text is to mention the type and name of the SP
            //command.CommandType = CommandType.StoredProcedure;
            //command.CommandText = "PR_User_SelectAll";

            if (string.IsNullOrEmpty(UserName) && string.IsNullOrEmpty(Email) && string.IsNullOrEmpty(MobileNo))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "PR_User_SelectAll";
            }
            else
            {
                // If any filter → call Search
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "PR_User_Search";

                command.Parameters.AddWithValue("@UserName", string.IsNullOrEmpty(UserName) ? "" : UserName);
                command.Parameters.AddWithValue("@Email", string.IsNullOrEmpty(Email) ? "" : Email);
                command.Parameters.AddWithValue("@MobileNo", string.IsNullOrEmpty(MobileNo) ? "" : MobileNo);
            }


            //reader is used to read the data from the command
            SqlDataReader reader = command.ExecuteReader();

            //3 methods we have
            //ExecuteReader() -- record is get only
            //ExecuteNonQuery() -- for insert, update, delete operations
            //ExuteScalar() -- for single value return

            //this is to catch the data from the reader and load it into a DataTable
            DataTable table = new DataTable();
            table.Load(reader);

            ViewBag.UserName = UserName;
            ViewBag.Email = Email;
            ViewBag.MobileNo = MobileNo;

            return View(table);
        }
        #endregion

        #region DeleteUser
        public IActionResult UserDelete(string UserID)
        {
            try
            {
                // 🔓 Decrypt the UserID first
                //int decryptedUserId = Convert.ToInt32(UrlEncryptor.Decrypt(UserID));
                string decodedUserId = HttpUtility.UrlDecode(UserID); // Decode first
                int decryptedUserId = Convert.ToInt32(UrlEncryptor.Decrypt(decodedUserId));
                string connectionString = _configuration.GetConnectionString("MyConnectionString");
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlCommand command = connection.CreateCommand();
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "PR_User_DeleteByPK";
                    command.Parameters.Add("@UserID", SqlDbType.Int).Value = decryptedUserId;

                    command.ExecuteNonQuery();
                }

                TempData["SuccessMessage"] = "✅ User deleted successfully!";
                return RedirectToAction("UserList");
            }
            catch (SqlException ex) when (ex.Number == 547) // FK constraint
            {
                TempData["ErrorMessage"] = "❌ Cannot delete this user. It is referenced somewhere else (foreign key constraint).";
                return RedirectToAction("UserList");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "❌ An error occurred while deleting the user: " + ex.Message;
                return RedirectToAction("UserList");
            }
        }
        #endregion

        #region Add User

        [HttpPost]

        public IActionResult UserAddEdit(UserModel usermodel)
        {
            if (ModelState.IsValid)
            {
                string ConnectionString = this._configuration.GetConnectionString("MyConnectionString");
                SqlConnection sqlConnection = new SqlConnection(ConnectionString);
                sqlConnection.Open();

                SqlCommand command = sqlConnection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                if(usermodel.UserID == 0)
                {
                    command.CommandText = "PR_User_Insert";
                }
                else
                {
                    command.CommandText = "PR_User_UpdateByPK";
                    command.Parameters.Add("@UserID", SqlDbType.Int).Value = usermodel.UserID;
                }
                command.Parameters.Add("@UserName", SqlDbType.NVarChar).Value = usermodel.UserName;
                command.Parameters.Add("@Password", SqlDbType.NVarChar).Value = usermodel.Password;
                command.Parameters.Add("@Email", SqlDbType.NVarChar).Value = usermodel.Email;
                command.Parameters.Add("@MobileNo", SqlDbType.NVarChar).Value = usermodel.MobileNo;
                command.Parameters.Add("@IsActive", SqlDbType.Bit).Value = usermodel.IsActive;
                command.ExecuteNonQuery();
                return RedirectToAction("UserList");
            }
            return View(usermodel);
        }
        #endregion

        #region Edit User

        [HttpGet]

        public IActionResult UserAddEdit(string? UserID)
        {
            
            UserModel model = new UserModel();

            if(UserID != null)
            {
                var decryptedUserId = HMS.Helpers.UrlEncryptor.Decrypt(UserID);
                int userIdInt = Convert.ToInt32(decryptedUserId);
                string ConnectionString = this._configuration.GetConnectionString("MyConnectionString");
                SqlConnection sqlConnection = new SqlConnection(ConnectionString);
                sqlConnection.Open();

                SqlCommand command = sqlConnection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "PR_User_SelectByPK";
                command.Parameters.Add("@UserID", SqlDbType.Int).Value = userIdInt;

                //we got the id now we have to load its all data 
                SqlDataReader reader = command.ExecuteReader();
                DataTable table = new DataTable();
                table.Load(reader);
                //UserModel model = new UserModel();

                foreach (DataRow dr in table.Rows)
                {
                    model.UserID = Convert.ToInt32(dr["UserID"]);
                    model.UserName = dr["UserName"].ToString();
                    model.Password = dr["Password"].ToString();
                    model.Email = dr["Email"].ToString();
                    model.MobileNo = dr["MobileNo"].ToString();
                    model.IsActive = Convert.ToBoolean(dr["IsActive"]);
                }
            }
            
            //data aavi gyo have a ne form redirect karsu
            return View(model);
        }

        #endregion

    }
}


// DB Connectiviity Check
// Step 1: Connection String consists : 
// Server name, Database, Authentication, Encryption


//defualt + attribute routing ek sathe chale

//[Route("User-Ne-Delete karo")]