using System;
using System.Linq;
using System.Resources;

namespace CB.Data.Common.CRUD
{
    public class DataServiceException : Exception
    {
        public const int ENTITY_NOT_FOUND = 0;
        public const int DUPLICATED_UNIQUE_FIELDS = 1;
        public const int DUPLICATED_ENTITY = 2;
        public const int ENTITY_OF_FK_NOT_FOUND = 3;
        public const int ENTITY_NOT_VALID = 4;

        public int ErrorCode { get; private set; }
        public static ResourceManager ResourceManager { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Exception"/> class.
        /// </summary>
        public DataServiceException(int errorCode, params string[] messageParams)
            : base(TranslateToMessage(errorCode, messageParams))
        {
            ErrorCode = errorCode;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Exception"/> class with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error. </param>
        /// <param name="errorCode"></param>
        public DataServiceException(string message, int errorCode)
            : base(message)
        {
            ErrorCode = errorCode;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Exception"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception. </param><param name="innerException">The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified. </param>
        /// <param name="errorCode"></param>
        public DataServiceException(string message, Exception innerException, int errorCode)
            : base(message, innerException)
        {
            ErrorCode = errorCode;
        }

        public static string ErrorCodeResourceKeyFormatNegative = "ErrorCode_{0}";
        public static string ErrorCodeResourceKeyFormat = "ErrorCode{0}";

        private static string TranslateToMessage(int errorCode, params string[] messageParams)
        {
            var resourceKey = errorCode < 0 ? string.Format(ErrorCodeResourceKeyFormatNegative, Math.Abs(errorCode)) : string.Format(ErrorCodeResourceKeyFormat, errorCode);
            var res = ResourceManager == null ? string.Empty : ResourceManager.GetString(resourceKey);
            var message = string.IsNullOrEmpty(res) ? string.Format("Error Code: {0}", errorCode) : res;
            if (!string.IsNullOrEmpty(res) && messageParams != null && messageParams.Length > 0)
            {
                message = string.Format(message, messageParams.Cast<object>().ToArray());
            }
            return message;
        }
    }
}
