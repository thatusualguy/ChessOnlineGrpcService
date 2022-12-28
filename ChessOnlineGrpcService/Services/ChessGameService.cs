using Google.Protobuf.WellKnownTypes;

using Grpc.Core;

namespace ChessOnlineGrpcService.Services
{
	public class ChessGameService : chess_game.chess_gameBase
	{
		private readonly ILogger<ChessGameService> _logger;
		public ChessGameService(ILogger<ChessGameService> logger)
		{
			_logger = logger;
		}

		public override Task<create_game_result> create_game(game_settings request, ServerCallContext context)
		{
			return base.create_game(request, context);
		}

		public override Task<enter_game_reply> enter_game(enter_game_request request, ServerCallContext context)
		{
			return base.enter_game(request, context);
		}

		public override Task<game_info_reply> get_game_info(Empty request, ServerCallContext context)
		{
			return base.get_game_info(request, context);
		}

		public override Task<waiting_games_reply> get_waiting_games(Empty request, ServerCallContext context)
		{
			waiting_games_reply reply = new();

			for (int i = 0; i < 20; i++)
			{
				reply.Games.Add(new waiting_game()
				{
					Id = new() { Value = i.ToString() },
					ChosedColor = (i % 2 == 0) ? color.White : color.Black,
					CreationTime = DateTime.UtcNow.AddMinutes(-i).ToTimestamp(),
					Creator = new()
					{
						Id = new()
						{
							Value = i.ToString()
						},
						Name = $"CoolPlayer{i}"
					},
					Elo = 200
				});
			}

			return Task.FromResult(reply);
		}

		public override Task<game_reply> play_game(game_request request, ServerCallContext context)
		{
			return base.play_game(request, context);
		}
	}
}
