namespace Lingtren.Application.Common.Dtos
{
    using System.ComponentModel.DataAnnotations;

    public class ContactRequestDto
    {
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Phone { get; set; }

        [Required]
        public string Message { get; set; }
    }
}
