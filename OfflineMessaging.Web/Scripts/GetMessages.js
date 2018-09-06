    $(function() {
        var userName;
        username = $(".chat_list.active_chat").attr("id");
        AjaxPost(userName);
            $(".chat_list").click(function (event) {
                userName = jQuery(this).attr("id");
                $(this).addClass('active_chat').siblings().removeClass('active_chat');
                AjaxPost(userName); 
            });
    });
    function AjaxPost(userName) {
        $.ajax({
            type: "GET",
            url: "/Message/GetMessages/",
            data: JSON.stringify({ "userName": userName }),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            xhrFields: {
                withCredentials: true
            },
            crossDomain: true,
            success: function (status) {
                if (status.Success) {

                    $("#status-text").html(status.Message);
                    window.location.replace('http://' + status.TargetURL);

                }
            },
            failure: function (status) {
                if (!status.Success) {
                    $("#status-text").html(status.Message);
                }
            }
        });
    }
