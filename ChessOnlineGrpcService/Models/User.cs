using System;
using System.Collections.Generic;

namespace ChessOnlineGrpcService.Models
{
    public partial class User
    {
        public User()
        {
            PlayedGamePlayer1Navigations = new HashSet<PlayedGame>();
            PlayedGamePlayer2Navigations = new HashSet<PlayedGame>();
        }

        public int Id { get; set; }
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string Fullname { get; set; } = null!;
        public int? Elo { get; set; }

        public virtual ICollection<PlayedGame> PlayedGamePlayer1Navigations { get; set; }
        public virtual ICollection<PlayedGame> PlayedGamePlayer2Navigations { get; set; }
    }
}
