using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FindYourSoulMate.Models.Exception
{
    public class AccountIsNotExistException : SystemException
    {
        public AccountIsNotExistException(string message) : base(message)
        {

        }
    }
}
