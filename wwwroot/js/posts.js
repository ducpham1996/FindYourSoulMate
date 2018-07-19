$(document).ready(function () {
    $("#comment_form").submit(function (e) {
        e.preventDefault();
        $.ajax({
            type: "Post",
            url: "/Comment/Create",
            data: $("#comment_form").serialize(),
            success: function (response) {
                alert(response.message);
            },
            error: function (xhr, response, error) {
                if (xhr.status == 400) {
                    var err = JSON.parse(xhr.responseText);
                    alert(err.message);
                } else {
                    $('#myModal').modal('show');
                }

            }
        });
        return false;
    });
});
function showReplyBox(comment_id) {
    var reply_box = $('#reply_box_' + comment_id);
    reply_box.css('display', 'block');
    //if (reply_box.css('display') == 'none') {
    //    reply_box.css('display', 'block');
    //} else {
    //    reply_box.css('display', 'none');
    //}
}
function reply_comment(comment_id, post_id) {
    $.ajax({
        type: "Post",
        url: "/Comment/Create_SubComment",
        data: $("#reply_box_form_" + comment_id).serialize(),
        success: function (response) {
            var data = JSON.parse(response);
            show_sub_comment(comment_id, data, post_id);
            var text = $("#reply_text_area_" + comment_id);
            text.val('');
        },
        error: function (xhr, response, error) {
            if (xhr.status == 400) {
                var err = JSON.parse(xhr.responseText);
                alert(err.message);
            } else {
                $('#myModal').modal('show');
            }

        }
    });
}
function comment(post_id) {
    var totalComments = $('#total_comment_' + post_id);
    var no_of_comment = $('#no_of_comment_' + post_id);
    $.ajax({
        type: "Post",
        url: "/Comment/Create",
        data: $("#comment_form").serialize(),
        success: function (response) {
            var data = JSON.parse(response);
            showcomment(data, post_id);
            var current_total = totalComments.val();
            totalComments.val(+current_total + 1);
            var noOfRecord = Math.ceil($('#total_comment_' + post_id).val() * 1.0 / 5);
            no_of_comment.val(noOfRecord);
        },
        error: function (xhr, response, error) {
            if (xhr.status == 400) {
                var err = JSON.parse(xhr.responseText);
                alert(err.message);
            } else {
                $('#myModal').modal('show');
            }

        }
    });
}
function showcomment(data, post_id) {
    var boxComments = $("#comments-list-" + post_id);
    //boxComments.find(' > li:nth-last-child(1)').before(comment_to_html(data, post_id));
    boxComments.append(comment_to_html(data, post_id));
}
function comment_to_html(data, post_id) {
    var encoded_comment_id = window.btoa(data._id);
    var noOfRecord = Math.ceil(data.comments * 1.0 / 5);
    var html_comment = "<li id='reply_list_" + encoded_comment_id + "'>" +
        "<div class='comment-main-level'>" +
        "<div class='media-left'>" +
        "<div class='comment-avatar'><img src='" + data.owner.user_picture + "' alt=''></div>" +
        "</div>" +
        "<div class='comment-box'>" +
        "<div class='comment-head'>" +
        "<div class='media-body'>" +
        "<h4 class='margin-t-0'><a href='" + data.owner._id + "'>" + data.owner.user_name + "</a></h4>" +
        "<p><span class='glyphicon glyphicon-time'></span> " + moment(data.date_created).fromNow() + "</p>" +
        "</div>" +
        "<div class='media-right'>";
    if (data.is_own == true) {
        html_comment += "<a onclick=\"onEditComment('" + encoded_comment_id + "')\"><i class='fa fa-edit'></i></a>" +
            "<a onclick=\"onDeleteComment('" + encoded_comment_id + "')\"><i class='fa fa-close'></i></a>";
    }

    html_comment += "</div>" +
        "</div>" +
        "<div class='comment-content'><form id='comment_form_" + encoded_comment_id + "'>" +
        "<input type='hidden' name='comment_id' value='" + encoded_comment_id + "' />" +
        "<input type='hidden' name='post_id' value ='" + post_id + "' />" +
        "<input type='hidden' id='total_sub_comments_" + encoded_comment_id + "'  name='total_sub_comments' value ='" + data.comments + "' />" +
        "<input id='no_of_sub_comment_" + encoded_comment_id + "' type='hidden' name='noOfComment' value='" + noOfRecord + "'/>" +
        "<textarea name='comment_text' id='edit_comment_text_area_" + encoded_comment_id + "' onkeypress=\"return enter_edit_comment(this, event, '" + encoded_comment_id + "')\" style='display: none' class='form-control'></textarea>" +
        "</form><p style='word-wrap: break-word;white-space: pre-line;' id='comment_text_" + encoded_comment_id + "'>" + data.message + "</p>" +
        "</div>" +
        "<div class='comment-content'><p style='float: left'>";
    if (data.has_like == 'none') {
        html_comment +=
            "<a value='' id='comment_" + encoded_comment_id + "' href=\"javascript:onLike('" + encoded_comment_id + "','like')\" onmouseover = \"onChoseEmoji('" + encoded_comment_id + "','c')\" >"
            + "<i id = 'icon_" + encoded_comment_id + "' class='glyphicon glyphicon-thumbs-up' ></i > Like</a >";
    } else {
        html_comment += emoji_to_text(encoded_comment_id, data.has_like , 'c');
    }
    var total_like = data.like + data.love + data.angry + data.scare + data.haha + data.sad + data.amaze + data.suprise;
    html_comment +=
        "<a href=\"javascript:showReplyBox('" + encoded_comment_id + "')\">" +
        "<i class='glyphicon glyphicon-comment'></i> Reply" +
        "</a>" +
        "</p><p style='float: right;margin-top: 6px;'>";
    if (data.like > 0) {
        html_comment += "<span class='Selector selectorFace20 like20'></span>";
    }
    if (data.love > 0) {
        html_comment += "<span class='Selector selectorFace20 love20'></span>";
    }
    if (data.angry > 0) {
        html_comment += "<span class='Selector selectorFace20 angry20'></span>";
    }
    if (data.scare > 0) {
        html_comment += "<span class='Selector selectorFace20 scare20'></span>";
    }
    if (data.haha > 0) {
        html_comment += "<span class='Selector selectorFace20 haha20'></span>";
    }
    if (data.sad > 0) {
        html_comment += "<span class='Selector selectorFace20 sad20'></span>";
    }
    if (data.amaze > 0) {
        html_comment += "<span class='Selector selectorFace20 amaze20'></span>";
    }
    if (data.suprise > 0) {
        html_comment += "<span class='Selector selectorFace20 suprise20'></span>";
    }
    html_comment += total_like
        + "</p></div > " +
        "</div>" +
        "</div><ul id = 'subCommentsContainer" + encoded_comment_id + "' class='comments-list reply-list' >";
    if (noOfRecord > 0) {
        html_comment +=
            "<li id=loadSubCommentsContainer_" + encoded_comment_id + "><a id='onLoadSubComments_" + encoded_comment_id + "' href=\"javascript:getMoreSubComment('" + encoded_comment_id + "','" + post_id + "')\"><i class='glyphicon glyphicon-share-alt' style='transform: scale(1, -1);'></i>  Show " + data.comments + " response(s)</a>" +
            "<a style='display:none' id = 'hideSubComment" + encoded_comment_id + "' href =\"javascript:onHideSubComment('" + post_id + "','" + encoded_comment_id + "')\"><i class= \"glyphicon glyphicon-share-alt\" style = \"transform: scale(-1, 1);\" ></i > Hide " + data.comments + " comments</a>" +
            "</li>";
    }
    html_comment += "</ul><ul style='display:none' id='reply_box_" + encoded_comment_id + "' class='comments-list reply-list'>" +
        "<li>" +
        "<div class='media-left'>" +
        "<div class='comment-avatar'><img src='" + data.owner.user_picture + "' alt=''></div>" +
        "</div>" +
        "<div class='comment-box'>" +
        "<form id='reply_box_form_" + encoded_comment_id + "'>" +
        "<input type='hidden' name='post_id' value='" + post_id + "' />" +
        "<input type='hidden' name='parent_comment_id' value='" + encoded_comment_id + "' />" +
        "<textarea id='reply_text_area_" + encoded_comment_id + "' name='comment_text' onkeypress=\"return enter_submit_sub_comment(this,event,'" + encoded_comment_id + "','" + post_id + "')\" required class='form-control' placeholder='What are you thinking?'></textarea>" +
        "</form>" +
        "</div>" +
        "</li>" +
        "</ul>" + "</li>";
    return html_comment;
}
function sub_comment_to_html(data, comment_id, post_id) {
    var encoded_sub_comment_id = window.btoa(data._id);
    var html_comment =
        "<li id='sub_comment_container_" + encoded_sub_comment_id + "'>" +
        "<div class=\"media-left\">" +
        "<div class=\"comment-avatar\"><img src=\"" + data.owner.user_picture + "\" alt=\"\"></div>" +
        "</div>" +
        "<div class=\"comment-box\">" +
        "<div class=\"comment-head\">" +
        "<div class=\"media-body\">" +
        "<h5 class=\"margin-t-0\"><a href=\"" + data.owner._id + "\">" + data.owner.user_name + "</a></h5>" +
        "<p><span class=\"glyphicon glyphicon-time\"></span> " + moment(data.date_created).fromNow() + "</p>" +
        "</div>" +
        "<div class='media-right'>";
    if (data.is_own == true) {
        html_comment += "<a href=\"javascript:onEditComment('" + encoded_sub_comment_id + "')\" > <i class='fa fa-edit'></i></a >" +
            "<a href=\"javascript:onDeleteSubComment('" + encoded_sub_comment_id + "')\"><i class='fa fa-close'></i></a>";
    }
    html_comment += "</div >" +
        "</div>" +
        "<div class=\"comment-content\">" +
        "<form id='comment_form_" + encoded_sub_comment_id + "'>" +
        "<input type = 'hidden' name = 'comment_id' value = '" + comment_id + "' />" +
        "<input type='hidden' name='post_id' value='" + post_id + "' />" +
        "<input type='hidden' name='sub_comment_id' value='" + encoded_sub_comment_id + "' />" +
        "<textarea name='comment_text' id='edit_comment_text_area_" + encoded_sub_comment_id + "' onkeypress=\"return enter_edit_sub_comment(this, event, '" + encoded_sub_comment_id + "')\" style='display: none' class='form-control'></textarea>" +
        "</form >" +
        "<p style='word-wrap: break-word;white-space: pre-line;' id='comment_text_" + encoded_sub_comment_id + "'>" + data.message + "</p>" +
        "</div>" +
        "<div class=\"comment-content\">" +
        "<p style='float: left'>";
    if (data.has_like == 'none') {
        html_comment +=
            "<a value='' id='comment_" + encoded_sub_comment_id + "' href=\"javascript:onLike('" + encoded_sub_comment_id + "','like')\" onmouseover = \"onChoseEmoji('" + encoded_sub_comment_id + "','c')\" >"
            + "<i id = 'icon_" + encoded_sub_comment_id + "' class='glyphicon glyphicon-thumbs-up' ></i > Like</a >";
    } else {
        html_comment += emoji_to_text(encoded_sub_comment_id, data.has_like , 'c');
    }
    var total_like = data.like + data.love + data.angry + data.scare + data.haha + data.sad + data.amaze + data.suprise;
    html_comment += "<a href=\"javascript:showReplyBox('" + comment_id + "')\">" +
        "<i class=\"glyphicon glyphicon-comment\"></i> Reply" +
        "</a>" +
        "</p><p style='float: right;margin-top: 6px;'>";
    if (data.like > 0) {
        html_comment += "<span class='Selector selectorFace20 like20'></span>";
    }
    if (data.love > 0) {
        html_comment += "<span class='Selector selectorFace20 love20'></span>";
    }
    if (data.angry > 0) {
        html_comment += "<span class='Selector selectorFace20 angry20'></span>";
    }
    if (data.scare > 0) {
        html_comment += "<span class='Selector selectorFace20 scare20'></span>";
    }
    if (data.haha > 0) {
        html_comment += "<span class='Selector selectorFace20 haha20'></span>";
    }
    if (data.sad > 0) {
        html_comment += "<span class='Selector selectorFace20 sad20'></span>";
    }
    if (data.amaze > 0) {
        html_comment += "<span class='Selector selectorFace20 amaze20'></span>";
    }
    if (data.suprise > 0) {
        html_comment += "<span class='Selector selectorFace20 suprise20'></span>";
    }
    html_comment +=
        total_like + "</p></div>" +
        "</div>" +
        "</li>";
    return html_comment;
}
function show_sub_comment(comment_id, data, post_id) {
    var comment = $("#subCommentsContainer" + comment_id);
    //comment.find(' > ul:nth-last-child(1)').before(sub_comment_to_html(data, comment_id, post_id));
    comment.append(sub_comment_to_html(data, comment_id, post_id));
}
function enter_submit_sub_comment(myfield, e, comment_id, post_id) {
    var keycode;
    if (window.event) keycode = window.event.keyCode;
    else if (e) keycode = e.which;
    else return true;

    if (keycode == 13) {
        reply_comment(comment_id, post_id);
        return false;
    }
    else
        return true;
}
function enter_submit_comment(myfield, e, post_id) {
    var keycode;
    if (window.event) keycode = window.event.keyCode;
    else if (e) keycode = e.which;
    else return true;

    if (keycode == 13) {
        comment(post_id)
        return false;
    }
    else
        return true;
}
function enter_edit_comment(myfield, e, comment_id) {
    var keycode;
    if (window.event) keycode = window.event.keyCode;
    else if (e) keycode = e.which;
    else return true;

    if (keycode == 13) {
        edit_comment(comment_id);
        return false;
    }
    else
        return true;
}

