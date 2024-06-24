using FlightDocsSystem.DataAccess.Repository.IRepository;
using FlightDocsSystem.Models;
using FlightDocsSystem.Models.DTO.Flight;
using FlightDocsSystem.Services.Docs.Interface;
using Microsoft.EntityFrameworkCore;
using FlightDocsSystem.Models.Response;

namespace FlightDocsSystem.Services.Docs
{
	public class FlightService : IFlightService
	{
		private readonly IUnitOfWork _unitOfWork;
		private ApiResponse<object> _res;
		public FlightService(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
			_res = new();
		}

		public async Task<ApiResponse<GetFilghtsResponseDTO>> GetFilghts()
		{
			ApiResponse<GetFilghtsResponseDTO> res = new();

			res.Result.Flights = await _unitOfWork.Flight.GetAll().ToListAsync();
			return res;
		}

		public async Task<ApiResponse<Flight>> GetFilght(int flightId)
		{
			ApiResponse<Flight> res = new();

			var flightInDb = await _unitOfWork.Flight.Get(x => x.Id == flightId, true).FirstOrDefaultAsync();

			if (flightInDb == null)
			{
				res.IsSuccess = false;
				return res;
			}

			res.Result = flightInDb;
			return res;
		}

		public async Task<ApiResponse<GetFilghtsResponseDTO>> SearchFilghts(int? flightId, DateTime? date, int? categoryId)
		{
			ApiResponse<GetFilghtsResponseDTO> res = new();

			var flightInDb = await _unitOfWork.Flight.GetAll().ToListAsync();

			var filteredFlights = flightInDb;

			if (flightId > 0 && flightId != null)
			{
				filteredFlights = filteredFlights.Where(x => x.Id == flightId).ToList();
			}

			if (date != null)
			{
				filteredFlights = filteredFlights.Where(x => x.Date == date).ToList();
			}

			res.Result.Flights = filteredFlights;
			return res;
		}

		public async Task<ApiResponse<object>> AddFilght(AddFlightRequestDTO model)
		{
			var flightInDb = await _unitOfWork.Flight.Get(x => x.FlightNo.Equals(model.FlightNo), true).FirstOrDefaultAsync();

			if (flightInDb != null)
			{
				_res.IsSuccess = false;
				_res.Errors = new Dictionary<string, List<string>>
					{
						{nameof(model.FlightNo), new List<string> { $"Trùng mã số chuyến bay." } }
					};
				return _res;
			}

			Flight newFlight = new()
			{
				FlightNo = model.FlightNo,
				Date = model.FlightDay,
				Route = model.Route,
				PointOfLoading = model.PointOfLoading,
				PointOfUnLoading = model.PointOfUnLoading,
			};

			_unitOfWork.Flight.Add(newFlight);
			_unitOfWork.Save();

			_res.Messages = "Đã thêm chuyến bay thành công.";
			return _res;
		}

		public async Task<ApiResponse<object>> UpdateFilght(int flightId, UpdateFilghtRequestDTO model)
		{
			var flightInDbWithId = await _unitOfWork.Flight.Get(x => x.Id == flightId, true).FirstOrDefaultAsync();

			if (flightInDbWithId == null)
			{
				_res.IsSuccess = false;
				_res.Errors = new Dictionary<string, List<string>>
					{
						{ "flightId", new List<string> { $"Không tìm thấy chuyến bay." } }
					};
				return _res;
			}

			var flightsInDbWithNo = await _unitOfWork.Flight.Get(x => x.FlightNo.Equals(model.FlightNo), true).FirstOrDefaultAsync();

			if (flightsInDbWithNo != null && flightsInDbWithNo.Id != flightInDbWithId.Id)
			{
				_res.IsSuccess = false;
				_res.Errors = new Dictionary<string, List<string>>
					{
						{nameof(model.FlightNo), new List<string> { $"Trùng mã số chuyến bay." } }
					};
				return _res;
			}

			flightInDbWithId.FlightNo = model.FlightNo;
			flightInDbWithId.Date = model.FlightDay;
			flightInDbWithId.Route = model.Route;
			flightInDbWithId.PointOfLoading = model.PointOfLoading;
			flightInDbWithId.PointOfUnLoading = model.PointOfUnLoading;

			_unitOfWork.Flight.Update(flightInDbWithId);
			_unitOfWork.Save();

			_res.Messages = "Đã cập nhật chuyến bay thành công.";
			return _res;
		}

		public async Task<ApiResponse<object>> DeleteFilght(int flightId)
		{
			var flightInDbWithId = await _unitOfWork.Flight.Get(x => x.Id == flightId, true).FirstOrDefaultAsync();

			if (flightInDbWithId == null)
			{
				_res.IsSuccess = false;
				_res.Errors = new Dictionary<string, List<string>>
					{
						{ "flightId", new List<string> { $"Không tìm thấy chuyến bay." } }
					};
				return _res;
			}

			_unitOfWork.Flight.Remove(flightInDbWithId);
			_unitOfWork.Save();

			_res.Messages = "Đã xóa chuyến bay thành công.";
			return _res;
		}

	}
}
