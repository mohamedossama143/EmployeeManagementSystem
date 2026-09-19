using System.ComponentModel.DataAnnotations;

namespace EmployeeManagementSystem.Models
{
    public class JobTitle
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Title { get; set; }

        public ICollection<Employee>? Employees { get; set; }
    }
}