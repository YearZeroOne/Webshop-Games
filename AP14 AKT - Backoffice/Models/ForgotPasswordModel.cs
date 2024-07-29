using Microsoft.Build.Framework;

using System.ComponentModel.DataAnnotations;

namespace AP14_AKT___Backoffice.Models
{
	public class ForgotPasswordModel
	{		
		[EmailAddress]
		public string Email { get; set; }
	}
}
