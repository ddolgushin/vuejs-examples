using System;

namespace RunTogether.Exceptions
{
	public class UserExistsException : Exception
	{
		public UserExistsException(string msg) : base(msg) { }

		public UserExistsException() { }
	}
}
