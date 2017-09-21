using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Linkup.Data;
using Sheng.WeixinConstruction.Service;
using Sheng.WeixinConstruction.Infrastructure;
using System.Data;

namespace Sheng.WeixinConstruction.WindowsService
{
    /// <summary>
    /// 处理一元抢购相关内容
    /// 1.商品的上架
    /// 2.已售完所有份数商品的抽奖
    /// </summary>
    [DisallowConcurrentExecution]
    class OneDollarBuyingJob : IJob
    {
        private LogService _log = LogService.Instance;
        private DatabaseWrapper _dataBase = ServiceUnity.Instance.Database;

        public void Execute(IJobExecutionContext context)
        {
            Draw();

            CommodityForSale();
        }

        /// <summary>
        /// 商品上架
        /// </summary>
        private void CommodityForSale()
        {
            _dataBase.ExecuteNonQuery(CommandType.StoredProcedure, "OneDollarBuyingCommodityPutOnSale");
        }

        /// <summary>
        /// 抽奖
        /// </summary>
        private void Draw()
        {
            _dataBase.ExecuteNonQuery(CommandType.StoredProcedure, "OneDollarBuyingCommodityDraw");
        }
    }
}
