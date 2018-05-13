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
using System.Text;
using System.Threading.Tasks;

namespace Sheng.WeixinConstruction.Infrastructure
{
    public class FileUploadParameter
    {
        private Guid _id = Guid.NewGuid();
        public Guid Id
        {
            get { return _id; }
            set { _id = value; }
        }

        public Guid UserId
        {
            get;
            set;
        }

        public Guid DomainId
        {
            get;
            set;
        }

        public string FileService
        {
            get;
            set;
        }
    }
}
