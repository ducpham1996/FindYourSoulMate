using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FindYourSoulMate.Models.Exception
{
    public class InvalidUserNameAndPasswordException : SystemException
    {
        public InvalidUserNameAndPasswordException(string message) : base(message)
        {

        }
    }
}
