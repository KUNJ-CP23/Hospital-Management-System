namespace HMS.Models
{
    public class DoctorDepartmentModel
    {
        public int DoctorDepartmentID { get; set; }
        public string DoctorName { get; set; }
        public string DepartmentName { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }
        public string UserName { get; set; }
    }
}
