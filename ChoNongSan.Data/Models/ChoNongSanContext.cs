using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace ChoNongSan.Data.Models
{
    public partial class ChoNongSanContext : DbContext
    {
        public ChoNongSanContext()
        {
        }

        public ChoNongSanContext(DbContextOptions<ChoNongSanContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Account> Accounts { get; set; }
        public virtual DbSet<Banner> Banners { get; set; }
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<HistoryMoney> HistoryMoneys { get; set; }
        public virtual DbSet<ImagePost> ImagePosts { get; set; }
        public virtual DbSet<Location> Locations { get; set; }
        public virtual DbSet<Love> Loves { get; set; }
        public virtual DbSet<Post> Posts { get; set; }
        public virtual DbSet<Review> Reviews { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<WeightType> WeightTypes { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Server=tcp:vvtazure.database.windows.net,1433;Initial Catalog=ChoNongSan;Persist Security Info=False;User ID=vothang;Password=Vt251199@;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

            modelBuilder.Entity<Account>(entity =>
            {
                entity.ToTable("Account");

                entity.Property(e => e.AccountId).HasColumnName("AccountID");

                entity.Property(e => e.Avatar).IsUnicode(false);

                entity.Property(e => e.CreateDate).HasColumnType("datetime");

                entity.Property(e => e.Email)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.FullName).HasMaxLength(100);

                entity.Property(e => e.KeySecurity)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.MoneyOfOver).HasColumnType("decimal(18, 0)");

                entity.Property(e => e.Password).IsUnicode(false);

                entity.Property(e => e.PhoneNumber)
                    .HasMaxLength(11)
                    .IsUnicode(false);

                entity.Property(e => e.RolesId).HasColumnName("RolesID");

                entity.Property(e => e.UserName)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.Roles)
                    .WithMany(p => p.Accounts)
                    .HasForeignKey(d => d.RolesId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Account_Roles");
            });

            modelBuilder.Entity<Banner>(entity =>
            {
                entity.ToTable("Banner");

                entity.Property(e => e.BannerId).HasColumnName("BannerID");

                entity.Property(e => e.CreateTime).HasColumnType("date");

                entity.Property(e => e.ImagePath).IsUnicode(false);

                entity.Property(e => e.Topic).IsUnicode(false);
            });

            modelBuilder.Entity<Category>(entity =>
            {
                entity.ToTable("Category");

                entity.Property(e => e.CategoryId).HasColumnName("CategoryID");

                entity.Property(e => e.CateName).HasMaxLength(20);
            });

            modelBuilder.Entity<HistoryMoney>(entity =>
            {
                entity.HasKey(e => e.HisId);

                entity.ToTable("HistoryMoney");

                entity.Property(e => e.HisId)
                    .ValueGeneratedNever()
                    .HasColumnName("HisID");

                entity.Property(e => e.AccountId).HasColumnName("AccountID");

                entity.Property(e => e.NumberMoney).HasColumnType("decimal(18, 0)");

                entity.Property(e => e.Time).HasColumnType("datetime");

                entity.HasOne(d => d.Account)
                    .WithMany(p => p.HistoryMoneys)
                    .HasForeignKey(d => d.AccountId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_HistoryMoney_Account");
            });

            modelBuilder.Entity<ImagePost>(entity =>
            {
                entity.HasKey(e => e.ImageId);

                entity.ToTable("ImagePost");

                entity.Property(e => e.ImageId).HasColumnName("ImageID");

                entity.Property(e => e.ImagePath).IsUnicode(false);

                entity.Property(e => e.PostId).HasColumnName("PostID");

                entity.HasOne(d => d.Post)
                    .WithMany(p => p.ImagePosts)
                    .HasForeignKey(d => d.PostId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ImagePost_Post");
            });

            modelBuilder.Entity<Location>(entity =>
            {
                entity.ToTable("Location");

                entity.Property(e => e.LocationId).HasColumnName("LocationID");

                entity.Property(e => e.Lat)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Lng)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Love>(entity =>
            {
                entity.ToTable("Love");

                entity.Property(e => e.LoveId).HasColumnName("LoveID");

                entity.Property(e => e.AccountId).HasColumnName("AccountID");

                entity.Property(e => e.PostId).HasColumnName("PostID");

                entity.HasOne(d => d.Account)
                    .WithMany(p => p.Loves)
                    .HasForeignKey(d => d.AccountId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Favorite_Account");

                entity.HasOne(d => d.Post)
                    .WithMany(p => p.Loves)
                    .HasForeignKey(d => d.PostId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Favorite_Post");
            });

            modelBuilder.Entity<Post>(entity =>
            {
                entity.ToTable("Post");

                entity.Property(e => e.PostId).HasColumnName("PostID");

                entity.Property(e => e.AccountId).HasColumnName("AccountID");

                entity.Property(e => e.Address).HasMaxLength(100);

                entity.Property(e => e.CategoryId).HasColumnName("CategoryID");

                entity.Property(e => e.Ctv).HasColumnName("CTV");

                entity.Property(e => e.Expiry).HasColumnType("date");

                entity.Property(e => e.LocationId).HasColumnName("LocationID");

                entity.Property(e => e.PhoneNumber)
                    .HasMaxLength(11)
                    .IsUnicode(false);

                entity.Property(e => e.PostTime).HasColumnType("date");

                entity.Property(e => e.Price).HasColumnType("decimal(18, 0)");

                entity.Property(e => e.ProductName).HasMaxLength(50);

                entity.Property(e => e.Quality).HasMaxLength(10);

                entity.Property(e => e.Title).HasMaxLength(50);

                entity.Property(e => e.WeightId).HasColumnName("WeightID");

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.Posts)
                    .HasForeignKey(d => d.CategoryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Post_Category");

                entity.HasOne(d => d.Location)
                    .WithMany(p => p.Posts)
                    .HasForeignKey(d => d.LocationId)
                    .HasConstraintName("FK_Post_Location");

                entity.HasOne(d => d.Weight)
                    .WithMany(p => p.Posts)
                    .HasForeignKey(d => d.WeightId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Post_WeightType");
            });

            modelBuilder.Entity<Review>(entity =>
            {
                entity.HasKey(e => e.ReviewsId);

                entity.Property(e => e.ReviewsId).HasColumnName("ReviewsID");

                entity.Property(e => e.AccountId).HasColumnName("AccountID");

                entity.Property(e => e.PostId).HasColumnName("PostID");

                entity.HasOne(d => d.Account)
                    .WithMany(p => p.Reviews)
                    .HasForeignKey(d => d.AccountId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Reviews_Account");

                entity.HasOne(d => d.Post)
                    .WithMany(p => p.Reviews)
                    .HasForeignKey(d => d.PostId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Reviews_Post");
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.HasKey(e => e.RolesId);

                entity.Property(e => e.RolesId).HasColumnName("RolesID");

                entity.Property(e => e.RolesName).HasMaxLength(50);
            });

            modelBuilder.Entity<WeightType>(entity =>
            {
                entity.HasKey(e => e.WeightId);

                entity.ToTable("WeightType");

                entity.Property(e => e.WeightId).HasColumnName("WeightID");

                entity.Property(e => e.WeightName).HasMaxLength(20);
            });

            //OnModelCreatingPartial(modelBuilder);
        }

        //public partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}