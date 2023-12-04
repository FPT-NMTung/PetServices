using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FEPetServices.Models.ErrorResult
{
    public class ErrorResult : ContentResult
    {
        public ErrorResult(string errorMessage)
        {
            Content = errorMessage;
            StatusCode = 400;
        }
    }
}
