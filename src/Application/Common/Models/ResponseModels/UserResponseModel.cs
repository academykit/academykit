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
        public string Bio { get; set; }
        public string ImageUrl { get; set; }
        public string PublicUrls { get; set; }
        public Guid? DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public UserStatus Status { get; set; }
        public DateTime CreatedOn { get; set; }
        public string FullName { get; set; }
        public IList<CourseCertificateIssuedResponseModel> Certificates { get; set; } = new List<CourseCertificateIssuedResponseModel>();
        public IList<ExternalCertificateResponseModel> ExternalCertificates { get; set; } = new List<ExternalCertificateResponseModel>();
        public string MemberId { get; set; }
        public DateTime? DateOfBirthAD { get; set; }
        public DateTime? DateOfBirthBS { get; set; }
        public MaritalStatus? MaritalStatus { get; set; }
        public BloodGroup? BloodGroup { get; set; }
        public Nationality? Nationality { get; set; }
        public string EmploymentType { get; set; }
        public Guid? BranchId { get; set; }
        public string BranchName { get; set; }
        public DateTime? JoinedDateAD { get; set; }
        public DateTime? JoinedDateBS { get; set; }
        #region Address
        #region Permanent
        public string PermanentCountry { get; set; }
        public string PermanentState { get; set; }
        public string PermanentCity { get; set; }
        public string PermanentDistrict { get; set; }
        public string PermanentMunicipality { get; set; }
        public string PermanentWard { get; set; }
        public string Address { get; set; }
        #endregion
        #region Current
        public bool AddressIsSame { get; set; }
        public string CurrentCountry { get; set; }
        public string CurrentState { get; set; }
        public string CurrentCity { get; set; }
        public string CurrentDistrict { get; set; }
        public string CurrentMunicipality { get; set; }
        public string CurrentWard { get; set; }
        public string CurrentAddress { get; set; }
        #endregion
        #endregion
        #region Identification
        public IdentityType? IdentityType { get; set; }
        public string IdentityNumber { get; set; }
        public string IdentityIssuedBy { get; set; }
        public DateTime? IdentityIssuedOn { get; set; }
        #endregion
        #region Family Details
        public string FatherName { get; set; }
        public string MotherName { get; set; }
        public string SpouseName { get; set; }
        public string GrandFatherName { get; set; }
        public string MemberPhone { get; set; }
        public string MemberPermanentAddress { get; set; }
        public string MemberCurrentAddress { get; set; }
        public bool FamilyAddressIsSame { get; set; }
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
            DateOfBirthAD = user.BirthDateAD;
            DateOfBirthBS = user.BirthDateBS;
            MaritalStatus = user.MaritalStatus;
            BloodGroup = user.BloodGroup;
            Nationality = user.Nationality;
            EmploymentType = user.EmploymentType;
            BranchId = user.BranchId;
            BranchName = user.Branch?.Name;
            JoinedDateAD = user.JoinedDateAD;
            JoinedDateBS = user.JoinedDateBS;
            #region Address
            #region Permanent
            PermanentCountry = user.PermanentCountry;
            PermanentState = user.PermanentState;
            PermanentCity = user.PermanentCity;
            PermanentDistrict = user.PermanentDistrict;
            PermanentMunicipality = user.PermanentMunicipality;
            PermanentWard = user.PermanentWard;
            #endregion
            #region Current
            AddressIsSame = user.AddressIsSame;
            CurrentCountry = user.CurrentCountry;
            CurrentState = user.CurrentState;
            CurrentCity = user.CurrentCity;
            CurrentDistrict = user.CurrentDistrict;
            CurrentMunicipality = user.CurrentMunicipality;
            CurrentWard = user.CurrentWard;
            CurrentAddress = user.CurrentAddress;
            #endregion
            #endregion
            #region Identification
            IdentityType = user.IdentityType;
            IdentityNumber = user.IdentityNumber;
            IdentityIssuedBy = user.IdentityIssuedBy;
            IdentityIssuedOn = user.IdentityIssuedOn;
            #endregion
            #region Family Details
            FatherName = user.FatherName;
            MotherName = user.MotherName;
            SpouseName = user.SpouseName;
            GrandFatherName = user.GrandFatherName;
            MemberPhone = user.MemberPhone;
            MemberPermanentAddress = user.MemberPermanentAddress;
            MemberCurrentAddress = user.MemberCurrentAddress;
            FamilyAddressIsSame = user.FamilyAddressIsSame;
            #endregion
        }

        public UserResponseModel()
        {
        }
    }
}