function enter_edit_sub_comment(myfield, e, sub_comment_id) {
    var keycode;
    if (window.event) keycode = window.event.keyCode;
    else if (e) keycode = e.which;
    else return true;

    if (keycode == 13) {
        edit_sub_comment(sub_comment_id);
        return false;
    }
    else
        return true;
}
function getComments(post_id) {
    var boxComments = $("#comments-list-" + post_id);
    var noOfComment = $('#no_of_comment_' + post_id);
    var totalComments = $('#total_comment_' + post_id);
    $('#on_click_' + post_id).attr('href', 'javascript:void(0)');
    $.ajax({
        type: "post",
        url: "/comment/getComments",
        data: $("#get_comment_form_" + post_id).serialize(),
        success: function (response) {
            var data = JSON.parse(response);
            var commet_left = totalComments.val() - data.length;
            var html_string = "<li ><a id='load_more_comments_" + post_id + "' href=\"javascript:getComments('" + post_id + "')\">Load more " + commet_left + " comments</a></li>";
            for (i = 0; i < data.length; i++) {
                html_string += comment_to_html(data[i], post_id);
            }
            boxComments.find(' > li:first').remove();
            if (noOfComment.val() > 0) {
                noOfComment.val(noOfComment.val() - 1);
                totalComments.val(totalComments.val() - data.length);
            }
            boxComments.prepend(html_string);
            if (noOfComment.val() == 0) {
                $('#load_more_comments_' + post_id).css('display', 'none');
            }

        },
        error: function (xhr, response, error) {
            var err = JSON.parse(xhr.responseText);
            alert(err.message);
        }
    });

}
function getMoreSubComment(comment_id, post_id) {
    var boxComments = $("#subCommentsContainer" + comment_id);
    var total_sub_comments = $("#total_sub_comments_" + comment_id);
    var onLoadSubComments = $('#onLoadSubComments_' + comment_id);
    onLoadSubComments.attr('href', 'javascript:void(0)');
    var noOfComment = $('#no_of_sub_comment_' + comment_id);
    $.ajax({
        type: "post",
        url: "/comment/GetSubComments",
        data: $("#comment_form_" + comment_id).serialize(),
        success: function (response) {
            var data = JSON.parse(response);
            var sub_comment_left = total_sub_comments.val() - data.length;
            var html_string = "<li id=\"loadSubCommentsContainer_" + comment_id + "\"><a id=\"onLoadSubComments_" + comment_id + "\" href=\"javascript:getMoreSubComment('" + comment_id + "', '" + post_id + "')\"> <i class=\"glyphicon glyphicon-share-alt\" style=\"transform: scale(1, -1); \"></i> Show " + sub_comment_left + " response(s)</a > <a style=\"display: none; \" id=\"hideSubComment" + comment_id + "\" href=\"javascript:onHideSubComment('" + post_id + "', '" + comment_id + "')\"><i class=\"glyphicon glyphicon-share-alt\" style=\"transform: scale(-1, 1); \"></i> Hide all comments</a></li > ";
            for (i = 0; i < data.length; i++) {
                html_string += sub_comment_to_html(data[i], comment_id, post_id);
            }
            boxComments.find(' > li:first').remove();
            if (noOfComment.val() > 0) {
                var clear_comments = Math.ceil(total_sub_comments.val() * 1.0 / 5)
                if (noOfComment.val() == clear_comments) {
                    $('#subCommentsContainer' + comment_id).empty();
                }
                total_sub_comments.val(sub_comment_left);
                noOfComment.val(noOfComment.val() - 1);
                onLoadSubComments.attr('href', "javascript:getMoreSubComment('" + comment_id + "', '" + post_id + "') ");
                onLoadSubComments.html('<i class="glyphicon glyphicon-share-alt" style="transform: scale(1, -1);"></i> Show ' + sub_comment_left + ' response(s)');
            }
            boxComments.prepend(html_string);
            if (noOfComment.val() == 0) {
                $('#hideSubComment' + comment_id).css('display', "block");
                $('#onLoadSubComments_' + comment_id).css('display', 'none');
            }

        },
        error: function (xhr, response, error) {
            var err = JSON.parse(xhr.responseText);
            alert(err.message);
        }
    });
}

