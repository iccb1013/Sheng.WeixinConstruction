using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sheng.WeixinConstruction.Infrastructure
{
    public class UserRegisterArgs
    {
        public string Account
        {
            get;
            set;
        }

        public string Password
        {
            get;
            set;
        }

        public string Name
        {
            get;
            set;
        }

        public string Email
        {
            get;
            set;
        }

        public string MobilePhone
        {
            get;
            set;
        }

        public string MobilePhoneValidateCode
        {
            get;
            set;
        }

        //public string ValidateCode
        //{
        //    get;
        //    set;
        //}
    }
}
