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
using Sheng.WeixinConstruction.WeixinContract;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Sheng.WeixinConstruction.Core
{
    public class UserManager
    {
        private static readonly UserManager _instance = new UserManager();
        public static UserManager Instance
        {
            get { return _instance; }
        }

        private DatabaseWrapper _dataBase = ServiceUnity.Instance.Database;

        private object _lockUserObj = new object();

        private static readonly DomainManager _domainManager = DomainManager.Instance;

        private UserManager()
        {

        }

        #region User

        /// <summary>
        /// 返回 0 成功 1 登录名被占用
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public NormalResult CreateUser(UserEntity user)
        {
            NormalResult result = new NormalResult(false);
            if (user == null)
            {
                result.Message = "参数错误。";
                return result;
            }

            lock (_lockUserObj)
            {
                List<CommandParameter> parameterList = new List<CommandParameter>();
                parameterList.Add(new CommandParameter("@account", user.Account));

                int accountCount = 0;
                _dataBase.ExecuteScalar<int>(@"SELECT Count(Id) FROM [User] WHERE [Account] = @account",
                    parameterList, (scalarValue) => { accountCount = scalarValue; });

                if (accountCount > 0)
                {
                    result.Reason = 1;
                    return result;
                }

                user.RegisterTime = DateTime.Now;
                _dataBase.Insert(user);
            }

            result.Success = true;
            return result;
        }

        /// <summary>
        /// 返回 0 成功 1 登录名被占用
        /// 在用户管理中更新用户信息，全面更新
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public NormalResult UpdateUser(UserEntity user)
        {
            NormalResult result = new NormalResult(false);
            if (user == null)
            {
                result.Message = "参数错误。";
                return result;
            }

            lock (_lockUserObj)
            {
                List<CommandParameter> parameterList = new List<CommandParameter>();
                parameterList.Add(new CommandParameter("@id", user.Id));
                parameterList.Add(new CommandParameter("@account", user.Account));

                int accountCount = int.Parse(_dataBase.ExecuteScalar(
                    "SELECT Count(Id) FROM [User] WHERE [Account] = @account AND [Id] <> @id", parameterList).ToString());
                if (accountCount > 0)
                {
                    result.Reason = 1;
                    return result;
                }

                SqlStructureBuild sqlBuild = new SqlStructureBuild();
                sqlBuild.Table = "User";
                sqlBuild.Type = SqlExpressionType.Update;
                sqlBuild.AddParameter("Id", user.Id, true);
                sqlBuild.AddParameter("Account", user.Account);
                sqlBuild.AddParameter("Name", user.Name);
                sqlBuild.AddParameter("Email", user.Email);
                sqlBuild.AddParameter("Telphone", user.Telphone);
                sqlBuild.AddParameter("MemberId", user.MemberId);
                sqlBuild.AddParameter("Remark", user.Remark);
                _dataBase.ExcuteSqlExpression(sqlBuild.GetSqlExpression());
            }

            result.Success = true;
            return result;
        }

        /// <summary>
        /// 用户更新自己的信息
        /// 不允许修改登录名，关联微信号等
        /// </summary>
        /// <param name="id"></param>
        /// <param name="user"></param>
        public void Update(Guid id, UserEntity user)
        {
            if (user == null)
            {
                Debug.Assert(false, "user 为空");
                return;
            }

            SqlStructureBuild sqlBuild = new SqlStructureBuild();
            sqlBuild.Table = "User";
            sqlBuild.Type = SqlExpressionType.Update;
            sqlBuild.AddParameter("Id", id, true);
            sqlBuild.AddParameter("Name", user.Name);
            sqlBuild.AddParameter("Email", user.Email);
            sqlBuild.AddParameter("Telphone", user.Telphone);
            _dataBase.ExcuteSqlExpression(sqlBuild.GetSqlExpression());
        }

        /// <summary>
        /// 0 删除成功 1 指定的用户是域的最初拥有者
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public NormalResult RemoveUser(Guid id)
        {
            NormalResult result = new NormalResult(false);
            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@id", id));

            //判断是否是DomainOwner
            bool domainOwner = false;
            _dataBase.ExecuteScalar<bool>(
                "SELECT [DomainOwner] FROM [User] WHERE [Removed] = 0 AND [Id] = @id",
                parameterList, (scalarValue) => { domainOwner = scalarValue; });
            if (domainOwner)
            {
                result.Reason = 1;
                return result;
            }

            _dataBase.ExecuteNonQuery("UPDATE [User] SET [Removed] = 1 WHERE [Id] = @id",
                parameterList);

            result.Reason = 0;
            result.Success = true;
            return result;
        }

        public UserEntity GetUser(Guid id)
        {
            UserEntity user = new UserEntity();
            user.Id = id;

            if (_dataBase.Fill<UserEntity>(user))
            {
                return user;
            }
            else
                return null;
        }

        public UserEntity GetUserByMemberId(Guid domainId, Guid memberId)
        {
            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@domainId", domainId));
            parameterList.Add(new CommandParameter("@memberId", memberId));

            List<UserEntity> userList = _dataBase.Select<UserEntity>(
                "SELECT * FROM [User] WHERE [Removed] = 0 AND [Domain] = @domainId AND [MemberId] = @memberId",
                parameterList);

            if (userList.Count == 0)
                return null;
            else
                return userList[0];
        }

        public GetItemListResult GetUserList(GetUserListArgs args)
        {
            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@domainId", args.DomainId));
            parameterList.Add(new CommandParameter("@appId", args.AppId));
            parameterList.Add(new CommandParameter("@page", args.Page));
            parameterList.Add(new CommandParameter("@pageSize", args.PageSize));
            parameterList.Add(new CommandParameter("@name", args.Name));
            parameterList.Add(new CommandParameter("@account", args.Account));

            DataSet dsResult =
                _dataBase.ExecuteDataSet(CommandType.StoredProcedure, "GetUserList", parameterList, new string[] { "result" });

            if (dsResult.Tables[0].Rows.Count == 0 && args.Page > 1)
            {
                args.Page--;
                return GetUserList(args);
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

        public UserRegisterResult Register(UserRegisterArgs args)
        {
            UserRegisterResult result = new UserRegisterResult();

            if (args == null)
            {
                Debug.Assert(false, "用户信息无效");
                result.Result = UserRegisterResultEnum.UserInfoInvalid;
                return result;
            }

            lock (_lockUserObj)
            {
                List<CommandParameter> parameterList = new List<CommandParameter>();
                parameterList.Add(new CommandParameter("@account", args.Account));

                int accountCount = int.Parse(_dataBase.ExecuteScalar(
                    "SELECT Count(Id) FROM [User] WHERE [Account] = @account", parameterList).ToString());
                if (accountCount > 0)
                {
                    result.Result = UserRegisterResultEnum.AccountInUse;
                    return result;
                }

                DomainEntity domain = new DomainEntity();
                domain.LastUpdateTime = DateTime.Now;
                _domainManager.Create(domain);

                UserEntity user = new UserEntity()
                {
                    Account = args.Account,
                    Password = args.Password,
                    Name = args.Name,
                    MobilePhone = args.MobilePhone,
                    Email = args.Email,
                    Domain = domain.Id,
                    DomainOwner = true,
                    RegisterTime = DateTime.Now
                };

                result.User = user;
                result.Domain = domain;

                _dataBase.Insert(user);
            }

            result.Result = UserRegisterResultEnum.Success;
            return result;
        }

        public UserEntity Verify(string account, string password)
        {
            if (String.IsNullOrEmpty(account) || String.IsNullOrEmpty(password))
            {
                return null;
            }

            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@account", account));
            parameterList.Add(new CommandParameter("@password", password));
            //1C4DA2965910D90AFA1F780FC003D60D  Cc123
            List<UserEntity> userList = _dataBase.Select<UserEntity>(
                "SELECT * FROM [User] WHERE [Removed] = 0 AND [Account] = @account AND [Password] = @password ",
                parameterList);

            if (userList.Count != 1)
                return null;
            else
                return userList[0];
        }

        public bool ResetPassword(ResetPasswordArgs args)
        {
            Random radom = new Random(DateTime.Now.Second);
            int newPassword = radom.Next(10000, 100000);

            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@account", args.Account));
            parameterList.Add(new CommandParameter("@email", args.Email));
            parameterList.Add(new CommandParameter("@password", IOHelper.GetMD5HashFromString(newPassword.ToString())));

            int rowsCount = _dataBase.ExecuteNonQuery(
                "UPDATE [User] Set [Password] = @password WHERE [Removed] = 0 AND [Account] = @account AND [Email] = @email",
                parameterList);

            if (rowsCount == 1)
            {
                //发邮件
                MailMessage mailMessage = new MailMessage();
                mailMessage.From = new MailAddress("linkup_noreply@163.com");
                mailMessage.To.Add(new MailAddress(args.Email));

                mailMessage.Subject = String.Format("您的密码已重置");

                string strBody = String.Format(@"<b>您的密码已重置</b><br/>
                            帐户：{0}<br/>
                            密码：{1}<br/>
                            登录地址：http://wxcm.shengxunwei.com<br/>
                            ", args.Account, newPassword);

                mailMessage.Body = strBody;
                mailMessage.IsBodyHtml = true;

                SmtpService.Instance.Send(mailMessage);
            }

            return rowsCount == 1;
        }

        public bool UpdatePassword(Guid id, UpdatePasswordArgs args)
        {
            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@id", id));
            parameterList.Add(new CommandParameter("@password", args.Password));
            parameterList.Add(new CommandParameter("@newPassword", args.NewPassword));

            return _dataBase.ExecuteNonQuery(
                "UPDATE [User] SET [Password] = @newPassword WHERE [Removed] = 0 AND [Id] = @id AND [Password] = @password",
                parameterList) == 1;
        }


        #endregion

        #region Role

        /// <summary>
        /// 获取角色列表
        /// </summary>
        /// <param name="domainId"></param>
        /// <returns></returns>
        public List<RoleEntity> GetRoleList(Guid domainId)
        {
            Dictionary<string, object> attachedWhere = new Dictionary<string, object>();
            attachedWhere.Add("Domain", domainId);
            List<RoleEntity> list = _dataBase.Select<RoleEntity>(attachedWhere);

            return list;
        }

        public void CreateRole(RoleEntity role)
        {
            if (role == null)
            {
                Debug.Assert(false, "role 为空");
                return;
            }

            _dataBase.Insert(role);
        }

        public void UpdateRole(RoleEntity role)
        {
            if (role == null)
            {
                Debug.Assert(false, "role 为空");
                return;
            }

            _dataBase.Update(role);
        }

        public void RemoveRole(Guid id)
        {
            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@id", id));

            _dataBase.ExecuteNonQuery("DELETE FROM [Role] WHERE [Id] = @id", parameterList);
            _dataBase.ExecuteNonQuery("DELETE FROM [RoleAuthorization] WHERE [Role] = @id", parameterList);
            _dataBase.ExecuteNonQuery("DELETE FROM [RoleUser] WHERE [Role] = @id", parameterList);

        }

        public RoleEntity GetRole(Guid id)
        {
            RoleEntity role = new RoleEntity();
            role.Id = id;
            if (_dataBase.Fill<RoleEntity>(role))
            {
                return role;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 获取指定角色下的权限集合
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public List<Authorization> GetAuthorizationListByRoleId(Guid roleId)
        {
            Dictionary<string, object> attachedWhere = new Dictionary<string, object>();
            attachedWhere.Add("Role", roleId);

            return _dataBase.Select<Authorization>(attachedWhere);
        }

        /// <summary>
        /// 更新指定角色的权限集合
        /// </summary>
        /// <param name="roleAuthorizationRelation"></param>
        public void UpdateAuthorizationListByRoleId(RoleAuthorizationRelation roleAuthorizationRelation)
        {
            if (roleAuthorizationRelation == null)
            {
                Debug.Assert(false, "authorizationWrapper 为空");
                return;
            }

            List<SqlExpression> sqlList = new List<SqlExpression>();

            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@role", roleAuthorizationRelation.Role));

            SqlExpression deleteSql = new SqlExpression()
            {
                Sql = "DELETE FROM [RoleAuthorization] WHERE [Role] = @role"
            };
            deleteSql.ParameterList = _dataBase.CommandParameterToSqlParameter(parameterList);

            sqlList.Add(deleteSql);

            foreach (var item in roleAuthorizationRelation.AuthorizationList)
            {
                RoleAuthorizationEntity roleAuthorizationr = new RoleAuthorizationEntity();
                roleAuthorizationr.Domain = roleAuthorizationRelation.Domain;
                roleAuthorizationr.Role = roleAuthorizationRelation.Role;
                roleAuthorizationr.AuthorizationKey = item.Key;

                sqlList.Add(RelationalMappingUnity.GetSqlExpression(
                    roleAuthorizationr, SqlExpressionType.Insert));
            }

            _dataBase.ExcuteSqlExpression(sqlList);
        }

        /// <summary>
        /// 将指定的用户添加到角色
        /// </summary>
        /// <param name="roleUser"></param>
        public void AddUserToRole(RoleUserEntity roleUser)
        {
            if (roleUser == null)
            {
                Debug.Assert(false, "roleUser 为空");
                return;
            }

            _dataBase.Insert(roleUser);
        }

        /// <summary>
        /// 将指定的用户从角色移除
        /// </summary>
        /// <param name="roleUser"></param>
        public void RemoveUserFromRole(RoleUserEntity roleUser)
        {
            if (roleUser == null)
            {
                Debug.Assert(false, "roleUser 为空");
                return;
            }

            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@role", roleUser.Role));
            parameterList.Add(new CommandParameter("@user", roleUser.User));

            _dataBase.ExecuteNonQuery(
                "DELETE FROM [RoleUser] WHERE [Role] = @role AND [User] = @user", parameterList);
        }

        //public List<UserEntity> GetUserListByRoleId(Guid roleId)
        //{
        //    List<CommandParameter> parameterList = new List<CommandParameter>();
        //    parameterList.Add(new CommandParameter("@role", roleId));

        //    DataSet dsUser = _dataBase.ExecuteDataSet(CommandType.StoredProcedure,
        //        "GetUserListByRoleId", parameterList, "userList");

        //    List<User> userList = RelationalMappingUnity.Select<User>(dsUser.Tables[0]);
        //    return userList;
        //}

        #endregion
    }
}
