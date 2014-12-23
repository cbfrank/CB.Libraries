using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CB.Data.Common.CRUD
{
    public static class Extensions
    {
        public static void ThrowException<T>(this ValidationResult<T> result)
        {
            if (result.ErrorCodes != null && result.ErrorCodes.Count > 0)
            {
                if (result.ErrorCodes.Count == 1)
                {
                    string[] msgParams = null;
                    if (result.ErrorsMessageParameters != null && result.ErrorsMessageParameters.Count > 0)
                    {
                        msgParams = result.ErrorsMessageParameters[0];
                    }
                    throw new DataServiceException(result.ErrorCodes[0], msgParams);
                }
                else
                {
                    var exps = new List<DataServiceException>();
                    for (int i = 0; i < result.ErrorCodes.Count; i++)
                    {
                        string[] msgParams = null;
                        if (result.ErrorsMessageParameters != null && result.ErrorsMessageParameters.Count > i)
                        {
                            msgParams = result.ErrorsMessageParameters[i];
                        }
                        exps.Add(new DataServiceException(result.ErrorCodes[i], msgParams));
                    }
                    throw new AggregateException(exps);
                }
            }
        }
    }
}
