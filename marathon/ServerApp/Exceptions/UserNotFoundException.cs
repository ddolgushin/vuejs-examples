using System;

namespace RunTogether.Exceptions
{
	public class UserNotFoundException : Exception
	{
		public UserNotFoundException(string msg) : base(msg) { }

		public UserNotFoundException() { }
	}
}
