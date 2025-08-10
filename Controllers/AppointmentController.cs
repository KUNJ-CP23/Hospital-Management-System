using HMS.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;

namespace HMS.Controllers
{
    public class AppointmentController : Controller
    {
        #region Config
        private IConfiguration _configuration;

        public AppointmentController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        #endregion

        #region SelectAllAppointment
        public IActionResult AppointmentList()
        {

            string ConnectionString = this._configuration.GetConnectionString("MyConnectionString");
            SqlConnection sqlConnection = new SqlConnection(ConnectionString);

            sqlConnection.Open();

            SqlCommand command = sqlConnection.CreateCommand();

            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "PR_Appointment_SelectAll";

            SqlDataReader reader = command.ExecuteReader();

            DataTable table = new DataTable();
            table.Load(reader);

            return View(table);
        }
        #endregion

        #region DeleteAppointment
        public IActionResult AppointmentDelete(int AppointmentId)
        {
            try
            {
                string connectionString = _configuration.GetConnectionString("MyConnectionString");
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlCommand command = connection.CreateCommand();
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "PR_Appointment_DeleteByPK";
                    command.Parameters.Add("@AppointmentId", SqlDbType.Int).Value = AppointmentId;

                    command.ExecuteNonQuery();
                }

                TempData["SuccessMessage"] = "Appointment deleted successfully!";
                return RedirectToAction("AppointmentList");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while deleting the appointment: " + ex.Message;
                return RedirectToAction("AppointmentList");
            }
        }
        #endregion

        #region AddAppointment
        [HttpPost]
        public IActionResult AppointmentAddEdit(AppointmentModel appointmentModel)
        {
            if (ModelState.IsValid)
            {
                string connectionString = _configuration.GetConnectionString("MyConnectionString");
                SqlConnection sqlConnection = new SqlConnection(connectionString);
                sqlConnection.Open();

                SqlCommand command = sqlConnection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;

                if (appointmentModel.AppointmentId == 0)
                {
                    command.CommandText = "PR_Appointment_Insert";
                }
                else
                {
                    command.CommandText = "PR_Appointment_UpdateByPK";
                    command.Parameters.Add("@AppointmentId", SqlDbType.Int).Value = appointmentModel.AppointmentId;
                }

                command.Parameters.AddWithValue("@AppointmentDate", appointmentModel.AppointmentDate);
                command.Parameters.AddWithValue("@AppointmentStatus", appointmentModel.AppointmentStatus);
                command.Parameters.AddWithValue("@Description", appointmentModel.Description);
                command.Parameters.AddWithValue("@SpecialRemarks", appointmentModel.SpecialRemarks);
                command.Parameters.AddWithValue("@TotalConsultedAmount", appointmentModel.TotalConsultedAmount);
                command.Parameters.AddWithValue("@PatientID", appointmentModel.PatientID);
                command.Parameters.AddWithValue("@DoctorID", appointmentModel.DoctorID);
                command.Parameters.AddWithValue("@UserID", appointmentModel.UserID);
                command.ExecuteNonQuery();
                return RedirectToAction("AppointmentList");
            }
            UsersKaDropDown();
            DoctorKaDropDown();
            PatientKaDropDown();
            return View(appointmentModel);
        }

        #endregion

        #region EditAppointment
        public IActionResult AppointmentAddEdit(int? AppointmentId)
        {
            
            AppointmentModel model = new AppointmentModel();

            if (AppointmentId != null)
            {
                string connectionString = this._configuration.GetConnectionString("MyConnectionString");
                SqlConnection sqlConnection = new SqlConnection(connectionString);
                sqlConnection.Open();

                SqlCommand command = sqlConnection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;

                command.CommandText = "PR_Appointment_SelectByPK";
                command.Parameters.Add("@AppointmentId", SqlDbType.Int).Value = AppointmentId;
                command.Parameters.Add("@UserID", SqlDbType.Int).Value = model.UserID;

                SqlDataReader reader = command.ExecuteReader();
                DataTable table = new DataTable();
                table.Load(reader);

                foreach (DataRow dr in table.Rows)
                {
                    model.AppointmentId = Convert.ToInt32(dr["AppointmentId"]);
                    model.AppointmentDate = Convert.ToDateTime(dr["AppointmentDate"]);
                    model.AppointmentStatus = dr["AppointmentStatus"].ToString();
                    model.Description = dr["Description"].ToString();
                    model.SpecialRemarks = dr["SpecialRemarks"].ToString();
                    model.TotalConsultedAmount = Convert.ToDecimal(dr["TotalConsultedAmount"]);
                    model.PatientID = Convert.ToInt32(dr["PatientID"]);
                    model.DoctorID = Convert.ToInt32(dr["DoctorID"]);
                    model.UserID = Convert.ToInt32(dr["UserID"]);
                }
            }
            UsersKaDropDown();
            DoctorKaDropDown();
            PatientKaDropDown();

            return View(model);
        }

        #endregion

        #region UserDropDown
        public void UsersKaDropDown()
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

        #region DoctorDropDown
        public void DoctorKaDropDown()
        {
            string connectionString = this._configuration.GetConnectionString("MyConnectionString");
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            SqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "PR_Doctor_DropdownForDoctor";
            SqlDataReader reader = command.ExecuteReader();
            DataTable dataTable = new DataTable();
            dataTable.Load(reader);

            List<DoctorDropDownModel> doctorList = new List<DoctorDropDownModel>();
            foreach (DataRow row in dataTable.Rows)
            {
                DoctorDropDownModel model = new DoctorDropDownModel();
                model.DoctorID = Convert.ToInt32(row["DoctorID"]);
                model.Name = row["Name"].ToString();
                doctorList.Add(model);
            }

            ViewBag.DoctorList = doctorList;
        }
        #endregion

        #region PatientDropDown
        public void PatientKaDropDown()
        {
            string connectionString = this._configuration.GetConnectionString("MyConnectionString");
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            SqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "PR_Patient_DropdownForPatient";
            SqlDataReader reader = command.ExecuteReader();
            DataTable dataTable = new DataTable();
            dataTable.Load(reader);

            List<PatientDropDownModel> patientList = new List<PatientDropDownModel>();
            foreach (DataRow row in dataTable.Rows)
            {
                PatientDropDownModel model = new PatientDropDownModel();
                model.PatientID = Convert.ToInt32(row["PatientID"]);
                model.Name = row["Name"].ToString();
                patientList.Add(model);
            }

            ViewBag.PatientList = patientList;
        }
        #endregion

    }
}
