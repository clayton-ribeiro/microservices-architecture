namespace Ordering.Domain.Common
{
    public class EntityBase
    {
        public int Id { get; protected set; }
        public string CreateBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedDate { get; set; }
    }
}
