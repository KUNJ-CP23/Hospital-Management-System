using System.ComponentModel.DataAnnotations;

namespace HMS.Models
{
    public class UserModel
    {
        public int UserID { get; set; }
        [Required(ErrorMessage = "User Name is Required")]
        [StringLength(100, ErrorMessage = "User Name cannot exceed 100 characters")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "Please enter the password")]
        [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d)(?=.*[^A-Za-z\d]).{8,}$",
        ErrorMessage = "Password must be at least 8 characters and include letters, numbers, and special characters")]
        public string Password { get; set; }
        [Required(ErrorMessage ="Enter your Email")]
        [EmailAddress(ErrorMessage = "Enter a valid email address")]
        public string Email { get; set; }
        [Required(ErrorMessage ="Enter the phone number")]
        [StringLength(15, ErrorMessage = "Mobile number cannot exceed 15 characters")]
        [RegularExpression(@"^[0-9]{7,15}$", ErrorMessage = "Enter a valid mobile number")]
        public string MobileNo { get; set; }
        public bool IsActive { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }

    }

    public class UserDropDownModel
    {
        public int UserID { get; set; }
        public string UserName { get; set; }
    }
}
