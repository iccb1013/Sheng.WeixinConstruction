using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sheng.WeixinConstruction.Infrastructure
{
    public class GetCampaign_MemberQRCodeItemListArgs : GetItemListArgs
    {
        public Guid CampaignId
        {
            get;
            set;
        }

        public string MemberName
        {
            get;
            set;
        }

        public const string DefaultOrderBy = "CreateTime";
        private string _orderBy = DefaultOrderBy;
        public string OrderBy
        {
            get { return _orderBy; }
            set { _orderBy = value; }
        }
    }

   

}
