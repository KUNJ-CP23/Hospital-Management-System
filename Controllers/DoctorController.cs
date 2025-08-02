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


        public IActionResult DoctorAddEdit()
        {
            return View();
        }
    }

}
