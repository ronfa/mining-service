using System.ComponentModel;

namespace MiningService.Core.Model
{
    public class ErrorResponse
    {
        public int Code { get; set; }
        public string Message { get; set; }
        public string TechnicalInfo { get; set; }

        public ErrorResponse(int code, string message, string technicalInfo)
        {
            Code = code;
            Message = message;
            TechnicalInfo = technicalInfo;
        }
    }

    public enum StatusCodes
    {
        [Description("No error")]
        NoError = 0,
        [Description("Job id is missing from request")]
        EmptyFieldJobId = 1,
        [Description("Job could not be found by id")]
        JobNotFound = 2,
        [Description("Internal server error")]
        InternalServerError = 9000
    }

}
