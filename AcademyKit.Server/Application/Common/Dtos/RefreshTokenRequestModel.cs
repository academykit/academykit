namespace AcademyKit.Application.Common.Dtos
{
    using System.ComponentModel.DataAnnotations;

    public class RefreshTokenRequestModel
    {
        [Required]
        public string Token { get; set; }
    }
}
