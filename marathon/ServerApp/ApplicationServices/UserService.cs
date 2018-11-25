using System;
using System.Linq;
using AutoMapper;
using RunTogether.ApplicationServices.Mapping.Dto;
using RunTogether.Exceptions;

namespace RunTogether.ApplicationServices
{
	public class UserService
	{
		private string _connectionString;
		private IMapper _mapper;

		public UserService(string connectionString, IMapper mapper)
		{
			_connectionString = connectionString;
			_mapper = mapper;
		}

		public UserDto Get(int id)
		{
			using (var context = new DatabaseContext(_connectionString))
			{
				var res = context.Users.FirstOrDefault(o => o.Id == id);

				return res != null
					? _mapper.Map<Mapping.Dto.UserDto>(res)
					: null;
			}
		}

		public void Create(UserDto user)
		{
			if (string.IsNullOrEmpty(user.Name))
				throw new Exception("Имя не задано");
			if (string.IsNullOrEmpty(user.Email))
				throw new Exception("Адрес почты не задан");
			if (user.Password != user.PasswordConfirm)
				throw new Exception("Пароли не совпадают");

			using (var context = new DatabaseContext(_connectionString))
			{
				var storedUser = context.Users.FirstOrDefault(o =>
					o.Name.Equals(user.Name, StringComparison.InvariantCultureIgnoreCase));

				if (storedUser != null)
					throw new UserExistsException();

				storedUser = _mapper.Map<Dal.Model.User>(user);
				storedUser.PasswordSalt = AuthService.GetSalt();
				storedUser.PasswordHash = AuthService.GetHash(user.Password + storedUser.PasswordSalt);

				context.Users.Add(storedUser);
				context.SaveChanges();
				user.Clear();

				user.Id = storedUser.Id;
				user.Name = storedUser.Name;
			}
		}
	}
}
