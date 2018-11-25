using System;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RunTogether.ApplicationServices;
using RunTogether.ApplicationServices.Mapping.Dto;
using RunTogether.Exceptions;

namespace RunTogether.Controllers
{
	[Route("api/[controller]")]
	public class UserController : TokenController
	{
		private readonly UserService _service;
		private readonly IMapper _mapper;
		private readonly ILogger _logger;

		public UserController(
			IConfiguration config,
			UserService service,
			IMapper mapper,
			ILoggerFactory loggerFactory) : base(config)
		{
			_service = service;
			_mapper = mapper;
			_logger = loggerFactory.CreateLogger(nameof(UserController));
		}

		[HttpGet("{id}")]
		public IActionResult Get(int id)
		{
			try
			{
				var res = _mapper.Map<UserModelDto>(_service.Get(id));

				return Json(res);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Ошибка получения данных о пользователе");

				return StatusCode(500, "Ошибка получения данных о пользователе: " + ex.Message);
			}
		}

		[HttpPost]
		public IActionResult Create([FromBody] UserDto user)
		{
			try
			{
				_service.Create(user);

				var userModel = _mapper.Map<UserModelDto>(user);

				userModel.Jwt = BuildToken(userModel);

				return Json(userModel);
			}
			catch (UserExistsException ex)
			{
				_logger.LogError(ex, "Ошибка регистрации");

				return StatusCode(500, "Ошибка регистрации: " + ex.Message);
			}
		}
	}
}
