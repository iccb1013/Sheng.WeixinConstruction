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
using System.Web;
using System.Web.Mvc;

namespace Sheng.WeixinConstruction.Management.Shell.Areas.Api.Controllers
{
    public class InformationController : BasalController
    {
        private static readonly DomainManager _domainManager = DomainManager.Instance;
        private static readonly InformationManager _informationManager = InformationManager.Instance;
        private static readonly OperatedLogManager _operatedLogManager = OperatedLogManager.Instance;

        public ActionResult CreateInformation()
        {
            InformationEntity informationEntity = RequestArgs<InformationEntity>();
            if (informationEntity == null)
            {
                return RespondResult(false, "参数无效。");
            }

            informationEntity.Domain = UserContext.User.Domain;
            informationEntity.AppId = DomainContext.AppId;
            informationEntity.CreateTime = DateTime.Now;
            informationEntity.CreateUser = UserContext.User.Id;
            _informationManager.CreateInformation(informationEntity);

            #region 操作日志

            _operatedLogManager.Create(new OperatedLogEntity()
            {
                Domain = DomainContext.Domain.Id,
                AppId = DomainContext.AppId,
                User = UserContext.User.Id,
                IP = Request.UserHostAddress,
                Module = EnumModule.Information,
                Description = "创建分类信息"
            });

            #endregion

            return RespondResult();
        }

        public ActionResult UpdateInformation()
        {
            InformationEntity informationEntity = RequestArgs<InformationEntity>();
            if (informationEntity == null)
            {
                return RespondResult(false, "参数无效。");
            }

            informationEntity.Domain = UserContext.User.Domain;
            informationEntity.AppId = DomainContext.AppId;
            _informationManager.UpdateInformation(informationEntity);

            #region 操作日志

            _operatedLogManager.Create(new OperatedLogEntity()
            {
                Domain = DomainContext.Domain.Id,
                AppId = DomainContext.AppId,
                User = UserContext.User.Id,
                IP = Request.UserHostAddress,
                Module = EnumModule.Information,
                Description = "更新分类信息"
            });

            #endregion

            return RespondResult();
        }

        public ActionResult RemoveInformation()
        {
            string id = Request.QueryString["id"];

            if (String.IsNullOrEmpty(id))
            {
                return RespondResult(false, "参数无效。");
            }

            _informationManager.RemoveInformation(Guid.Parse(id));

            #region 操作日志

            _operatedLogManager.Create(new OperatedLogEntity()
            {
                Domain = DomainContext.Domain.Id,
                AppId = DomainContext.AppId,
                User = UserContext.User.Id,
                IP = Request.UserHostAddress,
                Module = EnumModule.Information,
                Description = "删除分类信息"
            });

            #endregion

            return RespondResult();
        }

        public ActionResult GetInformation()
        {
            string id = Request.QueryString["id"];

            if (String.IsNullOrEmpty(id))
            {
                return RespondResult(false, "参数无效。");
            }

            InformationEntity information = _informationManager.GetInformation(Guid.Parse(id));

            return RespondDataResult(information);
        }

        public ActionResult GetInformationList()
        {
            GetItemListArgs args = RequestArgs<GetItemListArgs>();
            if (args == null)
            {
                return RespondResult(false, "参数无效。");
            }

            args.DomainId = UserContext.User.Domain;
            args.AppId = DomainContext.AppId;

            GetItemListResult result = _informationManager.GetInformationList(args);
            return RespondDataResult(result);
        }

        public ActionResult CreateCategory()
        {
            InformationCategoryEntity category = RequestArgs<InformationCategoryEntity>();
            if (category == null)
            {
                return RespondResult(false, "参数无效。");
            }

            category.Domain = UserContext.User.Domain;
            category.CreateTime = DateTime.Now;
            category.CreateUser = UserContext.User.Id;
            _informationManager.CreateCategory(category);

            #region 操作日志

            _operatedLogManager.Create(new OperatedLogEntity()
            {
                Domain = DomainContext.Domain.Id,
                AppId = DomainContext.AppId,
                User = UserContext.User.Id,
                IP = Request.UserHostAddress,
                Module = EnumModule.Information,
                Description = "创建分类信息分类"
            });

            #endregion

            return RespondResult();
        }

        public ActionResult UpdateCategory()
        {
            InformationCategoryEntity category = RequestArgs<InformationCategoryEntity>();
            if (category == null)
            {
                return RespondResult(false, "参数无效。");
            }

            category.Domain = UserContext.User.Domain;
            _informationManager.UpdateCategory(category);

            #region 操作日志

            _operatedLogManager.Create(new OperatedLogEntity()
            {
                Domain = DomainContext.Domain.Id,
                AppId = DomainContext.AppId,
                User = UserContext.User.Id,
                IP = Request.UserHostAddress,
                Module = EnumModule.Information,
                Description = "更新分类信息分类"
            });

            #endregion

            return RespondResult();
        }

