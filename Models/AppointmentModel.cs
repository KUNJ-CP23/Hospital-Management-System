using System.ComponentModel.DataAnnotations;

namespace HMS.Models
{
    public class AppointmentModel
    {
        public int AppointmentId { get; set; }
        [Required(ErrorMessage = "Apmt Date is required.")]
        public DateTime AppointmentDate { get; set; }
        [Required(ErrorMessage = "Apmt Status is required.")]
        public string AppointmentStatus { get; set; } = "Pending";
        [Required(ErrorMessage = "Apmt Desp is required.")]
        [MaxLength(250, ErrorMessage = "Description cannot exceed 250 characters.")]
        public string Description { get; set; }
        [Required(ErrorMessage = "Apmt Remarks is required.")]
        [MaxLength(250, ErrorMessage = "Special Remarks cannot exceed 250 characters.")]
        public string SpecialRemarks { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }
        [Required(ErrorMessage = "Total Amount is required.")]
        public Decimal TotalConsultedAmount { get; set; }
        [Required(ErrorMessage = "Patient ID is required.")]
        public int PatientID { get; set; }
        [Required(ErrorMessage = "Doctor ID is required.")]
        public int DoctorID { get; set; }
        [Required(ErrorMessage = "User ID is required.")]
        public int UserID { get; set; }

    }
}
