using System.ComponentModel.DataAnnotations;

namespace HMS.Models
{
    public class DoctorModel
    {
        public int DoctorID { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [MaxLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Phone number is required")]
        [MaxLength(20, ErrorMessage = "Phone number cannot exceed 20 characters")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [MaxLength(100, ErrorMessage = "Email cannot exceed 100 characters")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Qualification is required")]
        [MaxLength(100, ErrorMessage = "Qualification cannot exceed 100 characters")]
        public string Qualification { get; set; }

        [Required(ErrorMessage = "Specialization is required")]
        [MaxLength(100, ErrorMessage = "Specialization cannot exceed 100 characters")]
        public string Specialization { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }
        public int UserID { get; set; }

    }
    public class DoctorDropDownModel
    {
        public int DoctorID { get; set; }
        public string Name { get; set; }
    }
}
