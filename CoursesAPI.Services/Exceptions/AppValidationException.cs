using System;

namespace CoursesAPI.Services.Exceptions
{
	public class AppValidationException : ApplicationException
	{
		public AppValidationException(string msg)
			: base(msg)
		{
			
		}
	}
}
