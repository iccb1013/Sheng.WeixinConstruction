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
using Sheng.WeixinConstruction.ApiContract;
using Sheng.WeixinConstruction.Infrastructure;
using Sheng.WeixinConstruction.Service;
using Sheng.WeixinConstruction.WeixinContract;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sheng.WeixinConstruction.Core
{
    public class MaterialManager
    {
        private static readonly MaterialManager _instance = new MaterialManager();
        public static MaterialManager Instance
        {
            get { return _instance; }
        }

        private DatabaseWrapper _dataBase = ServiceUnity.Instance.Database;

        private MaterialManager()
        {

        }

        public void AddNormalMaterial(NormalMaterialEntity normalMaterial)
        {
            if (normalMaterial == null)
            {
                Debug.Assert(false, "normalMaterial == null");
                return;
            }

            _dataBase.Insert(normalMaterial);
        }

        public NormalMaterialEntity GetNormalMaterial(Guid id)
        {
            NormalMaterialEntity normalMaterial = new NormalMaterialEntity();
            normalMaterial.Id = id;
            if (_dataBase.Fill<NormalMaterialEntity>(normalMaterial) == false)
            {
                return null;
            }

            return normalMaterial;
        }

        public GetItemListResult<NormalMaterialEntity> GetNormalMaterialList(Guid domainId, string appId, GetMaterialListArgs args)
        {
            GetItemListResult<NormalMaterialEntity> result = new GetItemListResult<NormalMaterialEntity>();

            List<AttachedWhereItem> attachedWhere = new List<AttachedWhereItem>();
            attachedWhere.Add(new AttachedWhereItem("Domain", domainId));
            attachedWhere.Add(new AttachedWhereItem("AppId", appId));
            attachedWhere.Add(new AttachedWhereItem("Type", EnumHelper.GetEnumMemberValue(args.Type)));

            SqlExpressionPagingArgs pagingArgs = new SqlExpressionPagingArgs();
            pagingArgs.Page = args.Page;
            pagingArgs.PageSize = args.PageSize;

            result.ItemList = _dataBase.Select<NormalMaterialEntity>(attachedWhere, pagingArgs);
            result.TotalPage = pagingArgs.TotalPage;
            result.Page = pagingArgs.Page;

            if (result.ItemList.Count == 0 && result.Page > 1)
            {
                args.Page--;
                return GetNormalMaterialList(domainId, appId, args);
            }
            else
            {
                return result;
            }
        }

        public string RemoveNormalMaterial(DomainContext domainContext, Guid id, string mediaId)
        {
            RequestApiResult requestApiResult = MaterialApiWrapper.RemoveMaterial(domainContext, mediaId);
            if (requestApiResult.Success == false)
            {
                return requestApiResult.Message;
            }

            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@id", id));
            parameterList.Add(new CommandParameter("@mediaId", mediaId));

            _dataBase.ExecuteNonQuery("DELETE FROM [NormalMaterial] WHERE [Id] = @id AND [MediaId] = @mediaId",
                parameterList);

            return null;
        }

        /// <summary>
        /// 仅保存在不地，不发送到微信后台
        /// </summary>
        /// <param name="domainContext"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public AddArticleMaterialResult AddArticleMaterial(DomainContext domainContext, ArticleMaterialEntity args)
        {
            AddArticleMaterialResult result = new AddArticleMaterialResult();

            if (args == null || args.ArticleList == null)
            {
                result.Message = "参数错误。";
                return result;
            }

            //保存本地数据库
            args.WeixinStatus = 0;
            _dataBase.Insert(args);

            for (int i = 0; i < args.ArticleList.Count; i++)
            {
                ArticleMaterialItemEntity article = args.ArticleList[i];
                article.Index = i;
                article.Domain = domainContext.Domain.Id;
                article.ArticleMaterial = args.Id;
                if (article.ImgMappingList != null)
                {
                    article.ImgMapping = JsonHelper.Serializer(article.ImgMappingList);
                }
                _dataBase.Insert(article);
            }

            result.Success = true;
            result.Id = args.Id;
            return result;
        }

        /// <summary>
        /// 发布指定的图文素材到微信后台
        /// </summary>
        /// <param name="domainContext"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public PublishArticleMaterialResult PublishArticleMaterial(DomainContext domainContext, Guid id)
        {
            PublishArticleMaterialResult result = new PublishArticleMaterialResult();

            ArticleMaterialEntity args = GetArticleMaterial(id);

            if (args == null || args.ArticleList == null)
            {
                result.Message = "参数错误。";
                return result;
            }

            if (args.WeixinStatus == 0)
            {
                //向微信后台发送
                string addToWeixin = AddArticleMaterialToWeixin(domainContext, args);
                if (String.IsNullOrEmpty(addToWeixin))
                {
                    result.Success = true;
                    result.MediaId = args.MediaId;
                }
                else
                {
                    result.Message = addToWeixin;
                }
            }
            else if (args.WeixinStatus == 1)
            {
                //TODO:取回URL
            }
            else if (args.WeixinStatus == 2)
            {
                result.Message = "该图文消息已经发布过了。";
            }
            return result;
        }

        /// <summary>
        /// 仅未发布的素材才使用此方法
        /// 发布过的素材只能单图文更新
        /// </summary>
        /// <param name="domainContext"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public string UpdateArticleMaterial(DomainContext domainContext, ArticleMaterialEntity args)
        {
            //受限于微信接口，更新图文素材只能删除原素材重新添加
            //因为重新添加会导致media_id发生变化，所以本地在使用时，要用自己的ID并维护不变
            //但是！如果把微信素材库的素材删除，会导致已经发送给关注者的连接失效，打不开
            //所以这里只删除本地的，微信后台的保持不删除
            //但是这种方式还有一个问题，就是修改是假修改，已经发给用户的素材事实上不会变化 
            //而在微信后台修改图文内容是可以实现真修改的。
            //目前只能把本地保存和发布微信后台分开，一旦发布了微信后台，就不再允许添加删除文章

            if (args == null || args.ArticleList == null)
            {
                return "参数错误";
            }

            //先确保保存本地数据库，防止微信后台发布失败编辑的文章全部丢失
            //主表可UPDATE相关子段
            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@id", args.Id));
            parameterList.Add(new CommandParameter("@name", args.Name));
            //parameterList.Add(new CommandParameter("@weixinStatus", 0));
            //[WeixinStatus] = @weixinStatus
            _dataBase.ExecuteNonQuery("UPDATE [ArticleMaterial] SET [Name] = @name WHERE [Id] = @id",
                parameterList);

            //子表删除重新添加
            _dataBase.ExecuteNonQuery("DELETE FROM [ArticleMaterialItem] WHERE [ArticleMaterial] = @id",
               parameterList);

            for (int i = 0; i < args.ArticleList.Count; i++)
            {
                ArticleMaterialItemEntity article = args.ArticleList[i];
                article.Index = i;
                article.Domain = domainContext.Domain.Id;
                article.ArticleMaterial = args.Id;
                if (article.ImgMappingList != null)
                {
                    article.ImgMapping = JsonHelper.Serializer(article.ImgMappingList);
                }
                _dataBase.Insert(article);
            }

            return null;

            //调用微信接口删除现有素材
            //if (String.IsNullOrEmpty(args.MediaId) == false)
            //{
            //    RequestApiResult requestApiResult = MaterialApiWrapper.RemoveMaterial(domainContext, args.MediaId);
            //    if (requestApiResult.Success == false)
            //    {
            //        return requestApiResult.Message;
            //    }
            //}

            //调用微信添加素材接口重新添加
            //string addToWeixin = AddArticleMaterialToWeixin(domainContext, args);

            //return addToWeixin;
        }

        /// <summary>
        /// 对于已经提交微信后台的图文素材，只能按篇更新现有文章
        /// </summary>
        /// <param name="domainContext"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public string UpdateArticleMaterialItem(DomainContext domainContext, ArticleMaterialEntity args)
        {
            //受限于微信接口，更新图文素材只能删除原素材重新添加
            //因为重新添加会导致media_id发生变化，所以本地在使用时，要用自己的ID并维护不变
            //但是！如果把微信素材库的素材删除，会导致已经发送给关注者的连接失效，打不开
            //所以这里只删除本地的，微信后台的保持不删除
            //但是这种方式还有一个问题，就是修改是假修改，已经发给用户的素材事实上不会变化 
            //而在微信后台修改图文内容是可以实现真修改的。
            //目前只能把本地保存和发布微信后台分开，一旦发布了微信后台，就不再允许添加删除文章

            if (args == null || args.ArticleList == null)
            {
                return "参数错误";
            }

            for (int i = 0; i < args.ArticleList.Count; i++)
            {
                ArticleMaterialItemEntity article = args.ArticleList[i];
                article.Index = i;
                if (article.ImgMappingList != null)
                {
                    article.ImgMapping = JsonHelper.Serializer(article.ImgMappingList);
                }
                _dataBase.Update(article);
            }

            //发布微信后台
            for (int i = 0; i < args.ArticleList.Count; i++)
            {
                ArticleMaterialItemEntity article = args.ArticleList[i];

                //替换其中的图片地址为上传到微信服务器的地址
                if (article.ImgMappingList != null)
                {
                    foreach (var imgMappingItem in article.ImgMappingList)
                    {
                        if (String.IsNullOrEmpty(imgMappingItem.FileUrl) || String.IsNullOrEmpty(imgMappingItem.WeixinUrl))
                            continue;

                        article.Content =
                            article.Content.Replace(imgMappingItem.FileUrl, imgMappingItem.WeixinUrl);
                    }
                }

                WeixinUpdateArticleMaterialArgs weixinArgs = new WeixinUpdateArticleMaterialArgs();
                weixinArgs.MediaId = args.MediaId;
                weixinArgs.Index = i;
                weixinArgs.Article = article;

                RequestApiResult updateArticleMaterialResult =
                    MaterialApiWrapper.UpdateArticleMaterial(domainContext, weixinArgs);

                if (updateArticleMaterialResult.Success == false)
                    return updateArticleMaterialResult.Message;
            }

            return null;
        }

        /// <summary>
        /// 发布图文消息到微信后台
        /// </summary>
        /// <param name="domainContext"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        private string AddArticleMaterialToWeixin(DomainContext domainContext, ArticleMaterialEntity args)
        {
            //向微信后台发送
            WeixinAddArticleMaterialArgs weixinArgs = new WeixinAddArticleMaterialArgs();
            weixinArgs.ArticleList = new List<WeixinArticleMaterial>();
            for (int i = 0; i < args.ArticleList.Count; i++)
            {
                ArticleMaterialItemEntity item = args.ArticleList[i];
                item.Index = i;

                //替换其中的图片地址为上传到微信服务器的地址
                if (item.ImgMappingList != null)
                {
                    foreach (var imgMappingItem in item.ImgMappingList)
                    {
                        if (String.IsNullOrEmpty(imgMappingItem.FileUrl) || String.IsNullOrEmpty(imgMappingItem.WeixinUrl))
                            continue;

                        item.Content = 
                            item.Content.Replace(imgMappingItem.FileUrl, imgMappingItem.WeixinUrl);
                    }
                }

                weixinArgs.ArticleList.Add(item);
            }

            RequestApiResult<WeixinAddArticleMaterialResult> addArticleMaterialResult =
                MaterialApiWrapper.AddArticleMaterial(domainContext, weixinArgs);

            if (addArticleMaterialResult.Success)
            {
                //得到图文素材的MediaId
                string mediaId = addArticleMaterialResult.ApiResult.MediaId;
                args.MediaId = mediaId;

                //更新图文素材表中的MediaId字段
                List<CommandParameter> parameterList = new List<CommandParameter>();
                parameterList.Add(new CommandParameter("@id", args.Id));
                parameterList.Add(new CommandParameter("@mediaId", mediaId));

                _dataBase.ExecuteNonQuery("UPDATE [ArticleMaterial] SET [MediaId] = @mediaId,[WeixinStatus] = 1 WHERE [Id] = @id",
                    parameterList);

                //因为微信API在添加图文后不返回URL，所以需要通过获取素材接口再次获取以便得到URL
                //GetArticleMaterial
                RequestApiResult<WeixinArticleMaterialListItemContent> getArticleMaterialResult =
                    MaterialApiWrapper.GetArticleMaterial(domainContext, mediaId);

                if (getArticleMaterialResult.Success)
                {
                    for (int i = 0; i < getArticleMaterialResult.ApiResult.ItemList.Count; i++)
                    {
                        WeixinArticleMaterial item = getArticleMaterialResult.ApiResult.ItemList[i];

                        parameterList = new List<CommandParameter>();
                        parameterList.Add(new CommandParameter("@articleMaterial", args.Id));
                        parameterList.Add(new CommandParameter("@index", i));
                        parameterList.Add(new CommandParameter("@url", item.Url));

                        _dataBase.ExecuteNonQuery("UPDATE [ArticleMaterialItem] SET [Url] = @url WHERE [ArticleMaterial] = @articleMaterial AND [Index] = @index",
                            parameterList);
                    }

                    _dataBase.ExecuteNonQuery("UPDATE [ArticleMaterial] SET [WeixinStatus] = 2 WHERE [Id] = @articleMaterial",
                        parameterList);
                }
                else
                {
                    return getArticleMaterialResult.Message;
                }
            }
            else
            {
                return addArticleMaterialResult.Message;
            }

            return null;
        }

        public ArticleMaterialEntity GetArticleMaterial(Guid id)
        {
            ArticleMaterialEntity articleMaterial = new ArticleMaterialEntity();
            articleMaterial.Id = id;
            if (_dataBase.Fill<ArticleMaterialEntity>(articleMaterial) == false)
            {
                return null;
            }

            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@id", articleMaterial.Id));

            List<ArticleMaterialItemEntity> itemList =
                   _dataBase.Select<ArticleMaterialItemEntity>(
                   "SELECT * FROM [ArticleMaterialItem] WHERE [ArticleMaterial] = @id  ORDER BY [Index]",
                   parameterList);

            foreach (ArticleMaterialItemEntity item in itemList)
            {
                if (String.IsNullOrEmpty(item.ImgMapping) == false)
                {
                    item.ImgMappingList = JsonHelper.Deserialize<List<MaterialImgMapping>>(item.ImgMapping);
                }
            }

            articleMaterial.ArticleList = itemList;

            return articleMaterial;
        }

        public GetItemListResult<ArticleMaterialEntity> GetArticleMaterialList(Guid domainId, string appId, GetArticleMaterialListArgs args)
        {
            GetItemListResult<ArticleMaterialEntity> result = new GetItemListResult<ArticleMaterialEntity>();

            List<AttachedWhereItem> attachedWhere = new List<AttachedWhereItem>();
            attachedWhere.Add(new AttachedWhereItem("Domain", domainId));
            attachedWhere.Add(new AttachedWhereItem("AppId", appId));
            if (args.ExceptUnpublished)
            {
                attachedWhere.Add(new AttachedWhereItem("WeixinStatus", 2));
            }

            SqlExpressionPagingArgs pagingArgs = new SqlExpressionPagingArgs();
            pagingArgs.Page = args.Page;
            pagingArgs.PageSize = args.PageSize;

            result.ItemList = _dataBase.Select<ArticleMaterialEntity>(attachedWhere, pagingArgs);
            result.TotalPage = pagingArgs.TotalPage;
            result.Page = pagingArgs.Page;

            if (result.ItemList.Count == 0 && result.Page > 1)
            {
                args.Page--;
                return GetArticleMaterialList(domainId, appId, args);
            }
            else
            {
                if (result.ItemList.Count > 0)
                {
                    string sql = "SELECT [Id],[ArticleMaterial],[Title],[ThumbMediaId],[ThumbUrl],[ThumbName],[Url],[Index] FROM [ArticleMaterialItem] WHERE ";
                    List<CommandParameter> parameterList = new List<CommandParameter>();
                    for (int i = 0; i < result.ItemList.Count; i++)
                    {
                        parameterList.Add(new CommandParameter("@id" + i, result.ItemList[i].Id));
                        sql += " [ArticleMaterial] = @id" + i;
                        if (i < result.ItemList.Count - 1)
                        {
                            sql += " OR ";
                        }
                    }
                    sql += " ORDER BY [Index]";

                    List<ArticleMaterialItemEntity> itemList =
                        _dataBase.Select<ArticleMaterialItemEntity>(sql, parameterList);

                    foreach (ArticleMaterialEntity item in result.ItemList)
                    {
                        item.ArticleList = (from c in itemList where c.ArticleMaterial == item.Id select c).ToList();
                    }
                }

                return result;
            }
        }

        public string RemoveArticleMaterial(DomainContext domainContext, Guid id, string mediaId)
        {
            //未发布的素材无 mediaId
            if (String.IsNullOrEmpty(mediaId) == false)
            {
                RequestApiResult requestApiResult = MaterialApiWrapper.RemoveMaterial(domainContext, mediaId);
                if (requestApiResult.Success == false)
                {
                    return requestApiResult.Message;
                }
            }

            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@id", id));

            _dataBase.ExecuteNonQuery("DELETE FROM [ArticleMaterial] WHERE [Id] = @id",
                parameterList);

            _dataBase.ExecuteNonQuery("DELETE FROM [ArticleMaterialItem] WHERE [ArticleMaterial] = @id",
               parameterList);

            return null;
        }

        public string GetArticleMaterialMediaId(Guid id)
        {
            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@id", id));

            object objMediaId = _dataBase.ExecuteScalar("SELECT [MediaId] FROM ArticleMaterial WHERE [Id] = @id",
                parameterList);

            if (objMediaId != null && objMediaId != DBNull.Value)
                return objMediaId.ToString();
            else
                return String.Empty;
        }
    }
}
