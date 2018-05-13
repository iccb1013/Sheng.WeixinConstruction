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
    public class CouponManager
    {
        private static readonly CouponManager _instance = new CouponManager();
        public static CouponManager Instance
        {
            get { return _instance; }
        }

        private DatabaseWrapper _dataBase = ServiceUnity.Instance.Database;

        private CouponManager()
        {

        }

        public void CreateCoupon(CouponEntity coupon)
        {
            if (coupon == null)
            {
                Debug.Assert(false, "coupon 为空");
                return;
            }

            coupon.CreateTime = DateTime.Now;
            _dataBase.Insert(coupon);
        }

        public NormalResult UpdateCoupon(CouponEntity coupon)
        {
            if (coupon == null)
            {
                Debug.Assert(false, "coupon 为空");
                return new NormalResult("参数错误");
            }

            SqlStructureBuild sqlBuild = new SqlStructureBuild();
            sqlBuild.Table = "Coupon";
            sqlBuild.Type = SqlExpressionType.Update;
            sqlBuild.AddParameter("Id", coupon.Id, true);
            sqlBuild.AddParameter("Name", coupon.Name);
            sqlBuild.AddParameter("ImageUrl", coupon.ImageUrl);
            sqlBuild.AddParameter("Condition", coupon.Condition);
            sqlBuild.AddParameter("Description", coupon.Description);
            sqlBuild.AddParameter("InfiniteStock", coupon.InfiniteStock);
            sqlBuild.AddParameter("Remark", coupon.Remark);
            _dataBase.ExcuteSqlExpression(sqlBuild.GetSqlExpression());

            return new NormalResult();
        }

        public void RemoveCoupon(Guid id)
        {
            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@id", id));

            _dataBase.ExecuteNonQuery("UPDATE [Coupon] SET [Removed] = 1 WHERE [Id] = @id",
               parameterList);
        }

        /// <summary>
        /// 返回调整后的最新库存数量
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public int StockIncrement(CouponStockAdjustmentArgs args)
        {
            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@id", args.Id));
            parameterList.Add(new CommandParameter("@quantity", args.Quantity));

            _dataBase.ExecuteNonQuery("UPDATE [Coupon] SET [Stock] = [Stock] + @quantity WHERE [Id] = @id", parameterList);

            object objStock =
                _dataBase.ExecuteScalar("SELECT [Stock] FROM [Coupon] WHERE [Id] = @id", parameterList);

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
        public int StockDecrement(CouponStockAdjustmentArgs args)
        {
            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@id", args.Id));
            parameterList.Add(new CommandParameter("@quantity", args.Quantity));

            _dataBase.ExecuteNonQuery("UPDATE [Coupon] SET [Stock] = [Stock] - @quantity WHERE [Id] = @id", parameterList);

            object objStock =
                _dataBase.ExecuteScalar("SELECT [Stock] FROM [Coupon] WHERE [Id] = @id", parameterList);

            if (objStock == null || objStock == DBNull.Value)
            {
                return 0;
            }
            else
            {
                int stock = int.Parse(objStock.ToString());
                if (stock < 0)
                {
                    _dataBase.ExecuteNonQuery("UPDATE [Coupon] SET [Stock] = 0 WHERE [Id] = @id", parameterList);
                    stock = 0;
                }
                return stock;
            }
        }

        public CouponEntity GetCoupon(Guid id)
        {
            CouponEntity coupon = new CouponEntity();
            coupon.Id = id;

            if (_dataBase.Fill<CouponEntity>(coupon))
                return coupon;
            else
                return null;
        }

        public CouponRecordEntity CouponRecord(Guid id)
        {
            CouponRecordEntity couponRecord = new CouponRecordEntity();
            couponRecord.Id = id;

            if (_dataBase.Fill<CouponRecordEntity>(couponRecord))
                return couponRecord;
            else
                return null;
        }

        public CouponRecordEntity GetCouponRecordBySerialNumber(Guid domainId, string appId, string serialNumber)
        {
            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@domainId", domainId));
            parameterList.Add(new CommandParameter("@appId", appId));
            parameterList.Add(new CommandParameter("@serialNumber", serialNumber));

            List<CouponRecordEntity> recordList = _dataBase.Select<CouponRecordEntity>(
                "SELECT * FROM [CouponRecord] WHERE [Domain] = @domainId AND [AppId] = @appId AND [SerialNumber] = @serialNumber",
                parameterList);

            if (recordList.Count == 0)
                return null;
            else
                return recordList[0];
        }

        public GetItemListResult GetCouponList(GetCouponListArgs args)
        {
            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@domainId", args.DomainId));
            parameterList.Add(new CommandParameter("@appId", args.AppId));
            parameterList.Add(new CommandParameter("@page", args.Page));
            parameterList.Add(new CommandParameter("@pageSize", args.PageSize));
            parameterList.Add(new CommandParameter("@name", args.Name));

            DataSet dsResult =
                _dataBase.ExecuteDataSet(CommandType.StoredProcedure, "GetCouponList", parameterList, new string[] { "result" });

            if (dsResult.Tables[0].Rows.Count == 0 && args.Page > 1)
            {
                args.Page--;
                return GetCouponList(args);
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
        /// 获取指定会员的优惠券列表
        /// </summary>
        /// <returns></returns>
        public GetItemListResult GetMemberCouponList(GetMemberCouponListArgs args)
        {
            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@domainId", args.DomainId));
            parameterList.Add(new CommandParameter("@appId", args.AppId));
            parameterList.Add(new CommandParameter("@page", args.Page));
            parameterList.Add(new CommandParameter("@pageSize", args.PageSize));
            parameterList.Add(new CommandParameter("@memberId", args.MemberId));
            parameterList.Add(new CommandParameter("@status", args.Status));

            DataSet dsResult =
                _dataBase.ExecuteDataSet(CommandType.StoredProcedure, "GetMemberCouponList", parameterList, new string[] { "result" });

            if (dsResult.Tables[0].Rows.Count == 0 && args.Page > 1)
            {
                args.Page--;
                return GetMemberCouponList(args);
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
        /// 派发
        /// 0 分发成功 1 域或APPID不对应 2 库存不足 3 卡券已删除 4 卡券不存在 5 派发张数无效
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public NormalResult Distribute(CouponDistributeArgs args)
        {
            NormalResult result = new NormalResult();

            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@domainId", args.DomainId));
            parameterList.Add(new CommandParameter("@appId", args.AppId));
            parameterList.Add(new CommandParameter("@couponId", args.CouponId));
            parameterList.Add(new CommandParameter("@memberId", args.MemberId));
            parameterList.Add(new CommandParameter("@distributeUser", args.DistributeUser));
            parameterList.Add(new CommandParameter("@limitedTime", args.LimitedTime));
            parameterList.Add(new CommandParameter("@distributeIP", args.DistributeIP));
            parameterList.Add(new CommandParameter("@count", args.Count));

            DataSet dsResult =
               _dataBase.ExecuteDataSet(CommandType.StoredProcedure, "CouponDistribute", parameterList,
               new string[] { "result" });

            result.Reason = int.Parse(dsResult.Tables[0].Rows[0]["Result"].ToString());
            result.Success = result.Reason == 0;

            return result;
        }

        /// <summary>
        /// 核销
        /// </summary>
        /// <returns></returns>
        public NormalResult Charge(CouponChargeArgs args)
        {
            NormalResult result = new NormalResult();

            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@domainId", args.Domain));
            parameterList.Add(new CommandParameter("@appId", args.AppId));
            parameterList.Add(new CommandParameter("@couponRecordId", args.CouponRecordId));
            parameterList.Add(new CommandParameter("@chargeUser", args.ChargeUser));
            parameterList.Add(new CommandParameter("@chargeIP", args.ChargeIP));

            DataSet dsResult =
               _dataBase.ExecuteDataSet(CommandType.StoredProcedure, "CouponCharge", parameterList,
               new string[] { "result" });

            result.Reason = int.Parse(dsResult.Tables[0].Rows[0]["Result"].ToString());
            result.Success = result.Reason == 0;

            return result;
        }

        public GetItemListResult GetDistributedCouponList(GetDistributedCouponListArgs args)
        {
            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@domainId", args.DomainId));
            parameterList.Add(new CommandParameter("@appId", args.AppId));
            parameterList.Add(new CommandParameter("@page", args.Page));
            parameterList.Add(new CommandParameter("@pageSize", args.PageSize));
            parameterList.Add(new CommandParameter("@couponId", args.CouponId));
            parameterList.Add(new CommandParameter("@serialNumber", args.SerialNumber));
            parameterList.Add(new CommandParameter("@memberNickName", args.MemberNickName));
            if (args.Status.HasValue)
                parameterList.Add(new CommandParameter("@status", args.Status.Value));
            else
                parameterList.Add(new CommandParameter("@status", DBNull.Value));

            DataSet dsResult =
                _dataBase.ExecuteDataSet(CommandType.StoredProcedure, "GetDistributedCouponList", parameterList,
                new string[] { "result" });

            if (dsResult.Tables[0].Rows.Count == 0 && args.Page > 1)
            {
                args.Page--;
                return GetDistributedCouponList(args);
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

        public string GetRecordQRCodeImageUrl(Guid recordId)
        {
            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@id", recordId));

            string url = null;
            _dataBase.ExecuteScalar<string>("SELECT [QRCodeImageUrl] FROM [CouponRecord] WHERE [Id] = @id",
                parameterList, (scalarString) => { url = scalarString; });

            return url;
        }

        public void UpdateRecordQRCodeImageUrl(Guid recordId, string url)
        {
            SqlStructureBuild sqlBuild = new SqlStructureBuild();
            sqlBuild.Table = "CouponRecord";
            sqlBuild.Type = SqlExpressionType.Update;
            sqlBuild.AddParameter("Id", recordId, true);
            sqlBuild.AddParameter("QRCodeImageUrl", url);
            _dataBase.ExcuteSqlExpression(sqlBuild.GetSqlExpression());
        }

        public int GetValidCouponCountByMember(Guid domainId, string appId, Guid memberId)
        {
            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@domainId", domainId));
            parameterList.Add(new CommandParameter("@appId", appId));
            parameterList.Add(new CommandParameter("@memberId", memberId));

            int intCount = 0;
            _dataBase.ExecuteScalar<int>(
                "SELECT Count(1) FROM [CouponRecord] WHERE [Domain] = @domainId AND [AppId] = @appId AND [Member] = @memberId AND [Status] = 0",
                parameterList, (scalarValue) => { intCount = scalarValue; });

            return intCount;
        }

    }
}
