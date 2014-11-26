$(document).ready(function () {
    var feedItems;

    $('.unsubscribe').click(function (event) {
        var feedId = $(this).attr('id');
        var url = '/api/feedapi/' + feedId;
        var feedTitle = $(this).closest('div[role="tab"]').first().attr('id');
        $.ajax({
            url: url,
            type: "DELETE"
        })
            .done(function (data) {
                unsubscribedAlert(data, feedTitle);
                $('div[data-row-key="' + data + '"]').first().remove();
            })
            .fail(function (jqXHR, status, err) {
                alert(status + ": " + err);
            });
    });

    $('#feedSearchBtn, #viewAll').click(function (event) {

        $('#foundArticles').empty();

        var searchEntry = $(this).attr("id") == "feedSearchBtn" ? $('#feedSearchEntry').val() : "";
        if (feedItems == null) {
            feedItems = $('.media-body');
        }

        feedItems.each(function (index, article) {
            console.log($(this).children("h4:contains('" + searchEntry + "')").length);

            if ($(this).children("h4:contains('" + searchEntry + "')").length > 0 ||
                $(this).children("p:contains('" + searchEntry + "')").length > 0) {
                var articleToAdd = $(this).parents('.media');
                $(articleToAdd).appendTo("#foundArticles");
                $("<hr/>").appendTo("#foundArticles");
                $("#searchFeedResults").removeClass("hidden");
                $("#accordion").addClass("hidden");
            }
        });

        if ($('#foundArticles').children(".media").length == 0) {
            noResultsAlert($("#foundArticles"));
            addViewFeedsListener();
            $("#searchFeedResults").removeClass("hidden");
            $("#accordion").addClass("hidden");
        }
    });
});

function unsubscribedAlert(data, feedTitle) {
    var unsubscribedDiv = '<div class="alert alert-info alert-dismissible" role="alert" id="unsubscribed">' +
        '<button type="button" class="close" data-dismiss="alert"><span aria-hidden="true">&times;</span>' +
        '<span class="sr-only">Close</span></button>You have unsubscribed from <strong>' +
        feedTitle + '</strong>.</div>';

    $('body').append(unsubscribedDiv);
}

function noResultsAlert(element) {
    var alertDiv = '<div class="alert alert-info">There were no articles in any of your feeds matching that search.  Try another ' +
        'search or <a href="#" title="View Feeds" id="viewFeeds">go back to viewing all feeds</a></div>';

    $(alertDiv).appendTo(element);
};

function addViewFeedsListener() {
    $('#viewFeeds').click(function (event) {
        event.preventDefault();
        $('#foundArticles').empty();
        $("#searchFeedResults").addClass("hidden");
        $("#accordion").removeClass("hidden");
    });
};