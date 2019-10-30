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
                List<TestSessionRequestsParameters> TestSessionRequestsParameterList = new List<TestSessionRequestsParameters>();
                log.Info("Start prepared Requests for test. ");
                PreparedRequestsForTests(utility, out TestSessionRequestsParameterList);
                log.Info("End prepared Requests for test. ");
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
            int methodID = Convert.ToInt32(Session["EGovTest_ddlSelectedValue"]);
            switch (methodID)
            {
                case 2:
                    log.Info("Start Register user. ");
                    RegisterUserAutoTest();
                    log.Info("End Register user. ");
                    break;
                case 5:
                    Console.WriteLine(5);
                    break;
            }

            ScriptManager.RegisterStartupScript(this, GetType(), "SuccessSendingData", "SuccessSendingData();", true);
        }
        catch (Exception ex)
        {
            log.Error("Error on button click. " + ex.Message);
            ScriptManager.RegisterStartupScript(this, GetType(), "ErrorSendingData", "ErrorSendingData();", true);
        }
    }


    protected void PreparedRequestsForTests(ProjectUtility utility, out List<TestSessionRequestsParameters> TestSessionRequestsParameterFinal)
    {
        TestSessionRequestsParameterFinal = new List<TestSessionRequestsParameters>();
        try
        {
            List<TestSessionRequestsParameters> TestSessionRequestsParameterList = new List<TestSessionRequestsParameters>();

            TestSessionRequestsParameterList = utility.spCreateTestSessionRequests(Convert.ToInt32(Session["EGovTest_ddlSelectedValue"]));

            if (TestSessionRequestsParameterList.Count>0)
            {
                Session["EGovTest_TestSessionRequestsParameterList"] = TestSessionRequestsParameterList;
                ChangeVisibility(false);
            }

            TestSessionRequestsParameterFinal = TestSessionRequestsParameterList;
        }
        catch (Exception ex)
        {
            log.Error("Error on PreparedRequestsForTests. " + ex.Message);
            ScriptManager.RegisterStartupScript(this, GetType(), "ErrorSendingData", "ErrorSendingData();", true);
        }
        
    }

    protected void RegisterUserAutoTest()
    {
        try
        {
            int result = 0;
            ProjectUtility utility = new ProjectUtility();
            List<TestSessionRequestsParameters> TestSessionRequestsParameterList = new List<TestSessionRequestsParameters>();
            TestSessionRequestsParameterList = (List<TestSessionRequestsParameters>)Session["EGovTest_TestSessionRequestsParameterList"];
            log.Debug("TestCombinationList length: " + TestSessionRequestsParameterList.Count);

            //get TestSessionId from first object poarameter
            var firstElement = TestSessionRequestsParameterList.First();
            int TestSessionId = firstElement.TestSessionId;
            log.Info("TestSessionId is: " + TestSessionId);
            utility.testSessionStart(TestSessionId, out result);
            if (result != 0)
            {
                throw new Exception("Error while trying to start test session. Result is: " + result);
            }

            try
            {
                foreach (var item in TestSessionRequestsParameterList)
                {
                    log.Info("item.RequestData is " + item.RequestData);
                    utility.testCombinationStart(item.TestCombinationId, out result);
                    if (result != 0)
                    {
                        throw new Exception("Error while trying to start combination. Result is: " + result);
                    }

                    string username = string.Empty;
                    username = ParseRequestForUsername(item.RequestData);
                    WebAPICalls(item.RequestData, username, out string Response, out string ResponseStatus, out string ResponseExternal, out string ResponseStatusExternal, out bool FinalOutcome);

                    log.Info("testCombinationFinish with parameters: " + " RequestData - " + item.RequestData + " Response - " + Response + " ResponseStatus - " + ResponseStatus + " ResponseExternal - " + ResponseExternal + " ResponseStatusExternal - " + ResponseStatusExternal + " FinalOutcome - " + FinalOutcome);
                    utility.testCombinationFinish(item.TestCombinationId, Response, ResponseStatus, ResponseExternal, ResponseStatusExternal, FinalOutcome, out result);
                    if (result != 0)
                    {
                        throw new Exception("Error while trying to end combination. Result is: " + result);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error. " + ex.Message);
            }

            utility.testSessionFinish(TestSessionId, out result);
            if (result != 0)
            {
                throw new Exception("Error while trying to finish test session. Result is: " + result);
            }
        }
        catch (Exception ex)
        {
            throw new Exception("Error in function RegisterUserAutoTest. " + ex.Message);
        }
    }

    protected void WebAPICalls(string jsonData, string Username, out string Response, out string ResponseStatus, out string ResponseExternal, out string ResponseStatusExternal, out bool FinalOutcome)
    {
        Response = string.Empty;
        ResponseStatus = string.Empty;
        ResponseExternal = string.Empty;
        ResponseStatusExternal = string.Empty;
        FinalOutcome = false;
        bool FinalOutcomeFirstStep = false;
        bool FinalOutcomeSecondStep = false;

        try
        {
            //Register user API start
            log.Info("Register user API start. ");
            string RegisterUser_Response = ApiUtils.RegisterUserWebRequestCall(jsonData, out string resultResponse, out string statusCode, out string statusDescription, out string resulNotOK);
            ResponseStatus = statusCode + " " + statusDescription;
            if (Convert.ToInt32(statusCode) == ConstantsProject.REGISTER_USER_ОК)
            {
                FinalOutcomeFirstStep = true;
                Response = resultResponse;
            }
            else {
                Response = resulNotOK;
            }
            log.Info("Register user API end. Response result is: " + Response + " " + ResponseStatus);

            log.Info("Start SearchUserIDByUsername ");
            string resultFinalSearchUserIDByUsername = string.Empty;
            //todo string.empty   item.Username
            string SearchUserIDByUsername_Response = ApiUtils.SearchUserIDByUsernameWebRequestCall(string.Empty, Username, out resultFinalSearchUserIDByUsername, out string statusCodeSearch, out string statusDescriptionSearch, out string resulNotOKsearch);
            string UserId = ParseResponse(resultFinalSearchUserIDByUsername);
            log.Info("End SearchUserIDByUsername. Response result is: " + UserId);

            if (UserId != string.Empty)
            {
                log.Info("Start calling SCIM web service. ");
                string resultFinalSCIMcheckData = string.Empty;
                string SCIMcheckData_Response = ApiUtils.SCIMcheckDataWebRequestCall(string.Empty, UserId, out resultFinalSCIMcheckData, out string statusCodeSCIM, out string statusDescriptionSCIM, out string resultNotOKscim);
                ResponseExternal = resultFinalSCIMcheckData;
                ResponseStatusExternal = statusCodeSCIM + " " + statusDescriptionSCIM;
                if (Convert.ToInt32(statusCode) == ConstantsProject.REGISTER_USER_SCIM_ОК)
                {
                    FinalOutcomeSecondStep = true;
                }
                log.Info("End calling SCIM web service. ");


                FinalOutcome = CheckFinalOutcome(FinalOutcomeFirstStep, FinalOutcomeSecondStep);
                log.Info("FinalOutcome is - " + FinalOutcome); 
            }
        }
        catch (Exception ex)
        {
            log.Error("Error in function WebAPICalls. " + ex.Message);
            //throw new Exception("Error in function WebAPICalls. " + ex.Message);
        }
    }


    protected bool CheckFinalOutcome(bool firstStep, bool secondStep)
    {
        bool retValue = false;
        if (firstStep && secondStep)
        {
            retValue = true;
        }
        else {
            retValue = false;
        }
        return retValue;
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

    protected string ParseRequestForUsername(string jsonResponse)
    {
        string res = string.Empty;

        // Parse your Result to an Array
        var x = JObject.Parse(jsonResponse);
        var res1 = x["user"]["username"];
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


    //List<TestCombination> TestCombinationList = new List<TestCombination>();
    //TestCombinationList = (List<TestCombination>)Session["EGovTest_TestSessionRequestsParameterList"];
    //log.Debug("TestCombinationList length: " + TestCombinationList.Count);
    //foreach (var item in TestCombinationList)
    //{
    //    List<TestCombinationParameter> TestCombinationParameterList = new List<TestCombinationParameter>();
    //    TestCombinationParameterList = item.ParameterList;

    //    string requestTemplate = @"{    ""user"": {
    //           ""username"": ""%username"",
    //            ""realm"": ""%realm"",
    //            ""password"": ""%password"",
    //            ""claims"": [
    //             {
    //                    ""uri"": ""http://wso2.org/claims/externalid"",
    //                    ""value"": ""%externalid""
    //                },
    //                {
    //                    ""uri"": ""http://wso2.org/claims/givenname"",
    //                    ""value"": ""%givenname""
    //                },
    //                {
    //                    ""uri"": ""http://wso2.org/claims/lastname"",
    //                    ""value"": ""%lastname""
    //                },            
    //                {
    //                    ""uri"": ""http://wso2.org/claims/emailaddress"",
    //                    ""value"": ""%emailaddress""
    //                },
    //                {
    //                    ""uri"": ""http://wso2.org/claims/dob"",
    //                    ""value"": ""%dob""
    //                },
    //                {
    //                    ""uri"": ""http://wso2.org/claims/placeofbirth"",
    //                    ""value"": ""%placeofbirth""
    //                },  
    //                {
    //                    ""uri"": ""http://wso2.org/claims/gender"",
    //                    ""value"": ""%gender""
    //                }, 
    //                {
    //                    ""uri"": ""http://wso2.org/claims/streetaddress"",
    //                    ""value"": ""%streetaddress""
    //                },   
    //                {
    //                    ""uri"": ""http://wso2.org/claims/city"",
    //                    ""value"": ""%city""
    //                },   
    //                {
    //                    ""uri"": ""http://wso2.org/claims/postalcode"",
    //                    ""value"": ""%postalcode""
    //                },             
    //                {
    //                    ""uri"": ""http://wso2.org/claims/country"",
    //                    ""value"": ""%country""
    //                }
    //            ]
    //        },
    //        ""properties"": [],
    //        ""DuplicateHandling"": ""%DuplicateHandling"",
    //        ""AuthenticationMethod"": ""%AuthenticationMethod""
    //    }";

    //    string requestTemplatePart = string.Empty;
    //    string requestTemplateFinal = requestTemplate;

    //    foreach (var itemParameter in TestCombinationParameterList)
    //    {
    //        TestCombinationParameter TestCombinationParameter = (TestCombinationParameter)itemParameter;
    //        string parameterName = TestCombinationParameter.ParameterName;
    //        string parameterValue = TestCombinationParameter.ParameterValue;

    //        string forChange = "%" + parameterName;
    //        requestTemplatePart = requestTemplateFinal.Replace(forChange, parameterValue);
    //        requestTemplateFinal = requestTemplatePart;
    //    }

    //    log.Info("requestTemplate: " + requestTemplateFinal);

    //string jsonRegisterUser = ApiUtils.GetRegisterUserJson(item.Username, item.Realm, item.Password, item.Externalid, item.Givenname, item.Lastname, item.Emailaddress, item.DOB, item.Placeofbirth, item.Gender, item.Streetaddress, item.City, item.Postalcode, item.Country);
    //string jsonRegisterUser = ApiUtils.GetRegisterUserJson(item_Username, item_Realm, item_Password, item_Externalid, item_Givenname, item_Lastname, item_Emailaddress, item_DOB, item_Placeofbirth, item_Gender, item_Streetaddress, item_City, item_Postalcode, item_Country);

    //}
}