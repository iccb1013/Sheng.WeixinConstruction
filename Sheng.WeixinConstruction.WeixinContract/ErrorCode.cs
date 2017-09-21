using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sheng.WeixinConstruction.WeixinContract
{
    public class ErrorCode
    {
        private static readonly ErrorCode _instance = new ErrorCode();
        public static ErrorCode Instance
        {
            get { return _instance; }
        }

        Dictionary<int, string> _messageList = new Dictionary<int, string>();

        private ErrorCode()
        {
            _messageList.Add(-1, "系统繁忙，此时请开发者稍候再试");
            _messageList.Add("td2", "请求网页AccessToken失败");
            _messageList.Add("td3", "请求用户信息失败");
            _messageList.Add("td4", "只有微信认证服务号具备网页授权获取用户信息权限");

        }

        public string GetMessage(int code)
        {
            if (_messageList.ContainsKey(code) == false)
            {
                return "未知错误";
            }
            else
            {
                return _messageList[code];
            }
        }
    }
}
