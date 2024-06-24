using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Linq;
using System.Threading.Tasks;
using FlightDocsSystem.Models;
using Microsoft.EntityFrameworkCore;
using FlightDocsSystem.DataAccess.Data;
using System.Security.Claims;
using FlightDocsSystem.DataAccess.Repository.IRepository;
using FlightDocsSystem.Utilities;

namespace FlightDocsSystem.Attributes
{
	[AttributeUsage(AttributeTargets.Method)]
	public class AuthorizeClaimAttribute : Attribute, IAuthorizationFilter
	{
		private readonly string _claimValue;

		public AuthorizeClaimAttribute(string claimValue)
		{
			_claimValue = claimValue;
		}

		public async void OnAuthorization(AuthorizationFilterContext context)
		{
			var user = context.HttpContext.User;

			if (!user.Identity.IsAuthenticated)
			{
				context.Result = new UnauthorizedResult();
				return;
			}

			// lấy roleclaim từ token
			var roleClaim = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

			if (string.IsNullOrEmpty(roleClaim))
			{
				context.Result = new ForbidResult();
				return;
			}

			// kiểm tra có phải owner va admin
			if (!roleClaim.Equals(SD.Role_Owner) && !roleClaim.Equals(SD.Role_Admin))
			{
				// không phải owner
				// Lấy docId từ RouteData
				var docIdString = context.HttpContext.Request.RouteValues["id"]?.ToString();

				// kiểm tra có claim nào có type = docId, value = claimValue
				// nếu có value = Modify thì có toàn quyền
				var docClaimModify = user.Claims.FirstOrDefault(c => c.Type == docIdString && c.Value == SD.Claim_Modify)?.Value;

				if (docClaimModify == null)
				{
					// nếu có value = Read thì chỉ xem
					var docClaimRead = user.Claims.FirstOrDefault(c => c.Type == docIdString && c.Value == _claimValue)?.Value;

					if (docClaimRead == null)
					{
						context.Result = new ForbidResult();
						return;
					}
				}
			}

			// là owner thì có toàn quyền
		}
	}
}