function onHideSubComment(post_id, comment_id) {
    $('#subCommentsContainer' + comment_id + ' li:not(:first)').remove();
    var onLoadSubComments = $('#onLoadSubComments_' + comment_id);
    onLoadSubComments.css('display', 'block');
    $('#hideSubComment' + comment_id).css('display', 'none');
    $.ajax({
        type: "post",
        url: "/comment/GetComment",
        data: $("#get_comment_form_" + post_id).serialize() + "&comment_id=" + comment_id,
        success: function (response) {
            var data = JSON.parse(response);
            if (data.comments == 0) {
                onLoadSubComments.remove();
            }
            onLoadSubComments.empty();
            onLoadSubComments.html("<i class=\"glyphicon glyphicon-share-alt\" style=\"transform: scale(1, -1);\"></i> Show " + data.comments + " reponse(s)");
            $('#hideSubComment' + comment_id).html("<i class=\"glyphicon glyphicon-share-alt\" style=\"transform: scale(-1, 1);\"></i> Hide " + data.comments + " comments");
            var noOfRecord = Math.ceil(data.comments * 1.0 / 5);
            var noOfComment = $('#no_of_sub_comment_' + comment_id);
            var total_sub_comments = $("#total_sub_comments_" + comment_id);
            total_sub_comments.val("" + data.comments);
            noOfComment.val(noOfRecord);
        },
        error: function (xhr, response, error) {
            var err = JSON.parse(xhr.responseText);
            alert(err.message);
        }
    });
}
function onDeleteComment(comment_id) {
    $(this).attr('onclick', null);
    $.ajax({
        type: "post",
        url: "/comment/delete_comment",
        data: $("#comment_form_" + comment_id).serialize(),
        success: function (response) {
            $('#reply_list_' + comment_id).remove();
        },
        error: function (xhr, response, error) {
            var err = JSON.parse(xhr.responseText);
            alert(err.message);
        }
    });
}
function onDeleteSubComment(sub_comment_id) {
    $.ajax({
        type: "post",
        url: "/comment/delete_sub_comment",
        data: $("#comment_form_" + sub_comment_id).serialize(),
        success: function (response) {
            $('#sub_comment_container_' + sub_comment_id).remove();
        },
        error: function (xhr, response, error) {
            var err = JSON.parse(xhr.responseText);
            alert(err.message);
        }
    });
}
function onEditComment(comment_id) {
    var comment_text = $('#comment_text_' + comment_id);
    var comment_area = $('#edit_comment_text_area_' + comment_id);
    if (comment_area.css('display') == 'block') {
        comment_area.css('display', 'none');
        comment_text.css('display', 'block');
    } else {
        comment_text.css('display', 'none');
        comment_area.css('display', 'block');
        comment_area.text(comment_text.text());
    }
}
function edit_comment(comment_id) {
    $.ajax({
        type: "post",
        url: "/comment/edit_comment",
        data: $("#comment_form_" + comment_id).serialize(),
        success: function (response) {
            var comment_text = $('#comment_text_' + comment_id);
            var comment_area = $('#edit_comment_text_area_' + comment_id);
            comment_text.css('display', 'block');
            comment_area.css('display', 'none');
            comment_text.text(response);
        },
        error: function (xhr, response, error) {
            alert(error);
            var err = JSON.parse(xhr.responseText);
            alert(err.message);
        }
    });
}

