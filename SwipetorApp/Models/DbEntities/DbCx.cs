using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SwipetorApp.Models.Enums;
using Toolbelt.ComponentModel.DataAnnotations;
using WebLibServer.Contexts;
using WebLibServer.SharedLogic.Fx;
using WebLibServer.Videos;
using WebLibServer.WebSys;

namespace SwipetorApp.Models.DbEntities;

public class DbCx : DbContext
{
    private static readonly ILoggerFactory DevLoggerFactory
        = LoggerFactory.Create(builder =>
        {
            builder.ClearProviders().AddFile("/var/log/swipetor/sql-{Date}.log", isJson: true);
        });

    private readonly IConnectionCx _connCx;

    // public DbCx()
    // {
    //     _connCx = null;
    // }

    public DbCx(DbContextOptions options) : base(options)
    {
        _connCx = null;
    }

    public DbCx([CanBeNull] IConnectionCx connCx, DbContextOptions options) :
        base(options)
    {
        _connCx = connCx;
    }

    public DbSet<AuditLog> AuditLogs { get; set; }

    public DbSet<EnglishWord> EnglishWords { get; set; }
    public DbSet<Comment> Comments { get; set; }

    public DbSet<CustomDomain> CustomDomains { get; set; }
    public DbSet<Hub> Hubs { get; set; }
    public DbSet<KeyValue> KeyValues { get; set; }
    public DbSet<LoginRequest> LoginRequests { get; set; }
    public DbSet<LoginAttempt> LoginAttempts { get; set; }
    public DbSet<Location> Locations { get; set; }
    public DbSet<Notif> Notifs { get; set; }
    public DbSet<Photo> Photos { get; set; }
    public DbSet<PmMsg> PmMsgs { get; set; }
    public DbSet<PmPermission> PmPermissions { get; set; }
    public DbSet<PmThread> PmThreads { get; set; }
    public DbSet<PmThreadUser> PmThreadUsers { get; set; }
    public DbSet<Post> Posts { get; set; }
    public DbSet<PostHub> PostHubs { get; set; }
    public DbSet<PostMedia> PostMedias { get; set; }

    public DbSet<PostNotifBatch> PostNotifBatches { get; set; }

