using System;
using System.Collections.Generic;

namespace ChessOnlineGrpcService.Models
{
    public partial class PlayedGame
    {
        public int Id { get; set; }
        public int Player1 { get; set; }
        public int Player2 { get; set; }
        public int Winner { get; set; }
        public int TotalTurns { get; set; }
        public string P1Color { get; set; } = null!;

        public virtual User Player1Navigation { get; set; } = null!;
        public virtual User Player2Navigation { get; set; } = null!;
    }
}
