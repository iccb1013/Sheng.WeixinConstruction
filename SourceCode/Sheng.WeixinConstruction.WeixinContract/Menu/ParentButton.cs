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


using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Sheng.WeixinConstruction.WeixinContract
{
    [DataContract]
    public class ParentButton : ButtonBase
    {
        private List<ButtonBase> _subButton = new List<ButtonBase>();
        [DataMember(Name = "sub_button")]
        public List<ButtonBase> SubButton
        {
            get { return _subButton; }
            set { _subButton = value; }
        }
    }
}
