using FlightDocsSystem.DataAccess.Repository.IRepository;
using FlightDocsSystem.Models.Response;
using FlightDocsSystem.Models;
using Microsoft.AspNetCore.Identity;
using FlightDocsSystem.Services.Doc.Interfaces;
using FlightDocsSystem.Models.DTO.Doc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace FlightDocsSystem.Services.Doc
{
	public class DocServices : IDocServices
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly RoleManager<AppRole> _roleManager;
		private readonly IWebHostEnvironment _webHost;
		private ApiResponse<object> _res;
		public DocServices(IUnitOfWork unitOfWork, RoleManager<AppRole> roleManager, IWebHostEnvironment webHostEnvironment)
		{
			_unitOfWork = unitOfWork;
			_roleManager = roleManager;
			_res = new();
			_webHost = webHostEnvironment;
		}

		public async Task<ApiResponse<GetDocsResponseDTO>> GetDocs()
		{
			ApiResponse<GetDocsResponseDTO> res = new();

			res.Result.Docs = await _unitOfWork.Doc.GetAll().ToListAsync();
			return res;
		}

		public async Task<ApiResponse<Models.Doc>> GetDoc(int docId)
		{
			ApiResponse<Models.Doc> res = new();

			if (docId == 0)
			{
				res.IsSuccess = false;
				res.Errors = new Dictionary<string, List<string>>
					{
						{"id", new List<string> { $"Không tìm thấy id." } }
					};
				return res;
			}

			var docInDb = await _unitOfWork.Doc.Get(x => x.Id == docId, true)
				.Include(x => x.DocItems)
				.ThenInclude(x => x.User)
				.FirstOrDefaultAsync();

			if (docInDb == null)
			{
				res.IsSuccess = false;
				res.Errors = new Dictionary<string, List<string>>
					{
						{"id", new List<string> { $"Không tìm thấy tài liệu." } }
					};
				return res;
			}

			res.Result = docInDb;
			return res;
		}

		public async Task<ApiResponse<object>> AddDoc(AddOrUpdateDocRequestDTO model)
		{
			// Đọc nội dung của tệp
			byte[] fileBytes;
			using (var memoryStream = new MemoryStream())
			{
				await model.File.CopyToAsync(memoryStream);
				fileBytes = memoryStream.ToArray();
			}

			// Lưu tài liệu vào cơ sở dữ liệu
			var newDoc = new Models.Doc
			{
				Name = model.Name,
				File = fileBytes, // Lưu nội dung của tệp vào trường File
				FileExtension = Path.GetExtension(model.File.FileName),
				Note = model.Note,
				TypeId = model.DocTypeId,
				FlightId = model.FlightId,
			};

			_unitOfWork.Doc.Add(newDoc);
			_unitOfWork.Save();

			// lưu chi tiết tài liệu
			var NewDocItem = new Models.DocItem
			{
				Version = 1.0,
				DocId = newDoc.Id,
				UserId = model.UserId,
			};

			_unitOfWork.DocItem.Add(NewDocItem);
			_unitOfWork.Save();

			// lưu phân quyền
			foreach (var role in model.RoleNameList)
			{
				if (!string.IsNullOrEmpty(role))
				{
					var roleIndb = _roleManager.FindByNameAsync(role).GetAwaiter().GetResult();

					if (role != null)
					{
						// lấy ra roleClaimType của Role và DocType
						var roleClaimTypeInDb = await _unitOfWork.RoleClaimsType
							.Get(x => x.TypeId == model.DocTypeId && x.AppRoleId.Equals(roleIndb.Id), true)
							.FirstOrDefaultAsync();

						// ràng buộc cấp quyền cho role với DocType thì mới được cấp quyền vào tài liệu
						if (roleClaimTypeInDb != null)
						{
							var newRoleClaimDoc = new Models.RoleClaimsDoc
							{
								Type = newDoc.Id.ToString(),
								Value = roleClaimTypeInDb.Value,
								DocsId = newDoc.Id,
								AppRoleId = roleIndb.Id,
							};

							_unitOfWork.RoleClaimsDoc.Add(newRoleClaimDoc);
						}
					}
				}
			}
			_unitOfWork.Save();

			_res.Messages = "Đã thêm tài liệu thành công";
			return _res;
		}

		public async Task<ApiResponse<object>> UpdateDoc(int docId, AddOrUpdateDocRequestDTO model)
		{
			if (docId <= 0)
			{
				_res.Errors = new Dictionary<string, List<string>>
					{
						{"id", new List<string> { $"Sai Id tài liệu." } }
					};
				_res.IsSuccess = false;
				return _res;
			}

			// Đọc nội dung của tệp
			byte[] fileBytes;
			using (var memoryStream = new MemoryStream())
			{
				await model.File.CopyToAsync(memoryStream);
				fileBytes = memoryStream.ToArray();
			}

			var docInDb = await _unitOfWork.Doc.Get(x => x.Id == docId, true).FirstOrDefaultAsync();

			if (docInDb == null)
			{
				_res.Errors = new Dictionary<string, List<string>>
					{
						{nameof(model.Name), new List<string> { $"Không tìm thấy tài liệu." } }
					};
				_res.IsSuccess = false;
				return _res;

			}

			docInDb.Name = model.Name;
			docInDb.File = fileBytes; // Lưu nội dung của tệp vào trường File
			docInDb.FileExtension = Path.GetExtension(model.File.FileName);
			docInDb.Note = model.Note;
			docInDb.TypeId = model.DocTypeId;
			docInDb.FlightId = model.FlightId;

			_unitOfWork.Doc.Update(docInDb);
			_unitOfWork.Save();

			var docItemsInDb = await _unitOfWork.DocItem.Get(x => x.DocId == docId, true).OrderBy(x => x.CreateDate).LastOrDefaultAsync();

			if (docItemsInDb == null)
			{
				_res.Errors = new Dictionary<string, List<string>>
					{
						{nameof(model.Name), new List<string> { $"Không tìm thấy tài liệu chi tiết." } }
					};
				_res.IsSuccess = false;
				return _res;

			}

			// lưu chi tiết tài liệu
			var NewDocItem = new Models.DocItem
			{
				Version = docItemsInDb.Version + 0.1,
				DocId = docInDb.Id,
				UserId = model.UserId,
			};

			_unitOfWork.DocItem.Add(NewDocItem);
			_unitOfWork.Save();

			// xóa tất cả cấp quyền của doc
			var roleClaimDocsInDb = await _unitOfWork.RoleClaimsDoc
								.Get(x => x.DocsId == docInDb.Id, true)
								.ToListAsync();

			if (roleClaimDocsInDb != null)
			{
				_unitOfWork.RoleClaimsDoc.RemoveRange(roleClaimDocsInDb);
				_unitOfWork.Save();
			}

			// phân quyền
			foreach (var role in model.RoleNameList)
			{
				if (!string.IsNullOrEmpty(role))
				{
					var roleIndb = await _roleManager.FindByNameAsync(role);

					if (roleIndb != null)
					{
						// lấy ra roleClaimType của Role và DocType
						var roleClaimTypeInDb = await _unitOfWork.RoleClaimsType
							.Get(x => x.TypeId == model.DocTypeId && x.AppRoleId.Equals(roleIndb.Id), true)
							.FirstOrDefaultAsync();

						// ràng buộc cấp quyền cho role với DocType thì mới được cấp quyền vào tài liệu
						if (roleClaimTypeInDb != null)
						{
							var newRoleClaimDoc = new Models.RoleClaimsDoc
							{
								Type = docInDb.Id.ToString(),
								Value = roleClaimTypeInDb.Value,
								DocsId = docInDb.Id,
								AppRoleId = roleIndb.Id,
							};

							_unitOfWork.RoleClaimsDoc.Add(newRoleClaimDoc);
						}
					}
				}
			}
			_unitOfWork.Save();

			_res.Messages = "Đã cập nhật tài liệu thành công";
			return _res;
		}

		public async Task<ApiResponse<object>> DeleteDoc(int docId)
		{
			if (docId <= 0)
			{
				_res.Errors = new Dictionary<string, List<string>>
					{
						{"id", new List<string> { $"Sai Id tài liệu." } }
					};
				_res.IsSuccess = false;
				return _res;
			}

			var docInDb = await _unitOfWork.Doc.Get(x => x.Id == docId, true).FirstOrDefaultAsync();

			if (docInDb == null)
			{
				_res.Errors = new Dictionary<string, List<string>>
					{
						{"id", new List<string> { $"Không tìm thấy tài liệu." } }
					};
				_res.IsSuccess = false;
				return _res;

			}

			_unitOfWork.Doc.Remove(docInDb);
			_unitOfWork.Save();

			_res.Messages = "Xóa tài liệu thành công";
			return _res;
		}

		public async Task<ApiResponse<Models.Doc>> DownLoadDoc(int docId)
		{
			ApiResponse<Models.Doc> res = new();

			if (docId == 0)
			{
				res.IsSuccess = false;
				res.Errors = new Dictionary<string, List<string>>
				{
					{"id", new List<string> { "Không tìm thấy id." } }
				};
				return res;
			}

			// Lấy thông tin về tài liệu từ cơ sở dữ liệu
			var docInDb = await _unitOfWork.Doc.Get(x => x.Id == docId, true)
				.FirstOrDefaultAsync();

			if (docInDb == null)
			{
				res.IsSuccess = false;
				res.Errors = new Dictionary<string, List<string>>
				{
					{"id", new List<string> { "Không tìm thấy tài liệu." } }
				};
				return res;
			}

			//// Trả về nội dung của tệp dưới dạng FileContentResult
			//var fileContentResult = new FileContentResult(docInDb.File, "application/octet-stream")
			//{
			//	FileDownloadName = $"{docInDb.Name}.{docInDb.FileExtension}" // Đặt tên cho tệp tải xuống
			//};

			res.StatusCode = HttpStatusCode.OK;
			res.Result = docInDb;
			return res;
		}
	}
}
