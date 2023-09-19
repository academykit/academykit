namespace Lingtren.Domain.Entities
{
    using Lingtren.Domain.Common;

    public class Branch : AuditableEntity
    {
        public string Slug { get; set; }
        public string Name { get; set; }
        public string BranchCode { get; set; }
        public string NameNepali { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public string MobileNumber { get; set; }
        public bool IsActive { get; set; }
        public string IpAddress { get; set; }
        public string Remarks { get; set; }
        public string Location { get; set; }
        public string UnderBranch { get; set; }
        public string AreaBranch { get; set; }
        public string RegionalBranch { get; set; }
        public string Province { get; set; }
        public string ValleyType { get; set; }
        // public User OperationIncharge { get; set; }

        // public Guid BranchHeadId { get; set; }
        // public User BranchHead { get; set; }

        // public User User { get; set; }
        public int SolId { get; set; }
        // // public IList<User> Users { get; set; }
        // public IList<User> OperationsIncharge { get; set; }
        // public IList<User> BranchHeads { get; set; }
    }
}
