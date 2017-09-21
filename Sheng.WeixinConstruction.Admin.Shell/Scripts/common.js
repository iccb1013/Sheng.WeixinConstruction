//Layer弹出组件的动画方式
var _layerShift = 5;

var _fileService = "http://localhost:53627/";

function goUrl(url)
{
    window.location.href = url;
}

function getQueryString(name)
{
    var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)");
    var r = window.location.search.substr(1).match(reg);
    if (r != null) return unescape(r[2]); return null;
}

//Cookie 操作
function setCookie(name, value, expiredays)
{

    var exdate = new Date()
    exdate.setDate(exdate.getDate() + expiredays)

    var str = name + "=" + escape(value);
    if (expiredays != null)
    {
        str += ";expires=" + exdate.toGMTString();
    }

    document.cookie = str;
}

function getCookie(name)
{
    if (document.cookie.length > 0)
    {
        c_start = document.cookie.indexOf(name + "=")
        if (c_start != -1)
        {
            c_start = c_start + name.length + 1
            c_end = document.cookie.indexOf(";", c_start)
            if (c_end == -1) c_end = document.cookie.length
            return unescape(document.cookie.substring(c_start, c_end))
        }
    }
    return ""
}

function removeCookie(name)
{
    var exdate = new Date()
    exdate.setDate(exdate.getDate() - 10);
    document.cookie = name + "=v; expires=" + exdate.toGMTString();
}

////////

//Layer

function layerInputAlertMsg()
{
    layerMsg("请核对您的输入。");
}

function layerMsg(message,time)
{
    if (time != undefined && time != null)
    {
        layer.msg(message, {
            time: time
        });
    }
    else
    {
        layer.msg(message, {
            time: 1500
        });
    }
}

function layerAlert(message,callback)
{
    var alertLayerIndex = layer.alert(message, {
        title: false, closeBtn: false, shift: _layerShift,
        success: function (layero, index)
        {
            $(layero).focus();
            $(layero).keypress(function (e)
            {
                if (e.keyCode == 13)
                {
                    layer.close(alertLayerIndex);
                    if (callback != undefined && callback != null)
                    {
                        callback();
                    }
                } else if (e.keyCode == 27)
                {
                    layer.close(alertLayerIndex);
                    if (callback != undefined && callback != null)
                    {
                        callback();
                    }
                }
            });
            ////alert($(layero).find("a").length);
            //alert($($(layero).find("a")[0]).html());
            //$($(layero).find("a")[0]).focus();
        },
        yes: function (index)
        {
            layer.close(alertLayerIndex);
            if (callback != undefined && callback != null)
            {
                callback();
            }
        }
    });
}

///////////

//jquery validation

function showValidationErrors(errorMap, errorList)
{
    $.each(this.successList, function (index, value)
    {
        $(value).css("background-color", "#FFF");
    });

    if (errorList.length > 0)
    {
        var message = "";
        $.each(errorList, function (index, value)
        {
            $(value.element).css("background-color", "#FFD7D7");
            message += value.message + "</br>";
        });
        if (message != "")
        {
            layerAlert(message.substr(0, message.length - 5));
        }
    }

    // this.defaultShowErrors();
}

function hightlightValidationErrors(errorMap, errorList) {
    $.each(this.successList, function (index, value) {
        $(value).css("background-color", "#FFF");
    });

    if (errorList.length > 0) {
        $.each(errorList, function (index, value) {
            $(value.element).css("background-color", "#FFD7D7");
        });
    }

    // this.defaultShowErrors();
}

///////////

// 对Date的扩展，将 Date 转化为指定格式的String 
// 月(M)、日(d)、小时(h)、分(m)、秒(s)、季度(q) 可以用 1-2 个占位符， 
// 年(y)可以用 1-4 个占位符，毫秒(S)只能用 1 个占位符(是 1-3 位的数字) 
// 例子： 
// (new Date()).Format("yyyy-MM-dd hh:mm:ss.S") ==> 2006-07-02 08:09:04.423 
// (new Date()).Format("yyyy-M-d h:m:s.S")      ==> 2006-7-2 8:9:4.18 
// var submitTime = new Date(d[i].SubmitTime).format("yyyy-MM-dd hh:mm:ss");
// var time = new Date(d[i].LimitedTime.replace(/-/g,"/")).format("yyyy-MM-dd ")
Date.prototype.format = function (fmt)
{ //author: meizz 
    var o = {
        "M+": this.getMonth() + 1, //月份 
        "d+": this.getDate(), //日 
        "h+": this.getHours(), //小时 
        "m+": this.getMinutes(), //分 
        "s+": this.getSeconds(), //秒 
        "q+": Math.floor((this.getMonth() + 3) / 3), //季度 
        "S": this.getMilliseconds() //毫秒 
    };
    if (/(y+)/.test(fmt)) fmt = fmt.replace(RegExp.$1, (this.getFullYear() + "").substr(4 - RegExp.$1.length));
    for (var k in o)
        if (new RegExp("(" + k + ")").test(fmt)) fmt = fmt.replace(RegExp.$1, (RegExp.$1.length == 1) ? (o[k]) : (("00" + o[k]).substr(("" + o[k]).length)));
    return fmt;
}

String.format = function () {
    if (arguments.length == 0)
        return null;

    var str = arguments[0];
    for (var i = 1; i < arguments.length; i++) {
        var re = new RegExp('\\{' + (i - 1) + '\\}', 'gm');
        str = str.replace(re, arguments[i]);
    }
    return str;
}

//维持session
function heartbeat()
{
    setInterval(function ()
    {
        $.get("/Api/UserContext/Heartbeat");
    },60000);    
}