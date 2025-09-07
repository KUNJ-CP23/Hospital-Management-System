using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using HMS.Helpers;
using HMS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Net;
using System.Reflection.Metadata.Ecma335;

namespace HMS.Controllers
{
    public class DoctorController : Controller
    {
        #region Config
        private IConfiguration _configuration;

        public DoctorController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        #endregion

        #region SelectAllDoctor
        public IActionResult DoctorList(string Name = "", string Phone="", string Email = "")
        {

            string ConnectionString = this._configuration.GetConnectionString("MyConnectionString");
            SqlConnection sqlConnection = new SqlConnection(ConnectionString);

            sqlConnection.Open();

            SqlCommand command = sqlConnection.CreateCommand();

            if (string.IsNullOrEmpty(Name) && string.IsNullOrEmpty(Phone) && string.IsNullOrEmpty(Email))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "PR_Doctor_SelectAll";
            }
            else
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "PR_Doctor_Search";

                command.Parameters.AddWithValue("@Name", string.IsNullOrEmpty(Name) ? "" : Name);
                command.Parameters.AddWithValue("@Phone", string.IsNullOrEmpty(Phone) ? "" : Phone);
                command.Parameters.AddWithValue("@Email", string.IsNullOrEmpty(Email) ? "" : Email);
            }
            

            SqlDataReader reader = command.ExecuteReader();

            DataTable table = new DataTable();
            table.Load(reader);

            ViewBag.Name = Name;
            ViewBag.Phone = Phone;
            ViewBag.Email = Email;

            return View(table);
        }
        #endregion

        #region DeleteDoctor
        public IActionResult DoctorDelete(string DoctorID)
        {
            try
            {
                // 🔓 Decrypt the DoctorID first
                int decryptedDoctorId = Convert.ToInt32(UrlEncryptor.Decrypt(DoctorID));

                string connectionString = _configuration.GetConnectionString("MyConnectionString");
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlCommand command = connection.CreateCommand();
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "PR_Doctor_DeleteByPK";
                    command.Parameters.Add("@DoctorID", SqlDbType.Int).Value = decryptedDoctorId;

                    command.ExecuteNonQuery();
                }

                TempData["SuccessMessage"] = "✅ Doctor deleted successfully!";
                return RedirectToAction("DoctorList");
            }
            catch (SqlException ex) when (ex.Number == 547) // FK constraint
            {
                TempData["ErrorMessage"] = "❌ Cannot delete this doctor. It is referenced somewhere else (foreign key constraint).";
                return RedirectToAction("DoctorList");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "❌ An error occurred while deleting the doctor: " + ex.Message;
                return RedirectToAction("DoctorList");
            }
        }
        #endregion

        #region AddDoctor

        [HttpPost]

        public IActionResult DoctorAddEdit(DoctorModel doctormodel)
        {
            if (ModelState.IsValid)
            {
                string ConnectionString = this._configuration.GetConnectionString("MyConnectionString");
                SqlConnection sqlConnection = new SqlConnection(ConnectionString);
                sqlConnection.Open();

                SqlCommand command = sqlConnection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                if (doctormodel.DoctorID == 0)
                {
                    command.CommandText = "PR_Doctor_Insert";
                }
                else
                {
                    command.CommandText = "PR_Doctor_UpdateByPK";
                    command.Parameters.Add("@DoctorID", SqlDbType.Int).Value = doctormodel.DoctorID;
                }
                command.Parameters.Add("@Name", SqlDbType.NVarChar).Value = doctormodel.Name;
                command.Parameters.Add("@Phone", SqlDbType.NVarChar).Value = doctormodel.Phone;
                command.Parameters.Add("@Email", SqlDbType.NVarChar).Value = doctormodel.Email;
                command.Parameters.Add("@Qualification", SqlDbType.NVarChar).Value = doctormodel.Qualification;
                command.Parameters.Add("@Specialization", SqlDbType.NVarChar).Value = doctormodel.Specialization;
                command.Parameters.Add("@UserID", SqlDbType.NVarChar).Value = doctormodel.UserID;
                command.Parameters.Add("@IsActive", SqlDbType.Bit).Value = doctormodel.IsActive;
                command.ExecuteNonQuery();
                return RedirectToAction("DoctorList");
            }
            return View(doctormodel);
        }
        #endregion

        #region EditDoctor

        [HttpGet]
        public IActionResult DoctorAddEdit(string? DoctorID)
        {
            UserNuDropDown(); // load User dropdown
            DoctorModel model = new DoctorModel();

            if (DoctorID != null)
            {
                // 🔓 Decrypt DoctorID
                var decryptedDoctorId = HMS.Helpers.UrlEncryptor.Decrypt(DoctorID);
                int doctorIdInt = Convert.ToInt32(decryptedDoctorId);

                string connectionString = this._configuration.GetConnectionString("MyConnectionString");
                using (SqlConnection sqlConnection = new SqlConnection(connectionString))
                {
                    sqlConnection.Open();

                    SqlCommand command = sqlConnection.CreateCommand();
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "PR_Doctor_SelectByPK";
                    command.Parameters.Add("@DoctorID", SqlDbType.Int).Value = doctorIdInt;

                    SqlDataReader reader = command.ExecuteReader();
                    DataTable table = new DataTable();
                    table.Load(reader);

                    foreach (DataRow dr in table.Rows)
                    {
                        model.DoctorID = Convert.ToInt32(dr["DoctorID"]);
                        model.Name = dr["Name"].ToString();
                        model.Phone = dr["Phone"].ToString();
                        model.Email = dr["Email"].ToString();
                        model.Qualification = dr["Qualification"].ToString();
                        model.Specialization = dr["Specialization"].ToString();
                        model.UserID = Convert.ToInt32(dr["UserID"]);
                        model.IsActive = Convert.ToBoolean(dr["IsActive"]);
                    }
                }
            }
            return View(model);
        }

        #endregion

        #region UserDropDown
        public void UserNuDropDown()
        {
            string connectionString = this._configuration.GetConnectionString("MyConnectionString");
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            SqlCommand command2 = connection.CreateCommand();
            command2.CommandType = System.Data.CommandType.StoredProcedure;
            command2.CommandText = "PR_User_DropdownForUser";
            SqlDataReader reader2 = command2.ExecuteReader();
            DataTable dataTable2 = new DataTable();
            dataTable2.Load(reader2);
            List<UserDropDownModel> userList = new List<UserDropDownModel>();
            foreach (DataRow data in dataTable2.Rows)
            {
                UserDropDownModel model = new UserDropDownModel();
                model.UserID = Convert.ToInt32(data["UserID"]);
                model.UserName = data["UserName"].ToString();
                userList.Add(model);
            }
            ViewBag.UserList = userList;
        }
        #endregion

    }

}
