using System.ComponentModel.DataAnnotations;

namespace ProjetoFinalPos.Models
{
    public class Client
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [StringLength(50, ErrorMessage = "Name should be a maximum of 50 characters")]
        [MinLength(5, ErrorMessage = "Name should be a minimum of 5 characters")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; } = string.Empty;

        [Phone(ErrorMessage = "Invalid Phone Number")]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Address is required")]
        [StringLength(100, ErrorMessage = "Address should be a maximum of 100 characters")]
        [MinLength(5, ErrorMessage = "Name should be a minimum of 5 characters")]
        public string Address { get; set; } = string.Empty;
    }
}
