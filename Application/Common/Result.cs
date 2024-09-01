using Azure.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common
{
    public class Result
    {
        public bool IsSuccess {  get; private set; }
        public string ErrorMessage { get; private set; }

        private Result(bool isSucess , string errorMessage)
        {
            IsSuccess = isSucess;
           ErrorMessage = errorMessage ;
        }
        public static Result Success() => new Result(true, null);
        
        public static Result Failure (string errorMessage) => new Result(false, errorMessage);


    }
}
