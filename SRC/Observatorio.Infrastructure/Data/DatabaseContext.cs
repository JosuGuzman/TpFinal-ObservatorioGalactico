namespace Observatorio.Infrastructure.Data;

public class DatabaseContext : DbContext
{
    public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
    {
    }

    // Astronomic Entities
    public DbSet<Galaxy> Galaxies { get; set; }
    public DbSet<Star> Stars { get; set; }
    public DbSet<Planet> Planets { get; set; }

    // User Entities
    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }

    // Content Entities
    public DbSet<Article> Articles { get; set; }
    public DbSet<Event> Events { get; set; }
    public DbSet<Discovery> Discoveries { get; set; }

    // System Entities
    public DbSet<SystemLog> SystemLogs { get; set; }
    public DbSet<UserFavorite> UserFavorites { get; set; }
    public DbSet<ExplorationHistory> ExplorationHistories { get; set; }
    public DbSet<SavedSearch> SavedSearches { get; set; }
    public DbSet<DiscoveryVote> DiscoveryVotes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Galaxy configuration
        modelBuilder.Entity<Galaxy>(entity =>
        {
            entity.HasKey(e => e.GalaxyID);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Type).HasConversion<string>();
            entity.Property(e => e.DistanceLy).IsRequired();
            entity.Property(e => e.RA).IsRequired();
            entity.Property(e => e.Dec).IsRequired();
            entity.Property(e => e.Description).HasColumnType("TEXT");
            
            entity.HasIndex(e => e.Name).IsUnique();
            entity.HasIndex(e => e.Type);
            entity.HasIndex(e => e.DistanceLy);
        });

        // Star configuration
        modelBuilder.Entity<Star>(entity =>
        {
            entity.HasKey(e => e.StarID);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.SpectralType).HasConversion<string>();
            entity.Property(e => e.RA).IsRequired();
            entity.Property(e => e.Dec).IsRequired();
            
            entity.HasOne(e => e.Galaxy)
                  .WithMany(g => g.Stars)
                  .HasForeignKey(e => e.GalaxyID)
                  .OnDelete(DeleteBehavior.SetNull);
                  
            entity.HasIndex(e => e.Name);
            entity.HasIndex(e => e.SpectralType);
            entity.HasIndex(e => e.GalaxyID);
        });

        // Planet configuration
        modelBuilder.Entity<Planet>(entity =>
        {
            entity.HasKey(e => e.PlanetID);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.PlanetType).HasConversion<string>();
            
            entity.HasOne(e => e.Star)
                  .WithMany(s => s.Planets)
                  .HasForeignKey(e => e.StarID)
                  .OnDelete(DeleteBehavior.Cascade);
                  
            entity.HasIndex(e => e.Name);
            entity.HasIndex(e => e.PlanetType);
            entity.HasIndex(e => e.StarID);
            entity.HasIndex(e => e.HabitabilityScore);
        });

        // User configuration
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserID);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
            entity.Property(e => e.UserName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.PasswordHash).IsRequired().HasMaxLength(255);
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.IsActive).IsRequired();
            
            entity.HasOne(e => e.Role)
                  .WithMany(r => r.Users)
                  .HasForeignKey(e => e.RoleID)
                  .OnDelete(DeleteBehavior.Restrict);
                  
            entity.HasIndex(e => e.Email).IsUnique();
            entity.HasIndex(e => e.UserName);
            entity.HasIndex(e => e.RoleID);
            entity.HasIndex(e => e.IsActive);
        });

        // Role configuration
        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleID);
            entity.Property(e => e.RoleName).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Description).HasMaxLength(255);
            
            entity.HasIndex(e => e.RoleName).IsUnique();
        });

        // Article configuration
        modelBuilder.Entity<Article>(entity =>
        {
            entity.HasKey(e => e.ArticleID);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(300);
            entity.Property(e => e.Slug).IsRequired().HasMaxLength(300);
            entity.Property(e => e.Content).IsRequired().HasColumnType("LONGTEXT");
            entity.Property(e => e.State).HasConversion<string>();
            
            entity.HasOne(e => e.Author)
                  .WithMany()
                  .HasForeignKey(e => e.AuthorUserID)
                  .OnDelete(DeleteBehavior.Restrict);
                  
            entity.HasIndex(e => e.Slug).IsUnique();
            entity.HasIndex(e => e.Title);
            entity.HasIndex(e => e.AuthorUserID);
            entity.HasIndex(e => e.State);
            entity.HasIndex(e => e.PublishedAt);
        });

        // Event configuration
        modelBuilder.Entity<Event>(entity =>
        {
            entity.HasKey(e => e.EventID);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(250);
            entity.Property(e => e.Type).HasConversion<string>();
            entity.Property(e => e.EventDate).IsRequired();
            entity.Property(e => e.Description).HasColumnType("TEXT");
            
            entity.HasOne(e => e.CreatedBy)
                  .WithMany()
                  .HasForeignKey(e => e.CreatedByUserID)
                  .OnDelete(DeleteBehavior.Restrict);
                  
            entity.HasIndex(e => e.Name);
            entity.HasIndex(e => e.Type);
            entity.HasIndex(e => e.EventDate);
            entity.HasIndex(e => e.CreatedByUserID);
        });

        // Discovery configuration
        modelBuilder.Entity<Discovery>(entity =>
        {
            entity.HasKey(e => e.DiscoveryID);
            entity.Property(e => e.ObjectType).HasConversion<string>();
            entity.Property(e => e.SuggestedName).HasMaxLength(250);
            entity.Property(e => e.RA).IsRequired();
            entity.Property(e => e.Dec).IsRequired();
            entity.Property(e => e.Description).HasColumnType("TEXT");
            entity.Property(e => e.State).HasConversion<string>();
            
            entity.HasOne(e => e.Reporter)
                  .WithMany()
                  .HasForeignKey(e => e.ReporterUserID)
                  .OnDelete(DeleteBehavior.Cascade);
                  
            entity.HasIndex(e => e.ReporterUserID);
            entity.HasIndex(e => e.ObjectType);
            entity.HasIndex(e => e.State);
            entity.HasIndex(e => e.CreatedAt);
        });

        // DiscoveryVote configuration
        modelBuilder.Entity<DiscoveryVote>(entity =>
        {
            entity.HasKey(e => e.VoteID);
            entity.Property(e => e.Vote).IsRequired();
            entity.Property(e => e.Comment).HasMaxLength(500);
            entity.Property(e => e.CreatedAt).IsRequired();
            
            entity.HasOne(e => e.Discovery)
                  .WithMany(d => d.Votes)
                  .HasForeignKey(e => e.DiscoveryID)
                  .OnDelete(DeleteBehavior.Cascade);
                  
            entity.HasOne(e => e.Voter)
                  .WithMany()
                  .HasForeignKey(e => e.VoterUserID)
                  .OnDelete(DeleteBehavior.Cascade);
                  
            entity.HasIndex(e => e.DiscoveryID);
            entity.HasIndex(e => e.VoterUserID);
            entity.HasIndex(e => new { e.DiscoveryID, e.VoterUserID }).IsUnique();
        });

        // SystemLog configuration
        modelBuilder.Entity<SystemLog>(entity =>
        {
            entity.HasKey(e => e.LogID);
            entity.Property(e => e.EventType).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Description).HasColumnType("TEXT");
            entity.Property(e => e.Timestamp).IsRequired();
            entity.Property(e => e.IPAddress).HasMaxLength(45);
            entity.Property(e => e.Status).HasMaxLength(20);
            
            entity.HasOne(e => e.User)
                  .WithMany()
                  .HasForeignKey(e => e.UserID)
                  .OnDelete(DeleteBehavior.SetNull);
                  
            entity.HasIndex(e => e.Timestamp);
            entity.HasIndex(e => e.UserID);
            entity.HasIndex(e => e.EventType);
            entity.HasIndex(e => e.Status);
        });

        // UserFavorite configuration
        modelBuilder.Entity<UserFavorite>(entity =>
        {
            entity.HasKey(e => e.FavoriteID);
            entity.Property(e => e.ObjectType).HasConversion<string>();
            entity.Property(e => e.CreatedAt).IsRequired();
            
            entity.HasOne(e => e.User)
                  .WithMany(u => u.Favorites)
                  .HasForeignKey(e => e.UserID)
                  .OnDelete(DeleteBehavior.Cascade);
                  
            entity.HasIndex(e => e.UserID);
            entity.HasIndex(e => new { e.UserID, e.ObjectType, e.ObjectID }).IsUnique();
        });

        // ExplorationHistory configuration
        modelBuilder.Entity<ExplorationHistory>(entity =>
        {
            entity.HasKey(e => e.HistoryID);
            entity.Property(e => e.ObjectType).HasConversion<string>();
            entity.Property(e => e.AccessedAt).IsRequired();
            
            entity.HasOne(e => e.User)
                  .WithMany(u => u.ExplorationHistory)
                  .HasForeignKey(e => e.UserID)
                  .OnDelete(DeleteBehavior.Cascade);
                  
            entity.HasIndex(e => e.UserID);
            entity.HasIndex(e => e.AccessedAt);
            entity.HasIndex(e => new { e.UserID, e.ObjectType, e.ObjectID });
        });

        // SavedSearch configuration
        modelBuilder.Entity<SavedSearch>(entity =>
        {
            entity.HasKey(e => e.SavedSearchID);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Criteria).HasColumnType("JSON");
            entity.Property(e => e.CreatedAt).IsRequired();
            
            entity.HasOne(e => e.User)
                  .WithMany(u => u.SavedSearches)
                  .HasForeignKey(e => e.UserID)
                  .OnDelete(DeleteBehavior.Cascade);
                  
            entity.HasIndex(e => e.UserID);
        });
    }
}