using HMS.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;

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
    }
}
