﻿using log4net;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
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
            isEnabledButtons(false);
            uploadDocument.Visible = false;
        }
    }

    private void AvoidCashing()
    {
        Response.Cache.SetNoStore();
        Response.Cache.SetCacheability(HttpCacheability.NoCache);
    }

    protected void isEnabledButtons(bool value)
    {
        btnDeleteUsersOnSCIM.Enabled = value;
        btnBulkOnSCIM.Enabled = value;
        btnTestSecure.Enabled = value;
        btnDownloadFile.Enabled = value;
    }

    protected void ChangeVisibility(bool visible)
    {
        afterCreateTest.Visible = !visible;
        beforeCreateTest.Visible = visible;
    }

    protected void btnCreateTest_Click(object sender, EventArgs e)
    {
        try
        {
            Page.Validate("AddCustomValidatorToGroup");

            if (Page.IsValid)
            {
                ProjectUtility utility = new ProjectUtility();
                List<TestSessionRequestsParameters> TestSessionRequestsParameterList = new List<TestSessionRequestsParameters>();
                log.Info("Start prepared Requests for test. ");
                PreparedRequestsForTest(utility, out TestSessionRequestsParameterList);
                log.Info("End prepared Requests for test. ");
            }
        }
        catch (Exception ex)
        {
            log.Error("Error on button click. " + ex.Message);
            ScriptManager.RegisterStartupScript(this, GetType(), "ErrorSendingData", "ErrorSendingData();", true);
        }
    }

    protected void PreparedRequestsForTest(ProjectUtility utility, out List<TestSessionRequestsParameters> TestSessionRequestsParameterFinal)
    {
        TestSessionRequestsParameterFinal = new List<TestSessionRequestsParameters>();
        try
        {
            List<TestSessionRequestsParameters> TestSessionRequestsParameterList = new List<TestSessionRequestsParameters>();

            //---ResendMail---
            //int methodID = 25;
            int methodID = Convert.ToInt32(Session["EGovTest_ddlSelectedValue"]);
            
            if (methodID == ConstantsProject.UPLOAD_DOCUMENTS_METHOD_ID)
            {
                uploadDocument.Visible = false; //was true
            }

            TestSessionRequestsParameterList = utility.spCreateTestSessionRequests(methodID);

            if (TestSessionRequestsParameterList.Count > 0)
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

    protected void btnStartTest_Click(object sender, EventArgs e)
    {
        try
        {
            ProjectUtility utility = new ProjectUtility();
            //---ResendMail---
            //int methodID = 25;
            //log.Info("START TESTING METHOD: " + methodID);

            int methodID = Convert.ToInt32(Session["EGovTest_ddlSelectedValue"]);
            string MethodName = utility.getMethodName(methodID);
            log.Info("START TESTING METHOD: " + MethodName);
            WebApiCallsByMethod(methodID);
            log.Info("END TESTING METHOD: " + MethodName);
            //log.Info("END TESTING METHOD: " + methodID);

            ScriptManager.RegisterStartupScript(this, GetType(), "SuccessSendingData", "SuccessSendingData();", true);
        }
        catch (Exception ex)
        {
            log.Error("Error on button click. " + ex.Message);
            ScriptManager.RegisterStartupScript(this, GetType(), "ErrorSendingData", "ErrorSendingData();", true);
        }
    }

    protected void WebApiCallsByMethod(int MethodId)
    {
        int result = 0;
        ProjectUtility utility = new ProjectUtility();
        List<TestSessionRequestsParameters> TestSessionRequestsParameterList = new List<TestSessionRequestsParameters>();
        TestSessionRequestsParameterList = (List<TestSessionRequestsParameters>)Session["EGovTest_TestSessionRequestsParameterList"];
        log.Info("TestCombinationList length: " + TestSessionRequestsParameterList.Count);

        //get TestSessionId from first object parameter
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
                /////////BEFORE STEP-ONLY FOR REGISTER USER////////////
                if (!item.BeforeStep.Equals("-"))
                {
                    log.Info("BeforeSTEP. Request is " + item.BeforeStep);
                    string jsonDataSCIM_Update = item.BeforeStep;
                    string jsonDataSCIM_Update_Replace = jsonDataSCIM_Update.Replace(@"""""", @"""");
                    log.Info("After replacing is " + jsonDataSCIM_Update_Replace);
                    string Username = ApiUtils.ParseJsonOneValue(jsonDataSCIM_Update_Replace, "userName");

                    log.Info("Start SearchUserIDByUsername in BeforeSTEP ");
                    string resultFinalSearchUserIDByUsername = string.Empty;
                    //todo string.empty   item.Username
                    string SearchUserIDByUsername_Response = ApiUtils.SearchUserIDByUsername_WebRequestCall(string.Empty, Username, out resultFinalSearchUserIDByUsername, out string statusCodeSearch, out string statusDescriptionSearch, out string resulNotOKsearch);
                    string UserId = ApiUtils.ParseJsonOneValue(resultFinalSearchUserIDByUsername, "userId");
                    log.Info("End SearchUserIDByUsername in BeforeSTEP. Response result is: " + UserId);

                    //SCIM UPDATE
                    log.Info("Start SCIM update user in BeforeSTEP. ");
                    string SCIM_UpdateUser_Response = ApiUtils.SCIMcheckData_WebRequestCall(jsonDataSCIM_Update_Replace, UserId, ConstantsProject.PUT_METHOD, out string resultPUTFinal, out string statusCode, out string statusDescription, out string resultNotOK);
                    string UpdateResponseExternal = resultPUTFinal;
                    string UpdateResponseStatusExternal = statusCode + " " + statusDescription;
                    log.Info("End SCIM update user in BeforeSTEP. UserId is " + UserId);
                }
                ////////////////////////////////

                string username = string.Empty;
                string umcn = string.Empty;
                string password = string.Empty;
                if (MethodId == ConstantsProject.REGISTER_USER_ID)
                {
                    username = ApiUtils.ParseJsonTwoValues(item.RequestData, "user", "username");
                }
                if (MethodId == ConstantsProject.EXPORT_USER_INFO_BY_USERNAME || MethodId == ConstantsProject.SEARCH_USER_ID_BY_USERNAME || MethodId == ConstantsProject.EXPORT_AUTH_INFO_BY_USERNAME || MethodId == ConstantsProject.LIST_DOCUMENTS_METHOD_ID)
                {
                    username = ApiUtils.ParseJsonOneValue(item.RequestData, "username");
                }
                if (MethodId == ConstantsProject.SEARCH_USER_ID_BY_UMCN)
                {
                    umcn = ApiUtils.ParseJsonOneValue(item.RequestData, "umcn");
                }
                WebAPICalls(item.RequestData, umcn, username, MethodId, out string Response, out string ResponseStatus, out string ResponseExternal, out string ResponseStatusExternal, out bool FinalOutcome);
                    
                //log.Info("testCombinationFinish with parameters: " + " RequestData - " + item.RequestData + " Response - " + Response + " ResponseStatus - " + ResponseStatus + " ResponseExternal - " + ResponseExternal + " ResponseStatusExternal - " + ResponseStatusExternal + " FinalOutcome - " + FinalOutcome);
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

    public static List<Task> TaskList = new List<Task>();
    protected void TestMultitreadingTasks(List<UploadUsernamePath> UsernamePathList, List<DownloadUsernameDocID> usernameDocumentIDList, int MethodId, out string Response, out string ResponseStatus, out string ResponseExternal, out string ResponseStatusExternal, out bool FinalOutcome)
    {
        string ResponseEnd = string.Empty;
        string ResponseStatusEnd = string.Empty;
        string ResponseExternalEnd = string.Empty;
        string ResponseStatusExternalEnd = string.Empty;
        bool FinalOutcomeEnd = false;
        ProjectUtility utility = new ProjectUtility();

        if (MethodId == ConstantsProject.UPLOAD_DOCUMENTS_METHOD_ID)
        {
            log.Info("Start UploadDocuments_WebAPICalls. ");
            System.Threading.Thread.Sleep(1000);
            foreach (UploadUsernamePath item in UsernamePathList)
            {
                var UploadDocumentTask = Task.Run(() => DocumentMethods_WebAPICalls(string.Empty, item.Username, item.Path, string.Empty, ConstantsProject.UPLOAD_DOCUMENTS_METHOD_ID, out ResponseEnd, out ResponseStatusEnd, out ResponseExternalEnd, out ResponseStatusExternalEnd, out FinalOutcomeEnd)); 
                TaskList.Add(UploadDocumentTask);
            }
            Task.WaitAll(TaskList.ToArray());
            log.Info("End UploadDocuments_WebAPICalls. ");
        }
        else if (MethodId == ConstantsProject.DOWNLOAD_DOCUMENTS_METHOD_ID)
        {
            log.Info("Start DownloadDocuments_WebAPICalls. ");
            System.Threading.Thread.Sleep(1000);
            foreach (DownloadUsernameDocID item in usernameDocumentIDList)
            {
                var DownloadDocumentTask = Task.Run(() => DocumentMethods_WebAPICalls(string.Empty, item.Username, string.Empty, item.DocumentId, ConstantsProject.DOWNLOAD_DOCUMENTS_METHOD_ID, out ResponseEnd, out ResponseStatusEnd, out ResponseExternalEnd, out ResponseStatusExternalEnd, out FinalOutcomeEnd));
                TaskList.Add(DownloadDocumentTask);
            }
            Task.WaitAll(TaskList.ToArray());
            log.Info("End DownloadDocuments_WebAPICalls. ");
        }

        Response = ResponseEnd;
        ResponseStatus = ResponseStatusEnd;
        ResponseExternal = ResponseExternalEnd;
        ResponseStatusExternal = ResponseStatusExternalEnd;
        FinalOutcome = FinalOutcomeEnd;
    }

    protected void WebAPICalls(string jsonData, string UMCN, string Username, int MethodId, out string Response, out string ResponseStatus, out string ResponseExternal, out string ResponseStatusExternal, out bool FinalOutcome)
    {
        string ResponseEnd = string.Empty;
        string ResponseStatusEnd = string.Empty;
        string ResponseExternalEnd = string.Empty;
        string ResponseStatusExternalEnd = string.Empty;
        bool FinalOutcomeEnd = false;
        ProjectUtility utility = new ProjectUtility();
        List<UploadUsernamePath> usernamePathList = new List<UploadUsernamePath>();
        List<DownloadUsernameDocID> usernameDocumentIDList = new List<DownloadUsernameDocID>();
        ///For only SEARCH_USER_ID_BY_USERNAME
        bool FinalOutcomeSecondStep = false;

        try
        {
            switch (MethodId)
            {
                case ConstantsProject.REGISTER_USER_ID:
                    log.Info("Start RegisterUser_WebAPICalls. ");
                    RegisterUser_WebAPICalls(jsonData, Username, out ResponseEnd, out ResponseStatusEnd, out ResponseExternalEnd, out ResponseStatusExternalEnd, out FinalOutcomeEnd);
                    log.Info("End RegisterUser_WebAPICalls. ");
                    break;
                case ConstantsProject.VALIDATE_CODE_METHOD_ID:
                    log.Info("Start ValidateCode_WebAPICalls. ");
                    Validate_WebAPICalls(jsonData, ConstantsProject.VALIDATE_CODE_METHOD_ID, out ResponseEnd, out ResponseStatusEnd, out ResponseExternalEnd, out ResponseStatusExternalEnd, out FinalOutcomeEnd);
                    log.Info("End ValidateCode_WebAPICalls. ");
                    break;
                case ConstantsProject.VALIDATE_USERNAME_METHOD_ID:
                    log.Info("Start ValidateUsername_WebAPICalls. ");
                    Validate_WebAPICalls(jsonData, ConstantsProject.VALIDATE_USERNAME_METHOD_ID, out ResponseEnd, out ResponseStatusEnd, out ResponseExternalEnd, out ResponseStatusExternalEnd, out FinalOutcomeEnd);
                    log.Info("End ValidateUsername_WebAPICalls. ");
                    break;
                case ConstantsProject.VALIDATE_UMCN_METHOD_ID:
                    log.Info("Start ValidateUMCN_WebAPICalls. ");
                    Validate_WebAPICalls(jsonData, ConstantsProject.VALIDATE_UMCN_METHOD_ID, out ResponseEnd, out ResponseStatusEnd, out ResponseExternalEnd, out ResponseStatusExternalEnd, out FinalOutcomeEnd);
                    log.Info("End ValidateUMCN_WebAPICalls. ");
                    break;
                case ConstantsProject.EXPORT_USER_INFO_BY_USERNAME:
                    log.Info("Start ExportUserInfo_WebAPICalls. ");
                    ExportInfo_WebAPICalls(jsonData, Username, ConstantsProject.EXPORT_USER_INFO_BY_USERNAME, out ResponseEnd, out ResponseStatusEnd, out ResponseExternalEnd, out ResponseStatusExternalEnd, out FinalOutcomeEnd);
                    log.Info("End ExportUserInfo_WebAPICalls. ");
                    break;
                case ConstantsProject.SEARCH_USER_ID_BY_USERNAME:
                    log.Info("Start SearchUserIdByUsername_WebAPICalls. ");
                    SearchUserIDBy_WebAPICalls(string.Empty, Username, string.Empty, true, FinalOutcomeSecondStep, out FinalOutcomeEnd, out ResponseEnd, out ResponseStatusEnd, out ResponseExternalEnd, out ResponseStatusExternalEnd);
                    log.Info("End SearchUserIdByUsername_WebAPICalls. ");
                    break;
                case ConstantsProject.SEARCH_USER_ID_BY_UMCN:
                    log.Info("Start SearchUserIdByUMCN_WebAPICalls. ");
                    SearchUserIDBy_WebAPICalls(string.Empty, string.Empty, UMCN, true, FinalOutcomeSecondStep, out FinalOutcomeEnd, out ResponseEnd, out ResponseStatusEnd, out ResponseExternalEnd, out ResponseStatusExternalEnd);
                    log.Info("End SearchUserIdByUMCN_WebAPICalls. ");
                    break;
                case ConstantsProject.EXPORT_AUTH_INFO_BY_USERNAME:
                    log.Info("Start ExportAuthInfo_WebAPICalls. ");
                    ExportInfo_WebAPICalls(jsonData, Username, ConstantsProject.EXPORT_AUTH_INFO_BY_USERNAME, out ResponseEnd, out ResponseStatusEnd, out ResponseExternalEnd, out ResponseStatusExternalEnd, out FinalOutcomeEnd);
                    log.Info("End ExportAuthInfo_WebAPICalls. ");
                    break;
                case ConstantsProject.ADD_AUTHENTICATION_METHOD_ID:
                    log.Info("Start AddAuthentication_WebAPICalls. ");
                    Validate_WebAPICalls(jsonData, ConstantsProject.ADD_AUTHENTICATION_METHOD_ID, out ResponseEnd, out ResponseStatusEnd, out ResponseExternalEnd, out ResponseStatusExternalEnd, out FinalOutcomeEnd);
                    log.Info("End AddAuthentication_WebAPICalls. ");
                    break;
                case ConstantsProject.REMOVE_AUTHENTICATION_METHOD_ID:
                    log.Info("Start RemoveAuthentication_WebAPICalls. ");
                    Validate_WebAPICalls(jsonData, ConstantsProject.REMOVE_AUTHENTICATION_METHOD_ID, out ResponseEnd, out ResponseStatusEnd, out ResponseExternalEnd, out ResponseStatusExternalEnd, out FinalOutcomeEnd);
                    log.Info("End RemoveAuthentication_WebAPICalls. ");
                    break;
                case ConstantsProject.UPLOAD_DOCUMENTS_METHOD_ID:
                    log.Info("Start UploadDocuments_WebAPICalls. ");
                    //todo FOR TEST MULTITREADING - UPLOAD DOCUMENTS
                    List<string> UsernameList = new List<string>();
                    UsernameList = utility.getUsernamesUploadDocument();
                    log.Info("UsernameList count is " + UsernameList.Count);
                    foreach (var username in UsernameList)
                    {
                        CopyFileToUploadFolderWithUserIdName(username, out string destinationPath);
                        usernamePathList.Add(new UploadUsernamePath(username, destinationPath));
                    }
                    TestMultitreadingTasks(usernamePathList, usernameDocumentIDList, ConstantsProject.UPLOAD_DOCUMENTS_METHOD_ID, out ResponseEnd, out ResponseStatusEnd, out ResponseExternalEnd, out ResponseStatusExternalEnd, out FinalOutcomeEnd);
                    log.Info("End UploadDocuments_WebAPICalls. ");
                    break;
                case ConstantsProject.LIST_DOCUMENTS_METHOD_ID:
                    log.Info("Start ListDocuments_WebAPICalls. ");
                    DocumentMethods_WebAPICalls(string.Empty, Username, string.Empty, string.Empty, ConstantsProject.LIST_DOCUMENTS_METHOD_ID, out ResponseEnd, out ResponseStatusEnd, out ResponseExternalEnd, out ResponseStatusExternalEnd, out FinalOutcomeEnd);
                    log.Info("End ListDocuments_WebAPICalls. ");
                    break;
                case ConstantsProject.DOWNLOAD_DOCUMENTS_METHOD_ID:
                    log.Info("Start DownloadDocuments_WebAPICalls. ");
                    //todo FOR TEST MULTITREADING - DOWNLOAD DOCUMENTS
                    usernameDocumentIDList = utility.getUsernameAndDocumentIdDownloadDocument();
                    log.Info("usernameDocumentIDList count is " + usernameDocumentIDList.Count);
                    TestMultitreadingTasks(usernamePathList, usernameDocumentIDList, ConstantsProject.DOWNLOAD_DOCUMENTS_METHOD_ID, out ResponseEnd, out ResponseStatusEnd, out ResponseExternalEnd, out ResponseStatusExternalEnd, out FinalOutcomeEnd);
                    log.Info("End DownloadDocuments_WebAPICalls. ");
                    break;
                case ConstantsProject.SET_PASSWORD_METHOD_ID:
                    log.Info("Start SetPassword_WebAPICalls. ");
                    Validate_WebAPICalls(jsonData, ConstantsProject.SET_PASSWORD_METHOD_ID, out ResponseEnd, out ResponseStatusEnd, out ResponseExternalEnd, out ResponseStatusExternalEnd, out FinalOutcomeEnd);
                    log.Info("End SetPassword_WebAPICalls. ");
                    break;
                //ResendMail
                case 25:
                    log.Info("Start ResendMail_WebAPICalls. ");
                    ResendMail_WebAPICalls(jsonData, Username, out ResponseEnd, out ResponseStatusEnd, out ResponseExternalEnd, out ResponseStatusExternalEnd, out FinalOutcomeEnd);
                    log.Info("End ResendMail_WebAPICalls. ");
                    break;
                case 26:
                    break;
            }
        }
        catch (Exception ex)
        {
            log.Error("Error in function WebAPICalls. " + ex.Message);
            //throw new Exception("Error in function WebAPICalls. " + ex.Message);
        }

        Response = ResponseEnd;
        ResponseStatus = ResponseStatusEnd;
        ResponseExternal = ResponseExternalEnd;
        ResponseStatusExternal = ResponseStatusExternalEnd;
        FinalOutcome = FinalOutcomeEnd;
    }

    protected void CopyFileToUploadFolderWithUserIdName(string UserId, out string destinationPath_Final)
    {
        destinationPath_Final = string.Empty;
        try
        {
            string hrefDocumentsUpload = System.Configuration.ConfigurationManager.AppSettings["hrefDocumentsUpload"].ToString();
            string hrefFiles = System.Configuration.ConfigurationManager.AppSettings["hrefFiles"].ToString();
            string contentPath = Server.MapPath(hrefDocumentsUpload);
            string RandomFile = ApiUtils.getRandomFile(hrefFiles);
            string RandomFileName = Path.GetFileName(RandomFile);
            //Automaticaly create folder with user id
            string path = Path.Combine(contentPath, UserId);
            Directory.CreateDirectory(path);

            string sourcePath = RandomFile;
            string destinationPath = path + @"\" + RandomFileName;
            destinationPath_Final = destinationPath;
            log.Info("sourcePath is: " + sourcePath + " . destinationPath is: " + destinationPath);
            File.Copy(sourcePath, destinationPath);
        }
        catch (Exception ex)
        {
            log.Error("Error while trying to Upload folder. " + ex.Message);
        }
    }

    protected void ExportInfo_WebAPICalls(string jsonData, string Username, int MethodId, out string Response, out string ResponseStatus, out string ResponseExternal, out string ResponseStatusExternal, out bool FinalOutcome)
    {
        Response = string.Empty;
        ResponseStatus = string.Empty;
        ResponseExternal = string.Empty;
        ResponseStatusExternal = string.Empty;
        FinalOutcome = false;
        string resultFinalExport = string.Empty;
        string statusCodeExport = string.Empty;
        string statusDescriptionExport = string.Empty;
        string resulNotOKExport = string.Empty;

        try
        {
            string resultFinalSearchUserIDByUsername = string.Empty;
            //todo string.empty   item.Username

            if (MethodId == ConstantsProject.EXPORT_USER_INFO_BY_USERNAME)
            {
                string SearchUserIDByUsername_Response = ApiUtils.ExportUserInfoByUsername_WebRequestCall(string.Empty, Username, out resultFinalExport, out statusCodeExport, out statusDescriptionExport, out resulNotOKExport);
            }
            if (MethodId == ConstantsProject.EXPORT_AUTH_INFO_BY_USERNAME)
            {
                string ExportAuthInfoByUsername_Response = ApiUtils.ExportAuthInfo_WebRequestCall(string.Empty, Username, out resultFinalExport, out statusCodeExport, out statusDescriptionExport, out resulNotOKExport);
            }
            ResponseStatus = statusCodeExport + " " + statusDescriptionExport;
            if ((Convert.ToInt32(statusCodeExport) == ConstantsProject.EXPORT_USER_INFO_BY_USERNAME_ОК) || (Convert.ToInt32(statusCodeExport) == ConstantsProject.EXPORT_AUTH_INFO_BY_USERNAME_ОК))
            {
                FinalOutcome = true;
                Response = resultFinalExport;
            }
            else
            {
                Response = resulNotOKExport;
            }
            log.Info("ExportInfo API end. Response result is: " + Response + " " + ResponseStatus);
        }
        catch (Exception ex)
        {
            log.Error("Error in function ExportInfo_WebAPICalls. " + ex.Message);
        }
    }


    protected void RegisterUser_WebAPICalls(string jsonData, string Username, out string Response, out string ResponseStatus, out string ResponseExternal, out string ResponseStatusExternal, out bool FinalOutcome)
    {
        Response = string.Empty;
        ResponseStatus = string.Empty;
        ResponseExternal = string.Empty;
        ResponseStatusExternal = string.Empty;
        FinalOutcome = false;
        bool FinalOutcomeFirstStep = false;
        bool FinalOutcomeSecondStep = false;
        string jsonData_SearchUserIDByUsername = string.Empty;
        string ResponseSearch = string.Empty;
        string ResponseStatusSearch = string.Empty;

        try
        {
            //Register user API start
            log.Info("Register user API start. ");
            string RegisterUser_Response = ApiUtils.RegisterUser_WebRequestCall(jsonData, out string resultResponse, out string statusCode, out string statusDescription, out string resulNotOK);
            ResponseStatus = statusCode + " " + statusDescription;
            if (Convert.ToInt32(statusCode) == ConstantsProject.REGISTER_USER_ОК)
            {
                FinalOutcomeFirstStep = true;
                Response = resultResponse;
            }
            else
            {
                Response = resulNotOK;
            }
            log.Info("Register user API end. Response result is: " + Response + " " + ResponseStatus);

            SearchUserIDBy_WebAPICalls(jsonData_SearchUserIDByUsername, Username, string.Empty, FinalOutcomeFirstStep, FinalOutcomeSecondStep, out FinalOutcome, out ResponseSearch, out ResponseStatusSearch, out ResponseExternal, out ResponseStatusExternal);
        }
        catch (Exception ex)
        {
            log.Error("Error in function RegisterUser_WebAPICalls. " + ex.Message);
        }
    }

    protected void ResendMail_WebAPICalls(string jsonData, string Username, out string Response, out string ResponseStatus, out string ResponseExternal, out string ResponseStatusExternal, out bool FinalOutcome)
    {
        Response = string.Empty;
        ResponseStatus = string.Empty;
        ResponseExternal = string.Empty;
        ResponseStatusExternal = string.Empty;
        FinalOutcome = false;
        bool FinalOutcomeFirstStep = false;
        bool FinalOutcomeSecondStep = false;
        string jsonData_SearchUserIDByUsername = string.Empty;
        string ResponseSearch = string.Empty;
        string ResponseStatusSearch = string.Empty;

        try
        {
            //ResendMail user API start
            log.Info("ResendMail user API start. ");
            string ResendMail_Response = ApiUtils.ResendMail_WebRequestCall(jsonData, out string resultResponse, out string statusCode, out string statusDescription, out string resulNotOK);
            ResponseStatus = statusCode + " " + statusDescription;
            if (Convert.ToInt32(statusCode) == ConstantsProject.REGISTER_USER_ОК)
            {
                FinalOutcomeFirstStep = true;
                Response = resultResponse;
            }
            else
            {
                Response = resulNotOK;
            }
            log.Info("ResendMail user API end. Response result is: " + Response + " " + ResponseStatus);
        }
        catch (Exception ex)
        {
            log.Error("Error in function ResendMail_WebAPICalls. " + ex.Message);
        }
    }

    protected void SearchUserIDBy_WebAPICalls(string jsonData, string Username, string UMCN, bool FinalOutcomeFirstStep, bool FinalOutcomeSecondStep, out bool FinalOutcome, out string statusCodeIDP, out string statusDescriptionIDP, out string ResponseExternal, out string ResponseStatusExternal)
    {
        statusCodeIDP = string.Empty;
        statusDescriptionIDP = string.Empty;
        ResponseExternal = string.Empty;
        ResponseStatusExternal = string.Empty;
        FinalOutcome = false;
        string statusCodeSearch = string.Empty;
        string statusDescriptionSearch = string.Empty;
        string resulNotOKsearch = string.Empty;
        string resultFinalSearchUserIDBy = string.Empty;

        try
        {
            if (Username != string.Empty)
            {
                log.Info("Start SearchUserIDByUsername ");
                string SearchUserIDByUsername_Response = ApiUtils.SearchUserIDByUsername_WebRequestCall(jsonData, Username, out resultFinalSearchUserIDBy, out statusCodeSearch, out statusDescriptionSearch, out resulNotOKsearch);
            }
            if (UMCN != string.Empty)
            {
                log.Info("Start SearchUserIDByUMCN ");
                string SearchUserIDByUMCN_Response = ApiUtils.SearchUserIDByUMCN_WebRequestCall(jsonData, UMCN, out resultFinalSearchUserIDBy, out statusCodeSearch, out statusDescriptionSearch, out resulNotOKsearch);
            }
            statusCodeIDP = statusCodeSearch;
            statusDescriptionIDP = statusDescriptionSearch;
            string UserId = ApiUtils.ParseJsonOneValue(resultFinalSearchUserIDBy, "userId");
            log.Info("End SearchUserIDByUsername. Response result is: " + UserId);

            if (UserId != string.Empty)
            {
                log.Info("Start calling SCIM web service. ");
                string resultFinalSCIMcheckData = string.Empty;
                string SCIMcheckData_Response = ApiUtils.SCIMcheckData_WebRequestCall(string.Empty, UserId, string.Empty, out resultFinalSCIMcheckData, out string statusCodeSCIM, out string statusDescriptionSCIM, out string resultNotOKscim);
                ResponseExternal = resultFinalSCIMcheckData;
                ResponseStatusExternal = statusCodeSCIM + " " + statusDescriptionSCIM;
                if (Convert.ToInt32(statusCodeSCIM) == ConstantsProject.REGISTER_USER_SCIM_ОК)
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
            log.Error("Error in function SearchUserIDByUsername_WebAPICalls. " + ex.Message);
        }
    }


    protected void Validate_WebAPICalls(string jsonData, int methodID, out string Response, out string ResponseStatus, out string ResponseExternal, out string ResponseStatusExternal, out bool FinalOutcome)
    {
        Response = string.Empty;
        ResponseStatus = string.Empty;
        ResponseExternal = string.Empty;
        ResponseStatusExternal = string.Empty;
        string statusCode = string.Empty;
        string resultResponse = string.Empty;
        string statusDescription = string.Empty;
        string resulNotOK = string.Empty;
        FinalOutcome = false;
        bool FinalOutcomeFirstStep = false;

        try
        {
            log.Info("Validate API for method " + methodID + " start.");
            if (methodID == ConstantsProject.VALIDATE_CODE_METHOD_ID)
            {
                string ValidateCode_Response = ApiUtils.ValidateCode_WebRequestCall(jsonData, out resultResponse, out statusCode, out statusDescription, out resulNotOK);
                ApiResponse(methodID, statusCode, ConstantsProject.VALIDATE_CODE_ОК, resultResponse, statusDescription, resulNotOK, out ResponseStatus, out FinalOutcomeFirstStep, out Response);
            }
            else if (methodID == ConstantsProject.VALIDATE_USERNAME_METHOD_ID)
            {
                string ValidateUsername_Response = ApiUtils.ValidateUsername_WebRequestCall(jsonData, out resultResponse, out statusCode, out statusDescription, out resulNotOK);
                ApiResponse(methodID, statusCode, ConstantsProject.VALIDATE_USERNAME_ОК, resultResponse, statusDescription, resulNotOK, out ResponseStatus, out FinalOutcomeFirstStep, out Response);
            }
            else if (methodID == ConstantsProject.VALIDATE_UMCN_METHOD_ID)
            {
                string ValidateUMCN_Response = ApiUtils.ValidateUMCN_WebRequestCall(jsonData, out resultResponse, out statusCode, out statusDescription, out resulNotOK);
                ApiResponse(methodID, statusCode, ConstantsProject.VALIDATE_USERNAME_ОК, resultResponse, statusDescription, resulNotOK, out ResponseStatus, out FinalOutcomeFirstStep, out Response);
            }
            else if (methodID == ConstantsProject.ADD_AUTHENTICATION_METHOD_ID)
            {
                string AddAuthentication_Response = ApiUtils.AddAuthentication_WebRequestCall(jsonData, out resultResponse, out statusCode, out statusDescription, out resulNotOK);
                ApiResponse(methodID, statusCode, ConstantsProject.ADD_AUTHENTICATION_ОК, resultResponse, statusDescription, resulNotOK, out ResponseStatus, out FinalOutcomeFirstStep, out Response);
            }
            else if (methodID == ConstantsProject.REMOVE_AUTHENTICATION_METHOD_ID)
            {
                string RemoveAuthentication_Response = ApiUtils.RemoveAuthentication_WebRequestCall(jsonData, out resultResponse, out statusCode, out statusDescription, out resulNotOK);
                ApiResponse(methodID, statusCode, ConstantsProject.REMOVE_AUTHENTICATION_ОК, resultResponse, statusDescription, resulNotOK, out ResponseStatus, out FinalOutcomeFirstStep, out Response);
            }
            else if (methodID == ConstantsProject.SET_PASSWORD_METHOD_ID)
            {
                string SetPassword_Response = ApiUtils.SetPassword_WebRequestCall(jsonData, out resultResponse, out statusCode, out statusDescription, out resulNotOK);
                ApiResponse(methodID, statusCode, ConstantsProject.SET_PASSWORD_METHOD_ОК, resultResponse, statusDescription, resulNotOK, out ResponseStatus, out FinalOutcomeFirstStep, out Response);
            }

            log.Info("Validate API for method " + methodID + " end. Response result is: " + Response + " " + ResponseStatus);

            FinalOutcome = FinalOutcomeFirstStep;
            log.Info("FinalOutcome is - " + FinalOutcome);
        }
        catch (Exception ex)
        {
            log.Error("Error in function ValidateCode_WebAPICalls. " + ex.Message);
        }
    }

    protected void ApiResponse(int methodId, string statusCode, int responseResultOK, string resultResponse, string statusDescription, string resulNotOK, out string ResponseStatus, out bool FinalOutcomeFirstStep, out string Response)
    {
        ResponseStatus = string.Empty;
        FinalOutcomeFirstStep = false;
        Response = string.Empty;

        try
        {
            ResponseStatus = statusCode + " " + statusDescription;
            if (Convert.ToInt32(statusCode) == responseResultOK)
            {
                if (resultResponse == string.Empty)
                {
                    FinalOutcomeFirstStep = true;
                }
                else
                {
                    if (methodId != ConstantsProject.ADD_AUTHENTICATION_METHOD_ID || methodId != ConstantsProject.REMOVE_AUTHENTICATION_METHOD_ID)
                    {
                        string statusCodes = ApiUtils.ParseJsonOneValue(resultResponse, "statusCode");
                        log.Info("statusCode for is " + statusCodes);
                        if (Convert.ToInt32(statusCodes) == ConstantsProject.VALIDATE_UMCN_USERNAME_ОК)
                        {
                            FinalOutcomeFirstStep = true;
                        }
                    }
                    else
                    {
                        FinalOutcomeFirstStep = true;
                    }
                }
                Response = resultResponse;
            }
            else
            {
                Response = resulNotOK;
            }
        }
        catch (Exception ex)
        {
            log.Error("Error in function ApiResponse. " + ex.Message);
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

    protected void btnDeleteUsersOnSCIM_Click(object sender, EventArgs e)
    {
        try
        {
            SCIM_DeleteUsersById();
        }
        catch (Exception ex)
        {
            log.Error("Error. " + ex.Message);
            ScriptManager.RegisterStartupScript(this, GetType(), "ErrorSendingData", "ErrorSendingData();", true);
        }
    }

    protected void SCIM_DeleteUsersById()
    {
        string jsonDataSCIM = string.Empty;
        string Response = string.Empty;
        string ResponseStatus = string.Empty;
        string ResponseExternal = string.Empty;
        string ResponseStatusExternal = string.Empty;
        bool FinalOutcome = false;
        string statusCodeSCIM = string.Empty;
        string deleteAllUsers = System.Configuration.ConfigurationManager.AppSettings["deleteAllUsers"].ToString();
        List<string> UserIdListScim = new List<string>();
        List<User> UserIdList = new List<User>();
        List<string> UsernameList = new List<string>();
        string Username = string.Empty;
        string resultFinalSearchUserIDBy = string.Empty;
        ProjectUtility utility = new ProjectUtility();

        try
        {
            if (deleteAllUsers == "1")
            {
                log.Info("Start getting all users from SCIM web service. ");
                string resultFinalSCIMcheckData = string.Empty;
                string SCIMcheckData_Response = ApiUtils.SCIMcheckData_WebRequestCall_All(jsonDataSCIM, out resultFinalSCIMcheckData, out statusCodeSCIM, out string statusDescriptionSCIM, out string resultNotOKscim);
                ResponseExternal = resultFinalSCIMcheckData;
                UserIdListScim = ParseRequestForSCIMUsers(ResponseExternal);
                log.Info("UserIdList count is: " + UserIdList.Count);

                ResponseStatusExternal = statusCodeSCIM + " " + statusDescriptionSCIM;
                log.Info("SCIM Response Status + " + ResponseStatusExternal);

                log.Info("End getting all users from SCIM web service. ");
            }
            else
            {
                log.Info("-----Start getting users from Database. -----");
                try
                {
                    UsernameList = utility.getUsernameList();

                    foreach (var username in UsernameList)
                    {
                        log.Info("Start SearchUserIDByUsername " + username);
                        string SearchUserIDByUsername_Response = ApiUtils.SearchUserIDByUsername_WebRequestCall(string.Empty, username, out resultFinalSearchUserIDBy, out string statusCodeSearch, out string statusDescriptionSearch, out string resulNotOKsearch);

                        string UserId = ApiUtils.ParseJsonOneValue(resultFinalSearchUserIDBy, "userId");
                        log.Info("End SearchUserIDByUsername. Response result is: " + UserId);
                        UserIdList.Add(new User(UserId, username, false));
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Error while getting users from database" + ex);
                }

                log.Info("-----End getting users from Database. Count is " + UserIdList.Count + "-----");
            }
            
            log.Info("-----Start deleting users in SCIM web service. -----");

            foreach (User item in UserIdList)
            {
                string resultDeleteFinal = string.Empty;
                string SCIM_DeleteUser_Response = ApiUtils.SCIMcheckData_WebRequestCall(jsonDataSCIM, item.UserId, ConstantsProject.DELETE_METHOD, out resultDeleteFinal, out string statusCode, out string statusDescription, out string resultNotOK);
                ResponseExternal = resultDeleteFinal;
                ResponseStatusExternal = statusCode + " " + statusDescription;
                if (Convert.ToInt32(statusCode) == ConstantsProject.DELETE_USER_SCIM_ОК)
                {
                    FinalOutcome = true;
                    utility.updateIsUsernameDeleted(FinalOutcome, item.Username);
                    log.Info("Is user with id: " + item.UserId + " deleted: " + FinalOutcome);
                }
            }

            log.Info("-----End deleting users in SCIM web service. -----");
            
        }
        catch (Exception ex)
        {
            log.Error("Error in function SCIM_DeleteUsersById. " + ex.Message);
            throw new Exception("Error in function SCIM_DeleteUsersById. " + ex.Message);
        }
        
    }

    protected List<string> ParseRequestForSCIMUsers(string jsonResponse)
    {
        List<string> resultList = new List<string>();

        // Parse your Result to an Array
        var x = JObject.Parse(jsonResponse);
        //log.Info("x is " + x.ToString());
        var y = x["Resources"];

        foreach (JObject o in y.Children<JObject>())
        {
            var resultPrepared = o["id"];
            string result = resultPrepared.ToString();
            resultList.Add(result);
        }

        return resultList;
    }

    public void CheckBox1_CheckedChanged(object sender, EventArgs e)
    {
        if (CheckBox1.Checked == true)
        {
            isEnabledButtons(true);
        }
        else
        {
            isEnabledButtons(false);
        }
    }


    protected void btnBulkOnSCIM_Click(object sender, EventArgs e)
    {
        try
        {
            string Response = string.Empty;
            string ResponseStatus = string.Empty;
            string ResponseExternal = string.Empty;
            string ResponseStatusExternal = string.Empty;
            bool FinalOutcome = false;
            bool FinalOutcomeFirstStep = false;
            bool FinalOutcomeSecondStep = false;
            string jsonData_SearchUserIDByUsername = string.Empty;
            string ResponseSearch = string.Empty;
            string ResponseStatusSearch = string.Empty;

            ProjectUtility utility = new ProjectUtility();
            string jsonDataResult = utility.spBulkSet();

            //log.Info("request data is " + jsonDataResult);

             string jsonDataSCIM_BULK_Replace = jsonDataResult.Replace(@"""""", @"""");

            //log.Info("request data is " + jsonDataSCIM_BULK_Replace);

            log.Info("Register user in BULK start. " + DateTime.Now.ToString("yyyy MM dd HH:mm:ss:FFF"));
            string RegisterUser_Response = ApiUtils.CreateUsersInBulk_WebRequestCall(jsonDataSCIM_BULK_Replace, out string resultResponse, out string statusCode, out string statusDescription, out string resulNotOK);
            log.Info("Register user in BULK end1. " + DateTime.Now.ToString("yyyy MM dd HH:mm:ss:FFF"));
            ResponseStatus = statusCode + " " + statusDescription;
            if (Convert.ToInt32(statusCode) == ConstantsProject.REGISTER_USER_ОК)
            {
                FinalOutcomeFirstStep = true;
                Response = resultResponse;
            }
            else
            {
                Response = resulNotOK;
            }
            log.Info("Register user in BULK end2. " + DateTime.Now.ToString("yyyy MM dd HH:mm:ss:FFF"));
            //log.Info("Register user in BULK end. Response result is: " + Response + " " + ResponseStatus);
            ScriptManager.RegisterStartupScript(this, GetType(), "SuccessSendingData", "SuccessSendingData();", true);
        }
        catch (Exception ex)
        {
            log.Error("Error on click btnBulkOnSCIM. " + ex.Message);
            ScriptManager.RegisterStartupScript(this, GetType(), "ErrorSendingData", "ErrorSendingData();", true);
        }
    }


    /// <summary>
    /// DOCUMENT METHODS TESTING
    /// </summary>
    public string fileName { get; set; }
    public string fileFormat { get; set; }

    protected void btnUploadClick(object sender, EventArgs e)
    {
        HttpPostedFile file = Request.Files["myFile"];

        //check file was submitted
        if (file != null && file.ContentLength > 0)
        {
            fileName = Path.GetFileName(file.FileName);
            Session["EGovTest_fileName"] = fileName;
            fileFormat = Path.GetExtension(fileName);
            Session["EGovTest_fileFormat"] = fileFormat;
            log.Info("fileName is: " + fileName + " .fileFormat is: " + fileFormat);
            file.SaveAs(Server.MapPath(Path.Combine("~/App_Data/", fileName)));
        }
    }

    protected void DocumentMethods_WebAPICalls(string jsonData, string username, string path, string Download_DocumentId, int MethodId, out string Response, out string ResponseStatus, out string ResponseExternal, out string ResponseStatusExternal, out bool FinalOutcome)
    {
        Response = string.Empty;
        ResponseStatus = string.Empty;
        ResponseExternal = string.Empty;
        ResponseStatusExternal = string.Empty;
        FinalOutcome = false;
        string resultResponse = string.Empty;
        string statusCode = string.Empty;
        string statusDescription = string.Empty;
        string resulNotOK = string.Empty;
        Dictionary<string, object> postParameters = new Dictionary<string, object>();
        byte[] bytes1 = new byte[0];
        ProjectUtility utility = new ProjectUtility();

        try
        {
            if (MethodId == ConstantsProject.UPLOAD_DOCUMENTS_METHOD_ID)
            {
                string hrefDocumentsUploadFinal = System.Configuration.ConfigurationManager.AppSettings["hrefDocumentsUploadFinal"].ToString();
                string filePath = path;

                fileName = Path.GetFileName(filePath);
                fileFormat = Path.GetExtension(fileName);

                byte[] bytes = System.IO.File.ReadAllBytes(filePath);
                ApiUtils.FileParameter fileParameter = new ApiUtils.FileParameter(bytes, fileName, "multipart/form-data");
                //// Generate post objects
                postParameters.Add("filename", fileName);
                postParameters.Add("fileformat", fileFormat);
                postParameters.Add("file", fileParameter);

                int UserDocumentID = utility.getUserDocumentID(username);
                utility.insertStartTimeDocumentAction(username, UserDocumentID, ConstantsProject.UPLOAD_ACTION_TYPE, DateTime.Now);

                log.Info("Start document Upload. Username: " + username + " . " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:FFF"));
                string Test_Response = ApiUtils.Documents_WebRequestCall(MethodId, bytes, postParameters, jsonData, username, ConstantsProject.DOCUMENTS_POST_METHOD, string.Empty, out resultResponse, out statusCode, out statusDescription, out resulNotOK);
                log.Info("End document Upload. Username: " + username + " . " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:FFF"));

                utility.updateFinishTimeDocumentAction(UserDocumentID, ConstantsProject.UPLOAD_ACTION_TYPE, DateTime.Now);
                string DocumentId = ApiUtils.ParseJsonOneValue(resultResponse, "id");
                utility.updateDocumentIdDocument(username, DocumentId);
            }
            else if (MethodId == ConstantsProject.LIST_DOCUMENTS_METHOD_ID)
            { 
                log.Info("Start List documents. Username: " + username + " . " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:FFF"));
                string Test_Response = ApiUtils.Documents_WebRequestCall(MethodId, bytes1, postParameters, jsonData, username, ConstantsProject.DOCUMENTS_GET_METHOD, string.Empty, out resultResponse, out statusCode, out statusDescription, out resulNotOK);
                log.Info("End List documents. Username: " + username + " . " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:FFF"));
            }
            else if (MethodId == ConstantsProject.DOWNLOAD_DOCUMENTS_METHOD_ID)
            {
                int UserDocumentID = utility.getUserDocumentID(username);
                utility.insertStartTimeDocumentAction(username, UserDocumentID, ConstantsProject.DOWNLOAD_ACTION_TYPE, DateTime.Now);

                log.Info("Start Download document. Username: " + username + " . DocumentId: " + Download_DocumentId + " . " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:FFF"));
                string Test_Response = ApiUtils.Documents_WebRequestCall(MethodId, bytes1, postParameters, jsonData, username, ConstantsProject.DOCUMENTS_GET_METHOD, Download_DocumentId, out resultResponse, out statusCode, out statusDescription, out resulNotOK);
                log.Info("End Download document. Username: " + username + " . DocumentId: " + Download_DocumentId + " . " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:FFF"));

                utility.updateFinishTimeDocumentAction(UserDocumentID, ConstantsProject.DOWNLOAD_ACTION_TYPE, DateTime.Now);
            }

            ResponseStatus = statusCode + " " + statusDescription;
            log.Info("ResponseStatus: " + ResponseStatus);
            if (Convert.ToInt32(statusCode) == ConstantsProject.DOCUMENTS_METHOD_ОК)
            {
                FinalOutcome = true;
                Response = resultResponse;
            }
            else
            {
                Response = resulNotOK;
            }
            log.Info("End document Upload or Download. Username: " + username + " . " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:FFF"));
            ScriptManager.RegisterStartupScript(this, GetType(), "SuccessSendingData", "SuccessSendingData();", true);
        }
        catch (Exception ex)
        {
            log.Error("Error in function DocumentMethods_WebAPICalls. " + ex.Message);
            ScriptManager.RegisterStartupScript(this, GetType(), "ErrorSendingData", "ErrorSendingData();", true);
        }
    }

    protected void btnTestSecure_Click(object sender, EventArgs e)
    {
        try
        {
            string Response = string.Empty;
            string ResponseStatus = string.Empty;
            string ResponseExternal = string.Empty;
            string ResponseStatusExternal = string.Empty;
            string ResponseSearch = string.Empty;
            string ResponseStatusSearch = string.Empty;
            string jsonData = string.Empty;
            bool FinalOutcomeFirstStep = false;

            log.Info("Test secure connection start. " + DateTime.Now.ToString("yyyy MM dd HH:mm:ss:FFF"));
            string TestSecureConn_Response = ApiUtils.TestSecureConn_WebRequestCall(jsonData, out string resultResponse, out string statusCode, out string statusDescription, out string resulNotOK);
            log.Info("Test secure connection end1. " + DateTime.Now.ToString("yyyy MM dd HH:mm:ss:FFF"));
            ResponseStatus = statusCode + " " + statusDescription;
            if (Convert.ToInt32(statusCode) == ConstantsProject.REGISTER_USER_ОК)
            {
                FinalOutcomeFirstStep = true;
                Response = resultResponse;
            }
            else
            {
                Response = resulNotOK;
            }
            log.Info("Test secure connection end2. " + DateTime.Now.ToString("yyyy MM dd HH:mm:ss:FFF"));
            //log.Info("Register user in BULK end. Response result is: " + Response + " " + ResponseStatus);
            ScriptManager.RegisterStartupScript(this, GetType(), "SuccessSendingData", "SuccessSendingData();", true);
        }
        catch (Exception ex)
        {
            log.Error("Error on click btnTestSecure. " + ex.Message);
            ScriptManager.RegisterStartupScript(this, GetType(), "ErrorSendingData", "ErrorSendingData();", true);
        }

    }

    protected void btnDownloadFile_Click(object sender, EventArgs e)
    {
        try
        {
            string ResponseEnd = string.Empty;
            string ResponseStatusEnd = string.Empty;
            string ResponseExternalEnd = string.Empty;
            string ResponseStatusExternalEnd = string.Empty;
            string ResponseStatus = string.Empty;
            bool FinalOutcomeEnd = false;

            log.Info("Download File start. " + DateTime.Now.ToString("yyyy MM dd HH:mm:ss:FFF"));
            DocumentMethods_WebAPICalls(string.Empty, "user1.00145@test.com", string.Empty, "0080346c-8742-416a-ba70-7e61f4f2b2cd", ConstantsProject.DOWNLOAD_DOCUMENTS_METHOD_ID, out ResponseEnd, out ResponseStatusEnd, out ResponseExternalEnd, out ResponseStatusExternalEnd, out FinalOutcomeEnd);
            log.Info("Download File end. " + DateTime.Now.ToString("yyyy MM dd HH:mm:ss:FFF"));

            ScriptManager.RegisterStartupScript(this, GetType(), "SuccessSendingData", "SuccessSendingData();", true);
        }
        catch (Exception ex)
        {
            log.Error("Error while downloading file. " + ex.Message);
            ScriptManager.RegisterStartupScript(this, GetType(), "ErrorSendingData", "ErrorSendingData();", true);
        }
    }
}