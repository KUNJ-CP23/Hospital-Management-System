using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;

namespace HMS.Controllers
{
    public class ExporttoExcelController : Controller
    {
        private readonly IConfiguration _configuration;

        public ExporttoExcelController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [Route("ExportToExcel")]
        public IActionResult ExportToExcel(string sp)
        {
            try
            {
                DataTable dt = RetrieveData(sp);

                using (var workbook = new XLWorkbook())
                {
                    workbook.Worksheets.Add(dt, "States");

                    using (var stream = new MemoryStream())
                    {
                        workbook.SaveAs(stream);
                        var content = stream.ToArray();

                        return File(
                            content,
                            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                            "States.xlsx"
                        );
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the error (you can use ILogger)
                return BadRequest($"Error while exporting: {ex.Message}");
            }
        }

        private DataTable RetrieveData(string SP, int? PKID = 0, string PKName = "")
        {
            var dt = new DataTable();

            using (var db_conn_access = new SqlConnection(
                _configuration.GetConnectionString("MyConnectionString")))
            {
                db_conn_access.Open();

                using (var command = new SqlCommand(SP, db_conn_access))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    if (PKID != 0 && !string.IsNullOrEmpty(PKName))
                    {
                        command.Parameters.AddWithValue("@" + PKName, PKID);
                    }

                    using (var reader = command.ExecuteReader())
                    {
                        dt.Load(reader);
                    }
                }
            }
            return dt;
        }
    }
}