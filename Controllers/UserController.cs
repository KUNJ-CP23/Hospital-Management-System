using HMS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;

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

        public IActionResult UserList()
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
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "PR_User_SelectAll";

            //reader is used to read the data from the command
            SqlDataReader reader = command.ExecuteReader();

            //3 methods we have
            //ExecuteReader() -- record is get only
            //ExecuteNonQuery() -- for insert, update, delete operations
            //ExuteScalar() -- for single value return

            //this is to catch the data from the reader and load it into a DataTable
            DataTable table = new DataTable();
            table.Load(reader);

            return View(table);
        }
        #endregion

        #region DeleteUser
        public IActionResult UserDelete(int UserID)
        {
            try
            {
                string connectionString = _configuration.GetConnectionString("MyConnectionString");
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlCommand command = connection.CreateCommand();
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "PR_User_DeleteByPK";
                    command.Parameters.Add("@UserID", SqlDbType.Int).Value = UserID;

                    command.ExecuteNonQuery();
                }
                TempData["SuccessMessage"] = "✅ User deleted successfully!";
                return RedirectToAction("UserList");
                
            }
            
            catch (SqlException ex) when (ex.Number == 547)
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

        public IActionResult UserAddEdit(int? UserID)
        {
            //if (UserID == null)
            //{
            //    //var m = new UserModel
            //    //{
            //    //};
            //    return View(new UserModel());
            //}

            UserModel model = new UserModel();

            if(UserID != null)
            {
                string ConnectionString = this._configuration.GetConnectionString("MyConnectionString");
                SqlConnection sqlConnection = new SqlConnection(ConnectionString);
                sqlConnection.Open();

                SqlCommand command = sqlConnection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "PR_User_SelectByPK";
                command.Parameters.Add("@UserID", SqlDbType.Int).Value = UserID;

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