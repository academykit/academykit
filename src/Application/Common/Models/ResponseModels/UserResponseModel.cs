namespace Lingtren.Application.Common.Models.ResponseModels
{
    using Lingtren.Domain.Entities;
    using Lingtren.Domain.Enums;
    public class UserResponseModel
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string MobileNumber { get; set; }
        public UserRole Role { get; set; }
        public string Profession { get; set; }
        public string Address { get; set; }
        public string Bio { get; set; }
        public string ImageUrl { get; set; }
        public string PublicUrls { get; set; }
        public Guid? DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedOn { get; set; }
        public string FullName { get; set; }
        public IList<CourseCertificateIssuedResponseModel> Certificates { get; set; } = new List<CourseCertificateIssuedResponseModel>();
        public UserResponseModel(User user)
        {
            Id = user.Id;
            FirstName = user.FirstName;
            MiddleName = user.MiddleName;
            LastName = user.LastName;
            Email = user.Email;
            MobileNumber = user.MobileNumber;
            Role = user.Role;
            Profession = user.Profession;
            Address = user.Address;
            Bio = user.Bio;
            ImageUrl = user.ImageUrl;
            PublicUrls = user.PublicUrls;
            IsActive = user.IsActive;
            CreatedOn = user.CreatedOn;
            FullName = user.FullName;
            DepartmentId = user.DepartmentId;
            DepartmentName = user.Department?.Name;
        }
    }
}
