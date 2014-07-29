var selectorHTML =
    '<div"><table id="RSSSelectorTable">\
		<tr>\
		<td style="width:200px;"><ol id="RSSSelectableElements" style="overflow-y: scroll; height: 100px;"> </ol></td>\
		<td>\
		<table id="RSSSelectorTable">\
		<tr>\
		<td><button id=RSSSelectorOkay>Okay</button></td>\
		</tr>\
		<tr>\
		<td><button id=RSSSelectorCancel>Cancel</button></td>\
		</tr>\
		</table>\
		</td>\
		</table></div>';



function RSSReader()
{
    var Textbox;
    //contains all connected feeds.
    var feedList = [];
    //all RSS feed entries.
    var entries = [];
    //counts the number of the next unique id for entires.
    var idcount = 1;
    //Html for a selector modal dialog
    //enumerator for sort options. By default, the list is unsorted.
    //the default is ID, which is the number in which they were discovered.
    //Enum prototype borrowed from Stack Overflow
    function Enum(){
        for (var i = 0; i < arguments.length; ++i)
        {
            this[arguments[i]] = i;
        }
        return this;
    }
    function entry()
    {
        /*
            Uses the set methods to initialize an entry.
        */
        this.date = '';
        this.title = '';
        this.link = '';
        this.feed = '';
        this.id =0;
        this.initialize = function(d, t, l, f, i)
        {
            if (d !== undefined)
                this.date = d;
            else
                this.date = "";
            if (t !== undefined)
                this.title = t;
            else {
                this.title = "Unknown Title";
            }
            if (l !== undefined)
                this.link = l;
            else
                this.link="N/A";
            if (f !== undefined)
                this.feed = f;
            else
                this.feed ="Google.com";
            this.id = i;
        }
    }


    $(document).ready(function ()
    {
        Textbox = $('.MitchellRSSTB');
        //Add a new feed for the user and immediately connect if possible.
        $('.MitchellRSSAdd').click(function ()
        {
            var url = prompt("Please enter the RSS url", "");
            if (url != null)
                feedList.push(url);
            refreshFeeds();
        });

        $('.MitchellRSSRefresh').click(function ()
        {
            refreshFeeds();
        });

        $('.MitchellRSSRemove').click(function ()
        {
            openSelector(feedList);
        });

        $(".MitchellRSSSearch").click(function ()
        {
            searchRemove();
        });
        console.log($('#RSSTable'));
        $('#RSSTable').tablesorter();
    });

    /*
        refreshes all stored feeds and updates their entries
    */
    function refreshFeeds()
    {
        //reset id count
        idcount = 1;
        var url;
        entries = [];
        for(var k = 0; k<feedList.length; k++)
        {
            //get data from feed
            url = feedList[k];
            $.ajax(
                {
                    url: document.location.protocol + '//ajax.googleapis.com/ajax/services/feed/load?v=1.0&num=10&callback=?&q=' + encodeURIComponent(url),
                    dataType: 'json',
                    success: function (data)
                    {
                        var returnData = data;
                        console.log(returnData);
                        if (data.responseData != null)
                        {
                            //create references to data
                            var newEntries = returnData.responseData.feed.entries;
                            var max = newEntries.length;
                            console.log(newEntries);
                            //for each new entry, add to entry list.
                            for (var i = 0; i < max; i++) {
                                var e = new entry();
                                e.initialize(newEntries[i].publishedDate, newEntries[i].title, newEntries[i].link, data.responseData.feed.feedUrl, idcount);
                                entries.push(e);
                                idcount++;
                                displayEntries();
                            }
                            //after getting new enttries, sort and display them all.
                        }
                    }
                });
            
        }
        //redisplay nothing while waiting for entries.
        displayEntries();
    }
    //removes a feed.
    function removeFeed(index)
    {
        if (index>=0)
        {
            feedList.splice(index, 1);
            console.log(feedList);
        }
    }
    //Searching function of the RSS reader. Removes entries if they do not match the search term.
    function searchRemove()
    {
        var searchTerm = prompt("Please enter the search term");
        for(var i =0; i <entries.length; i++)
        {
            if(entries[i].title.indexOf(searchTerm)==-1)
            {
                entries.splice(i,1);
                i--;
                displayEntries();
            }
        }
    }

    //Displays all entries.
    function displayEntries()
    {
        //Remove previous table entries
        $table = $("#RSSTable");
        $.tablesorter.clearTableBody($table);
        for (var i = 0; i < entries.length; i++) {
            $table.append('<tr class="entry'+entries[i].id+'" value="'+entries[i].link+'"><td>' + entries[i].title + '</td><td>' + entries[i].date + '</td><td>' + entries[i].link + '</td><td>' +
                entries[i].feed + '</td></tr>');
            //Store the link in a form that we can reference easily later.
            $(".entry" + entries[i].id).data("url", entries[i].link);
            //on doubleclick, open window in new tab. Code for new tab
            //borrowed from stack exchange
            $(".entry" + entries[i].id).dblclick(function (event)
            {
                var win = window.open(event.currentTarget.childNodes[2].innerHTML, '_blank');
                win.focus();
            });
        }
        console.log($(".entry1"));
        $("#RSSTable").trigger("update");
    }



    //opens the select dialog for removing an RSS feed
    function openSelector(list) {
        var selectedIndex = -1;
        var dialogDiv = document.createElement("div");	// dynamically create div for the dialog
        dialogDiv.id = "RSSSelector";		// give the div an id
        dialogDiv.setAttribute("title", "Remove which feed?");// set the title accordingly
        dialogDiv.innerHTML = selectorHTML;				// set the inner HTML of this div
        document.body.appendChild(dialogDiv);			// append the div to the body

        $("#RSSSelector").dialog({ width: 325, modal: true, resizable: false});
        $("#RSSSelectorOkay").click(okayButton);
        $("#RSSSelectorCancel").click(cancelButton);
        $("#RSSSelector").on("dialogclose", cancelButton);

        select = document.getElementById("RSSSelectableElements");

        for (i = 0; i < list.length; i++) {
            var newNumberListItem = document.createElement("li");
            newNumberListItem.ondblclick = function () {
                okayButton();
            }
            newNumberListItem.className += " ui-widget-content index" + i;
            var numberListValue = document.createTextNode(list[i]);
            newNumberListItem.appendChild(numberListValue);
            select.appendChild(newNumberListItem);
        }

        //turns the selector into a jquery selectable
        $("#RSSSelectableElements").selectable(
        {
            //update the selectedIndex on select action.
            selected: function (event, ui) {
                selectedItem = ui.selected.textContent;
                var classNames = ui.selected.className;
                //look for "index" in the class names, then start parsing for the end.
                var start = classNames.indexOf("index") + 5;
                var ind = "";
                for (var i = start; i < classNames.length; i++) {
                    if (classNames.charAt(i) != " ") ind += classNames.charAt(i);
                    else break;
                }
                selectedIndex = parseFloat(ind);
            }
        });

        //send signal to remove the feed at the selected index.
        function okayButton() {
            removeFeed(selectedIndex);
            $("#RSSSelector").dialog("close");
            $("#RSSSelector").remove();
        }

        function cancelButton() {
            console.log('hi');
            removeFeed(-1);
            $("#RSSSelector").dialog("close");
            $("#RSSSelector").remove();
        }
    }
}
var start = new RSSReader;