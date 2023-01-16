using Google.Protobuf.Collections;
using Google.Protobuf.WellKnownTypes;

using Grpc.Core;

namespace ChessOnlineGrpcService.Services
{
	public class ChessAccountService : chess_account.chess_accountBase
	{
		private readonly ILogger<ChessAccountService> _logger;
		public ChessAccountService(ILogger<ChessAccountService> logger)
		{
			_logger = logger;
		}

		public override Task<history_reply> get_history(history_request request, ServerCallContext context)
		{
			_ = TryValidateUser(context);

			var reply = new history_reply();
			var rng = new Random();
			for (int i = request.Offset; i < request.Offset + request.Length; i++)
			{
				int winner = rng.Next(2);

				reply.Games.Add(new saved_game()
				{
					Id = new id_() { Value = i.ToString() },
					Player1 = new account()
					{
						Id = new id_() { Value = i.ToString() },
						Name = $"CoolPlayer{i}"
					},
					OldOpponentElo = rng.Next(100, 600),
					WinnerId = winner.ToString(),
					EloChange = winner == 0 ? -rng.Next(10, 55) : rng.Next(10, 55),
					CreationTime = DateTime.UtcNow.AddDays(i).ToTimestamp()
				});
			}

			return Task.FromResult(reply);
		}

		private static int TryValidateUser(ServerCallContext context)
		{
			var request = context.GetHttpContext().Request;
			//var token = request.Cookies["Token"];
			//string val = request.Headers.First(x => x.Key == "Authorization").Value.ToString().Split()[1];
			string val = request.Headers.First(x => x.Key == "Authorization").Value.ToString();


			var id = JWT.GetNameIdentifierClaim(val);
			if (id.HasValue)
				return id.Value;
			else
				return -1;
		}

		public override Task<history_length_reply> get_history_length(Empty request, ServerCallContext context)
		{
			history_length_reply reply = new history_length_reply();
			reply.Count = 35;
			return Task.FromResult(reply);
		}

		public override Task<account_info> get_info(Empty request, ServerCallContext context)
		{
			var id = TryValidateUser(context);

			using var db = new Models.ChessOnlineContext();
			var user = db.Users.First(x => x.Id == id);

			account_info account_Info = new account_info();

			account_Info.Email = user.Email;
			account_Info.Fullname = user.Fullname;

			return Task.FromResult(account_Info);
		}

		public override Task<stats_reply> get_stats(Empty request, ServerCallContext context)
		{
			int id = TryValidateUser(context);
			if (id == -1)
				return base.get_stats(request, context);

			using var db = new Models.ChessOnlineContext();
			var user = db.Users.First(x => x.Id == id);
			stats_reply reply = new();
			reply.Elo = new stat()
			{
				Value = user.Elo ?? 0,
				AddInfo = "top 1%"
			};

			reply.Wins = new stat()
			{
				Value = 42,
				AddInfo = "most wins"
			};

			return Task.FromResult(reply);
		}

		public override Task<login_reply> login(login_reqiest request, ServerCallContext context)
		{
			Console.WriteLine($"New request from {context.Peer}");

			using var db = new Models.ChessOnlineContext();

			var users = db?.Users?.Where(u => u.Email == request.Email);

			if (users.ToArray().Length < 1)
				return Task.FromResult(new login_reply() { ErrorMessage = "No user with such email." });

			Models.User? user = users.First();



			bool success = hashPassword(user!.Password) == hashPassword(request.Password);


			if (!success)
				return Task.FromResult(new login_reply() { ErrorMessage = "Wrong password!" });

			string jwt = JWT.Create(user.Id);

			return Task.FromResult(new login_reply()
			{
				Jwt = jwt
			});
		}

		public override Task<register_reply> register(register_request request, ServerCallContext context)
		{
			Models.User user = new();
			user.Email = request.Email;
			user.Fullname = request.Name;
			user.Password = request.Password;

			using var db = new Models.ChessOnlineContext();
			db.Users.Add(user);

			register_reply reply = new register_reply();
			reply.Success = true;

			db.SaveChanges();
			return Task.FromResult(reply);
		}

		public override Task<change_info_reply> set_info(change_info_request request, ServerCallContext context)
		{
			int id = TryValidateUser(context);
			using var db = new Models.ChessOnlineContext();
			var user = db.Users.First(x => x.Id == id);

			user.Fullname = request.NewInfo.Fullname.Length != 0 ? request.NewInfo.Fullname : user.Fullname;
			user.Email = request.NewInfo.Email.Length != 0 ? request.NewInfo.Email : user.Email;
			user.Password = request.NewInfo.Password.Length != 0 ? hashPassword(request.NewInfo.Password) : user.Password;

			db.SaveChanges();

			return Task.FromResult(new change_info_reply()
			{
				Success = true,
				CurrentInfo = new account_info()
				{
					Email = user.Email,
					Fullname = user.Fullname
				}
			});
		}

		string hashPassword(string password)
		{
			return password;
		}
	}

}