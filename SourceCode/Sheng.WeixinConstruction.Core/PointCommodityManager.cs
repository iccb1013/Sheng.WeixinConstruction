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


using Linkup.Common;
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
    public class PointCommodityManager
    {
        private static readonly PointCommodityManager _instance = new PointCommodityManager();
        public static PointCommodityManager Instance
        {
            get { return _instance; }
        }

        private static DomainManager _domainManager = DomainManager.Instance;
        private static PayManager _payManager = PayManager.Instance;
        private static MemberManager _memberManager = MemberManager.Instance;

        private DatabaseWrapper _dataBase = ServiceUnity.Instance.Database;

        private PointCommodityManager()
        {

        }

        public PointCommodityOrderStatisticData GetPointCommodityOrderStatisticData(Guid domainId, string appId, DateTime startDate, DateTime endDate)
        {
            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@domainId", domainId));
            parameterList.Add(new CommandParameter("@appId", appId));
            parameterList.Add(new CommandParameter("@startDate", startDate));
            parameterList.Add(new CommandParameter("@endDate", endDate));

            DataSet dsResult =
               _dataBase.ExecuteDataSet(CommandType.StoredProcedure, "GetPointCommodityOrderStatisticData", parameterList,
               new string[] { "totalOrder", "noPayment", "paid", "deal", "canceled" });

            PointCommodityOrderStatisticData result = new PointCommodityOrderStatisticData();

            if (dsResult.Tables["totalOrder"].Rows.Count > 0)
            {
                string strCount = dsResult.Tables["totalOrder"].Rows[0][0].ToString();

                if (string.IsNullOrEmpty(strCount) == false)
                    result.TotalOrder = int.Parse(strCount);
            }

            if (dsResult.Tables["noPayment"].Rows.Count > 0)
            {
                string strCount = dsResult.Tables["noPayment"].Rows[0][0].ToString();

                if (string.IsNullOrEmpty(strCount) == false)
                    result.NoPayment = int.Parse(strCount);
            }

            if (dsResult.Tables["paid"].Rows.Count > 0)
            {
                string strCount = dsResult.Tables["paid"].Rows[0][0].ToString();

                if (string.IsNullOrEmpty(strCount) == false)
                    result.Paid = int.Parse(strCount);
            }

            if (dsResult.Tables["deal"].Rows.Count > 0)
            {
                string strCount = dsResult.Tables["deal"].Rows[0][0].ToString();

                if (string.IsNullOrEmpty(strCount) == false)
                    result.Deal = int.Parse(strCount);
            }

            if (dsResult.Tables["canceled"].Rows.Count > 0)
            {
                string strCount = dsResult.Tables["canceled"].Rows[0][0].ToString();

                if (string.IsNullOrEmpty(strCount) == false)
                    result.Canceled = int.Parse(strCount);
            }

            return result;
        }


        #region PointCommodity

        public GetItemListResult<PointCommodityEntity> GetPointCommodityList(Guid domainId, string appId, GetPointCommodityListArgs args)
        {
            GetItemListResult<PointCommodityEntity> result = new GetItemListResult<PointCommodityEntity>();

            List<AttachedWhereItem> attachedWhere = new List<AttachedWhereItem>();
            attachedWhere.Add(new AttachedWhereItem("Domain", domainId));
            attachedWhere.Add(new AttachedWhereItem("AppId", appId));
            attachedWhere.Add(new AttachedWhereItem("Remove", false));
            if (String.IsNullOrEmpty(args.Name) == false)
            {
                attachedWhere.Add(new AttachedWhereItem("Name", args.Name) { Type = AttachedWhereType.Like });
            }
            if (args.ForSale.HasValue)
            {
                attachedWhere.Add(new AttachedWhereItem("ForSale", args.ForSale));
            }

            SqlExpressionPagingArgs pagingArgs = new SqlExpressionPagingArgs();
            pagingArgs.Page = args.Page;
            pagingArgs.PageSize = args.PageSize;

            result.ItemList = _dataBase.Select<PointCommodityEntity>(attachedWhere, pagingArgs);
            result.TotalPage = pagingArgs.TotalPage;
            result.Page = pagingArgs.Page;

            if (result.ItemList.Count == 0 && result.Page > 1)
            {
                args.Page--;
                return GetPointCommodityList(domainId, appId, args);
            }
            else
            {
                return result;
            }
        }

        public PointCommodityEntity GetPointCommodity(Guid id)
        {
            PointCommodityEntity pointCommodity = new PointCommodityEntity();
            pointCommodity.Id = id;

            if (_dataBase.Fill<PointCommodityEntity>(pointCommodity))
                return pointCommodity;
            else
                return null;
        }

        public NormalResult CreatePointCommodity(DomainContext domainContext, PointCommodityEntity pointCommodity)
        {
            if (pointCommodity == null)
            {
                Debug.Assert(false, "pointCommodity 为空");
                return new NormalResult("参数错误");
            }

            //if (domainContext.Domain.Type == EnumDomainType.Free)
            //{
            //    //最大数量不允许超过10个
            //    if (GetTotalCount(domainContext) >= 10)
            //    {
            //        return new NormalResult("免费帐户创建积分商品最多不可超过10个。");
            //    }
            //}

            pointCommodity.CreateTime = DateTime.Now;
            pointCommodity.Sort = _domainManager.GetSort(pointCommodity.Domain, "PointCommodity");
            _dataBase.Insert(pointCommodity);

            return new NormalResult();
        }

        public NormalResult UpdatePointCommodity(PointCommodityEntity pointCommodity)
        {
            if (pointCommodity == null)
            {
                Debug.Assert(false, "pointCommodity 为空");
                return new NormalResult("参数错误");
            }

            SqlStructureBuild sqlBuild = new SqlStructureBuild();
            sqlBuild.Table = "PointCommodity";
            sqlBuild.Type = SqlExpressionType.Update;
            sqlBuild.AddParameter("Id", pointCommodity.Id, true);
            sqlBuild.AddParameter("Domain", pointCommodity.Domain, true);
            sqlBuild.AddParameter("AppId", pointCommodity.AppId, true);
            sqlBuild.AddParameter("Name", pointCommodity.Name);
            sqlBuild.AddParameter("Price", pointCommodity.Price);
            sqlBuild.AddParameter("ForSale", pointCommodity.ForSale);
            sqlBuild.AddParameter("ImageUrl", pointCommodity.ImageUrl);
            sqlBuild.AddParameter("ImageList", JsonHelper.Serializer(pointCommodity.ImageList));
            sqlBuild.AddParameter("Introduction", pointCommodity.Introduction);
            _dataBase.ExcuteSqlExpression(sqlBuild.GetSqlExpression());

            return new NormalResult();
        }

        public void RemovePointCommodity(Guid id)
        {
            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@id", id));

            _dataBase.ExecuteNonQuery("UPDATE [PointCommodity] SET [Remove] = 1 WHERE [Id] = @id",
                parameterList);
        }

        public void BatchRemovePointCommodity(IdListArgs args)
        {
            if (args == null || args.IdList == null)
            {
                Debug.Assert(false, "args 为空");
                return;
            }

            foreach (string id in args.IdList)
            {
                RemovePointCommodity(Guid.Parse(id));
            }
        }

        /// <summary>
        /// 返回调整后的最新库存数量
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public int StockIncrement(PointCommodityStockAdjustmentArgs args)
        {
            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@id", args.Id));
            parameterList.Add(new CommandParameter("@quantity", args.Quantity));

            _dataBase.ExecuteNonQuery("UPDATE [PointCommodity] SET [Stock] = [Stock] + @quantity WHERE [Id] = @id", parameterList);

            object objStock =
                _dataBase.ExecuteScalar("SELECT [Stock] FROM [PointCommodity] WHERE [Id] = @id", parameterList);

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
        public int StockDecrement(PointCommodityStockAdjustmentArgs args)
        {
            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@id", args.Id));
            parameterList.Add(new CommandParameter("@quantity", args.Quantity));

            _dataBase.ExecuteNonQuery("UPDATE [PointCommodity] SET [Stock] = [Stock] - @quantity WHERE [Id] = @id", parameterList);

            object objStock =
                _dataBase.ExecuteScalar("SELECT [Stock] FROM [PointCommodity] WHERE [Id] = @id", parameterList);

            if (objStock == null || objStock == DBNull.Value)
            {
                return 0;
            }
            else
            {
                int stock = int.Parse(objStock.ToString());
                if (stock < 0)
                {
                    _dataBase.ExecuteNonQuery("UPDATE [PointCommodity] SET [Stock] = 0 WHERE [Id] = @id", parameterList);
                    stock = 0;
                }
                return stock;
            }
        }

        public int GetTotalCount(DomainContext domainContext)
        {
            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@domainId", domainContext.Domain.Id));
            parameterList.Add(new CommandParameter("@appId", domainContext.AppId));

            int intCount = 0;
            _dataBase.ExecuteScalar<int>(
                "SELECT Count(1) FROM [PointCommodity] WHERE [Domain] = @domainId AND [AppId] = @appId",
                parameterList, (scalarValue) => { intCount = scalarValue; });

            return intCount;
        }

        #endregion

        #region Cart

        /// <summary>
        /// 添加到购物车
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public NormalResult<PointCommodityShoppingCartOperateResult> AddToCart(PointCommodityAddToCartArgs args)
        {
            NormalResult<PointCommodityShoppingCartOperateResult> result =
                new NormalResult<PointCommodityShoppingCartOperateResult>(false);

            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@domainId", args.Domain));
            parameterList.Add(new CommandParameter("@appId", args.AppId));
            parameterList.Add(new CommandParameter("@memberId", args.Member));
            parameterList.Add(new CommandParameter("@pointCommodity", args.PointCommodity));
            parameterList.Add(new CommandParameter("@quantity", args.Quantity));

            DataSet dsResult =
                _dataBase.ExecuteDataSet(CommandType.StoredProcedure, "AddPointCommodityToCart", parameterList,
                new string[] { "result", "quantity" });

            result.Reason = int.Parse(dsResult.Tables[0].Rows[0]["Result"].ToString());

            if (result.Reason == 0)
            {
                result.Data = new PointCommodityShoppingCartOperateResult();
                result.Data.Quantity = int.Parse(dsResult.Tables[1].Rows[0]["Quantity"].ToString());
                result.Data.PointCommodityCount = int.Parse(dsResult.Tables[1].Rows[0]["PointCommodityCount"].ToString());
                result.Success = true;
            }

            return result;
        }

        /// <summary>
        /// 从购物车 消减 指定数量的商品
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public NormalResult<PointCommodityShoppingCartOperateResult> RemoveFormCart(PointCommodityAddToCartArgs args)
        {
            NormalResult<PointCommodityShoppingCartOperateResult> result =
                new NormalResult<PointCommodityShoppingCartOperateResult>(false);

            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@domainId", args.Domain));
            parameterList.Add(new CommandParameter("@appId", args.AppId));
            parameterList.Add(new CommandParameter("@memberId", args.Member));
            parameterList.Add(new CommandParameter("@pointCommodity", args.PointCommodity));
            parameterList.Add(new CommandParameter("@quantity", args.Quantity));

            DataSet dsResult =
                _dataBase.ExecuteDataSet(CommandType.StoredProcedure, "RemovePointCommodityFromCart", parameterList,
                new string[] { "result", "quantity" });

            result.Reason = int.Parse(dsResult.Tables[0].Rows[0]["Result"].ToString());
            if (result.Reason == 0)
            {
                result.Data = new PointCommodityShoppingCartOperateResult();
                result.Data.Quantity = int.Parse(dsResult.Tables[1].Rows[0]["Quantity"].ToString());
                result.Success = true;
            }

            return result;
        }

        /// <summary>
        /// 设置购物车中指定商品的数量
        /// </summary>
        /// <returns></returns>
        public NormalResult SetCartItemQuantity(PointCommodityAddToCartArgs args)
        {
            NormalResult result = new NormalResult();

            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@domainId", args.Domain));
            parameterList.Add(new CommandParameter("@appId", args.AppId));
            parameterList.Add(new CommandParameter("@memberId", args.Member));
            parameterList.Add(new CommandParameter("@pointCommodity", args.PointCommodity));
            parameterList.Add(new CommandParameter("@quantity", args.Quantity));

            DataSet dsResult =
                _dataBase.ExecuteDataSet(CommandType.StoredProcedure, "SetPointCommodityCartItemQuantity", parameterList,
                new string[] { "result" });

            result.Reason = int.Parse(dsResult.Tables[0].Rows[0]["Result"].ToString());
            if (result.Reason == 0)
            {
                result.Success = true;
            }
            return result;
        }

        public GetItemListResult GetShoppingCartItemList(Guid memberId)
        {
            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@memberId", memberId));

            DataSet dsResult =
                _dataBase.ExecuteDataSet(CommandType.StoredProcedure, "GetPointCommodityCartItemList",
                parameterList, new string[] { "result" });

            GetItemListResult result = new GetItemListResult();

            result.ItemList = dsResult.Tables[0];

            return result;
        }

        /// <summary>
        /// 获取购物车中的商品 种类 数量
        /// </summary>
        /// <returns></returns>
        public int GetShoppingCartPointCommodityCount(DomainContext domainContext, Guid memberId)
        {
            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@domainId", domainContext.Domain.Id));
            parameterList.Add(new CommandParameter("@appId", domainContext.AppId));
            parameterList.Add(new CommandParameter("@memberId", memberId));

            int intCount = 0;
            _dataBase.ExecuteScalar<int>(
                "SELECT Count(1) FROM [PointCommodityShoppingCart] WHERE [Domain] = @domainId AND [AppId] = @appId AND [Member] = @memberId",
                parameterList, (scalarValue) => { intCount = scalarValue; });

            return intCount;
        }

        #endregion

        #region Order

        /// <summary>
        /// 创建订单
        /// 如果订单中存在库存不足、已下架的商品
        /// 则整个订单都不下单
        /// 0 成功下单 1 积分不足 2 库存不足 3 已下架商品 
        /// 默认在1小时后过期
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public NormalResult<PointCommodityCheckoutOrderResult> CheckoutOrder(PointCommodityCheckoutOrderArgs args)
        {
            NormalResult<PointCommodityCheckoutOrderResult> result =
                new NormalResult<PointCommodityCheckoutOrderResult>(false);

            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@domainId", args.DomainId));
            parameterList.Add(new CommandParameter("@appId", args.AppId));
            parameterList.Add(new CommandParameter("@memberId", args.MemberId));

            string xml = XMLHelper.XmlSerialize(args.ItemList);
            parameterList.Add(new CommandParameter("@itemList", xml));

            /*
 <ArrayOfPointCommodity>
  <PointCommodity>
    <Id>61384bd7-18ff-4cfa-80e6-7278ae713e43</Id>
    <Quantity>1</Quantity>
  </PointCommodity>
  <PointCommodity>
    <Id>5a2606e4-92e3-479b-85a5-594685e87ff7</Id>
    <Quantity>2</Quantity>
  </PointCommodity>
</ArrayOfPointCommodity>
            */

            DataSet dsResult =
                _dataBase.ExecuteDataSet(CommandType.StoredProcedure, "PointCommodityCreateOrder", parameterList,
                new string[] { "result" });

            result.Reason = int.Parse(dsResult.Tables[0].Rows[0]["Result"].ToString());
            if (result.Reason == 0)
            {
                result.Success = true;

                result.Data = new PointCommodityCheckoutOrderResult();
                result.Data.OrderNumber = dsResult.Tables[0].Rows[0]["OrderNumber"].ToString();
                result.Data.OrderId = Guid.Parse(dsResult.Tables[0].Rows[0]["OrderId"].ToString());
            }
            return result;
        }

        //public OrderPointCommodityResult OrderPointCommodity(Guid domainId, string appId, OrderPointCommodityArgs args)
        //{
        //    OrderPointCommodityResult result = new OrderPointCommodityResult();

        //    List<CommandParameter> parameterList = new List<CommandParameter>();
        //    parameterList.Add(new CommandParameter("@id", args.Id));
        //    parameterList.Add(new CommandParameter("@appId", appId));
        //    parameterList.Add(new CommandParameter("@memberId", args.MemberId));

        //    DataSet dsResult =
        //        _dataBase.ExecuteDataSet(CommandType.StoredProcedure, "OrderPointCommodity", parameterList, new string[] { "result" });

        //    result.Reason = int.Parse(dsResult.Tables[0].Rows[0]["Result"].ToString());

        //    if (result.Success)
        //    {
        //        result.OrderNumber = dsResult.Tables[0].Rows[0]["OrderNumber"].ToString();
        //        result.OrderId = dsResult.Tables[0].Rows[0]["OrderId"].ToString();
        //    }

        //    return result;
        //}

        public PointCommodityOrderEntity GetOrder(Guid id)
        {
            PointCommodityOrderEntity pointCommodityOrder = new PointCommodityOrderEntity();
            pointCommodityOrder.Id = id;

            if (_dataBase.Fill<PointCommodityOrderEntity>(pointCommodityOrder))
                return pointCommodityOrder;
            else
                return null;
        }

        public List<PointCommodityOrderItemEntity> GetOrderItemList(Guid orderId)
        {
            Dictionary<string, object> attachedWhere = new Dictionary<string, object>();
            attachedWhere.Add("Order", orderId);
            List<PointCommodityOrderItemEntity> list = _dataBase.Select<PointCommodityOrderItemEntity>(attachedWhere);
            return list;
        }

        public PointCommodityOrderEntity GetOrderByOrderNumber(Guid domainId, string appId, string orderNumber)
        {
            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@domainId", domainId));
            parameterList.Add(new CommandParameter("@appId", appId));
            parameterList.Add(new CommandParameter("@orderNumber", orderNumber));


            List<PointCommodityOrderEntity> pointCommodityOrderList = _dataBase.Select<PointCommodityOrderEntity>(
                "SELECT * FROM [PointCommodityOrder] WHERE [Domain] = @domainId AND [AppId] = @appId AND [OrderNumber] = @orderNumber",
                parameterList);

            if (pointCommodityOrderList.Count == 0)
                return null;
            else
                return pointCommodityOrderList[0];
        }

        /// <summary>
        /// 获取指定会员的订单列表
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public GetItemListResult GetMemberOrderList(GetMemberPointCommodityOrderListArgs args)
        {
            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@domainId", args.DomainId));
            parameterList.Add(new CommandParameter("@appId", args.AppId));
            parameterList.Add(new CommandParameter("@page", args.Page));
            parameterList.Add(new CommandParameter("@pageSize", args.PageSize));
            //parameterList.Add(new CommandParameter("@orderNumber", args.OrderNumber));
            parameterList.Add(new CommandParameter("@member", args.Member));
            // parameterList.Add(new CommandParameter("@status", args.Status));

            DataSet dsResult =
                _dataBase.ExecuteDataSet(CommandType.StoredProcedure, "GetPointCommodityOrderListByMember", parameterList, new string[] { "result" });

            if (dsResult.Tables[0].Rows.Count == 0 && args.Page > 1)
            {
                args.Page--;
                return GetMemberOrderList(args);
            }

            GetItemListResult result = new GetItemListResult();

            result.ItemList = dsResult.Tables[0];

            int totalCount = int.Parse(dsResult.Tables[1].Rows[0][0].ToString());
            result.TotalPage = totalCount / args.PageSize;
            if (totalCount % args.PageSize > 0)
            {
                result.TotalPage++;
            }
            result.Page = args.Page;

            return result;

            #region Data

            //GetItemListResult<PointCommodityOrderEntity> result = new GetItemListResult<PointCommodityOrderEntity>();

            //List<AttachedWhereItem> attachedWhere = new List<AttachedWhereItem>();
            //attachedWhere.Add(new AttachedWhereItem("Member", args.Member));

            //SqlExpressionPagingArgs pagingArgs = new SqlExpressionPagingArgs();
            //pagingArgs.Page = args.Page;
            //pagingArgs.PageSize = args.PageSize;

            //result.ItemList = _dataBase.Select<PointCommodityOrderEntity>(attachedWhere, pagingArgs);
            //result.TotalPage = pagingArgs.TotalPage;
            //result.Page = pagingArgs.Page;

            //if (result.ItemList.Count == 0 && result.Page > 1)
            //{
            //    args.Page--;
            //    return GetMemberOrderList(args);
            //}
            //else
            //{
            //    return result;
            //}

            #endregion
        }

        public GetItemListResult GetOrderList(GetPointCommodityOrderListArgs args)
        {
            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@domainId", args.DomainId));
            parameterList.Add(new CommandParameter("@appId", args.AppId));
            parameterList.Add(new CommandParameter("@page", args.Page));
            parameterList.Add(new CommandParameter("@pageSize", args.PageSize));
            parameterList.Add(new CommandParameter("@orderNumber", args.OrderNumber));
            parameterList.Add(new CommandParameter("@memberNickName", args.MemberNickName));
            parameterList.Add(new CommandParameter("@status", args.Status));

            DataSet dsResult =
                _dataBase.ExecuteDataSet(CommandType.StoredProcedure, "GetPointCommodityOrderList", parameterList, new string[] { "result" });

            if (dsResult.Tables[0].Rows.Count == 0 && args.Page > 1)
            {
                args.Page--;
                return GetOrderList(args);
            }

            GetItemListResult result = new GetItemListResult();

            result.ItemList = dsResult.Tables[0];

            int totalCount = int.Parse(dsResult.Tables[1].Rows[0][0].ToString());
            result.TotalPage = totalCount / args.PageSize;
            if (totalCount % args.PageSize > 0)
            {
                result.TotalPage++;
            }
            result.Page = args.Page;

            return result;
        }

        public List<PointCommodityOrderLogEntity> GetOrderLogList(Guid orderId)
        {
            Dictionary<string, object> attachedWhere = new Dictionary<string, object>();
            attachedWhere.Add("Order", orderId);
            List<PointCommodityOrderLogEntity> list = _dataBase.Select<PointCommodityOrderLogEntity>(attachedWhere);
            return list;
        }

        /// <summary>
        /// 获取过期的地未付款的订单列表
        /// </summary>
        /// <returns></returns>
        public List<PointCommodityOrderEntity> GetOverdueOrderList()
        {
            //过期时间向后放15分钟，防止正好有人卡在过期那个时间点上去支付
            List<PointCommodityOrderEntity> orderList = _dataBase.Select<PointCommodityOrderEntity>(
                "SELECT * FROM [PointCommodityOrder] WHERE [Status] = 4 AND dateadd(mi,15, [ExpireTime]) <= GETDATE()");

            return orderList;
        }

        /// <summary>
        /// 兑换订单
        /// 0 成功 1未知状态 2该订单已兑换过 3该订单已取消
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public int DealOrder(Guid orderId, Guid operatorUserId)
        {
            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@orderId", orderId));
            parameterList.Add(new CommandParameter("@operatorUserId", operatorUserId));

            DataSet dsResult =
                _dataBase.ExecuteDataSet(CommandType.StoredProcedure, "DealPointCommodityOrder", parameterList, new string[] { "result" });

            return int.Parse(dsResult.Tables[0].Rows[0]["Result"].ToString());
        }

        /// <summary>
        /// 发起订单支付
        /// 0 支付成功 1 积分不足 2 账户余额不足（小于想使用的金额） 
        /// 3 需要微信支付，微信支付下单成功（返回微支付订单号需转入微信支付画面）
        /// 4 订单状态不是待支付 5 订单已经过期 6 订单不存在 7 微信支付下单失败 
        /// 8 已经有待支付的微信支付订单了且微信支付金额同这次一样（返回微支付订单号需转入微信支付画面）
        /// </summary>
        /// <returns></returns>
        public NormalResult<PointCommodityOrderPayResult> OrderPay(PointCommodityOrderPayArgs args, AuthorizerPayConfig config)
        {
            NormalResult<PointCommodityOrderPayResult> result = new NormalResult<PointCommodityOrderPayResult>(false);

            //判断订单状态
            PointCommodityOrderEntity order = GetOrder(args.OrderId);
            if (order == null)
            {
                result.Reason = 6;
                return result;
            }

            if (order.Status != PointCommodityOrderStatus.NoPayment)
            {
                result.Reason = 4;
                InsertOrderLog(order, 4);
                return result;
            }

            //判断订单是否已经过期
            if (order.ExpireTime <= DateTime.Now)
            {
                result.Reason = 5;
                InsertOrderLog(order, 5);
                return result;
            }

            //积分是否足够在前端提交时就判断了，到这里后不会再次判断
            //在存储过程中判断即可

            //判断账户余额是否足够，是否大于想使用的金额
            //这里初步判断，存储过程中还要判断（行锁定）
            int cashAccount = _memberManager.GetMemberCashAccountBalances(args.MemberId);
            if (cashAccount < args.CashAccountFee)
            {
                result.Reason = 2;
                InsertOrderLog(order, 2);
                return result;
            }

            //判断是否已经有待支付的微信支付订单了
            //如果有，但已过期，则关闭原订单
            //如果有，但金额和这次不一样，则关闭原订单
            //如果有，且未过期且金额与本次一致，返回微支付订单号需转入微信支付画面
            string outTradeNo = args.OrderId.ToString().Replace("-", "");
            List<PayOrderEntity> existPayOrderList = _payManager.GetPayOrderByOutTradeNo(outTradeNo);
            foreach (PayOrderEntity existPayOrder in existPayOrderList)
            {
                if (existPayOrder.TimeExpire <= DateTime.Now)
                {
                    _payManager.ClosePayOrder(existPayOrder.OutTradeNo, config);
                    continue;
                }

                if (existPayOrder.TotalFee != args.WeixinPayFee)
                {
                    _payManager.ClosePayOrder(existPayOrder.OutTradeNo, config);
                    continue;
                }

                result.Reason = 8;
                result.Data = new PointCommodityOrderPayResult();
                result.Data.PayOrderId = existPayOrder.Id;
                result.Success = true;
                return result;
            }

            //判断是否需要微信支付，如果需要则直接生成微信支付订单并返回订单号到前端跳转
            //订单的完成在支付成功的回调中处理
            //微信支付成功后，还是先把钱存到账户余额，然后再从余额中下帐成单
            if (args.WeixinPayFee > 0)
            {
                #region 创建微信支付定单

                CreatePayOrderArgs createPayOrderArgs = new CreatePayOrderArgs();

                createPayOrderArgs.MemberId = args.MemberId;
                createPayOrderArgs.OpenId = args.OpenId;
                createPayOrderArgs.Fee = args.WeixinPayFee;
                createPayOrderArgs.SpbillCreateIp = args.SpbillCreateIp;

                createPayOrderArgs.OrderType = EnumPayOrderType.PointCommodity;
                createPayOrderArgs.Body = order.OrderNumber;
                createPayOrderArgs.OutTradeNo = outTradeNo;
                NormalResult<CreatePayOrderResult> createPayOrderResult = _payManager.CreatePayOrder(createPayOrderArgs, config);

                if (createPayOrderResult.Success == false)
                {
                    InsertOrderLog(order, 7, createPayOrderResult.Message);
                    result.Reason = 7;
                    result.Message = createPayOrderResult.Message;
                    return result;
                }

                InsertOrderLog(order, 3);
                result.Reason = 3;
                result.Data = new PointCommodityOrderPayResult();
                result.Data.PayOrderId = createPayOrderResult.Data.PayOrderId;
                result.Success = true;
                return result;

                #endregion
            }

            //如果不需要微信支付，则转到存储过程开始处理
            NormalResult checkoutResult = Checkout(order);
            result.Reason = checkoutResult.Reason;

            if (result.Reason == 0)
            {
                result.Success = true;
            }

            return result;
        }

        internal NormalResult ProcessWeixinPayNotify(PayOrderEntity payOrder)
        {
            NormalResult result = new NormalResult(false);

            Guid orderGuid;
            if (String.IsNullOrEmpty(payOrder.OutTradeNo) || Guid.TryParse(payOrder.OutTradeNo, out orderGuid) == false)
            {
                result.Reason = 6;
                result.Message = "OutTradeNo 不存在或无效。" + (payOrder.OutTradeNo == null ? String.Empty : payOrder.OutTradeNo);
                return result;
            }

            PointCommodityOrderEntity order = GetOrder(orderGuid);
            if (order == null)
            {
                result.Reason = 6;
                result.Message = "该积分商城订单不存在。";
                return result;
            }

            if (order.ExpireTime <= DateTime.Now)
            {
                InsertOrderLog(order, 5);
                result.Reason = 5;
                result.Message = "该积分商城订单已过期。";
                return result;
            }

            if (order.Status != PointCommodityOrderStatus.NoPayment)
            {
                InsertOrderLog(order, 4);
                result.Reason = 4;
                result.Message = "该积分商城订单状态不是待支付。";
                return result;
            }

            result = Checkout(order);

            switch (result.Reason)
            {
                case 0:
                    result.Message = "微信支付成功。";
                    result.Success = true;
                    break;
                case 1:
                    result.Message = "积分不足。";
                    break;
                case 2:
                    result.Message = "账户余额不足。";
                    break;
                case 4:
                    result.Message = "该积分商城订单状态不是待支付。";
                    break;
                case 5:
                    result.Message = "该积分商城订单已过期。";
                    break;
                case 6:
                    result.Message = "该积分商城订单不存在。";
                    break;
                default:
                    result.Message = "未知错误。";
                    break;
            }

            return result;

        }

        /// <summary>
        /// 最终买单
        /// 走到这里表示不需要微信支付，或微信支付已经完成并到帐
        /// 0 支付成功 1 积分不足 2 账户余额不足 4 订单状态不是待支付 5 订单已经过期 6 订单不存在
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        private NormalResult Checkout(PointCommodityOrderEntity order)
        {
            NormalResult result = new NormalResult(false);

            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@orderId", order.Id));

            DataSet dsResult =
                _dataBase.ExecuteDataSet(CommandType.StoredProcedure, "PointCommodityCheckoutOrder", parameterList, new string[] { "result" });

            result.Reason = int.Parse(dsResult.Tables[0].Rows[0]["Result"].ToString());

            InsertOrderLog(order, result.Reason);

            if (result.Reason == 0)
            {
                result.Success = true;
            }

            return result;
        }

        private void InsertOrderLog(PointCommodityOrderEntity order, int reason)
        {
            InsertOrderLog(order, reason, null);
        }

        private void InsertOrderLog(PointCommodityOrderEntity order, int reason, string exMessage)
        {
            if (order == null)
                return;

            string message;

            /// 0 支付成功 1 积分不足 2 账户余额不足（小于想使用的金额） 3 需要微信支付，微信支付下单成功（返回微支付订单号需转入微信支付画面）
            /// 4 订单状态不是待支付 5 订单已经过期 6 订单不存在 7 微信支付下单失败

            switch (reason)
            {
                case 0:
                    message = "支付成功。";
                    break;
                case 1:
                    message = "积分不足。";
                    break;
                case 2:
                    message = "账户余额不足。";
                    break;
                case 3:
                    message = "发起微信支付。";
                    break;
                case 4:
                    message = "订单状态不是待支付。";
                    break;
                case 5:
                    message = "订单已经过期。";
                    break;
                case 7:
                    message = "微信支付下单失败。";
                    break;
                default:
                    message = "未知错误：" + reason;
                    break;
            }

            if (String.IsNullOrEmpty(exMessage) == false)
            {
                message += " > " + exMessage;
            }

            PointCommodityOrderLogEntity log = new PointCommodityOrderLogEntity();
            log.Domain = order.Domain;
            log.AppId = order.AppId;
            log.Order = order.Id;
            log.Time = DateTime.Now;
            log.Message = message;
            _dataBase.Insert(log);
        }

        #endregion
    }
}
