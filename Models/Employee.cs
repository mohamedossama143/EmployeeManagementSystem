using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EmployeeManagementSystem.Models
{
    public class Employee
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string FullName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Phone]
        public string PhoneNumber { get; set; }

        [DataType(DataType.Date)]
        public DateTime HireDate { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Salary { get; set; }

        public string? ProfileImagePath { get; set; }

        [NotMapped]
        public IFormFile? FeatureImage { get; set; }

        // Foreign Keys
        public int DepartmentId { get; set; }
        public Department Department { get; set; }

        public int JobTitleId { get; set; }
        public JobTitle JobTitle { get; set; }
    }
}