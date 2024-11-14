<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TrainingSummary.ascx.cs" Inherits="LST.UserControls.TrainingSummary" %>

<script type="text/javascript">
    $(function () {
        $("tr").each(function () {
            var col_val = $(this).find("td:eq(2)").text();
            var crs_val = $(this).find("td:eq(0)").text();
           
            var checkCourse, checkOtherCourse;
            //alert(txtval);
            var txt219 = $('input[id$=Txt219]').val();
            var txt219R = $('input[id$=Txt219R]').val();
            var txt396 = $('input[id$=Txt396]').val();
            if (($.trim(crs_val.toLowerCase()) === "253") || ($.trim(crs_val.toLowerCase()) === "253me") || ($.trim(crs_val.toLowerCase()) === "253pra") || ($.trim(crs_val.toLowerCase()) === "120"))
            {
                checkOtherCourse = true;
            }
            else
            {
                checkOtherCourse = false;
            }

                     

            if ((($.trim(col_val.toLowerCase()) === "overdue!") || (($.trim(col_val.toLowerCase()) === "never taken")))  && (checkOtherCourse)) {
              
                $(this).addClass('selected');
        
            }
            else
            {
                if ((($.trim(col_val.toLowerCase()) === "overdue!") || ($.trim(col_val.toLowerCase()) === "never taken") || ($.trim(col_val.toLowerCase()).substring(1, 3) === "com"))) {
                    if ($.trim(crs_val.toLowerCase()) === "219") {
                        
                        if (txt219 == "nev" && txt219R == "nev" && txt396 == "nev") {
                            $(this).addClass('selected');
                        }
                        else {
                            $(this).removeClass('selected');
                        }
                    }

                    if ($.trim(crs_val.toLowerCase()) === "219r") {
                        if (txt219 == "nev" && txt219R == "nev" && txt396 == "nev") {
                            $(this).removeClass('selected');
                        }
                        else if (txt396 == "val")
                        {
                            $(this).removeClass('selected');
                        }
                        else if (txt219 == "val")
                        {
                            $(this).removeClass('selected');
                        }
                        else {
                            $(this).addClass('selected');
                        }
                    }
                }
            }
               
                    
               

               
            //    if (($.trim(crs_val.toLowerCase()) === "219") && ($("#Txt396").val("nev") && $("#Txt219R").val("nev") && $("#Txt219").val("nev")))
            //            {
            //                $(this).addClass('selected');
            //    }
            //    else if (($.trim(crs_val.toLowerCase()) === "219r") && ($("#Txt396").val("nev") && $("#Txt219R").val("nev") && $("#Txt219").val("nev")))
            //    {
            //        $(this).removeClass('selected');
            //    }
            //    else if (($.trim(crs_val.toLowerCase()) === "219r") && ($("#Txt396").val("nev") && $("#Txt219R").val("nev") && $("#Txt219").val("ove"))) {
            //        $(this).addClass('selected');
            //    }
            //    //else if (($.trim(crs_val.toLowerCase()) === "219r") && $("#Txt219").val("nev") && $("#Txt396").val("nev") && $("#Txt219R").val("nev"))
            //    //        {
            //    //    $(this).removeClass('selected');
            //    //    alert("in 219r");
            //    //        }
               
            //            else
            //            {
            //        $(this).addClass('selected');
            //        alert("in else");
            //            }
            //}
           
            //else {
            //    $(this).removeClass('selected');
            //}
        });
    });
</script>
<style type="text/css">
    .selected{
  background-color:yellow;
}

</style>

<asp:Panel ID="PnlTraining" runat="server" GroupingText="Training Summary for Laser Worker" >
        <table class="table table-bordered table-condensed">
            <tr class="trhead">
                 <th>Course Name</th>
                 <th>Assigned in STA</th>
                 <th>Status</th>
            </tr>
            <tr id="tr253" runat="server">
                <td>253</td>
                <td><%=IsInSTA("253") %></td>
                <td class="status"><%=GetStatus("253") %></td>
            </tr>
            <tr class="tralternate" id="tr253me" runat="server">
                <td>253ME</td>
                <td><%=IsInSTA("253ME") %></td>
                <td class="status"><%=GetStatus("253ME") %></td>
            </tr>
            <tr id="tr131" runat="server">
                 <td>131</td>
                 <td><%=IsInSTA("131") %></td>
                 <td class="status"><%=GetStatus("131") %></td>
            </tr>
            <tr class="tralternate" id="tr120" runat="server">
                  <td>120</td>
                  <td><%=IsInSTA("120") %></td>
                  <td class="status"><%=GetStatus("120") %></td>
            </tr>
            <tr id="tr219" runat="server">
                <td>219</td>
                <td><%=IsInSTA("219") %></td>
                <td class="status"><%=GetStatus("219") %></td>
            </tr>
            <tr class="tralternate" id="tr219R" runat="server" >
                <td>219R</td>
                <td><%=IsInSTA("219R") %></td>
                <td class="status"><%=GetStatus("219R") %></td>
            </tr>
             <tr id="tr396" runat="server">
                <td>396</td>
                <td><%=IsInSTA("396") %></td>
                <td class="status"><%=GetStatus("396") %></td>
            </tr>
            <tr id="trQLO"  class="tralternate" runat="server" visible="false">
                <td>253PRA</td>
                <td><%=IsInSTA("253PRA") %></td>
                <td class="status"><%=GetStatus("253PRA") %></td>
            </tr>
           
        </table>
    <asp:HiddenField id="Txt219" runat="server"  ClientIDMode="Static"></asp:HiddenField>
    <asp:HiddenField ID="Txt396" runat="server"  ClientIDMode="Static"/>
     <asp:HiddenField ID="Txt219R" runat="server"  ClientIDMode="Static"/>
    </asp:Panel>