using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace Focusync.Middleware.Models
{
    public class COBFileUploadDto
    {
        public string stepId { get; set; }
        public string appId { get; set; }
        public string Id { get; set; }
        public string extra { get; set; }
        public string doIncreaseStep { get; set; }
        public string action { get; set; }
        public string modelData { get; set; }
        public string fileId { get; set; }
        public string fileName { get; set; }
    }
    public class ErrorModel
    {
        public string ErrorMessage { get; set; }
        public string Description { get; set; }
        public Exception ErrorException { get; set; }
        
        public string error
        {
            get { return ErrorMessage; }
            set
            {
                ErrorMessage = value;
            }
        }
        public string error_description
        {
            get { return Description; }
            set
            {
                Description = value;
            }
        }



    }
    public enum ResponseStatus
    {
        [Description("Failure")]
        Failure = 0,
        [Description("Success")]
        Success = 1,
        [Description("Warring")]
        Warning = 2,
        [Description("Error")]
        Error = 4,
    }
    public class ResponseModel
    {
        private ResponseStatus _status;

        public bool Status { get; set; }

        public string StatusTitle { get; set; }

        public ResponseStatus StatusCode { get { { return _status; }; } set { StatusTitle = value.ToString(); Status = (value == ResponseStatus.Success); _status = value; } }

        public string Message { get; set; }

        public string MessageCode { get; set; }

        public string Title { get; set; }

        public string TitleCode { get; set; }

        public ResponseModel()
        {
            Error = new ErrorModel();
            Result = null;
        }

        public int RecordCount { get; set; }


        [DefaultValue(null)]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public object Result { get; set; }

        public ErrorModel Error { get; set; }

        public int IdentityId { get; set; }
        
        public bool ShouldSerializeResult()
        {
            return Result != null;
        }

    }
    
}