using System.ComponentModel.DataAnnotations;

namespace HMS.Models
{
    public class UserModel
    {
        public int UserID { get; set; }
        [Required(ErrorMessage = "User Name is Required")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "Please enter the password")]
        public string Password { get; set; }
        [Required(ErrorMessage ="Email pan nay bhulta")]
        public string Email { get; set; }
        [Required(ErrorMessage ="Gimme the phone number")]
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
