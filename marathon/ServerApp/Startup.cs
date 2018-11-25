using System;
using System.Text;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using RunTogether.ApplicationServices;

namespace RunTogether
{
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		public void ConfigureServices(IServiceCollection services)
		{
			// Поддержка аутентификации пользователя на основе JWT (JSON Web Token).
			services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
				.AddJwtBearer(options =>
				{
					options.TokenValidationParameters = new TokenValidationParameters
					{
						ValidateIssuer = true,
						ValidateAudience = true,
						ValidateLifetime = true,
						ValidateIssuerSigningKey = true,
						ValidIssuer = Configuration["Jwt:Issuer"],
						ValidAudience = Configuration["Jwt:Issuer"],
						IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Key"]))
					};
				});
			// Поддержка межсайтовых запросов. Без этих сервисов Веб-приложение не сумеет
			// обратиться к серверу приложений через XHR ввиду ограничений безопасности.
			services.AddCors(options =>
			{
				options.AddPolicy("default",
					builder => builder
						.AllowAnyOrigin()
						.AllowAnyMethod()
						.AllowAnyHeader()
				);
			});
			services.AddMvc();
			services.AddEntityFrameworkNpgsql();
			services.AddAutoMapper(typeof(Startup));

			var connectionString = Configuration.GetConnectionString("DefaultConnection");
			var mapperService = services.BuildServiceProvider().GetService<IMapper>();

			// Добавляем "AdminService" в контейнер зависимостей для возможности
			// использования в контроллерах.
			services.AddSingleton<AdminService>(o => new AdminService(connectionString));
			// Добавляем сервисы для взаимодействия с "верхним" слоем контроллеров.
			services.AddSingleton<UserService>(o =>
				new UserService(connectionString, mapperService));
			services.AddSingleton<MarathonService>(o =>
				new MarathonService(connectionString, mapperService));
			services.AddSingleton<AuthService>(o =>
				new AuthService(connectionString, mapperService));
		}

		public void Configure(
			IApplicationBuilder app,
			IHostingEnvironment env,
			ILoggerFactory loggerFactory)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
				loggerFactory.AddConsole(LogLevel.Trace);
			}
			else
				loggerFactory.AddConsole();

			// Добавляем в конвейер обработку межсайтовых запросов.
			app.UseCors("default");
			// Встраиваем аутентификацию в конвейер.
			app.UseAuthentication();
			app.UseMvc();
		}
	}
}
