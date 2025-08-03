using HMS.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;

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
        public IActionResult PatientList()
        {

            string ConnectionString = this._configuration.GetConnectionString("MyConnectionString");
            SqlConnection sqlConnection = new SqlConnection(ConnectionString);

            sqlConnection.Open();

            SqlCommand command = sqlConnection.CreateCommand();

            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "PR_Patient_SelectAll";

            SqlDataReader reader = command.ExecuteReader();

            DataTable table = new DataTable();
            table.Load(reader);

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

                TempData["SuccessMessage"] = "Patient deleted successfully!";
                return RedirectToAction("PatientList");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while deleting the patient: " + ex.Message;
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
                command.ExecuteNonQuery();
                return RedirectToAction("PatientList");
            }
            return View(patientModel);
        }
        #endregion

        #region EditPatient
        public IActionResult PatientAddEdit(int? PatientID)
        {

            UserKaDropDown();
            PatientModel model = new PatientModel();

            if (PatientID != null)
            {
                string ConnectionString = this._configuration.GetConnectionString("MyConnectionString");
                SqlConnection sqlConnection = new SqlConnection(ConnectionString);
                sqlConnection.Open();

                SqlCommand command = sqlConnection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;

                command.CommandText = "PR_Patient_SelectByPK";
                command.Parameters.Add("@PatientID", SqlDbType.Int).Value = PatientID;
                command.Parameters.Add("@UserID", SqlDbType.Int).Value = model.UserID;

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
