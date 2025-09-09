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
    public class PatientController : Controller
    {
        #region Config
        private IConfiguration _configuration;

        public PatientController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        #endregion

        #region SelectAllPatient
        public IActionResult PatientList(string Name = "", string Email = "", string City = "")
        {

            string ConnectionString = this._configuration.GetConnectionString("MyConnectionString");
            SqlConnection sqlConnection = new SqlConnection(ConnectionString);

            sqlConnection.Open();

            SqlCommand command = sqlConnection.CreateCommand();

            if (string.IsNullOrEmpty(Name) && string.IsNullOrEmpty(Email) && string.IsNullOrEmpty(City))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "PR_Patient_SelectAll";
            }
            else
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "PR_Patient_Search";

                command.Parameters.AddWithValue("@Name", string.IsNullOrEmpty(Name) ? "" : Name);
                command.Parameters.AddWithValue("@Email", string.IsNullOrEmpty(Email) ? "" : Email);
                command.Parameters.AddWithValue("@City", string.IsNullOrEmpty(City) ? "" : City);
            }

            SqlDataReader reader = command.ExecuteReader();

            DataTable table = new DataTable();
            table.Load(reader);

            ViewBag.Name = Name;
            ViewBag.Email = Email;
            ViewBag.City = City;

            return View(table);
        }

        #endregion

        #region DeletePatient
        public IActionResult PatientDelete(int PatientID)
        {
            try
            {
                string connectionString = _configuration.GetConnectionString("MyConnectionString");
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlCommand command = connection.CreateCommand();
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "PR_Patient_DeleteByPK";
                    command.Parameters.Add("@PatientID", SqlDbType.Int).Value = PatientID;

                    command.ExecuteNonQuery();
                }

                TempData["SuccessMessage"] = "✅ Patient deleted successfully!";
                return RedirectToAction("PatientList");
            }
            catch (SqlException ex) when (ex.Number == 547) // FK constraint violation
            {
                TempData["ErrorMessage"] = "❌ Cannot delete this patient. It is referenced somewhere else (foreign key constraint).";
                return RedirectToAction("PatientList");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "❌ An error occurred while deleting the patient: " + ex.Message;
                return RedirectToAction("PatientList");
            }
        }

        #endregion

        #region AddPatient

        [HttpPost]
        public IActionResult PatientAddEdit(PatientModel patientModel)
        {
            if (ModelState.IsValid)
            {
                string ConnectionString = this._configuration.GetConnectionString("MyConnectionString");
                SqlConnection sqlConnection = new SqlConnection(ConnectionString);
                sqlConnection.Open();

                // Handle File Upload

                string fileName = patientModel.ImagePath; // old image as it is rye if user havent edited

                //agar image no path nay hoi to new image upload krdo
                if (patientModel.PatientImage != null && patientModel.PatientImage.Length > 0)
                {
                    //combine method current directory and folder path ne combine kare che
                    // "wwwroot/uploads/patients" folder ma upload thase
                    string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/patients");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    //guid is for giving unique name to the file every time
                    fileName = Guid.NewGuid().ToString() + Path.GetExtension(patientModel.PatientImage.FileName);

                    //aa actual path che j store thase
                    string filePath = Path.Combine(uploadsFolder, fileName);

                    //stream is used to read and write bytes to a file
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        patientModel.PatientImage.CopyTo(stream);
                    }
                }
                //File upload end

                SqlCommand command = sqlConnection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                if (patientModel.PatientID == 0)
                {
                    command.CommandText = "PR_Patient_Insert";
                }
                else
                {
                    command.CommandText = "PR_Patient_UpdateByPK";
                    command.Parameters.Add("@PatientID", SqlDbType.Int).Value = patientModel.PatientID;
                }
                command.Parameters.AddWithValue("@Name", patientModel.Name);
                command.Parameters.AddWithValue("@DateOfBirth", patientModel.DateOfBirth);
                command.Parameters.AddWithValue("@Gender", patientModel.Gender);
                command.Parameters.AddWithValue("@Email", patientModel.Email);
                command.Parameters.AddWithValue("@Phone", patientModel.Phone);
                command.Parameters.AddWithValue("@Address", patientModel.Address);
                command.Parameters.AddWithValue("@City", patientModel.City);
                command.Parameters.AddWithValue("@State", patientModel.State);
                command.Parameters.AddWithValue("@IsActive", patientModel.IsActive);
                command.Parameters.AddWithValue("@UserID", patientModel.UserID);
                command.Parameters.AddWithValue("@ImagePath", (object)fileName ?? DBNull.Value);

                command.ExecuteNonQuery();
                return RedirectToAction("PatientList");
            }
            return View(patientModel);
        }
        #endregion

        #region EditPatient
        [HttpGet]
        public IActionResult PatientAddEdit(int? PatientID)
        {
            UserKaDropDown(); // load User dropdown
            PatientModel model = new PatientModel();

            if (PatientID != null)
            {

                string connectionString = this._configuration.GetConnectionString("MyConnectionString");
                using (SqlConnection sqlConnection = new SqlConnection(connectionString))
                {
                    sqlConnection.Open();

                    SqlCommand command = sqlConnection.CreateCommand();
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "PR_Patient_SelectByPK";
                    command.Parameters.Add("@PatientID", SqlDbType.Int).Value = PatientID;

                    SqlDataReader reader = command.ExecuteReader();
                    DataTable table = new DataTable();
                    table.Load(reader);

                    foreach (DataRow dr in table.Rows)
                    {
                        model.PatientID = Convert.ToInt32(dr["PatientID"]);
                        model.Name = dr["Name"].ToString();
                        model.DateOfBirth = Convert.ToDateTime(dr["DateOfBirth"]);
                        model.Gender = dr["Gender"].ToString();
                        model.Email = dr["Email"].ToString();
                        model.Phone = dr["Phone"].ToString();
                        model.Address = dr["Address"].ToString();
                        model.City = dr["City"].ToString();
                        model.State = dr["State"].ToString();
                        model.IsActive = Convert.ToBoolean(dr["IsActive"]);
                        model.UserID = Convert.ToInt32(dr["UserID"]);
                        if (dr["ImagePath"] != DBNull.Value)
                        {
                            model.ImagePath = dr["ImagePath"].ToString();
                        }
                    }
                }
            }
            return View(model);
        }

        #endregion

        #region UserDropDown
        public void UserKaDropDown()
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
