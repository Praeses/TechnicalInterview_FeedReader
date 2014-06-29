function buildSubscriptionTabs(keyList, model) {

    $('#tabs').append('<li class="active"><a href="#all" role="tab" data-toggle="tab">Show All</a></li>')
    $('#tabDivContainer').append('<div class="tab-pane active backInGray padding20 roundBottomCorners fade in minw1085" id="all"><table id="allTable" class="table minw1085" cellspacing="0" width="100%"></div>');
    buildShowAllTable(keyList, model);

    for (var i = 0; i < keyList.length; i++) {
        var mapKey = keyList[i];
        var key = mapKey.replace(/ /g, '');
        $('#tabs').append('<li><a href="#' + key + '" role="tab" data-toggle="tab">' + mapKey + '</a></li>')
        $('#tabDivContainer').append('<div class="tab-pane backInGray padding20 roundBottomCorners fade minw940" id="' + key + '"><table id="' + key + 'Table" class="table minw1085" cellspacing="0" width="100%"></table></div>');

        buildTableForKey(mapKey, key + 'Table', model);
    }
}

function buildShowAllTable(keyList, model) {
    var dataSet = [];

    for (var i = 0; i < keyList.length; i++) {
        var key = keyList[i];
        var data = model[key];

        for (var j = 0; j < data.length; j++) {
            var obj = data[j];
            var formattedObj = { "Source": key, "Title": obj.Title, "Description": obj.Description, "Link": obj.Link, "PublicationDate": obj.PublicationDate };
            dataSet.push(formattedObj);
        }
    }

    var showAllTable = $('#allTable').dataTable({
        "aoColumnDefs": [
            { "aTargets": [0], "sTitle": "Source", "sClass": "text-center", "bSearchable": false, "bSortable": false, "sWidth": "10%" },
            { "aTargets": [1], "sTitle": "Title", "sClass": "text-center", "bSearchable": true, "bSortable": false, "sWidth": "10%" },
            { "aTargets": [2], "sTitle": "Description", "sClass": "text-center", "bSearchable": true, "bSortable": false, "sWidth": "65%" },
            { "aTargets": [3], "sTitle": "Publication Date", "sClass": "text-center", "bSearchable": false, "bSortable": true, "sWidth": "15%" }
        ],
        "sScrollY": "500px",
        "aaSorting": [[3, 'desc']],
        "bAutoWidth": false
    });


    for (var i = 0; i < dataSet.length; i++) {

        var shortTitle = dataSet[i].Title;
        var shortDesc = dataSet[i].Description;

        if (shortTitle.length > 45) {
            shortTitle = shortTitle.substring(0, 44) + "...";
        }

        if (shortDesc.length > 400) {
            shortDesc = shortDesc.substring(0, 299) + "...";
        }

        showAllTable.fnAddData([
            dataSet[i].Source,
            '<a href="' + dataSet[i].Link + '" target="_blank">' + shortTitle + '</a>',
            shortDesc + '<br /><a href="' + dataSet[i].Link + '" target="_blank">Read More</a>',
            dataSet[i].PublicationDate
        ]);
    }

    $('#allTable_filter').addClass('whiteText');
    $('#allTable_info').addClass('whiteText');
    $('#allTable_length').addClass('whiteText');
    $('select[name="allTable_length"').removeClass('whiteText');
}

function buildTableForKey(key, tableName, model) {
    var dataSet = [];

    var data = model[key];

    for (var j = 0; j < data.length; j++) {
        var obj = data[j];
        var formattedObj = { "Title": obj.Title, "Description": obj.Description, "Link": obj.Link, "PublicationDate": obj.PublicationDate };
        dataSet.push(formattedObj);
    }

    var table = $('#' + tableName).dataTable({
        "aoColumnDefs": [
            { "aTargets": [0], "sTitle": "Title", "sClass": "text-center", "bSearchable": true, "bSortable": false, "sWidth": "15%" },
            { "aTargets": [1], "sTitle": "Description", "sClass": "text-center", "bSearchable": true, "bSortable": false, "sWidth": "65%" },
            { "aTargets": [2], "sTitle": "Publication Date", "sClass": "text-center", "bSearchable": false, "bSortable": true, "sWidth": "20%" }
        ],
        "sScrollY": "500px",
        "aaSorting": [[2, 'desc']],
        "bAutoWidth": false
    });


    for (var i = 0; i < dataSet.length; i++) {
        var shortTitle = dataSet[i].Title;
        var shortDesc = dataSet[i].Description;

        if (shortTitle.length > 45) {
            shortTitle = shortTitle.substring(0, 44) + "...";
        }

        if (shortDesc.length > 400) {
            shortDesc = shortDesc.substring(0, 299) + "...";
        }

        table.fnAddData([
            '<a href="' + dataSet[i].Link + '" target="_blank">' + shortTitle + '</a>',
            shortDesc + '<br /><a href="' + dataSet[i].Link + '" target="_blank">Read More</a>',
            dataSet[i].PublicationDate
        ]);
    }

    $('#' + tableName + '_scrollHeadInner, #' + tableName + '_scrollHeadInner').width('100%');
    $('#' + tableName + '_filter').addClass('whiteText');
    $('#' + tableName + '_info').addClass('whiteText');
    $('#' + tableName + '_length').addClass('whiteText');
    $('select[name="' + tableName + '_length"').removeClass('whiteText');

}