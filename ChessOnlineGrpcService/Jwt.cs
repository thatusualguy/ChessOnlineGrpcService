using ChessOnlineGrpcService.Models;

using Microsoft.IdentityModel.Tokens;

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ChessOnlineGrpcService
{
	public static class JWT
	{

		private static readonly SecurityKey mySecurityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes("asdv234234^&%&^%&^hjsdfb2%%%"));
		private static readonly string myAudience = "https://chessonline.com";
		private static readonly string myIssuer = "https://chessonline.com";

		static public string Create(int id)
		{
			var tokenHandler = new JwtSecurityTokenHandler();
			var tokenDescriptor = new SecurityTokenDescriptor
			{
				Subject = new ClaimsIdentity(new Claim[]
				{
					new Claim(ClaimTypes.NameIdentifier, id.ToString()),
				}),
				Expires = DateTime.UtcNow.AddDays(1),
				Issuer = myIssuer,
				Audience = myAudience,
				SigningCredentials = new SigningCredentials(mySecurityKey, SecurityAlgorithms.HmacSha256Signature)
			};

			var token = tokenHandler.CreateToken(tokenDescriptor);
			return tokenHandler.WriteToken(token);
		}

		public static bool IsValid(string token)
		{
			var tokenHandler = new JwtSecurityTokenHandler();
			try
			{
				tokenHandler.ValidateToken(token, new TokenValidationParameters
				{
					ValidateIssuerSigningKey = true,
					ValidateIssuer = true,
					ValidateAudience = true,
					ValidIssuer = myIssuer,
					ValidAudience = myAudience,
					IssuerSigningKey = mySecurityKey
				}, out SecurityToken validatedToken);
			}
			catch
			{
				return false;
			}
			return true;
		}

		public static string? GetClaim(string token, string claimType)
		{
			var tokenHandler = new JwtSecurityTokenHandler();
			var securityToken = tokenHandler.ReadToken(token) as JwtSecurityToken;

			var stringClaimValue = securityToken?.Claims.First(claim => claim.Type == claimType).Value;
			return stringClaimValue;
		}

		public static int? GetNameIdentifierClaim(string token)
		{
			var claim = GetClaim(token, "nameid");
			
			if (int.TryParse(claim, out var NameId))
				return NameId;
			else
				return null;
		}
	}
}
