using HMS.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;

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
        public IActionResult DoctorList()
        {

            string ConnectionString = this._configuration.GetConnectionString("MyConnectionString");
            SqlConnection sqlConnection = new SqlConnection(ConnectionString);

            sqlConnection.Open();

            SqlCommand command = sqlConnection.CreateCommand();

            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "PR_Doctor_SelectAll";

            SqlDataReader reader = command.ExecuteReader();

            DataTable table = new DataTable();
            table.Load(reader);

            return View(table);
        }
        #endregion

        #region DeleteDoctor
        public IActionResult DoctorDelete(int DoctorID)
        {
            try
            {
                string connectionString = _configuration.GetConnectionString("MyConnectionString");
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlCommand command = connection.CreateCommand();
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "PR_Doctor_DeleteByPK";
                    command.Parameters.Add("@DoctorID", SqlDbType.Int).Value = DoctorID;

                    command.ExecuteNonQuery();
                }

                TempData["SuccessMessage"] = "Doctor deleted successfully!";
                return RedirectToAction("DoctorList");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while deleting the doctor: " + ex.Message;
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

        public IActionResult DoctorAddEdit(int? DoctorID)
        {
            UserNuDropDown();
            DoctorModel model = new DoctorModel();

            if (DoctorID != null)
            {
                string ConnectionString = this._configuration.GetConnectionString("MyConnectionString");
                SqlConnection sqlConnection = new SqlConnection(ConnectionString);
                sqlConnection.Open();

                SqlCommand command = sqlConnection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;

                command.CommandText = "PR_Doctor_SelectByPK";
                command.Parameters.Add("@DoctorID", SqlDbType.Int).Value = DoctorID;
                command.Parameters.Add("@UserID", SqlDbType.Int).Value = model.UserID;

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
