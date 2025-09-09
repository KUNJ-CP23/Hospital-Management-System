using HMS.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;
using TagHelper;

namespace HMS.Controllers
{
    [CheckAccess]
    public class HomeController : Controller
    {
        private IConfiguration _configuration;

        public HomeController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public IActionResult Dashboard()
        {
            DashboardModel model = new DashboardModel();

            string connectionString = _configuration.GetConnectionString("MyConnectionString");

            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand("PR_GetDashboardCounts", sqlConnection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    sqlConnection.Open();

                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        model.UsersCount = reader["UsersCount"] != DBNull.Value ? Convert.ToInt32(reader["UsersCount"]) : 0;
                        model.DoctorsCount = reader["DoctorsCount"] != DBNull.Value ? Convert.ToInt32(reader["DoctorsCount"]) : 0;
                        model.DepartmentsCount = reader["DepartmentsCount"] != DBNull.Value ? Convert.ToInt32(reader["DepartmentsCount"]) : 0;
                        model.PatientsCount = reader["PatientsCount"] != DBNull.Value ? Convert.ToInt32(reader["PatientsCount"]) : 0;
                        model.AppointmentsCount = reader["AppointmentsCount"] != DBNull.Value ? Convert.ToInt32(reader["AppointmentsCount"]) : 0;
                    }
                }
            }

            return View(model);
        }
    }
}
