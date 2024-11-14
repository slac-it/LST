using System;
using System.Web;
using System.Web.UI;


namespace LST
{
    public partial class ConfirmRequest : Page
    {
        
     protected void Page_Load()
        {
          
              if (!(Page.IsPostBack))
              {
                  string _page = Request.QueryString["page"];
                  if (_page.Equals("appr"))
                  {
                      LblMsg.Text = "Your action has been submitted successfully. The requester will be notified about your action";
                  }
              }
        }
        
    }
}