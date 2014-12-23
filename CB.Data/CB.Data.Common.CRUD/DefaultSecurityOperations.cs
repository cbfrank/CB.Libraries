using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CB.Data.Common.CRUD
{
    public static class DefaultSecurityOperations
    {
        public const string QUERY = "Query";
        public const string CREATE = "Create";
        public const string UPDATE = "Update";
        public const string DELETE = "Delete";
        /// <summary>
        /// for normal verified user, it means the user should be registered in current ECO application
        /// </summary>
        public const string PUBLIC = "Public";

        public static string FromCRUDActions(CRUDAction action)
        {
            switch (action)
            {
                case CRUDAction.Query:
                    return QUERY;
                case CRUDAction.Create:
                    return CREATE;
                case CRUDAction.Update:
                    return UPDATE;
                case CRUDAction.Delete:
                    return DELETE;
                default:
                    throw new ArgumentOutOfRangeException("action");
            }
        }
    }
}
