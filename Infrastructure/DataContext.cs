using Auth_Jwt.Domain;
using Microsoft.EntityFrameworkCore;

namespace Auth_Jwt.Infrastructure;

public class DataContext: DbContext
{
    private readonly IConfiguration _configuration;
    private readonly ILoggerFactory _loggerFactory;

    public DataContext(
        DbContextOptions options,
        IConfiguration configuration,
        ILoggerFactory loggerFactory) : base(options)
    {
        _configuration = configuration;
        _loggerFactory = loggerFactory;
    }

    public DbSet<User> Users { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }

    protected override void OnConfiguring(
        DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite(_configuration.GetConnectionString("DefaultConnection"));
        optionsBuilder.UseLoggerFactory(_loggerFactory);
    }

    public override Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = new())
    {
        UpdateAuditFields();
        return base.SaveChangesAsync(cancellationToken);
    }

    public override int SaveChanges()
    {
        UpdateAuditFields();
        return base.SaveChanges();
    }

    private void UpdateAuditFields()
    {
        // get entries that are being Added or Updated
        var modifiedEntries = ChangeTracker.Entries()
            .Where(x => (x.State == EntityState.Added || x.State == EntityState.Modified));

        foreach (var entry in modifiedEntries)
        {
            var entity = entry.Entity as Entity;
            
            if (entry.State == EntityState.Added)
            {
                entity.CreatedAt = DateTime.Now;
            }

            entity.UpdatedAt = DateTime.Now;
        }
    }

    protected override void OnModelCreating(
        ModelBuilder modelBuilder)
    {
        var navigation = modelBuilder.Entity<User>()
            .Metadata.FindNavigation(nameof(User.RefreshTokens));
        navigation.SetPropertyAccessMode(PropertyAccessMode.Field);

        modelBuilder.Entity<RefreshToken>()
            .HasOne(d => d.User)
            .WithMany(e => e.RefreshTokens)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade); ;

    }
}
