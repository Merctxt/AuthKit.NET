using AuthKit.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace AuthKit.Data;

public class AuthDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<Session> Sessions { get; set; }
    public DbSet<RefreshTokenRegistry> RefreshTokenRegistries { get; set; }
    public DbSet<UserTwoFactorSecret> UserTwoFactorSecrets { get; set; }
    public DbSet<LoginAttempt> LoginAttempts { get; set; }

    public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(builder =>
        {
            builder.HasKey(u => u.Id);
            builder.HasIndex(u => u.Email).IsUnique();
        });

        modelBuilder.Entity<Role>(builder =>
        {
            builder.HasKey(r => r.Id);
            builder.Property(r => r.Permissions)
                .HasConversion(
                    v => System.Text.Json.JsonSerializer.Serialize(v, new System.Text.Json.JsonSerializerOptions()),
                    v => System.Text.Json.JsonSerializer.Deserialize<List<string>>(v, new System.Text.Json.JsonSerializerOptions()) ?? new List<string>())
                .Metadata.SetValueComparer(new ValueComparer<List<string>>(
                    (left, right) => left == null ? right == null : right != null && left.SequenceEqual(right),
                    value => value.Aggregate(0, (hash, permission) => HashCode.Combine(hash, permission.GetHashCode())),
                    value => value.ToList()));
        });

        modelBuilder.Entity<Session>(builder =>
        {
            builder.HasKey(s => s.Id);
            builder.HasIndex(s => s.UserId);
            builder.HasOne(s => s.User)
                .WithMany(u => u.Sessions)
                .HasForeignKey(s => s.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<RefreshTokenRegistry>(builder =>
        {
            builder.HasKey(r => r.Jti);
            builder.HasIndex(r => r.SessionId);
            builder.HasIndex(r => r.ParentHash);
            builder.HasIndex(r => r.ChildHash);
            builder.HasOne(r => r.Session)
                .WithMany(s => s.RefreshTokenRegistries)
                .HasForeignKey(r => r.SessionId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<UserTwoFactorSecret>(builder =>
        {
            builder.HasKey(t => t.UserId);
            builder.HasOne(t => t.User)
                .WithMany(u => u.TwoFactorSecrets)
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<LoginAttempt>(builder =>
        {
            builder.HasKey(l => l.Id);
            builder.HasIndex(l => new { l.Identifier, l.AttemptedAt });
            builder.HasOne(l => l.User)
                .WithMany()
                .HasForeignKey(l => l.UserId)
                .OnDelete(DeleteBehavior.SetNull);
        });
    }
}
