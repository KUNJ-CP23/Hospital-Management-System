using HMS.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;
using System.Reflection.Metadata.Ecma335;

namespace HMS.Controllers
{
    public class DepartmentController : Controller
    {
        #region Config
        private IConfiguration _configuration;
        public DepartmentController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        #endregion

        #region SelectAllDepartment
        public IActionResult DepartmentList()
        {

            string ConnectionString = this._configuration.GetConnectionString("MyConnectionString");
            SqlConnection sqlConnection = new SqlConnection(ConnectionString);

            sqlConnection.Open();

            SqlCommand command = sqlConnection.CreateCommand();

            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "PR_Department_SelectAll";

            SqlDataReader reader = command.ExecuteReader();

            DataTable table = new DataTable();
            table.Load(reader);

            return View(table);
        }

        #endregion

        #region DeleteDepartment

        public IActionResult DepartmentDelete(int DepartmentID)
        {
            try
            {
                string connectionString = _configuration.GetConnectionString("MyConnectionString");
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlCommand command = connection.CreateCommand();
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "PR_Department_DeleteByPK";
                    command.Parameters.Add("@DepartmentID", SqlDbType.Int).Value = DepartmentID;

                    command.ExecuteNonQuery();
                }

                TempData["SuccessMessage"] = "Department deleted successfully!";
                return RedirectToAction("DepartmentList");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while deleting the department: " + ex.Message;
                return RedirectToAction("DepartmentList");
            }
        }

        #endregion

        #region Add Department

        [HttpPost]

        public IActionResult DepartmentAddEdit(DepartmentModel departmentmodel)
        {
            if (ModelState.IsValid)
            {
                string ConnectionString = this._configuration.GetConnectionString("MyConnectionString");
                SqlConnection sqlConnection = new SqlConnection(ConnectionString);
                sqlConnection.Open();

                SqlCommand command = sqlConnection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                if (departmentmodel.DepartmentID == 0)
                {
                    command.CommandText = "PR_Department_Insert";
                }
                else
                {
                    command.CommandText = "PR_Department_UpdateByPK";
                    command.Parameters.Add("@DepartmentID", SqlDbType.Int).Value = departmentmodel.DepartmentID;
                }
                command.Parameters.Add("@DepartmentName", SqlDbType.NVarChar).Value = departmentmodel.DepartmentName;
                command.Parameters.Add("@Description", SqlDbType.NVarChar).Value = departmentmodel.Description;
                command.Parameters.Add("@UserID", SqlDbType.NVarChar).Value = departmentmodel.UserID;
                command.Parameters.Add("@IsActive", SqlDbType.Bit).Value = departmentmodel.IsActive;
                command.ExecuteNonQuery();
                return RedirectToAction("DepartmentList");
            }
            return View(departmentmodel);
        }
        #endregion

        #region Edit Department
        public IActionResult DepartmentAddEdit(int? DepartmentID)
        {

            UserDropDown();
            DepartmentModel model = new DepartmentModel();

            if (DepartmentID != null)
            {
                string ConnectionString = this._configuration.GetConnectionString("MyConnectionString");
                SqlConnection sqlConnection = new SqlConnection(ConnectionString);
                sqlConnection.Open();

                SqlCommand command = sqlConnection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;

                command.CommandText = "PR_Department_SelectByPK";
                command.Parameters.Add("@DepartmentID", SqlDbType.Int).Value = DepartmentID;
                command.Parameters.Add("@UserID", SqlDbType.Int).Value = model.UserID;

                SqlDataReader reader = command.ExecuteReader();
                DataTable table = new DataTable();
                table.Load(reader);

                foreach (DataRow dr in table.Rows)
                {
                    model.DepartmentID = Convert.ToInt32(dr["DepartmentID"]);
                    model.DepartmentName = dr["DepartmentName"].ToString();
                    model.Description = dr["Description"].ToString();
                    model.UserID = Convert.ToInt32(dr["UserID"]);
                    model.IsActive = Convert.ToBoolean(dr["IsActive"]);
                }
            }
            return View(model);
        }
        #endregion

        #region USER DROPDOWN
        public void UserDropDown()
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
