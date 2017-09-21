//Layer弹出组件的动画方式
var _layerShift = 5;

var _fileService = "http://localhost:53627/";

(function (doc, win) {
    var docEl = doc.documentElement,
            resizeEvt = 'orientationchange' in window ? 'orientationchange' : 'resize',
            recalc = function () {
                var clientWidth = docEl.clientWidth;
				//alert(docEl.clientHeight);
                if (!clientWidth) return;
                docEl.style.fontSize = 100 * (clientWidth / 320) + 'px';
            };
    if (!doc.addEventListener) return;
    win.addEventListener(resizeEvt, recalc, false);
    doc.addEventListener('DOMContentLoaded', recalc, false);
})(document, window);

function goUrl(url)
{
    // alert(url);

    //var loadLayerIndex = layer.open({
    //    type: 2,
    //    shadeClose: false,
    //    content: '请稍候...'
    //});

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

function layerAlert(message)
{
    layer.open({
        content: message
    });   
}

///////////

///////////

//jquery validation

function showValidationErrors(errorMap, errorList) {
    $.each(this.successList, function (index, value) {
        $(value).css("background-color", "#FFF");
    });

    if (errorList.length > 0) {
        var message = "";
        $.each(errorList, function (index, value) {
            $(value.element).css("background-color", "#FFD7D7");
            message += value.message + "</br>";
        });
        if (message != "") {
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


//维持session
function heartbeat()
{
    setInterval(function ()
    {
        $.get("/Api/UserContext/Heartbeat");
    },60000);    
}

//设置文本框的光标位置
//http://feierky.iteye.com/blog/1929696
function setInputLocation(elm, n) {
    if (n > elm.value.length)
        n = elm.value.length;
    if (elm.createTextRange) {   // IE   
        var textRange = elm.createTextRange();
        textRange.moveStart('character', n);
        textRange.collapse();
        textRange.select();
    } else if (elm.setSelectionRange) { // Firefox   
        elm.setSelectionRange(n, n);
        elm.focus();
    }
}