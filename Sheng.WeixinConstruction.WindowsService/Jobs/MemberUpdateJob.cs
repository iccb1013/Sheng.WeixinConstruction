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
    /// 更新会员信息
    /// </summary>
    [DisallowConcurrentExecution]
    class MemberUpdateJob : IJob
    {
        private static readonly LogService _log = LogService.Instance;
        private static readonly DatabaseWrapper _dataBase = ServiceUnity.Instance.Database;
        private static readonly MemberManager _memberManager = MemberManager.Instance;
        private static readonly ServiceDomainPool _domainPool = ServiceDomainPool.Instance;

        public void Execute(IJobExecutionContext context)
        {
            List<MemberEntity> needUpdateMemberList = _memberManager.GetNeedUpdateList();
            if (needUpdateMemberList == null || needUpdateMemberList.Count == 0) 
                return;

            _log.Write("更新会员信息", String.Format("取得待更新会员数：{0}", needUpdateMemberList.Count), TraceEventType.Verbose);

            foreach (MemberEntity member in needUpdateMemberList)
            {
                DomainContext domainContext = _domainPool.GetDomainContext(member.Domain);

                if (domainContext == null)
                {
                    _memberManager.NeedUpdate(member.Id, false);
                    _log.Write("更新会员信息失败", "没有Domain信息\r\n" + JsonHelper.Serializer(member), TraceEventType.Warning);
                    continue;
                }

                RequestApiResult<WeixinUser> getUserInfoResult =
                    UserApiWrapper.GetUserInfo(domainContext, member.OpenId);

                if (getUserInfoResult.Success == false)
                {
                    _log.Write("更新会员信息失败", JsonHelper.Serializer(getUserInfoResult), TraceEventType.Warning);
                    continue;
                }

                if (getUserInfoResult.ApiResult.Subscribe == 0)
                {
                    _memberManager.NeedUpdate(member.Id, false);
                    continue;
                }

                AddMemberArgs args = new AddMemberArgs();
                args.WeixinUser = getUserInfoResult.ApiResult;
                //更新当前用户信息
                _memberManager.UpdateMember(member, args);

                _memberManager.NeedUpdate(member.Id, false);
            }

            _log.Write("更新会员信息", "更新完毕。", TraceEventType.Verbose);

        }
    }
}
