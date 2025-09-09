using System.ComponentModel.DataAnnotations;

namespace HMS.Models
{
    public class PatientModel
    {
        public int PatientID { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [MaxLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Date of Birth is required")]
        public DateTime DateOfBirth { get; set; }

        [Required(ErrorMessage = "Gender is required")]
        public string Gender { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [MaxLength(100, ErrorMessage = "Email cannot exceed 100 characters")]
        [EmailAddress(ErrorMessage = "Enter a valid email address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Phone number is required")]
        [MaxLength(20, ErrorMessage = "Phone number cannot exceed 20 characters")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Address is required")]
        [MaxLength(250, ErrorMessage = "Address cannot exceed 250 characters")]

        public string Address { get; set; }

        [Required(ErrorMessage = "City is required")]
        [MaxLength(100, ErrorMessage = "City cannot exceed 100 characters")]
        public string City { get; set; }

        [Required(ErrorMessage = "State is required")]
        [MaxLength(100, ErrorMessage = "State cannot exceed 100 characters")]
        public string State { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }
        public int UserID { get; set; }

        // New Properties for Image Upload
        public string? ImagePath { get; set; }
        public IFormFile PatientImage { get; set; }
    }
    public class PatientDropDownModel
    {
        public int PatientID { get; set; }
        public string Name { get; set; }
    }
}