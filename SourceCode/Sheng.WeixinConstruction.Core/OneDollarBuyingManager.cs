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
using Linkup.DataRelationalMapping;
using Sheng.WeixinConstruction.Infrastructure;
using Sheng.WeixinConstruction.Service;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sheng.WeixinConstruction.Core
{
    public class OneDollarBuyingManager
    {
        private static readonly OneDollarBuyingManager _instance = new OneDollarBuyingManager();
        public static OneDollarBuyingManager Instance
        {
            get { return _instance; }
        }

        private static DomainManager _domainManager = DomainManager.Instance;
        private static MemberManager _memberManager = MemberManager.Instance;

        private DatabaseWrapper _dataBase = ServiceUnity.Instance.Database;

        private OneDollarBuyingManager()
        {

        }

        public GetItemListResult GetCommodityList(GetOneDollarBuyingCommodityListArgs args)
        {
            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@domainId", args.DomainId));
            parameterList.Add(new CommandParameter("@appId", args.AppId));
            parameterList.Add(new CommandParameter("@page", args.Page));
            parameterList.Add(new CommandParameter("@pageSize", args.PageSize));
            parameterList.Add(new CommandParameter("@name", args.Name));
            parameterList.Add(new CommandParameter("@forSale", args.ForSale));

            DataSet dsResult =
                _dataBase.ExecuteDataSet(CommandType.StoredProcedure, "GetOneDollarBuyingCommodityList",
                parameterList, new string[] { "result" });

            if (dsResult.Tables[0].Rows.Count == 0 && args.Page > 1)
            {
                args.Page--;
                return GetCommodityList(args);
            }

            GetItemListResult result = new GetItemListResult();

            result.ItemList = dsResult.Tables[0];

            int totalCount = int.Parse(dsResult.Tables[1].Rows[0][0].ToString());
            result.TotalCount = totalCount;
            result.TotalPage = totalCount / args.PageSize;
            if (totalCount % args.PageSize > 0)
            {
                result.TotalPage++;
            }
            result.Page = args.Page;

            return result;
        }

        /// <summary>
        /// 获取指定的商品的销售列表（已销及在销）
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public GetItemListResult GetCommodityForSaleList(GetOneDollarBuyingCommodityForSaleListArgs args)
        {
            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@domainId", args.DomainId));
            parameterList.Add(new CommandParameter("@appId", args.AppId));
            parameterList.Add(new CommandParameter("@page", args.Page));
            parameterList.Add(new CommandParameter("@pageSize", args.PageSize));
            parameterList.Add(new CommandParameter("@commodityId", args.CommodityId));

            DataSet dsResult =
                _dataBase.ExecuteDataSet(CommandType.StoredProcedure, "GetOneDollarBuyingCommodityForSaleList",
                parameterList, new string[] { "result" });

            if (dsResult.Tables[0].Rows.Count == 0 && args.Page > 1)
            {
                args.Page--;
                return GetCommodityForSaleList(args);
            }

            GetItemListResult result = new GetItemListResult();

            result.ItemList = dsResult.Tables[0];

            int totalCount = int.Parse(dsResult.Tables[1].Rows[0][0].ToString());
            result.TotalCount = totalCount;
            result.TotalPage = totalCount / args.PageSize;
            if (totalCount % args.PageSize > 0)
            {
                result.TotalPage++;
            }
            result.Page = args.Page;

            return result;
        }

        /// <summary>
        /// 获取销售中的商品列表
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public GetItemListResult GetForSaleCommodityList(GetItemListArgs args)
        {
            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@domainId", args.DomainId));
            parameterList.Add(new CommandParameter("@appId", args.AppId));
            parameterList.Add(new CommandParameter("@page", args.Page));
            parameterList.Add(new CommandParameter("@pageSize", args.PageSize));

            DataSet dsResult =
                _dataBase.ExecuteDataSet(CommandType.StoredProcedure, "GetOneDollarBuyingForSaleCommodityList",
                parameterList, new string[] { "result" });

            if (dsResult.Tables[0].Rows.Count == 0 && args.Page > 1)
            {
                args.Page--;
                return GetForSaleCommodityList(args);
            }

            GetItemListResult result = new GetItemListResult();

            result.ItemList = dsResult.Tables[0];

            int totalCount = int.Parse(dsResult.Tables[1].Rows[0][0].ToString());
            result.TotalCount = totalCount;
            result.TotalPage = totalCount / args.PageSize;
            if (totalCount % args.PageSize > 0)
            {
                result.TotalPage++;
            }
            result.Page = args.Page;

            return result;
        }

        public OneDollarBuyingCommodityEntity GetCommodity(Guid id)
        {
            OneDollarBuyingCommodityEntity pointCommodity = new OneDollarBuyingCommodityEntity();
            pointCommodity.Id = id;

            if (_dataBase.Fill<OneDollarBuyingCommodityEntity>(pointCommodity))
                return pointCommodity;
            else
                return null;
        }

        public NormalResult CreateCommodity(OneDollarBuyingCommodityEntity commodity)
        {
            if (commodity == null)
            {
                Debug.Assert(false, "commodity 为空");
                return new NormalResult("参数错误");
            }

            commodity.CreateTime = DateTime.Now;
            commodity.Sort = _domainManager.GetSort(commodity.Domain, "OneDollarBuyingCommodity");
            _dataBase.Insert(commodity);

            PutOnSale(commodity.Id);

            return new NormalResult();
        }

        public NormalResult UpdateCommodity(OneDollarBuyingCommodityEntity commodity)
        {
            if (commodity == null)
            {
                Debug.Assert(false, "commodity 为空");
                return new NormalResult("参数错误");
            }

            SqlStructureBuild sqlBuild = new SqlStructureBuild();
            sqlBuild.Table = "OneDollarBuyingCommodity";
            sqlBuild.Type = SqlExpressionType.Update;
            sqlBuild.AddParameter("Id", commodity.Id, true);
            sqlBuild.AddParameter("Name", commodity.Name);
            sqlBuild.AddParameter("Price", commodity.Price);
            sqlBuild.AddParameter("ForSale", commodity.ForSale);
            sqlBuild.AddParameter("InfiniteStock", commodity.InfiniteStock);
            sqlBuild.AddParameter("ImageUrl", commodity.ImageUrl);
            sqlBuild.AddParameter("Introduction", commodity.Introduction);
            _dataBase.ExcuteSqlExpression(sqlBuild.GetSqlExpression());

            PutOnSale(commodity.Id);

            return new NormalResult();
        }

        public void RemoveCommodity(Guid id)
        {
            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@id", id));

            _dataBase.ExecuteNonQuery("UPDATE [OneDollarBuyingCommodity] SET [Remove] = 1 WHERE [Id] = @id",
                parameterList);
        }

        public void BatchRemoveCommodity(IdListArgs args)
        {
            if (args == null || args.IdList == null)
            {
                Debug.Assert(false, "args 为空");
                return;
            }

            foreach (string id in args.IdList)
            {
                RemoveCommodity(Guid.Parse(id));
            }
        }

        /// <summary>
        /// 返回调整后的最新库存数量
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public int StockIncrement(OneDollarBuyingCommodityStockAdjustmentArgs args)
        {
            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@id", args.Id));
            parameterList.Add(new CommandParameter("@quantity", args.Quantity));

            _dataBase.ExecuteNonQuery("UPDATE [OneDollarBuyingCommodity] SET [Stock] = [Stock] + @quantity WHERE [Id] = @id", parameterList);

            object objStock =
                _dataBase.ExecuteScalar("SELECT [Stock] FROM [OneDollarBuyingCommodity] WHERE [Id] = @id", parameterList);

            if (objStock == null || objStock == DBNull.Value)
                return 0;
            else
                return int.Parse(objStock.ToString());
        }

        /// <summary>
        /// 返回调整后的最新库存数量
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public int StockDecrement(OneDollarBuyingCommodityStockAdjustmentArgs args)
        {
            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@id", args.Id));
            parameterList.Add(new CommandParameter("@quantity", args.Quantity));

            _dataBase.ExecuteNonQuery("UPDATE [OneDollarBuyingCommodity] SET [Stock] = [Stock] - @quantity WHERE [Id] = @id", parameterList);

            object objStock =
                _dataBase.ExecuteScalar("SELECT [Stock] FROM [OneDollarBuyingCommodity] WHERE [Id] = @id", parameterList);

            if (objStock == null || objStock == DBNull.Value)
            {
                return 0;
            }
            else
            {
                int stock = int.Parse(objStock.ToString());
                if (stock < 0)
                {
                    _dataBase.ExecuteNonQuery("UPDATE [OneDollarBuyingCommodity] SET [Stock] = 0 WHERE [Id] = @id", parameterList);
                    stock = 0;
                }
                return stock;
            }
        }

        public OneDollarBuyingCommodityForSaleEntity GetCommodityForSale(Guid saleId)
        {
            OneDollarBuyingCommodityForSaleEntity forSale = new OneDollarBuyingCommodityForSaleEntity();
            forSale.Id = saleId;

            if (_dataBase.Fill<OneDollarBuyingCommodityForSaleEntity>(forSale))
                return forSale;
            else
                return null;
        }

        public List<int> GetPartNumberListByMember(Guid saleId, Guid memberId)
        {
            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@saleId", saleId));
            parameterList.Add(new CommandParameter("@memberId", memberId));

            DataSet dsResult = _dataBase.ExecuteDataSet(
                @"SELECT [PartNumber] FROM [OneDollarBuyingCommoditySoldPart]
                WHERE [SaleId] = @saleId AND [Member] = @memberId ORDER BY [PartNumber] ASC", parameterList, new string[] { "result" });

            List<int> partNumberList = new List<int>();
            foreach (DataRow dr in dsResult.Tables[0].Rows)
            {
                partNumberList.Add(int.Parse(dr["PartNumber"].ToString()));
            }
            return partNumberList;
        }

        public int GetPartNumberCountByMember(Guid saleId, Guid memberId)
        {
            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@saleId", saleId));
            parameterList.Add(new CommandParameter("@memberId", memberId));

            int count = 0;
            _dataBase.ExecuteScalar<int>(@"SELECT Count(1) FROM [OneDollarBuyingCommoditySoldPart] 
                WHERE [SaleId] = @saleId AND [Member] = @memberId",
                parameterList, (scalarValue) => { count = scalarValue; });
            return count;
        }

        public GetItemListResult GetForSaleCommodityMemberPartNumber(GetOneDollarBuyingCommodityMemberPartNumberArgs args)
        {
            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@page", args.Page));
            parameterList.Add(new CommandParameter("@pageSize", args.PageSize));
            parameterList.Add(new CommandParameter("@saleId", args.SaleId));

            DataSet dsResult =
                _dataBase.ExecuteDataSet(CommandType.StoredProcedure, "GetOneDollarBuyingForSaleCommodityMemberPartNumber",
                parameterList, new string[] { "result" });

            if (dsResult.Tables[0].Rows.Count == 0 && args.Page > 1)
            {
                args.Page--;
                return GetForSaleCommodityMemberPartNumber(args);
            }

            GetItemListResult result = new GetItemListResult();

            result.ItemList = dsResult.Tables[0];

            int totalCount = int.Parse(dsResult.Tables[1].Rows[0][0].ToString());
            result.TotalCount = totalCount;
            result.TotalPage = totalCount / args.PageSize;
            if (totalCount % args.PageSize > 0)
            {
                result.TotalPage++;
            }
            result.Page = args.Page;

            return result;
        }

        public OrderOneDollarBuyingCommodityResult OrderOneDollarBuyingCommodity(OrderOneDollarBuyingCommodityArgs args)
        {
            OrderOneDollarBuyingCommodityResult result = new OrderOneDollarBuyingCommodityResult();

            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@saleId", args.SaleId));
            parameterList.Add(new CommandParameter("@memberId", args.MemberId));
            parameterList.Add(new CommandParameter("@quantity", args.Quantity));

            DataSet dsResult =
                _dataBase.ExecuteDataSet(CommandType.StoredProcedure, "OrderOneDollarBuyingCommodity", parameterList, new string[] { "result" });

            result.Reason = int.Parse(dsResult.Tables[0].Rows[0]["Result"].ToString());

            if (result.Success)
            {
                result.OrderQuantity = int.Parse(dsResult.Tables[0].Rows[0]["OrderQuantity"].ToString());
            }

            return result;
        }

        public GetItemListResult GetLuckyList(GetItemListArgs args)
        {
            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@domainId", args.DomainId));
            parameterList.Add(new CommandParameter("@appId", args.AppId));
            parameterList.Add(new CommandParameter("@page", args.Page));
            parameterList.Add(new CommandParameter("@pageSize", args.PageSize));

            DataSet dsResult =
                _dataBase.ExecuteDataSet(CommandType.StoredProcedure, "GetOneDollarBuyingLuckyList",
                parameterList, new string[] { "result" });

            if (dsResult.Tables[0].Rows.Count == 0 && args.Page > 1)
            {
                args.Page--;
                return GetLuckyList(args);
            }

            GetItemListResult result = new GetItemListResult();

            result.ItemList = dsResult.Tables[0];

            int totalCount = int.Parse(dsResult.Tables[1].Rows[0][0].ToString());
            result.TotalCount = totalCount;
            result.TotalPage = totalCount / args.PageSize;
            if (totalCount % args.PageSize > 0)
            {
                result.TotalPage++;
            }
            result.Page = args.Page;

            return result;
        }

        /// <summary>
        /// 我参与的
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public GetItemListResult GetParticipatedList(GetOneDollarBuyingCommodityParticipatedListArgs args)
        {
            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@page", args.Page));
            parameterList.Add(new CommandParameter("@pageSize", args.PageSize));
            parameterList.Add(new CommandParameter("@memberId", args.Member));
            parameterList.Add(new CommandParameter("@lucky", args.Lucky));

            DataSet dsResult =
                _dataBase.ExecuteDataSet(CommandType.StoredProcedure, "GetOneDollarBuyingCommodityParticipatedList",
                parameterList, new string[] { "result" });

            if (dsResult.Tables[0].Rows.Count == 0 && args.Page > 1)
            {
                args.Page--;
                return GetParticipatedList(args);
            }

            GetItemListResult result = new GetItemListResult();

            result.ItemList = dsResult.Tables[0];

            int totalCount = int.Parse(dsResult.Tables[1].Rows[0][0].ToString());
            result.TotalCount = totalCount;
            result.TotalPage = totalCount / args.PageSize;
            if (totalCount % args.PageSize > 0)
            {
                result.TotalPage++;
            }
            result.Page = args.Page;

            return result;
        }

        public Guid? GetAvailableSaleId(Guid commodityId)
        {
            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@commodityId", commodityId));

            Guid? saleId = null;
            _dataBase.ExecuteScalar<Guid>(
                @"SELECT TOP 1 [Id] FROM [OneDollarBuyingCommodityForSale] WHERE [CommodityId] = @commodityId AND [SoldPart] < [TotalPart]",
                parameterList, (scalarValue) => { saleId = scalarValue; });
            return saleId;

        }

        public OneDollarBuyingDealInfo GetDealInfo(Guid saleId)
        {
            OneDollarBuyingDealInfo dealInfo = new OneDollarBuyingDealInfo();
            dealInfo.SaleId = saleId;

            if (_dataBase.Fill<OneDollarBuyingDealInfo>(dealInfo))
            {
                OneDollarBuyingCommodityEntity commodity = GetCommodity(dealInfo.CommodityId);
                dealInfo.CommodityName = commodity.Name;

                if (dealInfo.LuckyMemberId.HasValue)
                {
                    dealInfo.LuckyMember = _memberManager.GetMember(dealInfo.LuckyMemberId.Value);
                }
                return dealInfo;
            }
            else
            {
                return null;
            }
        }

        public OneDollarBuyingDealInfo GetDealInfoByPeriodNumber(Guid domainId, string appId, string periodNumber)
        {
            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@domainId", domainId));
            parameterList.Add(new CommandParameter("@appId", appId));
            parameterList.Add(new CommandParameter("@periodNumber", periodNumber));

            Guid? id = null;
            _dataBase.ExecuteScalar<string>(
                "SELECT [Id] FROM [OneDollarBuyingCommodityForSale] WHERE [Domain] = @domainId AND [AppId] = @appId AND [PeriodNumber] = @periodNumber",
                parameterList, (scalarString) => { id = Guid.Parse(scalarString); });

            if (id.HasValue)
                return GetDealInfo(id.Value);
            else
                return null;
        }

        /// <summary>
        /// 领取
        /// 0 成功 1该订单已领取过 2 未知错误
        /// </summary>
        /// <returns></returns>
        public int Deal(Guid saleId, Guid operatorUserId)
        {
            OneDollarBuyingDealInfo dealInfo = new OneDollarBuyingDealInfo();
            dealInfo.SaleId = saleId;

            if (_dataBase.Fill<OneDollarBuyingDealInfo>(dealInfo) == false)
            {
                return 2;
            }

            if (dealInfo.Deal)
                return 1;

            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@saleId", saleId));
            parameterList.Add(new CommandParameter("@dealUser", operatorUserId));

            _dataBase.ExecuteNonQuery(
                "UPDATE [OneDollarBuyingCommodityForSale] SET [Deal] = 1,[DealTime] = GETDATE(),[DealUser] = @dealUser WHERE [Id] = @saleId",
                parameterList);

            return 0;
        }

        /// <summary>
        /// 上架指定的商品
        /// 已存在在售期数则不上架
        /// 0 成功 1商品未上架 2无库存 3 存在在售期数 4商品已被删除
        /// </summary>
        /// <param name="commodityId"></param>
        /// <returns></returns>
        public int PutOnSale(Guid commodityId)
        {
            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@commodityId", commodityId));

            DataSet dsResult =
                _dataBase.ExecuteDataSet(CommandType.StoredProcedure, "OneDollarBuyingPutOnSaleCommodity", 
                parameterList, new string[] { "result" });

            return int.Parse(dsResult.Tables[0].Rows[0]["Result"].ToString());
        }
    }
}
