using HMS.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;

namespace HMS.Controllers
{
    public class DoctorDepartmentController : Controller
    {
        #region Config
        private IConfiguration _configuration;

        public DoctorDepartmentController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        #endregion

        #region SelectAllDoctorDepartment
        public IActionResult DoctorDepartmentList()
        {

            string ConnectionString = this._configuration.GetConnectionString("MyConnectionString");
            SqlConnection sqlConnection = new SqlConnection(ConnectionString);

            sqlConnection.Open();

            SqlCommand command = sqlConnection.CreateCommand();

            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "PR_DoctorDepartment_SelectAll";

            SqlDataReader reader = command.ExecuteReader();

            DataTable table = new DataTable();
            table.Load(reader);

            return View(table);
        }
        #endregion

        #region DeleteDoctorDepartment
        public IActionResult DoctorDepartmentDelete(int DoctorDepartmentID)
        {
            try
            {
                string connectionString = _configuration.GetConnectionString("MyConnectionString");
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlCommand command = connection.CreateCommand();
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "PR_DoctorDepartment_DeleteByPK";
                    command.Parameters.Add("@DoctorDepartmentID", SqlDbType.Int).Value = DoctorDepartmentID;

                    command.ExecuteNonQuery();
                }

                TempData["SuccessMessage"] = "DoctorDepartment deleted successfully!";
                return RedirectToAction("DoctorDepartmentList");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while deleting the doctordepartment: " + ex.Message;
                return RedirectToAction("DoctorDepartmentList");
            }
        }

        #endregion

        #region AddDoctorDepartment
        [HttpPost]
        public IActionResult DoctorDepartmentAddEdit(DoctorDepartmentModel doctorDepartmentModel)
        {
            if (ModelState.IsValid)
            {
                string ConnectionString = this._configuration.GetConnectionString("MyConnectionString");
                SqlConnection sqlConnection = new SqlConnection(ConnectionString);
                sqlConnection.Open();

                SqlCommand command = sqlConnection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                if (doctorDepartmentModel.DoctorDepartmentID == 0)
                {
                    command.CommandText = "PR_DoctorDepartment_Insert";
                }
                else
                {
                    command.CommandText = "PR_DoctorDepartment_UpdateByPK";
                    command.Parameters.Add("@DoctorDepartmentID", SqlDbType.Int).Value = doctorDepartmentModel.DoctorDepartmentID;
                }

                command.Parameters.AddWithValue("@DoctorID", doctorDepartmentModel.DoctorID);
                command.Parameters.AddWithValue("@UserID", doctorDepartmentModel.UserID);
                command.Parameters.AddWithValue("@DepartmentID", doctorDepartmentModel.DepartmentID);

                command.ExecuteNonQuery();
                return RedirectToAction("DoctorDepartmentList");
            }
            return View(doctorDepartmentModel);
        }
        #endregion

        #region EditDoctorDepartment
        public IActionResult DoctorDepartmentAddEdit(int? DoctorDepartmentID)
        {
            UsersDropDown();
            DepartmentDropDown();
            DoctorDropDown();

            DoctorDepartmentModel model = new DoctorDepartmentModel();

            if (DoctorDepartmentID != null)
            {
                string ConnectionString = this._configuration.GetConnectionString("MyConnectionString");
                SqlConnection sqlConnection = new SqlConnection(ConnectionString);
                sqlConnection.Open();

                SqlCommand command = sqlConnection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;

                command.CommandText = "PR_DoctorDepartment_SelectByPK";
                command.Parameters.Add("@DoctorDepartmentID", SqlDbType.Int).Value = DoctorDepartmentID;

                SqlDataReader reader = command.ExecuteReader();
                DataTable table = new DataTable();
                table.Load(reader);

                foreach (DataRow dr in table.Rows)
                {
                    model.DoctorDepartmentID = Convert.ToInt32(dr["DoctorDepartmentID"]);
                    model.DoctorID = Convert.ToInt32(dr["DoctorID"]);
                    model.DepartmentID = Convert.ToInt32(dr["DepartmentID"]);
                    model.UserID = Convert.ToInt32(dr["UserID"]);
                }
            }

            return View(model);
        }

        #endregion

        #region UserDropDown
        public void UsersDropDown()
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

        #region DepartmentDropDown
        public void DepartmentDropDown()
        {
            string connectionString = this._configuration.GetConnectionString("MyConnectionString");
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            SqlCommand command2 = connection.CreateCommand();
            command2.CommandType = System.Data.CommandType.StoredProcedure;
            command2.CommandText = "PR_Department_DropdownForDept";
            SqlDataReader reader2 = command2.ExecuteReader();
            DataTable dataTable2 = new DataTable();
            dataTable2.Load(reader2);
            List<DepartmentDropDownModel> departmentList = new List<DepartmentDropDownModel>();
            foreach (DataRow data in dataTable2.Rows)
            {
                DepartmentDropDownModel model = new DepartmentDropDownModel();
                model.DepartmentID = Convert.ToInt32(data["DepartmentID"]);
                model.DepartmentName = data["DepartmentName"].ToString();
                departmentList.Add(model);
            }
            ViewBag.DepartmentList = departmentList;
        }
        #endregion

        #region DoctorDropDown
        public void DoctorDropDown()
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
    }
}
