using Microsoft.AspNetCore.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common
{
    public class GenericResult<T>
    {
        public bool IsSuccess {  get; private set; }
        public T Value { get; private set; }
        public string ErrorMessage {  get; private set; }

        private GenericResult(bool isSucess , T value , string errorMessage)
        {
            IsSuccess = isSucess;
            Value = value; 
            ErrorMessage = errorMessage;
        }
        public static GenericResult<T> Success(T value) => new GenericResult<T>(true, value, null);
        public static GenericResult<T> Failure(string error) => new GenericResult<T>(false, default, error);
    }
}
