var removeSubLinkObj;
$('body').on('click', '.webGridRemoveAction', function () {
    //save link object for after confirmation
    removeSubLinkObj = $(this);
    $('#divConfirmRemove').dialog('open');
    return false; 
});
$('#divConfirmRemove').dialog({
    autoOpen: false, width: 400, resizable: false, modal: true, 
    buttons: {
        "Continue": function () {
            var url = $(this).data('request-url');
            var token = $(this).data('token');
            var str = removeSubLinkObj.attr("id").split("_");
            var id = str[1];
            $.ajax({
                type: "POST",
                contentType: "application/json",
                url: url,
                data: JSON.stringify({ "subscriptionId": id }),
                dataType: "json",
                headers: { '__RequestVerificationToken': token },
                success: function (data) {

                    if (data.result == true) {
                        $("#Remove_" + id).parents("tr").remove();
                    }
                    else {
                        alert('There was an error while removing the subscription');
                    }
                }
            });
            $(this).dialog("close");
        },
        "Cancel": function () {
            $(this).dialog("close");
        }
    }
});
//jQueryUI method to create dialog box
$("#divNewSubscription").dialog({
    autoOpen: false,
    modal: true,
    width: 450,
    title: "Add New Subscription"
});
$("#buttonNewSubscription").button().click(function () {
    $.ajax({
        //Call CreatePartialView action method

        url: "/Subscription/Add",
        type: 'Get',
        success: function (data) {
            $("#divNewSubscription").dialog("open");
            $("#divNewSubscription").empty().append(data);
        },
        error: function () {
            alert("something seems wrong");
        }
    });
});