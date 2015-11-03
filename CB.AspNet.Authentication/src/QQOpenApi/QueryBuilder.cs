using System.Collections.Generic;
using System.Collections;
using System.Text;
using Microsoft.Framework.WebEncoders;

namespace CB.QQ.QQOpenApi
{
    internal class QueryBuilder : IEnumerable<KeyValuePair<string, string>>
    {
        private readonly IList<KeyValuePair<string, string>> _Params;

        public QueryBuilder()
        {
            _Params = new List<KeyValuePair<string, string>>();
        }

        public QueryBuilder(IEnumerable<KeyValuePair<string, string>> parameters)
        {
            _Params = new List<KeyValuePair<string, string>>(parameters);
        }

        public void Add(string key, IEnumerable<string> values)
        {
            foreach (string str in values)
                _Params.Add(new KeyValuePair<string, string>(key, str));
        }

        public void Add(string key, string value)
        {
            _Params.Add(new KeyValuePair<string, string>(key, value));
        }

        public override string ToString()
        {
            var stringBuilder = new StringBuilder();
            var flag = true;
            foreach (KeyValuePair<string, string> keyValuePair in _Params)
            {
                stringBuilder.Append(flag ? "?" : "&");
                flag = false;
                stringBuilder.Append(UrlEncoder.Default.UrlEncode(keyValuePair.Key));
                stringBuilder.Append("=");
                stringBuilder.Append(UrlEncoder.Default.UrlEncode(keyValuePair.Value));
            }
            return stringBuilder.ToString();
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return this.ToString().Equals(obj);
        }

        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            return _Params.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this._Params.GetEnumerator();
        }
    }
}
