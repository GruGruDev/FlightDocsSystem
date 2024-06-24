using FlightDocsSystem.DataAccess.Data;
using FlightDocsSystem.DataAccess.Repository.IRepository;
using FlightDocsSystem.Models.Response;
using FlightDocsSystem.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using FlightDocsSystem.Models.DTO.Auth;
using FlightDocsSystem.Services.Auth.Innerfaces;
using Microsoft.EntityFrameworkCore;

namespace FlightDocsSystem.Services.Auth
{
	public class AuthServices : IAuthServices
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly ApplicationDbContext _db;
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly RoleManager<AppRole> _roleManager;
		public ApiResponse<object> _res;
		private string SecretKey;

		public AuthServices(IUnitOfWork unitOfWork,
			ApplicationDbContext db,
			RoleManager<AppRole> roleManager,
			UserManager<ApplicationUser> userManager,
			IConfiguration configuration)
		{
			_unitOfWork = unitOfWork;
			_db = db;
			_roleManager = roleManager;
			_userManager = userManager;
			_res = new();
			SecretKey = configuration.GetValue<string>("ApiSettings:SecretKey");
		}

		public async Task<ApiResponse<object>> Login(LoginRequestDTO loginRequestDTO)
		{
			ApplicationUser user = _db.Users.FirstOrDefault(x => x.UserName.ToLower() == loginRequestDTO.Username.ToLower());

			if (user == null)
			{
				_res.IsSuccess = false;
				_res.StatusCode = HttpStatusCode.NotFound;
				_res.Errors = new Dictionary<string, List<string>>
						{
							{ nameof(LoginRequestDTO.Username), new List<string> { $"Email không tồn tại." }}
						};
				return _res;
			}

			var isValid = await _userManager.CheckPasswordAsync(user, loginRequestDTO.Password);

			if (isValid == false)
			{
				_res.IsSuccess = false;
				_res.StatusCode = HttpStatusCode.BadRequest;
				_res.Errors = new Dictionary<string, List<string>>
						{
							{ nameof(LoginRequestDTO.Password), new List<string> { $"Sai mật khẩu." }}
						};
				_res.Result = new LoginRequestDTO();
				return _res;
			}

			//generate JWT Token
			LoginResponseDTO responseDTO = new()
			{
				Email = user.Email,
				Token = await GenerateToken(user),
			};

			if (responseDTO.Email == null || string.IsNullOrEmpty(responseDTO.Token))
			{
				_res.IsSuccess = false;
				_res.StatusCode = HttpStatusCode.BadRequest;
				_res.Errors = new Dictionary<string, List<string>>
						{
							{ nameof(LoginRequestDTO.Username), new List<string> { $"Không thể đăng nhập." }}
						};
				_res.Result = new LoginRequestDTO();
				return _res;
			}

			_res.StatusCode = HttpStatusCode.OK;
			_res.Result = responseDTO;
			return _res;
		}

		private async Task<string> GenerateToken(ApplicationUser user)
		{
			// lấy danh sách role của người dùng
			var roles = await _userManager.GetRolesAsync(user);

			JwtSecurityTokenHandler tokenHandler = new();
			byte[] key = Encoding.ASCII.GetBytes(SecretKey);

			List<Claim> claims = new List<Claim>
			{
				new Claim("fullName",user.FullName),
				new Claim("id",user.Id.ToString()),
				new Claim(ClaimTypes.Email,user.UserName),
				new Claim(ClaimTypes.Role,roles.FirstOrDefault())
			};

			// lấy RoleClaimsDoc
			var roleClaimsDocs = await _unitOfWork.RoleClaimsDoc.Get(x => x.AppRole.Name.Equals(roles.FirstOrDefault()), true).ToListAsync();

			claims.AddRange(roleClaimsDocs.Select(doc => new Claim(doc.Type, doc.Value)));

			//generate JWT Token
			SecurityTokenDescriptor tokenDescriptor = new()
			{
				Subject = new ClaimsIdentity(claims),
				Expires = DateTime.UtcNow.AddDays(7),
				SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
			};

			SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);

			return tokenHandler.WriteToken(token).ToString();
		}
	}
}

