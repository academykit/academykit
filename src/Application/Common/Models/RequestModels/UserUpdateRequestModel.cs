namespace AcademyKit.Application.Common.Models.RequestModels
{
    using AcademyKit.Domain.Enums;

    public class UserUpdateRequestModel : UserRequestModel
    {
        #region Basic
        public Gender? Gender { get; set; }
        public MaritalStatus? MaritalStatus { get; set; }
        public Nationality? Nationality { get; set; }
        public BloodGroup? BloodGroup { get; set; }
        public DateTime? BirthDateBS { get; set; }
        public DateTime? BirthDateAD { get; set; }
        #endregion
        #region Official Info
        public string EmploymentType { get; set; }
        public Guid? BranchId { get; set; }
        public DateTime? JoinedDateBS { get; set; }
        public DateTime? JoinedDateAD { get; set; }
        #endregion
        #region Address
        #region Permanent Address
        public string PermanentCountry { get; set; }
        public string PermanentState { get; set; }
        public string PermanentDistrict { get; set; }
        public string PermanentCity { get; set; }
        public string PermanentMunicipality { get; set; }
        public string PermanentWard { get; set; }
        #endregion
        #region Current Address
        public bool AddressIsSame { get; set; }
        public string CurrentCountry { get; set; }
        public string CurrentState { get; set; }
        public string CurrentDistrict { get; set; }
        public string CurrentCity { get; set; }
        public string CurrentMunicipality { get; set; }
        public string CurrentWard { get; set; }
        public string CurrentAddress { get; set; }
        #endregion
        #endregion
        #region Contact Details
        public string PersonalEmail { get; set; }
        public string MobileNumberSecondary { get; set; }
        #endregion
        #region Identification
        public IdentityType? IdentityType { get; set; }
        public string IdentityNumber { get; set; }
        public string IdentityIssuedBy { get; set; }
        public DateTime? IdentityIssuedOn { get; set; }
        #endregion
        #region Family Information
        public string FatherName { get; set; }
        public string MotherName { get; set; }
        public string SpouseName { get; set; }
        public string GrandFatherName { get; set; }
        public string MemberPhone { get; set; }
        public string MemberPermanentAddress { get; set; }
        public string MemberCurrentAddress { get; set; }
        public bool FamilyAddressIsSame { get; set; }
        #endregion
    }
}
