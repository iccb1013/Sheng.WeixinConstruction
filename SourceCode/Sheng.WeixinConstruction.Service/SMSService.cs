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


using Aliyun.Acs.Core;
using Aliyun.Acs.Core.Exceptions;
using Aliyun.Acs.Core.Profile;
using Aliyun.Acs.Dm.Model.V20151123;
using Linkup.Common;
using Sheng.WeixinConstruction.Infrastructure;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sheng.WeixinConstruction.Service
{
    /// <summary>
    /// https://help.aliyun.com/document_detail/35376.html?spm=5176.doc29434.6.124.ymCYzK
    /// </summary>
    public class SMSService
    {
        private static SMSService _instance;
        public static SMSService Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new SMSService();

                return _instance;
            }
        }

        private LogService _log = LogService.Instance;

        private static readonly AliyunConfigurationElement _configuration = ConfigService.Instance.Configuration.Aliyun;

        IAcsClient _client;

        private SMSService()
        {
            IClientProfile profile = DefaultProfile.GetProfile(
                _configuration.RegionId, _configuration.AccessKeyId, _configuration.Secret);
            _client = new DefaultAcsClient(profile);
        }

        public NormalResult Send(string signName, string templateCode, string recNum, string paramString)
        {
            NormalResult result = new NormalResult();

            SingleSendSmsRequest request = new SingleSendSmsRequest();
            try
            {
                request.SignName = signName;
                request.TemplateCode = templateCode;
                request.RecNum = recNum;
                request.ParamString = paramString;
                SingleSendSmsResponse httpResponse = _client.GetAcsResponse(request);
            }
            catch (ServerException ex)
            {
                _log.Write("短信发送失败 ServerException", ex.Message + "\r\n" + ex.StackTrace, TraceEventType.Warning);
                result.Success = false;
                result.Message = ex.Message;
            }
            catch (ClientException ex)
            {
                _log.Write("短信发送失败 ClientException", ex.Message + "\r\n" + ex.StackTrace, TraceEventType.Warning);
                result.Success = false;
                result.Message = ex.Message;
            }

            return result;
        }
    }
}
