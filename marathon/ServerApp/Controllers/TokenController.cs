using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using RunTogether.ApplicationServices.Mapping.Dto;

namespace RunTogether.Controllers
{
	public class TokenController : Controller
	{
		private IConfiguration _config;

		public TokenController(IConfiguration config)
		{
			_config = config;
		}

		// Формирует JWT, который представляет собой структуру из трёх секций, разделённых
		// точками и закодированных в Base64.
		//
		// Пример токена:
		// eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1c2VySWQiOiI2IiwiZXhwIjoxNTI4NDU5MzY5LCJpc3MiOiJodHRwOi8vbG9jYWxob3N0OjU2NzgvIiwiYXVkIjoiaHR0cDovL2xvY2FsaG9zdDo1Njc4LyJ9.53jHdFR7E2IPMmwet4Bdp0c7URSk8BQRJxlHgZzBTuU
		//
		// Раскодированный вид:
		// {"alg":"HS256","typ":"JWT"}{"userId":"6","exp":1528459369,"iss":"http://localhost:5678/","aud":"http://localhost:5678/"}縇tT{b2l瀝燻Q䰔'GᜁN倀
		//
		// Элементы токена:
		//   * Заголовок -- метаинформация о токене, его типе и алгоритме шифрования.
		//   * Тело -- содержит основные сведения (claims), которые описывают субъект
		//     взаимодействия (пользователя).
		//   * Подпись -- позволяет удостовериться в целостности первых двух секций.
		//
		// Подробнее: https://auth0.com/blog/securing-asp-dot-net-core-2-applications-with-jwts/
		protected string BuildToken(UserModelDto user)
		{
			var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
			var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
			var claims = new[] {
				new Claim("userId", user.Id.ToString())
			};
			var token = new JwtSecurityToken(
				_config["Jwt:Issuer"],
				_config["Jwt:Issuer"],
				claims,
				expires: DateTime.Now.AddMinutes(
					int.TryParse(_config["Jwt:TokenExpiration"], out var exp) ? exp : 30),
				signingCredentials: creds);

			return new JwtSecurityTokenHandler().WriteToken(token);
		}

		protected int GetUserId()
		{
			var userIdClaim = User.Claims.FirstOrDefault(o => o.Type == "userId")?.Value;
				
			if (!int.TryParse(userIdClaim, out var res))
				throw new Exception("Сессия не содержит данных об идентификаторе пользователя");
			
			return res;
		}
	}
}
