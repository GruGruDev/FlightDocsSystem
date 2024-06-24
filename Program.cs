using FlightDocsSystem.DataAccess.Data;
using FlightDocsSystem.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using System.Text.Json.Serialization;
using System.Text;
using FlightDocsSystem.DataAccess.Repository.IRepository;
using FlightDocsSystem.DataAccess.Repository;
using FlightDocsSystem.Services.Docs.Interface;
using FlightDocsSystem.Services.Docs;
using FlightDocsSystem.DataAccess.DbInitializer;
using FlightDocsSystem.Services.Auth.Innerfaces;
using FlightDocsSystem.Services.Auth;
using FlightDocsSystem.Services.User.Interfaces;
using FlightDocsSystem.Services.User;
using FlightDocsSystem.Services.DocType.Interfaces;
using FlightDocsSystem.Services.DocType;
using FlightDocsSystem.Services.Auth.Interfaces;
using FlightDocsSystem.Utilities;
using FlightDocsSystem.Services.Doc.Interfaces;
using FlightDocsSystem.Services.Doc;

namespace FlightDocsSystem
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			// Add services to the container.
			builder.Services.AddControllers();
			builder.Services.AddControllers().AddJsonOptions(options =>
			{
				options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
			});

			//config swagger jwt token
			builder.Services.AddSwaggerGen(o =>
			{
				o.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
				{
					Description = "JWT Authorization header using Bearer scheme. \r\n\r\n" +
								  "Enter 'Bearer' [space] and then your token in the text input below. \r\n\r\n" +
								  "Example: \"Bearer 123456abcdef\"",
					Name = "Authorization",
					In = ParameterLocation.Header,
					Scheme = JwtBearerDefaults.AuthenticationScheme,
				});
				o.AddSecurityRequirement(new OpenApiSecurityRequirement()
				{
					{
						new OpenApiSecurityScheme
						{
							Reference=new OpenApiReference
							{
								Type=ReferenceType.SecurityScheme,
								Id="Bearer"
							},
							Scheme="oauth2",
							Name="Bearer",
							In=ParameterLocation.Header,
						},
						new List<string>()
					}
				});
				o.MapType<TimeSpan>(() => new OpenApiSchema
				{
					Type = "string",
					Example = new OpenApiString("00:00:00")
				});
			});

			//dbcontext
			builder.Services.AddDbContext<ApplicationDbContext>
				(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

			//identity dbcontext
			builder.Services.AddIdentity<ApplicationUser, AppRole>()
			.AddEntityFrameworkStores<ApplicationDbContext>()
			.AddDefaultTokenProviders();

			//config password
			builder.Services.Configure<IdentityOptions>(ops =>
			{
				ops.Password.RequireDigit = false;
				ops.Password.RequiredLength = 1;
				ops.Password.RequireUppercase = false;
				ops.Password.RequireLowercase = false;
				ops.Password.RequireNonAlphanumeric = false;
			});

			//truyền dữ liệu từ appsetting vào trong lớp MailSettings theo đúng tên các thuộc tính.
			builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));

			//config authentication
			var key = builder.Configuration.GetValue<string>("ApiSettings:SecretKey");
			builder.Services.AddAuthentication(x =>
			{
				x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
				x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
			}).AddJwtBearer(x =>
			{
				x.RequireHttpsMetadata = false;
				x.SaveToken = true;
				x.TokenValidationParameters = new TokenValidationParameters
				{
					ValidateIssuerSigningKey = true,
					IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key)),
					ValidateIssuer = false,
					ValidateAudience = false,
				};
			});

			//builder.Services.AddCors();
			builder.Services.AddCors();

			//register services
			builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
			builder.Services.AddScoped<IDbInitializer, DbInitializer>();
			builder.Services.AddScoped<IFlightService, FlightService>();
			builder.Services.AddScoped<IAuthServices, AuthServices>();
			builder.Services.AddScoped<IUserServices, UserServices>();
			builder.Services.AddScoped<IDocTypeServices, DocTypeServices>();
			builder.Services.AddScoped<IMailService, MailService>();
			builder.Services.AddScoped<IForgotPasswordService, ForgotPasswordService>();
			builder.Services.AddScoped<IDocServices, DocServices>();

			builder.Services.AddEndpointsApiExplorer();
			builder.Services.AddSwaggerGen();

			var app = builder.Build();

			// Configure the HTTP request pipeline.
			if (app.Environment.IsDevelopment())
			{
				app.UseSwagger();
				app.UseSwaggerUI();
			}

			app.UseHttpsRedirection();

			app.UseCors(o => o.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());
			app.UseAuthentication();
			app.UseAuthorization();
			app.MapControllers();

			SeedData();

			app.Run();

			void SeedData()
			{
				using (var scope = app.Services.CreateScope())
				{
					var dbInititalizer = scope.ServiceProvider.GetRequiredService<IDbInitializer>();
					dbInititalizer.Initializer();
				}
			}
		}
	}
}