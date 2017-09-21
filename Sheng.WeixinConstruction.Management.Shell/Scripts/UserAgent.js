//////////////////userAgent封装
//用于判断设备类型，操作系统

var __device_PC = "1";
var __device_CellPhone = "2";
var __device_Pad = "3";

var __os_Windows = "1";
var __os_Macintosh = "2";
var __os_Linux = "3";
var __os_iOS = "4";
var __os_Android = "5";
var __os_WindowsPhone = "6";

function userAgentObj(userAgent)
{
    this.userAgent = userAgent;

    //设备类型。PC:电脑，CellPhone：手机，Pad：平板电脑
    this.device = "";
    //操作系统。Windows，Macintosh，iOS，Android，Windows Phone
    this.os = "";

    this.show = function ()
    {
        //alert(this.userAgent);
    }

    //开始判断
    //先判断智能设备，再判断电脑
    //因为 Linux 有可能既是 adnroid ，也有可能是 linux 电脑
    //国内有些浏览器明明是Android，但是 只包括 Linux 不包括 Android

    //判断是不是手机或平板

    //判断是不是 Windows Phone
    //必须先判断，因为微軟在Windows Phone 8.1 Update裡 把Windows Phone的IE11 User Agent改成:
    //Mozilla/5.0 (Mobile; Windows Phone 8.1; Android 4.0; ARM; Trident/7.0; Touch; rv:11.0; IEMobile/11.0; 
    //NOKIA; Lumia 930) like iPhone OS 7_0_3 Mac OS X AppleWebKit/537 (KHTML, like Gecko) Mobile Safari/537
    if (userAgent.indexOf("Windows Phone") > -1)
    {
        this.device = __device_CellPhone;
        this.os = __os_WindowsPhone;
    }

    else if (userAgent.indexOf("Windows NT") > -1 && userAgent.indexOf("WP") > -1)
    {
        this.device = __device_CellPhone;
        this.os = __os_WindowsPhone;
    }

    else if (userAgent.indexOf("iPad") > -1)
    {
        this.device = __device_Pad;
        this.os = __os_iOS;
    }

        //判断是不是 iPhone
    else if (userAgent.indexOf("iPhone") > -1)
    {
        this.device = __device_CellPhone;
        this.os = __os_iOS;
    }

        //国内有些奇葩浏览器可能不包含 iPhone，通过iOS判断
    else if (userAgent.indexOf("iOS") > -1)
    {
        this.device = __device_CellPhone;
        this.os = __os_iOS;
    }

        //判断是不是 Android 
    else if (userAgent.indexOf("Android") > -1)
    {
        this.device = __device_CellPhone;
        this.os = __os_Android;
    }

        //国内奇葩浏览器可能明明是Android但是只包含 Linux
        //QQ浏览器
        //MQQBrowser/3.6/Adr (Linux; U; 4.0.3; zh-cn; HUAWEI U8818 Build/U8818V100R001C17B926;480*800)
    else if (userAgent.indexOf("Linux") > -1 && userAgent.indexOf("MQQBrowser") > -1)
    {
        this.device = __device_CellPhone;
        this.os = __os_Android;
    }

        //UC浏览器
    else if (userAgent.indexOf("Linux") > -1 && userAgent.indexOf("UCBrowser") > -1)
    {
        this.device = __device_CellPhone;
        this.os = __os_Android;
    }

        //魅族UC浏览器(android)
        //JUC (Linux; U; 2.3.5; zh-cn; MEIZU MX; 640*960) UCWEB8.5.1.179/145/33232
    else if (userAgent.indexOf("Linux") > -1 && userAgent.indexOf("UCWEB") > -1)
    {
        this.device = __device_CellPhone;
        this.os = __os_Android;
    }

    else if (userAgent.indexOf("Linux") > -1 && userAgent.indexOf("MEIZU") > -1)
    {
        this.device = __device_CellPhone;
        this.os = __os_Android;
    }

        //三星手机
        //SAMSUNG-SGH-G508E/G508EZCIG2 SHP/VPP/R5 NetFront/3.4 Qtv5.3 SMM-MMS/1.2.0 profile/MIDP-2.0 configuration/CLDC-1.1
    else if (userAgent.indexOf("SAMSUNG") > -1)
    {
        this.device = __device_CellPhone;
        this.os = __os_Android;
    }

        //判断其它奇葩情况
        //UC 
        //iPhone 平台极速模式关闭状态下 UA 示例如下：（OBUA 为自带浏览器 UA）
        //OBUA UCBrowser/8.6.0.199 Mobile
        //WP平台极速模式开启状态下：（以 Nokia 900 为例）
        // UCWEB/2.0 (Windows; U; wds7.10; zh-CN; Nokia 900) U2/1.0.0 UCBrowser/8.6.0.199 U2/1.0.0 Mobile 
    else if (userAgent.indexOf("Mobile") > -1)
    {
        this.device = __device_CellPhone;
        if (userAgent.indexOf("Windows") > -1)
        {
            this.os = __os_WindowsPhone;
        }
        if (userAgent.indexOf("Linux") > -1)
        {
            this.os = __os_Android;
        }
        else if (userAgent.indexOf("OBUA") > -1)
        {
            this.os = __os_iOS;
        }
    }

    else if (userAgent.indexOf("Nokia") > -1)
    {
        this.device = __device_CellPhone;
        this.os = "";
    }

    else if (userAgent.indexOf("BlackBerry") > -1)
    {
        this.device = __device_CellPhone;
        this.os = "";
    }

    else if (userAgent.indexOf("Symbian") > -1)
    {
        this.device = __device_CellPhone;
        this.os = "";
    }

        //判断是不是电脑
        //是不是 Windows 操作系统
    else if (userAgent.indexOf("Windows NT") > -1 && userAgent.indexOf("WP") == -1 && userAgent.indexOf("Windows Phone") == -1)
    {
        this.device = __device_PC;
        this.os = __os_Windows;
    }
        //判断是不是 Macintosh 操作系统
    else if (userAgent.indexOf("Macintosh") > -1)
    {
        this.device = __device_PC;
        this.os = __os_Macintosh;
    }

    else if (userAgent.indexOf("Linux") > -1)
    {
        this.device = __device_PC;
        this.os = __os_Linux;
    }

}

//////////////////////////////////