function edit_sub_comment(sub_comment_id) {
    $.ajax({
        type: "post",
        url: "/comment/edit_SubComment",
        data: $("#comment_form_" + sub_comment_id).serialize(),
        success: function (response) {
            var comment_text = $('#comment_text_' + sub_comment_id);
            var comment_area = $('#edit_comment_text_area_' + sub_comment_id);
            comment_text.css('display', 'block');
            comment_area.css('display', 'none');
            comment_text.text(response);
        },
        error: function (xhr, response, error) {
            alert(error);
            var err = JSON.parse(xhr.responseText);
            alert(err.message);
        }
    });
}
function onLike(comment_id, emo) {
    $.ajax({
        type: "post",
        url: "/like/comment_like",
        data: $("#comment_form_" + comment_id).serialize() + "&emo=" + emo,
        success: function (response) {
            var button = $('#comment_' + comment_id);
            if (button.prop('name') == emo) {
                button.html("<i class='glyphicon glyphicon-thumbs-up'></i> Like");
                button.attr('style', '');
                button.attr('name', null);
                button.attr('href', "javascript:onLike('" + comment_id + "','like')");
            } else {
                emoji(comment_id, emo, 'c');
            }

        },
        error: function (xhr, response, error) {
            var err = JSON.parse(xhr.responseText);
            alert(err.message);
        }
    });
}

