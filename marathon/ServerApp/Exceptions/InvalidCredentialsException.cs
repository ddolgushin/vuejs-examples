using System;

namespace RunTogether.Exceptions
{
	public class InvalidCredentialsException : Exception
	{
		public InvalidCredentialsException(string msg) : base(msg) { }

		public InvalidCredentialsException() { }
	}
}
