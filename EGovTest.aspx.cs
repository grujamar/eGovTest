using log4net;
using Newtonsoft.Json.Linq;
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
                List<TestCombination> TestCombination = new List<TestCombination>();
                log.Info("Start prepared list for Test combinations. ");
                PreparedTestCombinationList(utility, out TestCombination);
                log.Info("End prepared list for Test combinations. ");
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


    protected void PreparedTestCombinationList(ProjectUtility utility, out List<TestCombination> TestCombinationListFinal)
    {
        TestCombinationListFinal = new List<TestCombination>();
        try
        {
            List<TestCombination> TestCombinationList = new List<TestCombination>();

            TestCombinationList = utility.spCreateTestSession(Convert.ToInt32(Session["EGovTest_ddlSelectedValue"]));

            if (TestCombinationList.Count>0)
            {
                Session["EGovTest_TestCombinationList"] = TestCombinationList;
                ChangeVisibility(false);
            }

            TestCombinationListFinal = TestCombinationList;
        }
        catch (Exception ex)
        {
            log.Error("Error on PreparedTestCombinationList. " + ex.Message);
            ScriptManager.RegisterStartupScript(this, GetType(), "ErrorSendingData", "ErrorSendingData();", true);
        }
        
    }

    protected void RegisterUserAutoTest()
    {
        //string item_Username = "testPIS41";
        //string item_Realm = "PRIMARY";
        //string item_Password = "P@ssword123456!";
        //string item_Externalid = "1809985500255";
        //string item_Givenname = "Perica";
        //string item_Lastname = "Pisaric";
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
            List<TestCombination> TestCombinationList = new List<TestCombination>();
            TestCombinationList = (List<TestCombination>)Session["EGovTest_TestCombinationList"];
            log.Debug("TestCombinationList length: " + TestCombinationList.Count);
            foreach (var item in TestCombinationList)
            {
                List<TestCombinationParameter> TestCombinationParameterList = new List<TestCombinationParameter>();
                TestCombinationParameterList = item.ParameterList;

                string requestTemplate = @"{    ""user"": {
                       ""username"": ""%username"",
                        ""realm"": ""%realm"",
                        ""password"": ""%password"",
                        ""claims"": [
        	                {
                                ""uri"": ""http://wso2.org/claims/externalid"",
                                ""value"": ""%externalid""
                            },
                            {
                                ""uri"": ""http://wso2.org/claims/givenname"",
                                ""value"": ""%givenname""
                            },
                            {
                                ""uri"": ""http://wso2.org/claims/lastname"",
                                ""value"": ""%lastname""
                            },            
                            {
                                ""uri"": ""http://wso2.org/claims/emailaddress"",
                                ""value"": ""%emailaddress""
                            },
                            {
                                ""uri"": ""http://wso2.org/claims/dob"",
                                ""value"": ""%dob""
                            },
                            {
                                ""uri"": ""http://wso2.org/claims/placeofbirth"",
                                ""value"": ""%placeofbirth""
                            },  
                            {
                                ""uri"": ""http://wso2.org/claims/gender"",
                                ""value"": ""%gender""
                            }, 
                            {
                                ""uri"": ""http://wso2.org/claims/streetaddress"",
                                ""value"": ""%streetaddress""
                            },   
                            {
                                ""uri"": ""http://wso2.org/claims/city"",
                                ""value"": ""%city""
                            },   
                            {
                                ""uri"": ""http://wso2.org/claims/postalcode"",
                                ""value"": ""%postalcode""
                            },             
                            {
                                ""uri"": ""http://wso2.org/claims/country"",
                                ""value"": ""%country""
                            }
                        ]
                    },
                    ""properties"": [],
                    ""DuplicateHandling"": ""%DuplicateHandling"",
                    ""AuthenticationMethod"": ""%AuthenticationMethod""
                }";


                foreach (var itemParameter in TestCombinationParameterList)
                {

                    TestCombinationParameter TestCombinationParameter = (TestCombinationParameter)itemParameter;
                    string parameterName = TestCombinationParameter.ParameterName;
                    string parameterValue = TestCombinationParameter.ParameterValue;

                    string forChange = "" + "%" + parameterName + "";
                    requestTemplate.Replace(forChange, parameterValue);

                    log.Info("requestTemplate: " + requestTemplate);

                    //string jsonRegisterUser = ApiUtils.GetRegisterUserJson(item.Username, item.Realm, item.Password, item.Externalid, item.Givenname, item.Lastname, item.Emailaddress, item.DOB, item.Placeofbirth, item.Gender, item.Streetaddress, item.City, item.Postalcode, item.Country);
                    //string jsonRegisterUser = ApiUtils.GetRegisterUserJson(item_Username, item_Realm, item_Password, item_Externalid, item_Givenname, item_Lastname, item_Emailaddress, item_DOB, item_Placeofbirth, item_Gender, item_Streetaddress, item_City, item_Postalcode, item_Country);
                    string RegisterUser_Response = ApiUtils.RegisterUserWebRequestCall(requestTemplate);

                    log.Info("Start SearchUserIDByUsername ");
                    string resultFinalSearchUserIDByUsername = string.Empty;
                    //todo string.empty   item.Username
                    string SearchUserIDByUsername_Response = ApiUtils.SearchUserIDByUsernameWebRequestCall(string.Empty, string.Empty, out resultFinalSearchUserIDByUsername);
                    string UserId = ParseResponse(resultFinalSearchUserIDByUsername);
                    log.Info("End SearchUserIDByUsername. Response result is: " + UserId);

                    log.Info("Start calling SCIM web service for retrieving data. ");
                    string resultFinalSCIMcheckData = string.Empty;
                    string SCIMcheckData_Response = ApiUtils.SCIMcheckDataWebRequestCall(string.Empty, UserId, out resultFinalSCIMcheckData);

                    log.Info("End calling SCIM web service. Response result is: " + resultFinalSCIMcheckData);
                }  
            }
        }
        catch (Exception ex)
        {
            throw new Exception("Error in function RegisterUserAutoTest. " + ex.Message);
        }
    }




    protected string ParseResponse(string jsonResponse)
    {
        string res = string.Empty;

        // Parse your Result to an Array
        var x = JObject.Parse(jsonResponse);
        var res1 = x["userId"];
        res = res1.ToString();

        return res;
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