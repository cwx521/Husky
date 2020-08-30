using System;
using System.Collections.Generic;
using System.Linq;
using Husky.Principal;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Husky.GridQuery
{
	public class GridHandlerController : Controller
	{
		public GridHandlerController(IPrincipalUser principal, IHttpContextAccessor httpContextAccessor) {
			_me = principal;
			_http = httpContextAccessor.HttpContext;
		}

		private readonly IPrincipalUser _me;
		private readonly HttpContext _http;

		private readonly CookieOptions _cookieOptions = new CookieOptions {
			Expires = DateTimeOffset.Now.AddYears(10)
		};

		public IActionResult ShowColumn(string dataSourceUrl, string columns) {
			var key = _me.GetGridCookieKey(dataSourceUrl, "Hide");
			var array = columns.Split(',');

			if ( _http.Request.Cookies.TryGetValue(key, out var str) ) {
				var current = string.IsNullOrEmpty(str) ? new List<string>() : str.Split(',').ToList();
				_http.Response.Cookies.Append(key, string.Join(',', current.Except(array).ToArray()), _cookieOptions);
			}
			return new EmptyResult();
		}

		public IActionResult HideColumn(string dataSourceUrl, string columns) {
			var key = _me.GetGridCookieKey(dataSourceUrl, "Hide");
			var array = columns.Split(',');

			_http.Request.Cookies.TryGetValue(key, out var str);
			var current = string.IsNullOrEmpty(str) ? new List<string>() : str.Split(',').ToList();

			_http.Response.Cookies.Append(key, string.Join(',', current.Concat(array).Distinct().ToArray()), _cookieOptions);
			return new EmptyResult();
		}

		public IActionResult ReorderColumn(string dataSourceUrl, string columns) {
			var key = _me.GetGridCookieKey(dataSourceUrl, "Order");
			_http.Response.Cookies.Append(key, columns, _cookieOptions);
			return new EmptyResult();
		}
	}
}
