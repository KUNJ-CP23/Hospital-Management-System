using System.ComponentModel.DataAnnotations;
namespace HMS.Models
{
    public class DoctorDepartmentModel
    {
        public int DoctorDepartmentID { get; set; }
        [Required(ErrorMessage = "Doctor selection is Required")]
        public int DoctorID { get; set; }
        [Required(ErrorMessage = "Department selection is Required")]
        public int DepartmentID { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }
        public int UserID { get; set; }
    }
}
