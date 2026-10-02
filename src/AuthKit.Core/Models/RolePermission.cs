namespace AuthKit.Core.Models;

public class RolePermission
{
    public Guid RoleId { get; set; }
    public string Permission { get; set; } = string.Empty;

    public Role Role { get; set; } = null!;
}
