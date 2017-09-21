using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sheng.WeixinConstruction.Infrastructure
{
    public class MemberContext
    {
        public MemberEntity Member
        {
            get;
            set;
        }

        /// <summary>
        /// 所关联的User（管理员）
        /// </summary>
        public UserEntity User
        {
            get;
            set;
        }

        public MemberContext(MemberEntity member)
        {
            Member = member;
        }
    }
}
