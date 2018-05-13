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
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Sheng.WeixinConstruction.WeixinContract
{
	public class HashAndMd5
	{
		public static String Sha1(string s)
		{
			char[] hexDigits = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9',
					'a', 'b', 'c', 'd', 'e', 'f' };
			try
			{
				byte[] btInput = System.Text.Encoding.Default.GetBytes(s);
				SHA1 sha = new System.Security.Cryptography.SHA1CryptoServiceProvider();

				byte[] md = sha.ComputeHash(btInput);
				// 把密文转换成十六进制的字符串形式
				int j = md.Length;
				char[] str = new char[j * 2];
				int k = 0;
				for (int i = 0; i < j; i++)
				{
					byte byte0 = md[i];
					str[k++] = hexDigits[(int)(((byte)byte0) >> 4) & 0xf];
					str[k++] = hexDigits[byte0 & 0xf];
				}
				return new string(str);
			}
			catch (Exception e)
			{
				Console.Error.WriteLine(e.StackTrace);
				return null;
			}
		}

		/// <summary>
		/// 使用md5加密
		/// </summary>
		/// <param name="str">待加密的字符串</param>
		/// <returns>返回加密后的md5字符串</returns>
		public static string GetMd5String(string str)
		{
			StringBuilder pwd = new StringBuilder();

			MD5 md5 = MD5.Create();
			byte[] s = md5.ComputeHash(Encoding.UTF8.GetBytes(str));
			for (int i = 0; i < s.Length; i++)
			{
				pwd.Append(s[i].ToString("X").PadLeft(2, '0'));
			}
			return pwd.ToString();
		}

        /// <summary>
        /// 微信支付3.0，MD5加密函数
        /// </summary>
        /// <param name="encypStr"></param>
        /// <returns></returns>
        public static string GetMD5(string encypStr)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] data = System.Text.Encoding.UTF8.GetBytes(encypStr);
            byte[] md5data = md5.ComputeHash(data);
            md5.Clear();
            string str = "";
            for (int i = 0; i < md5data.Length; i++)
            {
                str += md5data[i].ToString("x").PadLeft(2, '0');
            }
            return str;
        }
	}
}
