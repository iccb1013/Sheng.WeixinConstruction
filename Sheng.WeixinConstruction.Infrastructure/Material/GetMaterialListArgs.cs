using Sheng.WeixinConstruction.WeixinContract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sheng.WeixinConstruction.Infrastructure
{
    public class GetMaterialListArgs : GetItemListArgs
    {

        public MaterialType Type
        {
            get;
            set;
        }
    }
}
