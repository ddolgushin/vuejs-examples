using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RunTogether.ApplicationServices;

namespace RunTogether.Controllers
{
	// Вспомогательный контроллер. Служит для целей сброса данных в БД в начальное
	// состояние. Естественно, в финальной версии ПО его быть не должно.
	[Route("api/[controller]")]
	public class AdminController : Controller
	{
		private AdminService _service;
		private readonly ILogger _logger;

		public AdminController(
			AdminService adminService,
			ILoggerFactory loggerFactory)
		{
			_service = adminService;
			_logger = loggerFactory.CreateLogger(nameof(AdminController));
		}

		[HttpGet("restore")]
		public void Get()
		{
			try
			{
				_service.RestoreData();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Ошибка сброса данных");
			}
		}
	}
}
