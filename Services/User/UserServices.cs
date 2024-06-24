using FlightDocsSystem.DataAccess.Data;
using FlightDocsSystem.DataAccess.Repository.IRepository;
using FlightDocsSystem.Models;
using FlightDocsSystem.Models.DTO.User;
using FlightDocsSystem.Models.Response;
using FlightDocsSystem.Services.User.Interfaces;
using FlightDocsSystem.Utilities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace FlightDocsSystem.Services.User
{
	public class UserServices : IUserServices
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly ApplicationDbContext _db;
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly RoleManager<AppRole> _roleManager;
		private ApiResponse<object> _res;
		public UserServices(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager, RoleManager<AppRole> roleManager, ApplicationDbContext db)
		{
			_unitOfWork = unitOfWork;
			_roleManager = roleManager;
			_userManager = userManager;
			_db = db;
			_res = new();
		}

		public async Task<ApiResponse<GetUsersResponseDTO>> GetUsersAsync()
		{
			ApiResponse<GetUsersResponseDTO> res = new();

			var usersInDb = await _unitOfWork.ApplicationUser.GetAll().ToListAsync();

			foreach (var item in usersInDb)
			{
				var role = _userManager.GetRolesAsync(item).GetAwaiter().GetResult().FirstOrDefault();
				if (role != null)
				{
					item.Role = role;
				}
			}

			res.Result.Users = usersInDb;
			return res;
		}

		public async Task<ApiResponse<ApplicationUser>> GetUserAsync(string userId)
		{
			ApiResponse<ApplicationUser> res = new();

			var userInDb = await _unitOfWork.ApplicationUser.Get(x => x.Id.Equals(userId), true).FirstOrDefaultAsync();

			if (userInDb == null)
			{
				res.Errors = new Dictionary<string, List<string>>
							{
								{ "userId", new List<string> { $"Không tìm thấy người dùng" }}
							};
				res.IsSuccess = false;
				return res;
			}

			var role = _userManager.GetRolesAsync(userInDb).GetAwaiter().GetResult().FirstOrDefault();
			if (role != null)
			{
				userInDb.Role = role;
			}

			res.Result = userInDb;
			return res;
		}

		public async Task<ApiResponse<object>> CreateUserAsync(AddUserResquestDTO model)
		{
			var role = await _roleManager.FindByNameAsync(model.RoleName);
			if (role != null && model.PassWord != null)
			{
				var userInDbWithEmail = await _unitOfWork.ApplicationUser.Get(x => x.Email.Equals(model.Email), true).FirstOrDefaultAsync();

				//kiểm tra trùng username
				if (userInDbWithEmail != null)
				{
					_res.Errors = new Dictionary<string, List<string>>
							{
								{ nameof(model.Email), new List<string> { $"Trùng email" }}
							};
					_res.IsSuccess = false;
					return _res;
				}

				var userInDbWithName = await _unitOfWork.ApplicationUser.Get(x => x.UserName.Equals(model.Name), true).FirstOrDefaultAsync();

				//kiểm tra trùng username
				if (userInDbWithEmail != null)
				{
					_res.Errors = new Dictionary<string, List<string>>
							{
								{ nameof(model.Name), new List<string> { $"Trùng tên" }}
							};
					_res.IsSuccess = false;
					return _res;
				}

				var newUser = new ApplicationUser()
				{
					FullName = model.Name,
					Email = model.Email,
					PhoneNumber = model.Phone,
					UserName = model.Name,
				};
				var result = await _userManager.CreateAsync(newUser, model.PassWord);

				if (!result.Succeeded)
				{
					_res.Errors = new Dictionary<string, List<string>>
							{
								{ "error", new List<string> { $"Không thể tạo mới" }}
							};
					_res.IsSuccess = false;
					return _res;
				}

				var user = await _db.ApplicationUsers.FirstOrDefaultAsync(x => x.Email == newUser.Email);
				if (user == null)
				{
					_res.Errors = new Dictionary<string, List<string>>
							{
								{ "error", new List<string> { $"Không tìm thấy người dùng đã tạo" }}
							};
					_res.IsSuccess = false;
					return _res;
				}

				//thêm role cho người dùng
				await _userManager.AddToRoleAsync(user, role.Name);

				_res.Messages = "Tạo mới người dùng thành công";
				return _res;
			}
			else
			{
				_res.Errors = new Dictionary<string, List<string>>
						{
							{ "roleName", new List<string> { $"Vai trò không tồn tại." }}
						};
				_res.IsSuccess = false;
			}
			return _res;
		}

		public async Task<ApiResponse<object>> UpdateUserAsync(string userId, UpdateUserRequestDTO model)
		{
			if (string.IsNullOrEmpty(userId))
			{
				_res.Errors = new Dictionary<string, List<string>>
					{
						{ "userId", new List<string> { $"Không có id người dùng" }}
					};
				_res.IsSuccess = false;
				return _res;
			}

			var role = await _roleManager.FindByNameAsync(model.RoleName);

			if (role == null)
			{
				_res.Errors = new Dictionary<string, List<string>>
							{
								{ nameof(UpdateUserRequestDTO.RoleName), new List<string> { $"Vai trò không tồn tại." }}
							};
				_res.IsSuccess = false;
				return _res;
			}

			var userInDb = await _unitOfWork.ApplicationUser.Get(x => x.Id == userId, true).FirstOrDefaultAsync();
			if (userInDb == null)
			{
				_res.Errors = new Dictionary<string, List<string>>
					{
						{ "userId", new List<string> { $"Không tìm thấy người dùng" }}
					};
				_res.IsSuccess = false;
				return _res;
			}

			var userInDbToCheckEmail = await _unitOfWork.ApplicationUser
				.Get(x => (x.Email.Equals(model.Email)) && x.Id != userInDb.Id, true)
				.FirstOrDefaultAsync();

			//kiểm tra trùng tên
			if (userInDbToCheckEmail != null)
			{
				_res.Errors = new Dictionary<string, List<string>>
							{
								{ "error", new List<string> { $"Email đã tồn tại" }}
							};
				_res.IsSuccess = false;
				return _res;
			}

			userInDb.FullName = model.Name; ;
			userInDb.Email = model.Email;
			userInDb.PhoneNumber = model.Phone;

			_unitOfWork.ApplicationUser.Update(userInDb);
			_unitOfWork.Save();

			// Kiểm tra xem người dùng có role cũ không
			var oldRole = _userManager.GetRolesAsync(userInDb).GetAwaiter().GetResult().FirstOrDefault();

			if (!oldRole.Contains(model.RoleName) || string.IsNullOrEmpty(oldRole))
			{
				// Xóa role cũ
				await _userManager.RemoveFromRoleAsync(userInDb, oldRole);
			}

			// Thêm role mới
			await _userManager.AddToRoleAsync(userInDb, role.Name);

			_res.Messages = "Cập nhật người dùng thành công";
			return _res;
		}

		public async Task<ApiResponse<object>> DeleteUserAsync(string userId)
		{
			if (string.IsNullOrEmpty(userId))
			{
				_res.Errors = new Dictionary<string, List<string>>
					{
						{ "error", new List<string> { $"Không có id người dùng" }}
					};
				_res.IsSuccess = false;
				return _res;
			}

			var userInDb = await _unitOfWork.ApplicationUser.Get(x => x.Id == userId, true).FirstOrDefaultAsync();

			if (userInDb == null)
			{
				_res.Errors = new Dictionary<string, List<string>>
				{
					{ "error", new List<string> { $"Không tìm thấy người dùng" }}
				};
				_res.IsSuccess = false;
				return _res;
			}

			_unitOfWork.ApplicationUser.Remove(userInDb);
			_unitOfWork.Save();

			_res.Messages = "Đã xóa người dùng thành công";
			return _res;
		}

		//role
		public async Task<ApiResponse<GetRolesResponseDTO>> GetRolesAsync()
		{
			ApiResponse<GetRolesResponseDTO> res = new();

			var roles = await _unitOfWork.AppRole.GetAll()
				.Include(x => x.RoleClaimsTypes)
				.Include(x => x.RoleClaimsDocs)
				.ToListAsync();

			res.Result.Roles = roles;
			return res;
		}

		public async Task<ApiResponse<AppRole>> GetRoleAsync(string roleId)
		{
			ApiResponse<AppRole> res = new();

			var role = await _unitOfWork.AppRole.GetAll().FirstOrDefaultAsync();

			if (role == null)
			{
				res.IsSuccess = false;
				res.Errors = new Dictionary<string, List<string>>
				{
					{ "roleId", new List<string> { "Không tìm thấy vai trò" } }
				};
				return res;
			}

			res.Result = role;
			return res;
		}

		public async Task<ApiResponse<object>> CreateRoleAsync(AddOrUpdateRoleRequestDTO model)
		{
			var role = new AppRole { Name = model.RoleName, NormalizedName = model.RoleName.ToLower(), Note = model.Note };
			_unitOfWork.AppRole.Add(role);
			_unitOfWork.Save();

			_res.Messages = "Vai trò đã được tạo thành công";
			return _res;
		}

		public async Task<ApiResponse<object>> UpdateRoleAsync(string roleId, AddOrUpdateRoleRequestDTO model)
		{
			var role = await _unitOfWork.AppRole.Get(x => x.Id == roleId, true).FirstOrDefaultAsync();

			if (role == null)
			{
				_res.IsSuccess = false;
				_res.Errors = new Dictionary<string, List<string>>
				{
					{ "roleId", new List<string> { "Không tìm thấy vai trò" } }
				};
				return _res;
			}

			role.Name = model.RoleName;
			role.NormalizedName = model.RoleName.ToLower();
			role.Note = model.Note;

			_unitOfWork.AppRole.Update(role);
			_unitOfWork.Save();

			_res.Messages = "Vai trò đã được cập nhật thành công";
			return _res;
		}

		public async Task<ApiResponse<object>> DeleteRoleAsync(string roleId)
		{
			var role = await _unitOfWork.AppRole.Get(x => x.Id == roleId, true).FirstOrDefaultAsync();

			if (role == null)
			{
				_res.IsSuccess = false;
				_res.Errors = new Dictionary<string, List<string>>
				{
					{ "roleId", new List<string> { "Không tìm thấy vai trò" } }
				};
				return _res;
			}

			_unitOfWork.AppRole.Remove(role);
			_unitOfWork.Save();

			_res.Messages = "Vai trò đã được xóa thành công";
			return _res;
		}


		//lock and unlock
		public async Task<ApiResponse<object>> LockUserAsync(string userId)
		{
			if (string.IsNullOrEmpty(userId))
			{
				_res.Errors = new Dictionary<string, List<string>>
					{
						{ "error", new List<string> { $"Không có id người dùng" }}
					};
				_res.IsSuccess = false;
				return _res;
			}

			var userInDb = await _unitOfWork.ApplicationUser.Get(x => x.Id == userId, true).FirstOrDefaultAsync();

			if (userInDb == null)
			{
				_res.Errors = new Dictionary<string, List<string>>
				{
					{ "error", new List<string> { $"Không tìm thấy người dùng" }}
				};
				_res.IsSuccess = false;
				return _res;
			}

			userInDb.LockoutEnd = DateTimeOffset.MaxValue;

			_unitOfWork.ApplicationUser.Update(userInDb);
			_unitOfWork.Save();

			_res.Messages = "Đã khóa người dùng";
			return _res;
		}

		public async Task<ApiResponse<object>> LockUsersAsync(LockOrUnlockUsersRequestDTO model)
		{
			if (model.UserIdList.Count == 0 || model.UserIdList == null)
			{
				_res.Errors = new Dictionary<string, List<string>>
					{
						{ nameof(model.UserIdList), new List<string> { $"Chọn người dùng để khóa" }}
					};
				_res.IsSuccess = false;
				return _res;
			}

			foreach (var item in model.UserIdList)
			{
				var userInDb = await _unitOfWork.ApplicationUser.Get(x => x.Id == item, true).FirstOrDefaultAsync();

				if (userInDb == null)
				{
					_res.Errors = new Dictionary<string, List<string>>
					{
						{ "error", new List<string> { $"Không tìm thấy người dùng" }}
					};
					_res.IsSuccess = false;
					return _res;
				}

				userInDb.LockoutEnd = DateTimeOffset.MaxValue;

				_unitOfWork.ApplicationUser.Update(userInDb);
			}
			_unitOfWork.Save();

			_res.Messages = "Đã khóa các người dùng";
			return _res;
		}

		public async Task<ApiResponse<object>> UnLockUserAsync(string userId)
		{
			if (string.IsNullOrEmpty(userId))
			{
				_res.Errors = new Dictionary<string, List<string>>
					{
						{ "error", new List<string> { $"Không có id người dùng" }}
					};
				_res.IsSuccess = false;
				return _res;
			}

			var userInDb = await _unitOfWork.ApplicationUser.Get(x => x.Id == userId, true).FirstOrDefaultAsync();

			if (userInDb == null)
			{
				_res.Errors = new Dictionary<string, List<string>>
				{
					{ "error", new List<string> { $"Không tìm thấy người dùng" }}
				};
				_res.IsSuccess = false;
				return _res;
			}

			userInDb.LockoutEnd = DateTimeOffset.UtcNow;

			_unitOfWork.ApplicationUser.Update(userInDb);
			_unitOfWork.Save();

			_res.Messages = "Đã mở khóa người dùng";
			return _res;
		}

		public async Task<ApiResponse<object>> UnLockUsersAsync(LockOrUnlockUsersRequestDTO model)
		{
			if (model.UserIdList.Count == 0 || model.UserIdList == null)
			{
				_res.Errors = new Dictionary<string, List<string>>
					{
						{ nameof(model.UserIdList), new List<string> { $"Chọn người dùng để khóa" }}
					};
				_res.IsSuccess = false;
				return _res;
			}

			foreach (var item in model.UserIdList)
			{
				var userInDb = await _unitOfWork.ApplicationUser.Get(x => x.Id == item, true).FirstOrDefaultAsync();

				if (userInDb == null)
				{
					_res.Errors = new Dictionary<string, List<string>>
					{
						{ "error", new List<string> { $"Không tìm thấy người dùng" }}
					};
					_res.IsSuccess = false;
					return _res;
				}

				userInDb.LockoutEnd = DateTimeOffset.UtcNow;

				_unitOfWork.ApplicationUser.Update(userInDb);
			}
			_unitOfWork.Save();

			_res.Messages = "Đã mở khóa các người dùng";
			return _res;
		}

		//set owner
		public async Task<ApiResponse<object>> SetOwnerAsync(SetOwnerRequestDTO model)
		{
			// láy ra user
			var userInDb = await _unitOfWork.ApplicationUser
				.Get(x => x.Id.Equals(model.UserId), true)
				.FirstOrDefaultAsync();

			if (userInDb == null)
			{
				_res.Errors = new Dictionary<string, List<string>>
					{
						{ nameof(model.UserId), new List<string> { $"Không tìm thấy người dùng" }}
					};
				_res.IsSuccess = false;
				return _res;
			}

			// kiểm tra mật khẩu tài khoản admin
			var adminInDb = await _unitOfWork.ApplicationUser
				.Get(x => x.Id.Equals(model.UserIdOfAdmin), true)
				.FirstOrDefaultAsync();

			if (adminInDb == null)
			{
				_res.Errors = new Dictionary<string, List<string>>
					{
						{ nameof(model.UserIdOfAdmin), new List<string> { $"Không tìm thấy admin" }}
					};
				_res.IsSuccess = false;
				return _res;
			}

			var result = _userManager.CheckPasswordAsync(adminInDb, model.Password).GetAwaiter().GetResult();

			if (!result)
			{
				_res.Errors = new Dictionary<string, List<string>>
					{
						{ nameof(model.Password), new List<string> { $"Sai mật khẩu" }}
					};
				_res.IsSuccess = false;
				return _res;
			}

			// Lấy thông tin owner hiện tại
			var ownerInDb = await _unitOfWork.ApplicationUser
				.Get(x => x.IsOwner == true, true)
				.FirstOrDefaultAsync();

			if (ownerInDb != null)
			{
				// Chuyển vai trò của owner hiện tại thành nhân viên
				ownerInDb.IsOwner = false;
				ownerInDb.IsEmployee = true;
				_unitOfWork.ApplicationUser.Update(ownerInDb);

				// Xóa vai trò owner cũ và thêm vai trò mới
				await _userManager.RemoveFromRoleAsync(ownerInDb, SD.Role_Owner);
				await _userManager.AddToRoleAsync(ownerInDb, SD.Role_Crew);
			}
			//lưu thay đổi
			_unitOfWork.Save();

			// cập nhật owner mới
			userInDb.IsOwner = true;
			userInDb.IsAdmin = false;
			userInDb.IsEmployee = false;
			_unitOfWork.ApplicationUser.Update(userInDb);
			// lấy danh sách role của người dùng
			var roles = await _userManager.GetRolesAsync(userInDb);
			// Xóa vai trò cũ
			await _userManager.RemoveFromRoleAsync(userInDb, roles.FirstOrDefault());
			// Thay đổi vai trò thành Owner
			await _userManager.AddToRoleAsync(userInDb, SD.Role_Owner);

			//lưu thay đổi
			_unitOfWork.Save();

			_res.Messages = "Cập nhật owner thành công";
			return _res;
		}
	}
}
