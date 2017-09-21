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