function onPostLike(post_id, emo) {
    $.ajax({
        type: "post",
        url: "/like/post_like",
        data: "post_id=" + post_id + "&emo=" + emo,
        success: function (response) {
            var button = $('#post_' + post_id);
            if (button.prop('name') == emo) {
                button.html("<i class='glyphicon glyphicon-thumbs-up'></i> Like");
                button.attr('style', '');
                button.attr('name', null);
                button.attr('href', "javascript:onPostLike('" + post_id + "','like')");
            } else {
                emoji(post_id, emo, 'p');
            }

        },
        error: function (xhr, response, error) {
            var err = JSON.parse(xhr.responseText);
            alert(err.message);
        }
    });
}
function onChoseEmoji(e, data, action) {
    var timer;
    var delay = 500;
    timer = setTimeout(function () {
        var RatonX = e.pageX - 20; var RatonY = e.pageY - 60;
        $(".faceMocion").css({ top: RatonY, left: RatonX });
        $(".faceMocion").show();
        $(".faceMocion").attr('id', data);
        $(".faceMocion").attr('property', action);
    }, delay);

    var entry = "";
    if (action == 'c') {
        entry = "#comment_";
    }
    if (action == "p") {
        entry = "#post_";
    }
    //$(entry + data).hover(function (e) {
    //    timer = setTimeout(function () {
    //        //var RatonX = e.pageX - 20; var RatonY = e.pageY - 60;
    //        //$(".faceMocion").css({ top: RatonY, left: RatonX });
    //        //$(".faceMocion").show();
    //        //$(".faceMocion").attr('id', data);
    //        //$(".faceMocion").attr('property', action);
    //    }, delay);
    //}, function () {
    //    clearTimeout(timer);
    //});
}
function emoji(_id, emo, action) {
    var button = null;
    if (action == "p") {
        button = $('#post_' + _id);
        button.attr('href', "javascript:onPostLike('" + _id + "','" + emo + "')");
    }
    if (action == "c") {
        button = $('#comment_' + _id);
        button.attr('href', "javascript:onLike('" + _id + "','" + emo + "')");
    }
    if (emo == 'like') {
        button.html("<i class='Selector selectorFace20 " + emo + "20'></i>Like");
        button.attr('style', 'color:blue');
    }
    if (emo == 'love') {
        button.html("<i  class='Selector selectorFace20 " + emo + "20'></i>Love");
        button.attr('style', 'color:#e03c59');
    }
    if (emo == 'angry') {
        button.html("<i  class='Selector selectorFace20 " + emo + "20'></i>Angry");
        button.attr('style', 'color:#f36769');
    }
    if (emo == 'scare') {
        button.html("<i  class='Selector selectorFace20 " + emo + "20'></i>Scare");
        button.attr('style', 'color:#9e9e01');
    }
    if (emo == 'haha') {
        button.html("<i  class='Selector selectorFace20 " + emo + "20'></i>Haha");
        button.attr('style', 'color:#d6d606');
    }
    if (emo == 'sad') {
        button.html("<i  class='Selector selectorFace20 " + emo + "20'></i>Sad");
        button.attr('style', 'color:#d6d606');
    }
    if (emo == 'amaze') {
        button.html("<i  class='Selector selectorFace20 " + emo + "20'></i>Amaze");
        button.attr('style', 'color:#d6d606');
    }
    if (emo == 'suprise') {
        button.html("<i  class='Selector selectorFace20 " + emo + "20'></i>WOW");
        button.attr('style', 'color:#d6d606');
    }
    button.attr('name', emo);
}
function emoji_to_text(_id, emo, action) {
    var value = "";
    var onclick = "";
    var access_id = "";
    if (action == "p") {
        access_id = "post_" + _id;
        onclick = "javascript:onPostLike('" + _id + "','" + emo + "')";
    }
    if (action == "c") {
        access_id = "comment_" + _id;
        onclick = "javascript:onLike('" + _id + "','" + emo + "')";
    }
    if (emo == 'like') {
        value = "<a name='" + emo + "' style='color:blue' id='" + access_id + "' href=\"" + onclick + "\" onmouseover = \"onChoseEmoji('" + _id + "','" + action + "')\" >"
            + "<i id = 'icon_" + _id + "' class='Selector selectorFace20 " + emo + "20' ></i>Like</a >";
    }
    if (emo == 'love') {
        value = "<a name='" + emo + "' style='color:#e03c59' id='" + access_id + "' href=\"" + onclick + "\" onmouseover = \"onChoseEmoji('" + _id + "','" + action + "')\" >"
            + "<i id = 'icon_" + _id + "' class='Selector selectorFace20 " + emo + "20' ></i>Love</a>";
    }
    if (emo == 'angry') {
        value = "<a name='" + emo + "' style='color:#f36769' id='" + access_id + "' href=\"" + onclick + "\" onmouseover = \"onChoseEmoji('" + _id + "','" + action + "')\">"
            + "<i id = 'icon_" + _id + "' class='Selector selectorFace20 " + emo + "20' ></i>Angry</a>";
    }
    if (emo == 'scare') {
        value = "<a name='" + emo + "' style='color:#9e9e01' id='" + access_id + "' href=\"" + onclick + "\" onmouseover = \"onChoseEmoji('" + _id + "','" + action + "')\">"
            + "<i id = 'icon_" + _id + "' class='Selector selectorFace20 " + emo + "20' ></i>Scare</a >";
    }
    if (emo == 'haha') {
        value = "<a name='" + emo + "' style='color:#d6d606' id='" + access_id + "' href=\"" + onclick + "\" onmouseover = \"onChoseEmoji('" + _id + "','" + action + "')\" >"
            + "<i id = 'icon_" + _id + "' class='Selector selectorFace20 " + emo + "20' ></i>Haha</a >";
    }
    if (emo == 'sad') {
        value = "<a name='" + emo + "' style='color:#d6d606' id='" + access_id + "' href=\"" + onclick + "\" onmouseover = \"onChoseEmoji('" + _id + "','" + action + "')\" >"
            + "<i id = 'icon_" + _id + "' class='Selector selectorFace20 " + emo + "20' ></i>Sad</a>";
    }
    if (emo == 'amaze') {
        value = "<a name='" + emo + "' style='color:#d6d606' id='" + access_id + "' href=\"" + onclick + "\" onmouseover = \"onChoseEmoji('" + _id + "','" + action + "')\" >"
            + "<i id = 'icon_" + _id + "' class='Selector selectorFace20 " + emo + "20' ></i >Amaze</a >";
    }
    if (emo == 'suprise') {
        value = "<a name='" + emo + "' style='color:#d6d606' id='" + access_id + "' href=\"" + onclick + "\" onmouseover = \"onChoseEmoji('" + _id + "','" + action + "')\" >"
            + "<i id = 'icon_" + _id + "' class='Selector selectorFace20 " + emo + "20' ></i>WOW</a >";
    }
    return value;
}
function openModal() {
    document.getElementById('myModal').style.display = "block";
    $("body").css({ "overflow": "hidden" });
    var container = $('#modal_post_content');
    container.empty();
}

