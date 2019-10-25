using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class EGovTest : System.Web.UI.Page
{
    //Lofg4Net declare log variable
    private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
    public RegisterUser RegisterUser;
    public string SettingsFile { get; private set; }
    public string Url { get; set; }
    public string ContentType { get; set; }
    public string Method { get; set; }

    protected void Page_Load(object sender, EventArgs e)
    {
        ProjectUtility utility = new ProjectUtility();
        bool ConnectionActive = utility.IsAvailableConnection();
        if (!ConnectionActive)
        {
            Response.Redirect("GreskaBaza.aspx", false);
        }
        AvoidCashing();

        if (!Page.IsPostBack)
        {
            string pageName = Path.GetFileName(Page.AppRelativeVirtualPath);
            log.Info(pageName + " page start.");
            ChangeVisibility(true);
            Session["EGovTest_ddlSelectedValue"] = 0;
        }     
    }

    private void AvoidCashing()
    {
        Response.Cache.SetNoStore();
        Response.Cache.SetCacheability(HttpCacheability.NoCache);
    }

    protected void ChangeVisibility(bool visible)
    {
        afterCreateTest.Visible = !visible;
        //afterCreateTest.Visible = visible;
        beforeCreateTest.Visible = visible;
    }

    protected void btnCreateTest_Click(object sender, EventArgs e)
    {
        try
        {
            Page.Validate("AddCustomValidatorToGroup");

            if (Page.IsValid)
            {
                //todo call procedure for Create Test
                ProjectUtility utility = new ProjectUtility();
                List<RegisterUserTest> RegisterUsers = new List<RegisterUserTest>();
                log.Info("Start prepared list for Register user. ");
                PreparedRegisterUsersList(utility, out RegisterUsers);
                log.Info("End prepared list for Register user. ");
            }
        }
        catch (Exception ex)
        {
            log.Error("Error on button click. " + ex.Message);
            ScriptManager.RegisterStartupScript(this, GetType(), "ErrorSendingData", "ErrorSendingData();", true);
        }
    }

    protected void btnStartTest_Click(object sender, EventArgs e)
    {
        try
        {
            log.Info("Start Register user. ");
            RegisterUserAutoTest();
            log.Info("End Register user. ");


            ScriptManager.RegisterStartupScript(this, GetType(), "SuccessSendingData", "SuccessSendingData();", true);
        }
        catch (Exception ex)
        {
            log.Error("Error on button click. " + ex.Message);
            ScriptManager.RegisterStartupScript(this, GetType(), "ErrorSendingData", "ErrorSendingData();", true);
        }
    }


    protected void PreparedRegisterUsersList(ProjectUtility utility, out List<RegisterUserTest> RegisterUsersFinal)
    {
        RegisterUsersFinal = new List<RegisterUserTest>();
        try
        {
            List<RegisterUserTest> RegisterUsers = new List<RegisterUserTest>();

            RegisterUsers = utility.SelectAllCombinationsRegisterUser(Convert.ToInt32(Session["EGovTest_ddlSelectedValue"]));

            if (RegisterUsers.Count>0)
            {
                Session["EGovTest_RegisterUsers"] = RegisterUsers;
                ChangeVisibility(false);
            }

            RegisterUsersFinal = RegisterUsers;
        }
        catch (Exception ex)
        {
            log.Error("Error on PreparedRegisterUsersList. " + ex.Message);
            ScriptManager.RegisterStartupScript(this, GetType(), "ErrorSendingData", "ErrorSendingData();", true);
        }
        
    }

    protected void RegisterUserAutoTest()
    {
        //string item_Username = "testPIS16";
        //string item_Realm = "PRIMARY";
        //string item_Password = "P@ssword123456!";
        //string item_Externalid = "646464";
        //string item_Givenname = "Pera1";
        //string item_Lastname = "Pisar1";
        //string item_Emailaddress = "test16@pis.rs";
        //string item_DOB = "20020202";
        //string item_Placeofbirth = "serbia";
        //string item_Gender = "male";
        //string item_Streetaddress = "StreetaddressTest";
        //string item_City = "belgrade";
        //string item_Postalcode = "11000";
        //string item_Country = "serbia";

        try
        {
            List<RegisterUserTest> RegisterUsers = new List<RegisterUserTest>();
            RegisterUsers = (List<RegisterUserTest>)Session["EGovTest_RegisterUsers"];
            log.Debug("Register user list length: " + RegisterUsers.Count);
            foreach (var item in RegisterUsers)
            {
                string json = ApiUtils.GetRegisterUserJson(item.Username, item.Realm, item.Password, item.Externalid, item.Givenname, item.Lastname, item.Emailaddress, item.DOB, item.Placeofbirth, item.Gender, item.Streetaddress, item.City, item.Postalcode, item.Country);
                string WebResponse = ApiUtils.RegisterUserWebRequestCall(json);
            }
        }
        catch (Exception ex)
        {
            throw new Exception("Error in function RegisterUserAutoTest. " + ex.Message);
        }
    }

    protected void Cvmethod_ServerValidate(object source, ServerValidateEventArgs args)
    {
        try
        {
            string ErrorMessage = string.Empty;
            string IDItem = "0";

            args.IsValid = ApiUtils.ValidateDropDown(ddlmethod.SelectedValue, IDItem, "Method", out ErrorMessage);
            cvmethod.ErrorMessage = ErrorMessage;
        }
        catch (Exception)
        {
            cvmethod.ErrorMessage = string.Empty;
            args.IsValid = false;
        }
    }


    protected void ddlmethod_SelectedIndexChanged(object sender, EventArgs e)
    {
        int SelectedValue = Convert.ToInt32(ddlmethod.SelectedValue);
        Session["EGovTest_ddlSelectedValue"] = SelectedValue;
    }

}