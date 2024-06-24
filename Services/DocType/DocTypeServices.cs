using FlightDocsSystem.DataAccess.Repository.IRepository;
using FlightDocsSystem.Models;
using FlightDocsSystem.Models.DTO.DocType;
using FlightDocsSystem.Models.Response;
using FlightDocsSystem.Services.DocType.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System;

namespace FlightDocsSystem.Services.DocType
{
	public class DocTypeServices : IDocTypeServices
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly RoleManager<AppRole> _roleManager;
		private ApiResponse<object> _res;
		public DocTypeServices(IUnitOfWork unitOfWork, RoleManager<AppRole> roleManager)
		{
			_unitOfWork = unitOfWork;
			_roleManager = roleManager;
			_res = new();
		}

		public async Task<ApiResponse<GetDocTypesResponseDTO>> GetDocTypes()
		{
			ApiResponse<GetDocTypesResponseDTO> res = new();

			res.Result.DocTypes = await _unitOfWork.DocType
				.GetAll()
				.Include(x => x.RoleClaimsTypes)
				.ToListAsync();
			return res;
		}

		public async Task<ApiResponse<Models.DocType>> GetDocType(int docTypeId)
		{
			ApiResponse<Models.DocType> res = new();
			if (docTypeId == 0)
			{
				res.IsSuccess = false;
				res.Errors = new Dictionary<string, List<string>>
					{
						{"id", new List<string> { $"Không tìm thấy id." } }
					};
				return res;
			}

			var docTypeInDb = await _unitOfWork.DocType
				.Get(x => x.Id == docTypeId, true)
				.FirstOrDefaultAsync();

			if (docTypeInDb == null)
			{
				res.IsSuccess = false;
				res.Errors = new Dictionary<string, List<string>>
					{
						{"id", new List<string> { $"Không tìm thấy loại tài liệu." } }
					};
				return res;
			}

			res.Result = docTypeInDb;
			return res;
		}

		public async Task<ApiResponse<object>> AddDocType(AddOrUpdateDocTypeRequestDTO model)
		{
			Models.DocType newDocType = new()
			{
				Name = model.Name,
				Note = model.Note ?? string.Empty,
			};

			_unitOfWork.DocType.Add(newDocType);
			_unitOfWork.Save();

			foreach (var item in model.RoleAndRoleClaimList)
			{
				var role = _roleManager.FindByNameAsync(item.RoleName).GetAwaiter().GetResult();

				if (role != null)
				{
					RoleClaimsType nenwRoleClaimsType = new()
					{
						Type = newDocType.Id.ToString(),
						Value = item.value,
						TypeId = newDocType.Id,
						AppRoleId = role.Id,
					};
					_unitOfWork.RoleClaimsType.Add(nenwRoleClaimsType);
				}
			}
			_unitOfWork.Save();

			_res.Messages = "Đã thêm loại thành công";
			return _res;
		}

		public async Task<ApiResponse<object>> UpdateDocType(int docTypeId, AddOrUpdateDocTypeRequestDTO model)
		{
			if (docTypeId == 0)
			{
				_res.IsSuccess = false;
				_res.Errors = new Dictionary<string, List<string>>
					{
						{"id", new List<string> { $"Không tìm thấy id." } }
					};
				return _res;
			}

			var docTypeInDb = await _unitOfWork.DocType
				.Get(x => x.Id == docTypeId, true)
				.FirstOrDefaultAsync();

			if (docTypeInDb == null)
			{
				_res.IsSuccess = false;
				_res.Errors = new Dictionary<string, List<string>>
					{
						{"id", new List<string> { $"Không tìm thấy loại tài liệu." } }
					};
				return _res;
			}

			docTypeInDb.Name = model.Name;
			docTypeInDb.Note = model.Note ?? string.Empty;

			_unitOfWork.DocType.Update(docTypeInDb);
			_unitOfWork.Save();

			foreach (var item in model.RoleAndRoleClaimList)
			{
				var role = _roleManager.FindByNameAsync(item.RoleName).GetAwaiter().GetResult();

				if (role != null)
				{
					var roleClaimsTypeInDb = await _unitOfWork.RoleClaimsType
						.Get(x => x.TypeId == docTypeInDb.Id && x.AppRoleId == role.Id, true)
						.FirstOrDefaultAsync();

					if (roleClaimsTypeInDb != null)
					{
						// đã có claim, cập nhật lại
						roleClaimsTypeInDb.Type = docTypeInDb.Id.ToString();
						roleClaimsTypeInDb.Value = item.value;

						_unitOfWork.RoleClaimsType.Update(roleClaimsTypeInDb);
					}
					else
					{
						//chưa có claim, thêm mới
						RoleClaimsType nenwRoleClaimsType = new()
						{
							Type = docTypeInDb.Id.ToString(),
							Value = item.value,
							TypeId = docTypeInDb.Id,
							AppRoleId = role.Id,
						};
						_unitOfWork.RoleClaimsType.Add(nenwRoleClaimsType);
					}
				}
			}
			_unitOfWork.Save();

			_res.Messages = "Đã cập nhật loại thành công";
			return _res;
		}

		public async Task<ApiResponse<object>> DeleteDocType(int docTypeId)
		{
			if (docTypeId == 0)
			{
				_res.IsSuccess = false;
				_res.Errors = new Dictionary<string, List<string>>
					{
						{"id", new List<string> { $"Không tìm thấy id." } }
					};
				return _res;
			}

			var docTypeInDb = await _unitOfWork.DocType
				.Get(x => x.Id == docTypeId, true)
				.FirstOrDefaultAsync();

			if (docTypeInDb == null)
			{
				_res.IsSuccess = false;
				_res.Errors = new Dictionary<string, List<string>>
					{
						{"id", new List<string> { $"Không tìm thấy loại tài liệu." } }
					};
				return _res;
			}

			_unitOfWork.DocType.Remove(docTypeInDb);
			_unitOfWork.Save();

			_res.Messages = "Đã xóa loại tài liệu thành công";
			return _res;
		}
	}
}