        public ActionResult RemoveCategory()
        {
            string id = Request.QueryString["id"];

            if (String.IsNullOrEmpty(id))
            {
                return RespondResult(false, "参数无效。");
            }

            _informationManager.RemoveCategory(Guid.Parse(id));

            #region 操作日志

            _operatedLogManager.Create(new OperatedLogEntity()
            {
                Domain = DomainContext.Domain.Id,
                AppId = DomainContext.AppId,
                User = UserContext.User.Id,
                IP = Request.UserHostAddress,
                Module = EnumModule.Information,
                Description = "删除分类信息分类"
            });

            #endregion

            return RespondResult();
        }

        public ActionResult BatchRemoveCategory()
        {
            IdListArgs args = RequestArgs<IdListArgs>();
            if (args == null)
            {
                return RespondResult(false, "参数无效。");
            }

            _informationManager.BatchRemoveCategory(args.IdList);

            #region 操作日志

            _operatedLogManager.Create(new OperatedLogEntity()
            {
                Domain = DomainContext.Domain.Id,
                AppId = DomainContext.AppId,
                User = UserContext.User.Id,
                IP = Request.UserHostAddress,
                Module = EnumModule.Information,
                Description = "删除分类信息分类（批量删除）"
            });

            #endregion

            return RespondResult();
        }

        public ActionResult GetCategory()
        {
            string id = Request.QueryString["id"];

            if (String.IsNullOrEmpty(id))
            {
                return RespondResult(false, "参数无效。");
            }

            InformationCategoryEntity category = _informationManager.GetCategory(Guid.Parse(id));

            return RespondDataResult(category);
        }

        public ActionResult GetCategoryList()
        {
            GetCategoryListArgs args = RequestArgs<GetCategoryListArgs>();
            if (args == null)
            {
                return RespondResult(false, "参数无效。");
            }

            GetItemListResult result = _informationManager.GetCategoryList(args);
            return RespondDataResult(result);
        }

        public ActionResult CreateInformationItem()
        {
            InformationItemEntity item = RequestArgs<InformationItemEntity>();
            if (item == null)
            {
                return RespondResult(false, "参数无效。");
            }

            item.Domain = UserContext.User.Domain;
            item.CreateTime = DateTime.Now;
            item.CreateUser = UserContext.User.Id;
            _informationManager.CreateInformationItem(item);

            #region 操作日志

            _operatedLogManager.Create(new OperatedLogEntity()
            {
                Domain = DomainContext.Domain.Id,
                AppId = DomainContext.AppId,
                User = UserContext.User.Id,
                IP = Request.UserHostAddress,
                Module = EnumModule.Information,
                Description = "创建分类信息条目"
            });

            #endregion

            return RespondResult();
        }

        public ActionResult UpdateInformationItem()
        {
            InformationItemEntity item = RequestArgs<InformationItemEntity>();
            if (item == null)
            {
                return RespondResult(false, "参数无效。");
            }

            item.Domain = UserContext.User.Domain;
            _informationManager.UpdateInformationItem(item);

            #region 操作日志

            _operatedLogManager.Create(new OperatedLogEntity()
            {
                Domain = DomainContext.Domain.Id,
                AppId = DomainContext.AppId,
                User = UserContext.User.Id,
                IP = Request.UserHostAddress,
                Module = EnumModule.Information,
                Description = "更新分类信息条目"
            });

            #endregion

            return RespondResult();
        }

        public ActionResult RemoveInformationItem()
        {
            string id = Request.QueryString["id"];

            if (String.IsNullOrEmpty(id))
            {
                return RespondResult(false, "参数无效。");
            }

            _informationManager.RemoveInformationItem(Guid.Parse(id));

            #region 操作日志

            _operatedLogManager.Create(new OperatedLogEntity()
            {
                Domain = DomainContext.Domain.Id,
                AppId = DomainContext.AppId,
                User = UserContext.User.Id,
                IP = Request.UserHostAddress,
                Module = EnumModule.Information,
                Description = "删除分类信息条目"
            });

            #endregion

            return RespondResult();
        }

        public ActionResult BatchRemoveInformationItem()
        {
            IdListArgs args = RequestArgs<IdListArgs>();
            if (args == null)
            {
                return RespondResult(false, "参数无效。");
            }

            _informationManager.BatchRemoveInformationItem(args.IdList);

            #region 操作日志

            _operatedLogManager.Create(new OperatedLogEntity()
            {
                Domain = DomainContext.Domain.Id,
                AppId = DomainContext.AppId,
                User = UserContext.User.Id,
                IP = Request.UserHostAddress,
                Module = EnumModule.Information,
                Description = "删除分类信息条目（批量删除）"
            });

            #endregion

            return RespondResult();
        }

        public ActionResult GetInformationItem()
        {
            string id = Request.QueryString["id"];

            if (String.IsNullOrEmpty(id))
            {
                return RespondResult(false, "参数无效。");
            }

            InformationItemEntity item = _informationManager.GetInformationItem(Guid.Parse(id));

            return RespondDataResult(item);
        }

        public ActionResult GetInformationItemList()
        {
            GetInformationItemListArgs args = RequestArgs<GetInformationItemListArgs>();
            if (args == null)
            {
                return RespondResult(false, "参数无效。");
            }

            GetItemListResult result = _informationManager.GetInformationItemList(args);
            return RespondDataResult(result);
        }
    }
}