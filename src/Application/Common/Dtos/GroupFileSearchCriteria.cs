namespace AcademyKit.Application.Common.Dtos
{
    using System.ComponentModel.DataAnnotations;

    public class GroupFileSearchCriteria : BaseSearchCriteria
    {
        [Required]
        public string GroupIdentity { get; set; }
    }
}
