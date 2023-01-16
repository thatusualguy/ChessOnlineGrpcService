using Microsoft.EntityFrameworkCore;

namespace ChessOnlineGrpcService.Models
{
    public partial class ChessOnlineContext : DbContext
    {
        public ChessOnlineContext()
        {
        }

        public ChessOnlineContext(DbContextOptions<ChessOnlineContext> options)
            : base(options)
        {
        }

        public virtual DbSet<PlayedGame> PlayedGames { get; set; } = null!;
        public virtual DbSet<User> Users { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
				var mem = new ConfigurationBuilder()
						.SetBasePath(Directory.GetCurrentDirectory())
						.AddJsonFile("appSettings.json", optional: true, reloadOnChange: true);
				IConfiguration _configuration = mem.Build();
				var connection = _configuration.GetConnectionString("Postgres");
			
				optionsBuilder.UseNpgsql(connection);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PlayedGame>(entity =>
            {
                entity.ToTable("played_games");

                entity.HasIndex(e => new { e.Player1, e.Player2 }, "player_id_1_idx");

                entity.HasIndex(e => e.Player2, "player_id_2_idx");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasDefaultValueSql("nextval('played_games_seq'::regclass)");

                entity.Property(e => e.P1Color).HasColumnName("p_1_color");

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

                entity.HasIndex(e => e.Email, "email_unique")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasDefaultValueSql("nextval('users_seq'::regclass)");

                entity.Property(e => e.Elo)
                    .HasColumnName("elo")
                    .HasDefaultValueSql("0");

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

            modelBuilder.HasSequence("played_games_seq");

            modelBuilder.HasSequence("users_seq");

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
