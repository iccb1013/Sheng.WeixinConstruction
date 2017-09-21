using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sheng.WeixinConstruction.Infrastructure
{
    public class AuthorizationWrapper
    {
        private Hashtable _authorizationHashtable = new Hashtable();

        private List<Authorization> authorizationList = new List<Authorization>();

        public List<Authorization> AuthorizationList
        {
            get { return authorizationList; }
            set
            {
                authorizationList = value;
                _authorizationHashtable.Clear();

                if (value != null)
                {
                    foreach (var item in authorizationList)
                    {
                        if (item == null || String.IsNullOrEmpty(item.Key))
                            continue;

                        _authorizationHashtable.Add(item.Key, item);
                    }
                }
            }
        }

        public bool Verify(string key)
        {
            if (String.IsNullOrEmpty(key))
                return true;

            return _authorizationHashtable.ContainsKey(key);
        }
    }
}
