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
                command.Parameters.Add("@DoctorName", SqlDbType.NVarChar).Value = doctormodel.DoctorName;
                command.Parameters.Add("@Description", SqlDbType.NVarChar).Value = doctormodel.Description;
                command.Parameters.Add("@UserID", SqlDbType.NVarChar).Value = doctormodel.UserID;
                command.Parameters.Add("@IsActive", SqlDbType.Bit).Value = doctormodel.IsActive;
                command.ExecuteNonQuery();
                return RedirectToAction("DoctorList");
            }
            return View(doctormodel);
        }
        #endregion

        #region EditDoctor
        #endregion

        #region UserDropDown
        #endregion

    }

}
