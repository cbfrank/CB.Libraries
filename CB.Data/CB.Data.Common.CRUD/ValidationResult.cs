using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CB.Data.Common.CRUD
{
    public class ValidationResult<TEntity>
    {
        private readonly List<int> _ErrorCodes;
        private readonly List<string[]> _ErrorsMessageParameters;

        public ValidationResult()
        {
            _ErrorCodes = new List<int>();
            ErrorCodes = new ReadOnlyCollection<int>(_ErrorCodes);

            _ErrorsMessageParameters = new List<string[]>();
            ErrorsMessageParameters = new ReadOnlyCollection<string[]>(_ErrorsMessageParameters);
        }

        public ReadOnlyCollection<int> ErrorCodes { get; private set; }

        /// <summary>
        /// first index is the corresponding ErrorCodes index
        /// </summary>
        public ReadOnlyCollection<string[]> ErrorsMessageParameters { get; private set; }

        public void AddError(int errorCode, params string[] messageParams)
        {
            _ErrorCodes.Add(errorCode);
            if (messageParams == null || messageParams.Length <= 0)
            {
                _ErrorsMessageParameters.Add(new string[0]);
            }
            else
            {
                _ErrorsMessageParameters.Add(messageParams);
            }
        }

        public TEntity ExistingEntity { get; set; }
    }
}
