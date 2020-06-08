using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DuolingoSk
{
    public class ResponseDataModel<T>
    {
        /// <summary>
        /// IsError, True means there is error, otherwise there will be data.
        /// </summary>
        public bool IsError { get; set; } = false;

        /// <summary>
        /// If error then only ErrorData would have data.
        /// </summary>
        public List<String> ErrorData { get; set; } = null;

        /// <summary>
        /// Add/Update case single object data.
        /// </summary>
        public T Data { get; set; }

        public void AddError(string errorMessage)
        {
            if (this.ErrorData == null)
            {
                this.ErrorData = new List<string>();
            }
            this.ErrorData.Add(errorMessage);
            this.IsError = true;
        }

        public void AddError(List<string> listError)
        {
            if (!listError.Any())
            {
                return;
            }

            if (this.ErrorData == null)
            {
                this.ErrorData = new List<string>();
            }
            this.ErrorData.AddRange(listError);
            this.IsError = true;
        }
    }
}