using System;
using System.Collections.Generic;
using System.Web;

namespace Sheng.WeixinConstruction.WeixinContract.PayApi
{
    public class WxPayException : Exception 
    {
        public WxPayException(string msg) : base(msg) 
        {

        }
     }
}