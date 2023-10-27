using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace PetServices.Models
{
    public partial class PetServicesContext : DbContext
    {
        public PetServicesContext()
        {
        }

        public PetServicesContext(DbContextOptions<PetServicesContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Account> Accounts { get; set; } = null!;
        public virtual DbSet<Blog> Blogs { get; set; } = null!;
        public virtual DbSet<Booking> Bookings { get; set; } = null!;
        public virtual DbSet<OrderProductDetail> OrderProductDetails { get; set; } = null!;
        public virtual DbSet<Otp> Otps { get; set; } = null!;
        public virtual DbSet<PartnerInfo> PartnerInfos { get; set; } = null!;
        public virtual DbSet<Payment> Payments { get; set; } = null!;
        public virtual DbSet<PetInfo> PetInfos { get; set; } = null!;
        public virtual DbSet<Product> Products { get; set; } = null!;
        public virtual DbSet<ProductCategory> ProductCategories { get; set; } = null!;
        public virtual DbSet<Role> Roles { get; set; } = null!;
        public virtual DbSet<Room> Rooms { get; set; } = null!;
        public virtual DbSet<RoomCategory> RoomCategories { get; set; } = null!;
        public virtual DbSet<RoomService> RoomServices { get; set; } = null!;
        public virtual DbSet<Service> Services { get; set; } = null!;
        public virtual DbSet<ServiceCategory> ServiceCategories { get; set; } = null!;
        public virtual DbSet<UserInfo> UserInfos { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var conf = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
                if (!optionsBuilder.IsConfigured)
                {
                    optionsBuilder.UseSqlServer(conf.GetConnectionString("DbConnection"));
                }
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Account>(entity =>
            {
                entity.Property(e => e.AccountId).HasColumnName("AccountID");

                entity.Property(e => e.Email)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Otpid).HasColumnName("OTPID");

                entity.Property(e => e.PartnerInfoId).HasColumnName("PartnerInfoID");

                entity.Property(e => e.Password)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.RoleId).HasColumnName("RoleID");

                entity.Property(e => e.UserInfoId).HasColumnName("UserInfoID");

                entity.HasOne(d => d.Otp)
                    .WithMany(p => p.Accounts)
                    .HasForeignKey(d => d.Otpid)
                    .HasConstraintName("FK_Accounts_OTPS");

                entity.HasOne(d => d.PartnerInfo)
                    .WithMany(p => p.Accounts)
                    .HasForeignKey(d => d.PartnerInfoId)
                    .HasConstraintName("FK_Accounts_PartnerInfo");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.Accounts)
                    .HasForeignKey(d => d.RoleId)
                    .HasConstraintName("FK_Accounts_Roles");

                entity.HasOne(d => d.UserInfo)
                    .WithMany(p => p.Accounts)
                    .HasForeignKey(d => d.UserInfoId)
                    .HasConstraintName("FK_Accounts_UserInfo");
            });

            modelBuilder.Entity<Blog>(entity =>
            {
                entity.Property(e => e.BlogId).HasColumnName("BlogID");

                entity.Property(e => e.ImageUrl).HasColumnName("ImageURL");

                entity.Property(e => e.PublisheDate).HasColumnType("date");
            });

            modelBuilder.Entity<Booking>(entity =>
            {
                entity.ToTable("Booking");

                entity.Property(e => e.BookingId).HasColumnName("BookingID");

                entity.Property(e => e.BookingDate).HasColumnType("date");

                entity.Property(e => e.BookingStatus)
                    .HasMaxLength(20)
                    .IsFixedLength();

                entity.Property(e => e.UserInfoId).HasColumnName("UserInfoID");

                entity.HasOne(d => d.UserInfo)
                    .WithMany(p => p.Bookings)
                    .HasForeignKey(d => d.UserInfoId)
                    .HasConstraintName("FK_Booking_UserInfo");
            });

            modelBuilder.Entity<OrderProductDetail>(entity =>
            {
                entity.HasKey(e => new { e.ProductId, e.BookingId });

                entity.ToTable("OrderProductDetail");

                entity.Property(e => e.ProductId).HasColumnName("ProductID");

                entity.Property(e => e.BookingId).HasColumnName("BookingID");

                entity.Property(e => e.Address).HasMaxLength(500);

                entity.Property(e => e.Commune).HasMaxLength(500);

                entity.Property(e => e.District).HasMaxLength(500);

                entity.Property(e => e.Province).HasMaxLength(500);

                entity.HasOne(d => d.Booking)
                    .WithMany(p => p.OrderProductDetails)
                    .HasForeignKey(d => d.BookingId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_OrderProductDetail_Booking");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.OrderProductDetails)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_OrderProductDetail_Product");
            });

            modelBuilder.Entity<Otp>(entity =>
            {
                entity.ToTable("OTPS");

                entity.Property(e => e.Otpid).HasColumnName("OTPID");

                entity.Property(e => e.Code)
                    .HasMaxLength(6)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<PartnerInfo>(entity =>
            {
                entity.ToTable("PartnerInfo");

                entity.Property(e => e.PartnerInfoId).HasColumnName("PartnerInfoID");

                entity.Property(e => e.Address).HasMaxLength(1000);

                entity.Property(e => e.CardNumber)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Commune).HasMaxLength(500);

                entity.Property(e => e.District).HasMaxLength(500);

                entity.Property(e => e.FirstName).HasMaxLength(100);

                entity.Property(e => e.ImageCertificate).IsUnicode(false);

                entity.Property(e => e.ImagePartner).IsUnicode(false);

                entity.Property(e => e.LastName).HasMaxLength(100);

                entity.Property(e => e.Phone)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.Province).HasMaxLength(500);
            });

            modelBuilder.Entity<Payment>(entity =>
            {
                entity.ToTable("Payment");

                entity.Property(e => e.PaymentId).HasColumnName("PaymentID");
            });

            modelBuilder.Entity<PetInfo>(entity =>
            {
                entity.ToTable("PetInfo");

                entity.Property(e => e.PetInfoId).HasColumnName("PetInfoID");

                entity.Property(e => e.Descriptions).IsUnicode(false);

                entity.Property(e => e.ImagePet).IsUnicode(false);

                entity.Property(e => e.PetName).HasMaxLength(500);

                entity.Property(e => e.Species).HasMaxLength(500);

                entity.Property(e => e.UserInfoId).HasColumnName("UserInfoID");

                entity.HasOne(d => d.UserInfo)
                    .WithMany(p => p.PetInfos)
                    .HasForeignKey(d => d.UserInfoId)
                    .HasConstraintName("FK_PetInfo_UserInfo");
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.ToTable("Product");

                entity.Property(e => e.ProductId).HasColumnName("ProductID");

                entity.Property(e => e.CreateDate).HasColumnType("date");

                entity.Property(e => e.Picture).IsUnicode(false);

                entity.Property(e => e.ProCategoriesId).HasColumnName("ProCategoriesID");

                entity.Property(e => e.ProductName).HasMaxLength(500);

                entity.HasOne(d => d.ProCategories)
                    .WithMany(p => p.Products)
                    .HasForeignKey(d => d.ProCategoriesId)
                    .HasConstraintName("FK_Product_ProductCategories");
            });

            modelBuilder.Entity<ProductCategory>(entity =>
            {
                entity.HasKey(e => e.ProCategoriesId);

                entity.Property(e => e.ProCategoriesId).HasColumnName("ProCategoriesID");

                entity.Property(e => e.Desciptions).HasMaxLength(500);

                entity.Property(e => e.Picture).HasMaxLength(500);

                entity.Property(e => e.ProCategoriesName).HasMaxLength(500);
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.Property(e => e.RoleId).HasColumnName("RoleID");

                entity.Property(e => e.RoleName)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Room>(entity =>
            {
                entity.ToTable("Room");

                entity.Property(e => e.RoomId).HasColumnName("RoomID");

                entity.Property(e => e.Picture).IsUnicode(false);

                entity.Property(e => e.RoomCategoriesId).HasColumnName("RoomCategoriesID");

                entity.Property(e => e.RoomName).HasMaxLength(500);

                entity.HasOne(d => d.RoomCategories)
                    .WithMany(p => p.Rooms)
                    .HasForeignKey(d => d.RoomCategoriesId)
                    .HasConstraintName("FK_Room_RoomCategories");

                entity.HasMany(d => d.Bookings)
                    .WithMany(p => p.Rooms)
                    .UsingEntity<Dictionary<string, object>>(
                        "BookingRoomDetail",
                        l => l.HasOne<Booking>().WithMany().HasForeignKey("BookingId").OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_BookingRoomDetail_Booking"),
                        r => r.HasOne<Room>().WithMany().HasForeignKey("RoomId").OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_BookingRoomDetail_Room"),
                        j =>
                        {
                            j.HasKey("RoomId", "BookingId");

                            j.ToTable("BookingRoomDetail");

                            j.IndexerProperty<int>("RoomId").HasColumnName("RoomID");

                            j.IndexerProperty<int>("BookingId").HasColumnName("BookingID");
                        });
            });

            modelBuilder.Entity<RoomCategory>(entity =>
            {
                entity.HasKey(e => e.RoomCategoriesId);

                entity.Property(e => e.RoomCategoriesId).HasColumnName("RoomCategoriesID");

                entity.Property(e => e.Picture).HasMaxLength(500);

                entity.Property(e => e.RoomCategoriesName).HasMaxLength(500);
            });

            modelBuilder.Entity<RoomService>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.RoomId).HasColumnName("RoomID");

                entity.Property(e => e.ServiceId).HasColumnName("ServiceID");
            });

            modelBuilder.Entity<Service>(entity =>
            {
                entity.Property(e => e.ServiceId).HasColumnName("ServiceID");

                entity.Property(e => e.Picture).IsUnicode(false);

                entity.Property(e => e.SerCategoriesId).HasColumnName("SerCategoriesID");

                entity.Property(e => e.ServiceName).HasMaxLength(500);

                entity.HasOne(d => d.SerCategories)
                    .WithMany(p => p.Services)
                    .HasForeignKey(d => d.SerCategoriesId)
                    .HasConstraintName("FK_Services_ServiceCategories");

                entity.HasMany(d => d.Bookings)
                    .WithMany(p => p.Services)
                    .UsingEntity<Dictionary<string, object>>(
                        "BookingServicesDetail",
                        l => l.HasOne<Booking>().WithMany().HasForeignKey("BookingId").OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_BookingServicesDetail_Booking"),
                        r => r.HasOne<Service>().WithMany().HasForeignKey("ServiceId").OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_BookingServicesDetail_Services"),
                        j =>
                        {
                            j.HasKey("ServiceId", "BookingId");

                            j.ToTable("BookingServicesDetail");

                            j.IndexerProperty<int>("ServiceId").HasColumnName("ServiceID");

                            j.IndexerProperty<int>("BookingId").HasColumnName("BookingID");
                        });
            });

            modelBuilder.Entity<ServiceCategory>(entity =>
            {
                entity.HasKey(e => e.SerCategoriesId);

                entity.Property(e => e.SerCategoriesId).HasColumnName("SerCategoriesID");

                entity.Property(e => e.Picture).HasMaxLength(500);

                entity.Property(e => e.SerCategoriesName).HasMaxLength(500);
            });

            modelBuilder.Entity<UserInfo>(entity =>
            {
                entity.ToTable("UserInfo");

                entity.Property(e => e.UserInfoId).HasColumnName("UserInfoID");

                entity.Property(e => e.Address).HasMaxLength(1000);

                entity.Property(e => e.Commune).HasMaxLength(500);

                entity.Property(e => e.District).HasMaxLength(500);

                entity.Property(e => e.FirstName).HasMaxLength(100);

                entity.Property(e => e.ImageUser).IsUnicode(false);

                entity.Property(e => e.LastName).HasMaxLength(100);

                entity.Property(e => e.Phone)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.Province).HasMaxLength(500);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