    public DbSet<PostView> PostViews { get; set; }
    public DbSet<PushDevice> PushDevices { get; set; }
    public DbSet<FavPost> FavPosts { get; set; }
    public DbSet<RemoteVideoInfo> RemoteVideoInfos { get; set; }
    public DbSet<Sprite> Sprites { get; set; }
    public DbSet<Sub> Subscriptions { get; set; }
    public DbSet<SubPlan> SubPlans { get; set; }
    public DbSet<Video> Videos { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<UserFollow> UserFollows { get; set; }


    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseLazyLoadingProxies().UseSnakeCaseNamingConvention();
        optionsBuilder.ConfigureWarnings(warn => warn.Ignore(CoreEventId.DetachedLazyLoadingWarning));

        if (!AppEnv.IsProduction)
        {
            optionsBuilder.EnableSensitiveDataLogging().UseLoggerFactory(DevLoggerFactory);
            // optionsBuilder.ConfigureWarnings(warnings =>
            //     warnings.Throw(RelationalEventId.MultipleCollectionIncludeWarning));
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        #region Evaluate attributes

        modelBuilder.BuildIndexesFromAnnotations();

        #endregion

        #region Composite Indexes

        modelBuilder.Entity<PostHub>().HasKey(e => new { e.PostId, e.HubId });
        modelBuilder.Entity<PostView>().HasKey(e => new { e.UserId, e.PostId });
        modelBuilder.Entity<RemoteVideoInfo>().HasKey(e => new { e.RefDomain, e.RefId });

        #endregion

        #region Manually set indexes and navigation properties

        modelBuilder.Entity<Notif>().HasOne(n => n.ReceiverUser).WithMany(u => u.Notifs)
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<Notif>().HasOne(n => n.SenderUser).WithMany().OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<PmPermission>().HasKey(e => new { e.ReceiverUserId, e.SenderUserId });

        modelBuilder.Entity<PmMsg>().HasOne(msg => msg.Thread).WithMany(th => th.Msgs).OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<PmThreadUser>()
            .HasMany(tu => tu.Msgs).WithOne(tu => tu.ThreadUser).HasForeignKey(m => m.ThreadUserId)
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<PmThreadUser>().HasOne(tu => tu.FirstUnreadMsg).WithMany()
            .HasForeignKey(tu => tu.FirstUnreadMsgId).OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<PmThreadUser>().HasOne(tu => tu.LastReadMsg).WithMany()
            .HasForeignKey(tu => tu.LastReadMsgId).OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<PmThread>().HasOne(t => t.LastMsg).WithMany()
            .HasForeignKey(t => t.LastMsgId).OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Post>()
            .HasOne(p => p.NotifBatch)
            .WithOne(n => n.Post)
            .HasForeignKey<PostNotifBatch>(n => n.PostId).OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<User>()
            .HasOne(u => u.CustomDomain)
            .WithOne(cd => cd.User)
            .HasForeignKey<CustomDomain>(cd => cd.UserId).OnDelete(DeleteBehavior.Cascade);

        #region User <-> Subscriptions <-> SubscriptionPlans relationships

        modelBuilder.Entity<User>()
            .HasMany(u => u.Subscriptions)
            .WithOne(s => s.User)
            .HasForeignKey(s => s.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Sub>()
            .HasOne(s => s.User)
            .WithMany(u => u.Subscriptions)
            .HasForeignKey(s => s.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Sub>()
            .HasOne(s => s.Plan)
            .WithMany(p => p.Subscriptions)
            .HasForeignKey(s => s.PlanId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<SubPlan>()
            .HasMany(p => p.Subscriptions)
            .WithOne(s => s.Plan)
            .HasForeignKey(s => s.PlanId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<SubPlan>()
            .HasOne(p => p.OwnerUser)
            .WithMany(u => u.OwnedSubscriptionPlans)
            .HasForeignKey(p => p.OwnerUserId)
            .OnDelete(DeleteBehavior.Cascade);

        #endregion

        #endregion

        #region Follows

        // Configure the UserFollow entity
        modelBuilder.Entity<UserFollow>()
            .HasKey(uf => new { uf.FollowerUserId, uf.FollowedUserId });

        modelBuilder.Entity<UserFollow>()
            .HasOne(uf => uf.FollowerUser) // the follower
            .WithMany(u => u.Following) // the user has many following users
            .HasForeignKey(uf => uf.FollowerUserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<UserFollow>()
            .HasOne(uf => uf.FollowedUser) // the user being followed
            .WithMany(u => u.Followers) // the followed user has many followers
            .HasForeignKey(uf => uf.FollowedUserId)
            .OnDelete(DeleteBehavior.Cascade);

        #endregion

        #region Conversions

        var jsonNullIgnoreConf = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };

        modelBuilder.Entity<Video>().Property(e => e.Formats).HasConversion(
            v => JsonConvert.SerializeObject(v, jsonNullIgnoreConf),
            v => JsonConvert.DeserializeObject<List<VideoResolution>>(v, jsonNullIgnoreConf)
        ).Metadata.SetValueComparer(new ValueComparer<List<VideoResolution>>(
            (c1, c2) => c1.SequenceEqual(c2),
            c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
            c => c.ToList()));

        modelBuilder.Entity<PostMedia>().Property(e => e.ClipTimes).HasConversion(
            v => JsonConvert.SerializeObject(v, jsonNullIgnoreConf),
            v => JsonConvert.DeserializeObject<List<List<double>>>(v, jsonNullIgnoreConf)
        ).Metadata.SetValueComparer(new ValueComparer<List<List<double>>>(
            (c1, c2) => c1.SequenceEqual(c2),
            c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
            c => c.ToList()));

        modelBuilder.Entity<SubPlan>()
            .Property(e => e.Currency)
            .HasConversion(
                v => v.ToString(), // Convert Currency to string
                v => new Currency(v) // Convert string to Currency
            );

        modelBuilder.Entity<RemoteVideoInfo>().Property(e => e.Action)
            .HasConversion(v => v.ToString(), v => new RemoteVideoInfoAction(v));

        // Other configurations...

        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<AuditLog>(ConfigureAuditLog);

        #endregion

        #region GIST index

        modelBuilder.Entity<Location>()
            .HasGeneratedTsVectorColumn(
                p => p.SearchVector,
                "english", // Text search config
                p => new { p.Name, p.NameAscii, p.FullName }) // Included properties
            .HasIndex(p => p.SearchVector)
            .HasMethod("GIN"); // Index method on the search vector (GIN or GIST)

        #endregion

        // modelBuilder.Entity<Board>().Property(e => e.Config).HasConversion(
        // 	v => JsonConvert.SerializeObject(v, jsonNullIgnoreConf),
        // 	v => JsonConvert.DeserializeObject<BoardConfigModel>(v, jsonNullIgnoreConf));

        // Search vectors
        // modelBuilder.Entity<Buyer>().HasIndex(p => p.SearchVector).HasMethod("GIN");

        base.OnModelCreating(modelBuilder);
    }

    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        SetEntityIpAddrAndDates();
        return base.SaveChanges(acceptAllChangesOnSuccess);
    }

    public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess,
        CancellationToken cancellationToken = default)
    {
        SetEntityIpAddrAndDates();
        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    /// <summary>
    ///     Sets the IP address on changed entities.
    /// </summary>
    protected virtual void SetEntityIpAddrAndDates()
    {
        if (_connCx == null) return;

        var addedEntities = ChangeTracker.Entries()
            .Where(e => e.State == EntityState.Added)
            .ToList();

        foreach (var entry in addedEntities)
        {
            var prop = entry.Entity.GetType().GetProperty("CreatedIp");
            if (prop != null && prop.PropertyType == typeof(string) && prop.GetValue(entry.Entity) == null)
                prop.SetValue(entry.Entity, _connCx.IpAddress);

            prop = entry.Entity.GetType().GetProperty("CreatedAt");
            if (prop != null && prop.PropertyType == typeof(DateTime) && (prop.GetValue(entry.Entity) == null ||
                                                                          (DateTime)prop.GetValue(entry.Entity) ==
                                                                          DateTime.MinValue))
                prop.SetValue(entry.Entity, DateTime.UtcNow);

            prop = entry.Entity.GetType().GetProperty("BrowserAgent");
            if (prop != null && prop.PropertyType == typeof(string) && prop.GetValue(entry.Entity) == null)
                prop.SetValue(entry.Entity, _connCx.BrowserAgent);
        }

        var modifiedEntities = ChangeTracker.Entries()
            .Where(e => e.State == EntityState.Modified)
            .ToList();

        foreach (var entry in modifiedEntities)
        {
            var prop = entry.Entity.GetType().GetProperty("ModifiedIp");
            if (prop != null && prop.PropertyType == typeof(string)) prop.SetValue(entry.Entity, _connCx.IpAddress);

            prop = entry.Entity.GetType().GetProperty("ModifiedAt");
            if (prop != null && prop.PropertyType == typeof(DateTime)) prop.SetValue(entry.Entity, DateTime.UtcNow);

            prop = entry.Entity.GetType().GetProperty("BrowserAgent");
            if (prop != null && prop.PropertyType == typeof(string)) prop.SetValue(entry.Entity, _connCx.BrowserAgent);
        }
    }

    private void ConfigureAuditLog(EntityTypeBuilder<AuditLog> builder)
    {
        var auditActionConverter = new ValueConverter<AuditAction, string>(
            v => v.ToString(), // Convert from AuditAction to string
            v => (AuditAction)v // Convert from string to AuditAction
        );

        builder.Property(e => e.Action)
            .HasConversion(auditActionConverter);
    }
}