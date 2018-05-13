/*
********************************************************************
*
*    曹旭升（sheng.c）
*    E-mail: cao.silhouette@msn.com
*    QQ: 279060597
*    https://github.com/iccb1013
*    http://shengxunwei.com
*
*    © Copyright 2016
*
********************************************************************/


using Sheng.WeixinConstruction.Core;
using Sheng.WeixinConstruction.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Sheng.WeixinConstruction.Management.Core
{
    /// <summary>
    /// 抽奖活动抽奖循环
    /// </summary>
    public class Campaign_LotteryDraw
    {
        private static readonly Campaign_LotteryDraw _instance = new Campaign_LotteryDraw();
        public static Campaign_LotteryDraw Instance
        {
            get { return _instance; }
        }

        private CampaignManager _campaignManager = CampaignManager.Instance;

        private Timer _drawTimer;

        private Campaign_LotteryDraw()
        {
            //不从载入数据库中的所有Domain，因为可能存在许多僵尸号，无需为其加载资源

            //1 * 60 * 1000
            _drawTimer = new System.Threading.Timer(DrawTimerCallback, null, 0, 1 * 60 * 1000);
        }

        private void DrawTimerCallback(object state)
        {
            _campaignManager.Lottery.LotteryDraw();
        }
    }
}
