using System;
using System.Threading.Tasks;
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
	public class AuthController : TokenController
	{
		private readonly AuthService _service;
		private IMapper _mapper;
		private readonly ILogger _logger;

		public AuthController(
			IConfiguration config,
			AuthService service,
			IMapper mapper,
			ILoggerFactory loggerFactory) : base(config)
		{
			_service = service;
			_mapper = mapper;
			_logger = loggerFactory.CreateLogger(nameof(AuthController));
		}

		[HttpPost]
		public IActionResult Login([FromBody] LoginModelDto login)
		{
			try
			{
				var userModel = _service.Authenticate(login);

				userModel.Jwt = BuildToken(userModel);

				return Json(userModel);
			}
			catch (UserNotFoundException ex)
			{
				_logger.LogError(ex, "Ошибка авторизации");

				return NotFound("Ошибка авторизации: " + ex.Message);
			}
			catch (InvalidCredentialsException ex)
			{
				_logger.LogError(ex, "Ошибка авторизации");

				return Unauthorized();
			}
			catch (Exception ex) {
				_logger.LogError(ex, "Ошибка авторизации");
				
				return StatusCode(500, "Ошибка авторизации: " + ex.Message);
			}
		}
	}
}
