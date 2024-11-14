using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using LST.Data;

namespace LST.Business
{
    public class DynamicTemplate : System.Web.UI.ITemplate
    {
       
        private ListItemType templateType;
        private string columnName;
       // private int columnNo;
        DML_Util objDml = new  DML_Util();

        public DynamicTemplate(ListItemType type, string colName)
        {
            templateType = type;
            columnName = colName;
           
        }

        public void InstantiateIn(System.Web.UI.Control container)
        {
            //http://apartha77.blogspot.com/2011/07/creating-dynamically-gridview-with.html
            switch (templateType)
            {
                case ListItemType.Header:
                    LiteralControl lc = new LiteralControl();
                    lc.Text = columnName; 
                    container.Controls.Add(lc);
                    break;
                case ListItemType.Item:
                    Label lb = new Label();
                    lb.ID = "lbl" + columnName.Replace(" ", string.Empty);
                    lb.ClientIDMode = ClientIDMode.Static;
                    lb.DataBinding += new EventHandler(this.lb_DataBinding);
                    container.Controls.Add(lb);
                    break;
                case ListItemType.EditItem:

                    int field_id = objDml.GetFieldID(columnName);
                    DropDownList dd = new DropDownList();
                    string _newColName = Regex.Replace(columnName, @"[^0-9a-zA-Z]+", string.Empty);
                    dd.ID = "dd" + _newColName + field_id;
                    dd.ClientIDMode = ClientIDMode.Static;
                    dd.Items.Add(new ListItem("Y", "Y"));
                    dd.Items.Add(new ListItem("N", "N"));
                    dd.DataBinding += new EventHandler(this.dd_DataBinding);
                    container.Controls.Add(dd);
                    break;
                default:
                    break;


            }
        }

        private void lb_DataBinding(object sender, EventArgs e)
        {
            Label l = (Label)sender;
            GridViewRow row = (GridViewRow)l.NamingContainer;
            l.Text = DataBinder.Eval(row.DataItem, this.columnName.ToString()).ToString();
        }

        private void dd_DataBinding(object sender, EventArgs e)
        {
            DropDownList ddl = (DropDownList)sender;
            GridViewRow row = (GridViewRow)ddl.NamingContainer;
            ddl.SelectedValue = DataBinder.Eval(row.DataItem, this.columnName.ToString()).ToString();
        }
    }
}