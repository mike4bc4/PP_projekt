using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BedAndBreakfast.Models.ServicesLogic
{
	/// <summary>
	/// Container for methods related to session management.
	/// </summary>
	public static class SessionExtensions
	{
		/// <summary>
		/// Serializes object and stores it under defined key.
		/// </summary>
		/// <param name="session"></param>
		/// <param name="key"></param>
		/// <param name="value"></param>
		public static void SetObject(this ISession session, string key, object value) {
			session.SetString(key, JsonConvert.SerializeObject(value));
		}
		/// <summary>
		/// Deserializes object of specified type and key and returns it. May
		/// also return default value for type if value under specified key is null.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="session"></param>
		/// <param name="key"></param>
		/// <returns></returns>
		public static T GetObject<T>(this ISession session, string key) {
			var value = session.GetString(key);
			if (value != null)
			{
				return JsonConvert.DeserializeObject<T>(value);
			}
			else {
				return default(T);
			}
		}
	}
}
