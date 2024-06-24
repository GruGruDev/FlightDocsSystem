using MailKit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FlightDocsSystem.DataAccess.Data;
using FlightDocsSystem.Models;
using FlightDocsSystem.Models.DTO.Auth;
using FlightDocsSystem.Models.Response;
using FlightDocsSystem.Services.Auth.Interfaces;
using FlightDocsSystem.Utilities;

namespace FlightDocsSystem.Services.Auth
{
	public class ForgotPasswordService : ControllerBase, IForgotPasswordService
	{
		private readonly ApplicationDbContext _db;
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly Interfaces.IMailService _mailService; // Đối tượng dịch vụ gửi email
		private ApiResponse<object> _res;

		public ForgotPasswordService(UserManager<ApplicationUser> userManager, Interfaces.IMailService mailService, ApplicationDbContext db)
		{
			_userManager = userManager;
			_mailService = mailService;
			_db = db;
			_res = new();
		}

		public async Task<ApiResponse<object>> ForgotPassword(ForgotPasswordRequestDTO model)
		{
			if (ModelState.IsValid)
			{
				var user = await _db.ApplicationUsers.FirstOrDefaultAsync(x => x.Email == model.Email);

				if (user == null)
				{
					_res.Errors = new Dictionary<string, List<string>>
						{
							{ nameof(ForgotPasswordRequestDTO.Email), new List<string> { $"Email không tồn tại." }}
						};
					_res.IsSuccess = false;
					return _res;
				}

				var token = await _userManager.GeneratePasswordResetTokenAsync(user);

				string pStyle = "style='font-size:20px;'";
				string btnStyle = "style='color: white; background-color: #4CAF50; padding: 10px; border: none; border-radius: 5px; text-align: center; text-decoration: none; display: inline-block; font-size: 16px;'";
				string mailBody = $"<p {pStyle}>Please reset your password by clicking here: <button {btnStyle}>Click</button></p>" +
					$"<p> <strong>UserId</strong>: {user.Id} </p>" +
					$"<p> <strong>Token</strong>: {token} </p>";
				string subject = "Reset Password";

				// Gửi email chứa mã xác nhận
				await _mailService.SendEmailAsync(user.FullName, user.Email, subject, mailBody);

				_res.Messages = "Email đặt lại mật khẩu đã được gửi";
				return _res;
			}

			_res.IsSuccess = false;
			return _res;
		}

		public async Task<ApiResponse<object>> ChangePassword(ChangePasswordRequestDTO model)
		{
			if (ModelState.IsValid)
			{
				var user = await _db.ApplicationUsers.FirstOrDefaultAsync(x => x.Id == model.UserId);

				if (user == null)
				{
					_res.Errors = new Dictionary<string, List<string>>
						{
							{ nameof(ForgotPasswordRequestDTO.Email), new List<string> { $"Không tìm thấy người dùng." }}
						};
					_res.IsSuccess = false;
					return _res;
				}

				var result = await _userManager.ResetPasswordAsync(user, model.ResetToken, model.Password);

				if (result.Succeeded)
				{
					// Hủy token sau khi đổi mật khẩu thành công
					var removeTokenResult = await _userManager.SetAuthenticationTokenAsync(user, "Default", "ResetPassword", null);
					if (!removeTokenResult.Succeeded)
					{
						// Xử lý lỗi nếu không thể hủy token
						_res.Errors = new Dictionary<string, List<string>>
						{
							{ nameof(model.ResetToken), new List<string> { $"Không thể hủy token." }}
						};
						_res.IsSuccess = false;
						return _res;
					}

					// Lưu các thay đổi vào cơ sở dữ liệu
					// Ví dụ: _db.SaveChanges(); hoặc _db.SaveChangesAsync();

					_res.Messages = "Đổi mật khẩu thành công";
					return _res;
				}

				// Xử lý lỗi nếu không thể hủy token
				_res.Errors = new Dictionary<string, List<string>>
						{
							{ nameof(model.ResetToken), new List<string> { $"Token không có hiệu lực." }}
						};
				_res.IsSuccess = false;
				return _res;
			}

			_res.IsSuccess = false;
			return _res;
		}
	}
}
