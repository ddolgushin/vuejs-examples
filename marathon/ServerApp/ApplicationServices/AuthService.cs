using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using AutoMapper;
using RunTogether.Exceptions;
using RunTogether.ApplicationServices.Mapping.Dto;

namespace RunTogether.ApplicationServices
{
	public class AuthService
	{
		private string _connectionString;
		private IMapper _mapper;

		public AuthService(string connectionString, IMapper mapper)
		{
			_connectionString = connectionString;
			_mapper = mapper;
		}

		public UserModelDto Authenticate(LoginModelDto login)
		{
			using (var context = new DatabaseContext(_connectionString))
			{
				var storedUser = context.Users.FirstOrDefault(o =>
					o.Name.Equals(login.Name, StringComparison.InvariantCultureIgnoreCase));

				if (storedUser == null)
					throw new UserNotFoundException();

				var passwordPair = login.Password + storedUser.PasswordSalt;

				if (storedUser.PasswordHash != GetHash(passwordPair))
					throw new InvalidCredentialsException();
				
				return _mapper.Map<UserModelDto>(storedUser);
			}
		}

		internal static string GetHash(string password)
		{
			using (var sha256 = SHA256.Create())
			{
				var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));

				return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
			}
		}

		internal static string GetSalt()
		{
			var bytes = new byte[16];

			using (var keyGenerator = RandomNumberGenerator.Create())
			{
				keyGenerator.GetBytes(bytes);

				return BitConverter.ToString(bytes).Replace("-", "").ToLower();
			}
		}
	}
}
