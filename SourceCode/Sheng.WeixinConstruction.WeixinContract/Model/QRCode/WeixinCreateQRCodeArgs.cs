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
    /*
     *http://mp.weixin.qq.com/wiki/18/8a8bbd4f0abfa3e58d7f68ce7252c0d6.html 
     * 这个接口支持两种json，使用后者
     * {"action_name": "QR_LIMIT_SCENE", "action_info": {"scene": {"scene_id": 123}}}
     * {"action_name": "QR_LIMIT_STR_SCENE", "action_info": {"scene": {"scene_str": "123"}}}
     *
     */

    [DataContract]
    public class WeixinCreateQRCodeArgs
    {
        private string _actionName = "QR_LIMIT_STR_SCENE";
        [DataMember(Name = "action_name")]
        public string ActionName
        {
            get { return _actionName; }
            set { _actionName = value; }
        }

        private WeixinCreateQRCodeArgs_ActionInfo _actionInfo = new WeixinCreateQRCodeArgs_ActionInfo();
        [DataMember(Name = "action_info")]
        public WeixinCreateQRCodeArgs_ActionInfo ActionInfo
        {
            get { return _actionInfo; }
            set { _actionInfo = value; }
        }
    }

    [DataContract]
    public class WeixinCreateQRCodeArgs_ActionInfo
    {
        private WeixinCreateQRCodeArgs_ActionInfo_Scene _scene = new WeixinCreateQRCodeArgs_ActionInfo_Scene();
        [DataMember(Name = "scene")]
        public WeixinCreateQRCodeArgs_ActionInfo_Scene Scene
        {
            get { return _scene; }
            set { _scene = value; }
        }
    }

    [DataContract]
    public class WeixinCreateQRCodeArgs_ActionInfo_Scene
    {
        [DataMember(Name = "scene_str")]
        public string SceneId
        {
            get;
            set;
        }
    }
}
