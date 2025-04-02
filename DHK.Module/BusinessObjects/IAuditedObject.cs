namespace DHK.Module.BusinessObjects;

public interface IAuditedObject
{
    public ApplicationUser CreatedBy { get; set; }

    public DateTime CreatedOn { get; set; }

    public ApplicationUser UpdatedBy { get; set; }

    public DateTime UpdatedOn { get; set; }
}