function closeModal() {
    document.getElementById('myModal').style.display = "none";
    $("body").css({ "overflow": "visible" });
    var container = $('#modal_post_content');
    container.empty();
}
function getVideoImageHTML(data, viewall, post_id) {
    var value = "";
    if (data.type == '.mp4') {
        value += "<div class='column'><div class='imgs-grid imgs-grid-1'><div class='imgs-grid-image'><div class='image-wrap' >" +
            "<video autoplay muted class='demo cursor' src='" + data.link + "' style = 'width:100%;height:80px' onclick=\"onViewSubPost('" + window.btoa(data._id) + "','" + post_id + "')\" ></video>" +
            viewall + "</div></div></div></div>";
    } else {
        value += "<div class='column'><div class='imgs-grid imgs-grid-1'><div class='imgs-grid-image'><div class='image-wrap' >" +
            "<img class='demo cursor' src='" + data.link + "' style = 'width:100%;height:80px' onclick=\"onViewSubPost('" + window.btoa(data._id) + "','" + post_id + "')\">" +
            viewall + "</div></div></div></div>";
    }
    return value;
}

function onViewSubPost(sub_post_id, post_id) {
    $.ajax({
        type: "post",
        url: "/posts/getVideoImage",
        data: 'post_id=' + post_id + '&sub_post_id=' + sub_post_id,
        success: function (response) {
            var data = JSON.parse(response);
            var container = $('#modal_post_content');
            var owner_img = $('#owner_img');
            var owner_name = $('#owner_name');
            var creation_time = $('#creation_time');
            owner_img.attr('src', data.owner.user_picture);
            owner_name.text(data.owner.user_name);
            creation_time.html("<span class='glyphicon glyphicon-time'></span> " + moment(data.date_created).fromNow());
            var prev = $('#prev');
            var next = $('#next');
            container.empty();
            if (data.video_image[1].type == '.mp4') {
                container.append("<video class='img-responsive' style='width: 100%;height:300px' src='" + data.video_image[1].link + "' autoplay controls></video>");
            } else {
                container.append("<img class='img-responsive' style='width: 100%;height:300px' src='" + data.video_image[1].link + "'></img>");
            }
            if (data.video_image[0].type == null) {
                prev.attr('style', 'display:none');
            } else {
                prev.attr('onclick', "onViewSubPost('" + window.btoa(data.video_image[0]._id) + "','" + post_id + "')");
                prev.attr('style', 'display:block');
            }
            if (data.video_image[2] == null) {
                next.attr('style', 'display:none');
            } else {
                next.attr('onclick', "onViewSubPost('" + window.btoa(data.video_image[2]._id) + "','" + post_id + "')");
                next.attr('style', 'display:block');
            }
            var preview = "";
            var data_length = data.video_image.length;
            if (data.video_image.length > 5) {
                data_length = 5;
            }
            preview += getVideoImageHTML(data.video_image[1], "<div class='view-all'><span class='view-all-cover'></span><span class='view-all-text'>Viewing</span></div>", post_id);
            for (i = 2; i < data_length; i++) {
                preview += getVideoImageHTML(data.video_image[i], "", post_id);
            }
            var file_preview = $('#file_preview');
            file_preview.empty();
            file_preview.append(preview);
        },
        error: function (xhr, response, error) {
            alert(error);
            var err = JSON.parse(xhr.responseText);
            alert(err.message);
        }
    });
}