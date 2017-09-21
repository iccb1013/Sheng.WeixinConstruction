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
using Sheng.WeixinConstruction.Core;
using Sheng.WeixinConstruction.WeixinContract;
using System.Diagnostics;
using Linkup.Common;

namespace Sheng.WeixinConstruction.WindowsService
{
    /// <summary>
    /// 释放过期的积分商城订单
    /// </summary>
    [DisallowConcurrentExecution]
    class ReleaseOverduePointCommodityOrderJob : IJob
    {
        private static readonly LogService _log = LogService.Instance;
        private static readonly DatabaseWrapper _dataBase = ServiceUnity.Instance.Database;
        private static readonly PointCommodityManager _pointCommodityManager = PointCommodityManager.Instance;
        private static readonly ServiceDomainPool _domainPool = ServiceDomainPool.Instance;

        public void Execute(IJobExecutionContext context)
        {
            List<PointCommodityOrderEntity> orderList = _pointCommodityManager.GetOverdueOrderList();
            if (orderList == null || orderList.Count == 0)
                return;

            _log.Write("释放过期的积分商城订单", String.Format("取得过期订单：{0}", orderList.Count), TraceEventType.Verbose);

            foreach (PointCommodityOrderEntity item in orderList)
            {
                List<CommandParameter> parameterList = new List<CommandParameter>();
                parameterList.Add(new CommandParameter("@orderId", item.Id));

                _dataBase.ExecuteNonQuery(CommandType.StoredProcedure, "ReleaseOverduePointCommodityOrder",
                    parameterList);
            }
        }
    }
}
