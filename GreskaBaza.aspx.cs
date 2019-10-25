using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class GreskaBaza : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }

    protected void btnBack_Click(object sender, EventArgs e)
    {
        Response.Redirect("EGovTest.aspx", false); // this will tell .NET framework not to stop the execution of the current thread and hence the error will be resolved.
    }
}