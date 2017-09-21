using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sheng.WeixinConstruction.Client.Core
{
    class TextMessageHandle
    {
        private TextMessageHandleMode _mode = TextMessageHandleMode.Text;
        public TextMessageHandleMode Mode
        {
            get { return _mode; }
            set { _mode = value; }
        }

        public string Name
        {
            get;
            set;
        }

        public string Handle()
        {
            return String.Empty;
        }
    }

    enum TextMessageHandleMode
    {
        Text,
        Regex
    }
}
