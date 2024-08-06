namespace AcademyKit.Application.Common.Models.ResponseModels
{
    using AcademyKit.Domain.Entities;
    using AcademyKit.Domain.Enums;

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
        public string Bio { get; set; }
        public string ImageUrl { get; set; }
        public string PublicUrls { get; set; }
        public Guid? DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public UserStatus Status { get; set; }
        public DateTime CreatedOn { get; set; }
        public string FullName { get; set; }
        public IList<CourseCertificateIssuedResponseModel> Certificates { get; set; } =
            new List<CourseCertificateIssuedResponseModel>();
        public IList<ExternalCertificateResponseModel> ExternalCertificates { get; set; } =
            new List<ExternalCertificateResponseModel>();
        public List<SkillsUserResponseModel> Skills { get; set; } =
            new List<SkillsUserResponseModel>();

        public string MemberId { get; set; }
        #region Address
        #region Permanent
        public string Address { get; set; }
        #endregion

        #endregion
        #region Identification
        public IdentityType? IdentityType { get; set; }
        public string IdentityNumber { get; set; }
        public string IdentityIssuedBy { get; set; }
        public DateTime? IdentityIssuedOn { get; set; }
        #endregion
        #region Family Details
        #endregion
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
            Status = user.Status;
            CreatedOn = user.CreatedOn;
            FullName = user.FullName;
            DepartmentId = user.DepartmentId;
            DepartmentName = user.Department?.Name;
            MemberId = user.MemberId;
            Skills = user
                .UserSkills?.Select(sk => new SkillsUserResponseModel
                {
                    Id = sk.SkillId,
                    SkillName = sk.Skills?.SkillName
                })
                .ToList();

            #region Address
            #endregion
        }

        public UserResponseModel() { }

        public class SkillsUserResponseModel
        {
            public Guid Id { get; set; }
            public string SkillName { get; set; }

            public SkillsUserResponseModel(Skills model)
            {
                Id = model.Id;
                SkillName = model.SkillName;
            }

            public SkillsUserResponseModel() { }
        }
    }
}
