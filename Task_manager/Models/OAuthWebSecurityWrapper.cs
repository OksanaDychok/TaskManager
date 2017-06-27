namespace Task_manager.Models
{
	using System.Collections.Generic;
	using Microsoft.Web.WebPages.OAuth;

	public class OAuthWebSecurityWrapper : IOAuthWebSecurity
	{
		public bool HasLocalAccount(int userId)
		{
			return OAuthWebSecurity.HasLocalAccount(userId);
		}

	}
}