using Microsoft.EntityFrameworkCore;

namespace ChessOnlineGrpcService.Models
{
    public partial class chess_onlineContext : DbContext
    {
        public chess_onlineContext()
        {
        }

        public chess_onlineContext(DbContextOptions<chess_onlineContext> options)
            : base(options)
        {
        }

        public virtual DbSet<PlayedGame> PlayedGames { get; set; } = null!;
        public virtual DbSet<User> Users { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseMySql("name=ChessOnline", ServerVersion.Parse("8.0.28-mysql"));
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.UseCollation("utf8mb4_0900_ai_ci")
                .HasCharSet("utf8mb4");

            modelBuilder.Entity<PlayedGame>(entity =>
            {
                entity.ToTable("played_games");

                entity.HasIndex(e => new { e.Player1, e.Player2 }, "player_id_1_idx");

                entity.HasIndex(e => e.Player2, "player_id_2_idx");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.P1Color)
                    .HasColumnType("enum('white','black')")
                    .HasColumnName("p_1_color");

                entity.Property(e => e.Player1).HasColumnName("player_1");

                entity.Property(e => e.Player2).HasColumnName("player_2");

                entity.Property(e => e.TotalTurns).HasColumnName("total_turns");

                entity.Property(e => e.Winner).HasColumnName("winner");

                entity.HasOne(d => d.Player1Navigation)
                    .WithMany(p => p.PlayedGamePlayer1Navigations)
                    .HasForeignKey(d => d.Player1)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("player_id_1");

                entity.HasOne(d => d.Player2Navigation)
                    .WithMany(p => p.PlayedGamePlayer2Navigations)
                    .HasForeignKey(d => d.Player2)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("player_id_2");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("users");

                entity.HasIndex(e => e.Email, "email_UNIQUE")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Elo)
                    .HasColumnName("elo")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Email)
                    .HasMaxLength(45)
                    .HasColumnName("email");

                entity.Property(e => e.Fullname)
                    .HasMaxLength(45)
                    .HasColumnName("fullname");

                entity.Property(e => e.Password)
                    .HasMaxLength(125)
                    .HasColumnName("password");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
