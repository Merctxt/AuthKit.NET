namespace AuthKit.Core.Models;

public class Tenant
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Domain { get; set; }
    public string Settings { get; set; } = "{}";
    public bool IsActive { get; set; }

    public ICollection<User> Users { get; set; } = new List<User>();
    public ICollection<Role> Roles { get; set; } = new List<Role>();
}
