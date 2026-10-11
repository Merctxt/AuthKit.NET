namespace AuthKit.Core.Models;

public class Role
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }

    public ICollection<User> Users { get; set; } = new List<User>();
    public List<string> Permissions { get; set; } = new();
}
