namespace MediaPortal.Data.DataAccess
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using MediaPortal.Data.EntitiesModel;

    public partial class MediaPortalDbContext : DbContext
    {
        public MediaPortalDbContext(string connectionString)
            : base(connectionString)
        {
        }

        public virtual DbSet<AspNetRole> AspNetRoles { get; set; }
        public virtual DbSet<AspNetUserClaim> AspNetUserClaims { get; set; }
        public virtual DbSet<AspNetUserLogin> AspNetUserLogins { get; set; }
        public virtual DbSet<AspNetUser> AspNetUsers { get; set; }
        public virtual DbSet<FileSystem> FileSystems { get; set; }
        public virtual DbSet<SharedAccess> SharedAccesses { get; set; }
        public virtual DbSet<Tag> Tags { get; set; }
        //public virtual DbSet<FileSystemTag> FileSystemTags { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {          
            modelBuilder.Entity<AspNetRole>()
                .HasMany(e => e.AspNetUsers)
                .WithMany(e => e.AspNetRoles)
                .Map(m => m.ToTable("AspNetUserRoles").MapLeftKey("RoleId").MapRightKey("UserId"));

            modelBuilder.Entity<AspNetUser>()
                .HasMany(e => e.AspNetUserClaims)
                .WithRequired(e => e.AspNetUser)
                .HasForeignKey(e => e.UserId);

            modelBuilder.Entity<AspNetUser>()
                .HasMany(e => e.AspNetUserLogins)
                .WithRequired(e => e.AspNetUser)
                .HasForeignKey(e => e.UserId);

            modelBuilder.Entity<AspNetUser>()
                .HasMany(e => e.FileSystems)
                .WithRequired(e => e.AspNetUser)
                .HasForeignKey(e => e.UserId);

            modelBuilder.Entity<AspNetUser>()
                .HasMany(e => e.SharedAccesses)
                .WithOptional(e => e.AspNetUser)
                .HasForeignKey(e => e.UserId);

            modelBuilder.Entity<FileSystem>()
                .HasMany(e => e.FileSystem1)
                .WithOptional(e => e.FileSystem2)
                .HasForeignKey(e => e.ParentId);

            modelBuilder.Entity<FileSystem>()
                .HasMany(e => e.Tags)
                .WithMany(e => e.FileSystems)
                .Map(m => m.ToTable("FileSystemTag").MapLeftKey("FileSystemId").MapRightKey("TagId"));            
        }
    }
}
