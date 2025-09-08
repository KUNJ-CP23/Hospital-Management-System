using System.ComponentModel.DataAnnotations;

namespace HMS.Models
{
    public class DepartmentModel
    {
        public int DepartmentID { get; set; }
        [Required(ErrorMessage = "Dept. Name is Required")]
        [StringLength(100, ErrorMessage = "Dept. Name cannot exceed 100 characters")]
        public string DepartmentName { get; set; }

        [Required(ErrorMessage = "Dept. Description is Required")]
        [StringLength(250, ErrorMessage = "Description cannot exceed 250 characters")]
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }
        [Required(ErrorMessage = "Please select a user")]
        public int? UserID { get; set; }

    }
    public class DepartmentDropDownModel
    {
        public int DepartmentID { get; set; }
        public string DepartmentName { get; set; }
    }
}
