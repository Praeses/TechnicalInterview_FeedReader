$(document).ready(function () {
    $('.subscribe').click(function (event) {
        var feedId = $(this).attr('id');
        var url = '/api/feedapi/' + feedId;
        var feedTitle = $(this).siblings('a').first().attr('title');
        $.post(url)
            .done(function (data) {
                mediaRemovedAlert(data, feedTitle);
                $('button#' + data).parents('div.media').remove();
            })
            .fail(function (jqXHR, status, err) {
                alert(status + ": " + err);
            });
    });
});

function mediaRemovedAlert(data, feedTitle) {

    var mediaRemovedDiv = '<div class="alert alert-success alert-dismissible" role="alert" id="mediaRemoved">' +
        '<button type="button" class="close" data-dismiss="alert"><span aria-hidden="true">&times;</span>' +
        '<span class="sr-only">Close</span></button>You have successfully subscribed to <strong>' +
        feedTitle + '</strong>.</div>';

    $('body').append(mediaRemovedDiv);
};