$(function () {

    $("#TxtExpirydate").datepicker({

        showOn: "button",

        buttonImage: "Images/cal.gif",

        buttonImageOnly: true,

        buttonText: "Select date",
        changeMonth: true, changeYear: true

    });

    $("#TxtSOP").datepicker({

        showOn: "button",

        buttonImage: "Images/cal.gif",

        buttonImageOnly: true,

        buttonText: "Select date",
        changeMonth: true, changeYear: true

    });

    $("#TxtFromDate").datepicker({ showOn: "button", buttonImage: "Images/cal.gif", buttonImageOnly: true, buttonText: "Select date", changeMonth: true, changeYear: true });

    $("#TxtToDate").datepicker({ showOn: "button", buttonImage: "Images/cal.gif", buttonImageOnly: true, buttonText: "Select date", changeMonth: true, changeYear: true });

    $("#TxtOJTDate").datepicker({ showOn: "button", buttonImage: "Images/cal.gif", buttonImageOnly: true, buttonText: "Select date", changeMonth: true, changeYear: true });

    $("#TxtNewExpDate").datepicker({ showOn: "button", buttonImage: "Images/cal.gif", buttonImageOnly: true, buttonText: "Select date", changeMonth: true, changeYear: true });
});

function JQueryParentClose(divid) {
    window.parent.$('#' + divid).dialog('close');
    return false;
}

function DialogCloseNRefresh(divid) {
    window.parent.$('#' + divid).dialog('close');
    window.parent.RefreshGrid();  //Make sure there is a function Refresh grid on the parent page
    return false;
}

function DialogClose(divid)
{
    $('#' + divid).dialog('close');
    return false;
}


function Onsuccess(divid, divid2) {
    $("#" + divid).dialog({
        modal: true,
        buttons: {
            Ok: function () {
                $(this).dialog("close");
                window.parent.$('#' + divid2).dialog('close');
            }
        }
    });
}

function OnsuccessRefreshParent(divid, divid2, mode) {
   
      $("#" + divid).dialog({
        modal: true,
        buttons: {
            Ok: function () {
                
                $(this).dialog("close");
                window.parent.$('#' + divid2).dialog('close');
                window.parent.RefreshGrid(mode);
             }
        }
    });
}



function OpenJQueryDialog(divid, w, h, link) {
    $('#' + divid).dialog({
        closeOnEscape: false,
        autoOpen: false,
        title: "Add New",
        bgiframe: true,
        modal: true,
        width: w,
        height: h
                 
    });

    $('#' + divid).dialog('open');
    $('#' + divid).parent().appendTo($("form:first"));

     if (link != '')
    {
      
     $('#modal' + divid).attr('src', link);
    }
    return false;
}

function OpenDialogCustomTitle(divid, w, h, link, title) {
    $('#' + divid).dialog({
        closeOnEscape: false,
        autoOpen: false,
        title: title,
        bgiframe: true,
        modal: true,
        width: w,
        height: h

    });

    $('#' + divid).dialog('open');
    $('#' + divid).parent().appendTo($("form:first"));

    if (link != '') {

        $('#modal' + divid).attr('src', link);
    }
    return false;
}

function OpenDialogForAdding(divid) {
    $('#' + divid).dialog({ autoOpen: false, bgiframe: true, modal: true, title: "", width: 500, height: 400});
    $('#' + divid).dialog('open');
    $('#' + divid).parent().appendTo($("form:first"))

    return false;
}

function OpenDialogFac(divid, url) {
    $("#" + divid).dialog({
        modal: true,
        width: 400,
        height: 300,
        buttons: {
            Close: function (event, ui) {
                window.onbeforeunload = null;
                window.location.href = url;
               
            }

        }
    });
}
function OpenDialog(divid) {
        $("#" + divid).dialog({
            modal: true,
            width: 500, 
            height: 400,
            buttons: {
                Close: function () {
                    $(this).dialog("close");

                   }
            }
        });
    }

function toggleme(ucontrolId, btnShowId, btnHideId) {
    $("#" + ucontrolId).toggle();
    $("#" + btnShowId).toggle();
    $("#" + btnHideId).toggle();

};


 function OpenDialogForName(divid, ctrlid,ctrl2id) {

        $('#' + divid).dialog({ autoOpen: false, title: "Select Employee", bgiframe: true, modal: true, width: 600, height: 650 });
        $('#' + divid).dialog('open');
        $('#' + divid).parent().appendTo($("form:first"));
        link = 'NameFinder.aspx';
        $('#modal' + divid).attr('src', link + '?field=' + ctrlid + '&dialog=' + divid + '&field2=' + ctrl2id);
        return false;
 }
 

 //function OpenDialogNameSmall(divid, ctrlid, ctrl2id) {
 //     alert('hello');
 //       $('#' + divid).dialog({ autoOpen: false, title: "Select Employee", bgiframe: true, modal: true, width: 450, height: 500 });
 //       $('#' + divid).dialog('open');
 //       $('#' + divid).parent().appendTo($("form:first"));
 //       link = 'NameFinderDD.aspx';
 //       $('#modal' + divid).attr('src', link + '?field=' + ctrlid + '&dialog=' + divid + '&field2=' + ctrl2id);
   
 //       return false;
 //   }

    function textboxMultilineMaxNumber(txt, maxLen) {
        if (txt.value.length > (maxLen - 1)) return false;
    }


    function onKeypress(btnid) {
        if ((event.which && event.which == 13) || (event.keyCode && event.keyCode == 13))
        {
            $('#' + btnid).click();
            return false;
        }
        else
            return true;
    }

