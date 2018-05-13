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


using Linkup.Data;
using Sheng.WeixinConstruction.Infrastructure;
using Sheng.WeixinConstruction.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sheng.WeixinConstruction.Core
{
    public class Campaign_ShakingLotteryGiftPool
    {
        private DatabaseWrapper _dataBase = ServiceUnity.Instance.Database;

        private Random _random = new Random(DateTime.Now.Millisecond);

        private int _startedIndex = 0;

        /// <summary>
        /// 隶属活动
        /// </summary>
        public Guid CampaignId
        {
            get;
            set;
        }

        public Guid? PeriodId
        {
            get;
            set;
        }

        public DateTime CreatedTime
        {
            get;
            set;
        }

        private List<Campaign_ShakingLotteryGiftWrapper> _giftList = new List<Campaign_ShakingLotteryGiftWrapper>();
        public List<Campaign_ShakingLotteryGiftWrapper> GiftList
        {
            get
            {
                return _giftList;
            }
        }

        private List<Campaign_ShakingLotteryGiftWrapper> _allGiftList = new List<Campaign_ShakingLotteryGiftWrapper>();
        public List<Campaign_ShakingLotteryGiftWrapper> AllGiftList
        {
            get
            {
                return _allGiftList;
            }
        }

        public Campaign_ShakingLotteryGiftPool(Guid campaignId, Guid? periodId)
        {
            CampaignId = campaignId;
            PeriodId = periodId;

            LoadGiftList();
        }

        public void LoadGiftList()
        {
            CreatedTime = DateTime.Now;

            _startedIndex = 0;
            _giftList.Clear();
            _allGiftList.Clear();

            List<Campaign_ShakingLotteryGiftEntity> giftList;

            if (this.PeriodId.HasValue)
            {
                giftList = CampaignManager.Instance.ShakingLottery.GetGiftList(CampaignId, PeriodId.Value);
            }
            else
            {
                giftList = CampaignManager.Instance.ShakingLottery.GetGiftList(CampaignId);
            }

            foreach (Campaign_ShakingLotteryGiftEntity gift in giftList)
            {
                Campaign_ShakingLotteryGiftWrapper wrapper = new Campaign_ShakingLotteryGiftWrapper();
                wrapper.Gift = gift;
                _allGiftList.Add(wrapper);

                if (gift.Probability == 0)
                    continue;

                wrapper.StartedIndex = _startedIndex;
                _startedIndex = _startedIndex + gift.Probability;
                _giftList.Add(wrapper);
            }
        }

        public Campaign_ShakingLotteryGiftEntity GetGift()
        {
            int x = _random.Next(0, _startedIndex);

            foreach (var item in _giftList)
            {
                if (item.StartedIndex <= x && item.Gift.Probability + item.StartedIndex >= x)
                {
                    return item.Gift;
                }
            }

            return null;
        }
    }
}
