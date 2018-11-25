using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RunTogether.ApplicationServices;
using RunTogether.ApplicationServices.Mapping.Dto;

namespace RunTogether.Controllers
{
	[Route("api/[controller]")]
	public class MarathonController : TokenController
	{
		private readonly MarathonService _service;
		private readonly ILogger _logger;

		public MarathonController(
			IConfiguration config,
			MarathonService service,
			ILoggerFactory loggerFactory) : base(config)
		{
			_service = service;
			_logger = loggerFactory.CreateLogger(nameof(MarathonController));
		}

		[HttpGet]
		public IActionResult Get()
		{
			try
			{
				return Json(_service.Get());
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Ошибка получения данных");

				return StatusCode(500, "Ошибка получения данных: " + ex.Message);
			}
		}

		[HttpGet("{id}")]
		public IActionResult Get(int id)
		{
			try
			{
				return Json(_service.Get(id));
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Ошибка получения данных о марафоне");

				return StatusCode(500, "Ошибка получения данных о марафоне: " + ex.Message);
			}
		}

		[HttpGet("{id}/participants")]
		public IActionResult GetParticipants(int id)
		{
			try
			{
				return Json(_service.GetParticipants(id));
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Ошибка получения списка участников");

				return StatusCode(500, "Ошибка получения списка участников: " + ex.Message);
			}
		}

		[HttpGet, Authorize]
		[Route("assign/{marathonId}")]
		public IActionResult AssignParticipant(int marathonId)
		{
			try
			{
				_service.AssignParticipant(marathonId, GetUserId());

				return Ok();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Ошибка записи на марафон");

				return StatusCode(500, "Ошибка записи на марафон: " + ex.Message);
			}
		}

		[HttpGet, Authorize]
		[Route("decline/{marathonId}")]
		public IActionResult DeclineParticipant(int marathonId)
		{
			try
			{
				_service.DeclineParticipant(marathonId, GetUserId());

				return Ok();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Ошибка отказа от участия");

				return StatusCode(500, "Ошибка отказа от участия: " + ex.Message);
			}
		}

		[HttpPost, Authorize]
		public IActionResult Create()
		{
			try
			{
				// Вычисляем ID пользователя, который хранится в токене и был десериализован
				// в процессе обработки запроса конвейером.
				var userIdClaim = User.Claims.FirstOrDefault(o => o.Type == "userId")?.Value;

				if (string.IsNullOrEmpty(userIdClaim))
					throw new Exception();
				
				return Json(_service.Create(int.Parse(userIdClaim)));
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Ошибка создания объекта");

				return StatusCode(500, "Ошибка создания объекта: " + ex.Message);
			}
		}

		[HttpPut, Authorize]
		public IActionResult Update([FromBody] MarathonDto marathon)
		{
			try
			{
				_service.Update(marathon);

				return Ok();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Ошибка обновления объекта");

				return StatusCode(500, "Ошибка обновления объекта: " + ex.Message);
			}
		}

		[HttpGet("{marathonId}/ownedby/{userId}")]
		public IActionResult OwnedBy(int marathonId, int userId)
		{
			try
			{
				return Json(_service.OwnedBy(marathonId, userId));
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Ошибка получения данных");

				return StatusCode(500, "Ошибка получения данных: " + ex.Message);
			}
		}
	}
}
