using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.XPath;
using Newtonsoft.Json.Linq;

public static class ApiUtils
{
    public static string SettingsFile { get; private set; }
    public static string RegisterUser_Url_Out { get; set; }
    public static string RegisterUser_ContentType_Out { get; set; }
    public static string RegisterUser_Method_Out { get; set; }
    public static string RegisterUser_Username_Out { get; set; }
    public static string RegisterUser_Password_Out { get; set; }
    public static string SearchUserIDByUsername_Url_Out { get; set; }
    public static string SearchUserIDByUsername_ContentType_Out { get; set; }
    public static string SearchUserIDByUsername_Method_Out { get; set; }
    public static string SearchUserIDByUsername_Username_Out { get; set; }
    public static string SearchUserIDByUsername_Password_Out { get; set; }
    public static string SCIMcheckData_Url_Out { get; set; }
    public static string SCIMcheckData_ContentType_Out { get; set; }
    public static string SCIMcheckData_Method_Out { get; set; }
    public static string SCIMcheckData_Username_Out { get; set; }
    public static string SCIMcheckData_Password_Out { get; set; }
    public static string ValidateCode_Url_Out { get; set; }
    public static string ValidateCode_ContentType_Out { get; set; }
    public static string ValidateCode_Method_Out { get; set; }
    public static string ValidateCode_Username_Out { get; set; }
    public static string ValidateCode_Password_Out { get; set; }
    public static string ValidateUsername_Url_Out { get; set; }
    public static string ValidateUsername_ContentType_Out { get; set; }
    public static string ValidateUsername_Method_Out { get; set; }
    public static string ValidateUsername_Username_Out { get; set; }
    public static string ValidateUsername_Password_Out { get; set; }
    public static string ValidateUMCN_Url_Out { get; set; }
    public static string ValidateUMCN_ContentType_Out { get; set; }
    public static string ValidateUMCN_Method_Out { get; set; }
    public static string ValidateUMCN_Username_Out { get; set; }
    public static string ValidateUMCN_Password_Out { get; set; }
    public static string ExportUserInfoByUsername_Url_Out { get; set; }
    public static string ExportUserInfoByUsername_ContentType_Out { get; set; }
    public static string ExportUserInfoByUsername_Method_Out { get; set; }
    public static string ExportUserInfoByUsername_Username_Out { get; set; }
    public static string ExportUserInfoByUsername_Password_Out { get; set; }
    public static string SearchUserIDByUMCN_Url_Out { get; set; }
    public static string SearchUserIDByUMCN_ContentType_Out { get; set; }
    public static string SearchUserIDByUMCN_Method_Out { get; set; }
    public static string SearchUserIDByUMCN_Username_Out { get; set; }
    public static string SearchUserIDByUMCN_Password_Out { get; set; }
    public static string CreateUsersInBulk_Url_Out { get; set; }
    public static string CreateUsersInBulk_ContentType_Out { get; set; }
    public static string CreateUsersInBulk_Method_Out { get; set; }
    public static string CreateUsersInBulk_Username_Out { get; set; }
    public static string CreateUsersInBulk_Password_Out { get; set; }
    public static string ExportAuthInfo_Url_Out { get; set; }
    public static string ExportAuthInfo_ContentType_Out { get; set; }
    public static string ExportAuthInfo_Method_Out { get; set; }
    public static string ExportAuthInfo_Username_Out { get; set; }
    public static string ExportAuthInfo_Password_Out { get; set; }
    /******BASIC AUTH*******/
    public static string RegisterUser_BasicAuth { get; set; }
    public static string SearchUserIDByUsername_BasicAuth { get; set; }
    public static string SCIMcheckData_BasicAuth { get; set; }
    public static string ValidateCode_BasicAuth { get; set; }
    public static string ValidateUsername_BasicAuth { get; set; }
    public static string ValidateUMCN_BasicAuth { get; set; }
    public static string ExportUserInfoByUsername_BasicAuth { get; set; }
    public static string SearchUserIDByUMCN_BasicAuth { get; set; }
    public static string CreateUsersInBulk_BasicAuth { get; set; }
    public static string ExportAuthInfo_BasicAuth { get; set; }

    private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

    static ApiUtils()
    {
        SettingsFile = AppDomain.CurrentDomain.BaseDirectory + "APIsettings.xml";
        initializeSetup();
    }

    public static void initializeSetup()
    {
        getSettings();
    }

    public static void getSettings()
    {
        try
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(SettingsFile);
            XPathNavigator navigator = xmlDoc.CreateNavigator();

            navigator.MoveToRoot();
            navigator.MoveToFollowing(System.Xml.XPath.XPathNodeType.Element);//parameters
            if (navigator.HasChildren)
            {
                navigator.MoveToFirstChild();//<RegisterUser>
                do
                {
                    if (navigator.Name == "RegisterUser")
                    {
                        LoopingThrowNavigatorChild(navigator, out string RegisterUser_Url_Out_Final, out string RegisterUser_ContentType_Out_Final, out string RegisterUser_Method_Out_Final, out string RegisterUser_Username_Out_Final, out string RegisterUser_Password_Out_Final);
                        RegisterUser_Url_Out = RegisterUser_Url_Out_Final;
                        RegisterUser_ContentType_Out = RegisterUser_ContentType_Out_Final;
                        RegisterUser_Method_Out = RegisterUser_Method_Out_Final;
                        RegisterUser_Username_Out = RegisterUser_Username_Out_Final;
                        RegisterUser_Password_Out = RegisterUser_Password_Out_Final;
                        RegisterUser_BasicAuth = RegisterUser_Username_Out + ":" + RegisterUser_Password_Out;
                        navigator.MoveToFollowing(XPathNodeType.Element);
                    }
                    if (navigator.Name == "SearchUserIDByUsername")
                    {
                        LoopingThrowNavigatorChild(navigator, out string SearchUserIDByUsername_Url_Out_Final, out string SearchUserIDByUsername_ContentType_Out_Final, out string SearchUserIDByUsername_Method_Out_Final, out string SearchUserIDByUsername_Username_Out_Final, out string SearchUserIDByUsername_Password_Out_Final);
                        SearchUserIDByUsername_Url_Out = SearchUserIDByUsername_Url_Out_Final;
                        SearchUserIDByUsername_ContentType_Out = SearchUserIDByUsername_ContentType_Out_Final;
                        SearchUserIDByUsername_Method_Out = SearchUserIDByUsername_Method_Out_Final;
                        SearchUserIDByUsername_Username_Out = SearchUserIDByUsername_Username_Out_Final;
                        SearchUserIDByUsername_Password_Out = SearchUserIDByUsername_Password_Out_Final;
                        SearchUserIDByUsername_BasicAuth = SearchUserIDByUsername_Username_Out + ":" + SearchUserIDByUsername_Password_Out;
                        navigator.MoveToFollowing(XPathNodeType.Element);
                        navigator.MoveToNext();
                    }
                    if (navigator.Name == "SCIMcheckData")
                    {
                        LoopingThrowNavigatorChild(navigator, out string SCIMcheckData_Url_Out_Final, out string SCIMcheckData_ContentType_Out_Final, out string SCIMcheckData_Method_Out_Final, out string SCIMcheckData_Username_Out_Final, out string SCIMcheckData_Password_Out_Final);
                        SCIMcheckData_Url_Out = SCIMcheckData_Url_Out_Final;
                        SCIMcheckData_ContentType_Out = SCIMcheckData_ContentType_Out_Final;
                        SCIMcheckData_Method_Out = SCIMcheckData_Method_Out_Final;
                        SCIMcheckData_Username_Out = SCIMcheckData_Username_Out_Final;
                        SCIMcheckData_Password_Out = SCIMcheckData_Password_Out_Final;
                        SCIMcheckData_BasicAuth = SCIMcheckData_Username_Out + ":" + SCIMcheckData_Password_Out;
                        navigator.MoveToFollowing(XPathNodeType.Element);
                        navigatorMoveToNextLoop(navigator, 2);
                    }
                    if (navigator.Name == "ValidateCode")
                    {
                        LoopingThrowNavigatorChild(navigator, out string ValidateCode_Url_Out_Final, out string ValidateCode_ContentType_Out_Final, out string ValidateCode_Method_Out_Final, out string ValidateCode_Username_Out_Final, out string ValidateCode_Password_Out_Final);
                        ValidateCode_Url_Out = ValidateCode_Url_Out_Final;
                        ValidateCode_ContentType_Out = ValidateCode_ContentType_Out_Final;
                        ValidateCode_Method_Out = ValidateCode_Method_Out_Final;
                        ValidateCode_Username_Out = ValidateCode_Username_Out_Final;
                        ValidateCode_Password_Out = ValidateCode_Password_Out_Final;
                        ValidateCode_BasicAuth = ValidateCode_Username_Out + ":" + ValidateCode_Password_Out;
                        navigator.MoveToFollowing(XPathNodeType.Element);
                        navigatorMoveToNextLoop(navigator, 3);
                    }
                    if (navigator.Name == "ValidateUsername")
                    {
                        LoopingThrowNavigatorChild(navigator, out string ValidateUsername_Url_Out_Final, out string ValidateUsername_ContentType_Out_Final, out string ValidateUsername_Method_Out_Final, out string ValidateUsername_Username_Out_Final, out string ValidateUsername_Password_Out_Final);
                        ValidateUsername_Url_Out = ValidateUsername_Url_Out_Final;
                        ValidateUsername_ContentType_Out = ValidateUsername_ContentType_Out_Final;
                        ValidateUsername_Method_Out = ValidateUsername_Method_Out_Final;
                        ValidateUsername_Username_Out = ValidateUsername_Username_Out_Final;
                        ValidateUsername_Password_Out = ValidateUsername_Password_Out_Final;
                        ValidateUsername_BasicAuth = ValidateUsername_Username_Out + ":" + ValidateUsername_Password_Out;
                        navigator.MoveToFollowing(XPathNodeType.Element);
                        navigatorMoveToNextLoop(navigator, 4);
                    }
                    if (navigator.Name == "ValidateUMCN")
                    {
                        LoopingThrowNavigatorChild(navigator, out string ValidateUMCN_Url_Out_Final, out string ValidateUMCN_ContentType_Out_Final, out string ValidateUMCN_Method_Out_Final, out string ValidateUMCN_Username_Out_Final, out string ValidateUMCN_Password_Out_Final);
                        ValidateUMCN_Url_Out = ValidateUMCN_Url_Out_Final;
                        ValidateUMCN_ContentType_Out = ValidateUMCN_ContentType_Out_Final;
                        ValidateUMCN_Method_Out = ValidateUMCN_Method_Out_Final;
                        ValidateUMCN_Username_Out = ValidateUMCN_Username_Out_Final;
                        ValidateUMCN_Password_Out = ValidateUMCN_Password_Out_Final;
                        ValidateUMCN_BasicAuth = ValidateUMCN_Username_Out + ":" + ValidateUMCN_Password_Out;
                        navigator.MoveToFollowing(XPathNodeType.Element);
                        navigatorMoveToNextLoop(navigator, 5);
                    }
                    if (navigator.Name == "ExportUserInfoByUsername")
                    {
                        LoopingThrowNavigatorChild(navigator, out string ExportUserInfoByUsername_Url_Out_Final, out string ExportUserInfoByUsername_ContentType_Out_Final, out string ExportUserInfoByUsername_Method_Out_Final, out string ExportUserInfoByUsername_Username_Out_Final, out string ExportUserInfoByUsername_Password_Out_Final);
                        ExportUserInfoByUsername_Url_Out = ExportUserInfoByUsername_Url_Out_Final;
                        ExportUserInfoByUsername_ContentType_Out = ExportUserInfoByUsername_ContentType_Out_Final;
                        ExportUserInfoByUsername_Method_Out = ExportUserInfoByUsername_Method_Out_Final;
                        ExportUserInfoByUsername_Username_Out = ExportUserInfoByUsername_Username_Out_Final;
                        ExportUserInfoByUsername_Password_Out = ExportUserInfoByUsername_Password_Out_Final;
                        ExportUserInfoByUsername_BasicAuth = ExportUserInfoByUsername_Username_Out + ":" + ExportUserInfoByUsername_Password_Out;
                        navigator.MoveToFollowing(XPathNodeType.Element);
                        navigatorMoveToNextLoop(navigator, 6);
                    }
                    if (navigator.Name == "SearchUserIDByUMCN")
                    {
                        LoopingThrowNavigatorChild(navigator, out string SearchUserIDByUMCN_Url_Out_Final, out string SearchUserIDByUMCN_ContentType_Out_Final, out string SearchUserIDByUMCN_Method_Out_Final, out string SearchUserIDByUMCN_Username_Out_Final, out string SearchUserIDByUMCN_Password_Out_Final);
                        SearchUserIDByUMCN_Url_Out = SearchUserIDByUMCN_Url_Out_Final;
                        SearchUserIDByUMCN_ContentType_Out = SearchUserIDByUMCN_ContentType_Out_Final;
                        SearchUserIDByUMCN_Method_Out = SearchUserIDByUMCN_Method_Out_Final;
                        SearchUserIDByUMCN_Username_Out = SearchUserIDByUMCN_Username_Out_Final;
                        SearchUserIDByUMCN_Password_Out = SearchUserIDByUMCN_Password_Out_Final;
                        SearchUserIDByUMCN_BasicAuth = SearchUserIDByUMCN_Username_Out + ":" + SearchUserIDByUMCN_Password_Out;
                        navigator.MoveToFollowing(XPathNodeType.Element);
                        navigatorMoveToNextLoop(navigator, 7);
                    }
                    if (navigator.Name == "CreateUsersInBulk")
                    {
                        LoopingThrowNavigatorChild(navigator, out string CreateUsersInBulk_Url_Out_Final, out string CreateUsersInBulk_ContentType_Out_Final, out string CreateUsersInBulk_Method_Out_Final, out string CreateUsersInBulk_Username_Out_Final, out string CreateUsersInBulk_Password_Out_Final);
                        CreateUsersInBulk_Url_Out = CreateUsersInBulk_Url_Out_Final;
                        CreateUsersInBulk_ContentType_Out = CreateUsersInBulk_ContentType_Out_Final;
                        CreateUsersInBulk_Method_Out = CreateUsersInBulk_Method_Out_Final;
                        CreateUsersInBulk_Username_Out = CreateUsersInBulk_Username_Out_Final;
                        CreateUsersInBulk_Password_Out = CreateUsersInBulk_Password_Out_Final;
                        CreateUsersInBulk_BasicAuth = CreateUsersInBulk_Username_Out + ":" + CreateUsersInBulk_Password_Out;
                        navigator.MoveToFollowing(XPathNodeType.Element);
                        navigatorMoveToNextLoop(navigator, 8);
                    }
                    if (navigator.Name == "ExportAuthInfo")
                    {
                        LoopingThrowNavigatorChild(navigator, out string ExportAuthInfo_Url_Out_Final, out string ExportAuthInfo_ContentType_Out_Final, out string ExportAuthInfo_Method_Out_Final, out string ExportAuthInfo_Username_Out_Final, out string ExportAuthInfo_Password_Out_Final);
                        ExportAuthInfo_Url_Out = ExportAuthInfo_Url_Out_Final;
                        ExportAuthInfo_ContentType_Out = ExportAuthInfo_ContentType_Out_Final;
                        ExportAuthInfo_Method_Out = ExportAuthInfo_Method_Out_Final;
                        ExportAuthInfo_Username_Out = ExportAuthInfo_Username_Out_Final;
                        ExportAuthInfo_Password_Out = ExportAuthInfo_Password_Out_Final;
                        ExportAuthInfo_BasicAuth = ExportAuthInfo_Username_Out + ":" + ExportAuthInfo_Password_Out;
                        navigator.MoveToFollowing(XPathNodeType.Element);
                    }
                } while (navigator.MoveToNext());
            }
        }
        catch (Exception ex)
        {
            log.Error("Error while reading configuration data. " + ex.Message);
        }
    }

    public static void navigatorMoveToNextLoop(XPathNavigator navigator, int howManyTimes)
    {
        for (int i = 0; i < howManyTimes; i++)
        {
            navigator.MoveToNext();
        }
    }

    public static void LoopingThrowNavigatorChild(XPathNavigator navigator, out string Url_Out, out string ContentType_Out, out string Method_Out, out string Username_Out, out string Password_Out)
    {
        Url_Out = string.Empty;
        ContentType_Out = string.Empty;
        Method_Out = string.Empty;
        Username_Out = string.Empty;
        Password_Out = string.Empty;

        do
        {
            navigator.MoveToFirstChild();
            if (navigator.Name == "url")
            {
                Url_Out = navigator.Value;
            }
            navigator.MoveToFollowing(XPathNodeType.Element);
            if (navigator.Name == "contentType")
            {
                ContentType_Out = navigator.Value;
            }
            navigator.MoveToFollowing(XPathNodeType.Element);
            if (navigator.Name == "method")
            {
                Method_Out = navigator.Value;
            }
            navigator.MoveToFollowing(XPathNodeType.Element);
            if (navigator.Name == "username")
            {
                Username_Out = navigator.Value;
            }
            navigator.MoveToFollowing(XPathNodeType.Element);
            if (navigator.Name == "password")
            {
                Password_Out = navigator.Value;
            }
            log.Info("Get parameters from settings file : URL - " + Url_Out + " . Content Type - " + ContentType_Out + " . Method - " + Method_Out + " . Username - " + Username_Out + " . Password - " + Password_Out);
            navigator.MoveToFollowing(XPathNodeType.Element);

            navigator.MoveToParent();

        } while (navigator.MoveToNext());  
    }




    public static string RegisterUser_WebRequestCall(string data, out string result_Final_RegisterUser, out string StatusCode_Final_RegisterUser, out string StatusDescription_Final_RegisterUser, out string result_Final_NotOK)
    {
        StatusCode_Final_RegisterUser = string.Empty;
        StatusDescription_Final_RegisterUser = string.Empty;
        result_Final_RegisterUser = string.Empty;
        result_Final_NotOK = string.Empty;
        /*******************************/
        string WebCall = WebRequestCall(data, RegisterUser_Url_Out, RegisterUser_Method_Out, RegisterUser_ContentType_Out, RegisterUser_BasicAuth, out string resultFinal, out string StatusCodeFinal, out string StatusDescriptionFinal, out string resultFinalBad);
        /*******************************/
        StatusCode_Final_RegisterUser = StatusCodeFinal;
        StatusDescription_Final_RegisterUser = StatusDescriptionFinal;
        result_Final_RegisterUser = resultFinal;
        result_Final_NotOK = resultFinalBad;
        /*******************************/
        return WebCall;
    }

    public static string SearchUserIDByUsername_WebRequestCall(string data, string username, out string result_Final_SearchUserIDByUsername, out string StatusCode_Final_SearchUserIDByUsername, out string StatusDescription_Final_SearchUserIDByUsername, out string result_Final_NotOK)
    {
        StatusCode_Final_SearchUserIDByUsername = string.Empty;
        StatusDescription_Final_SearchUserIDByUsername = string.Empty;
        result_Final_SearchUserIDByUsername = string.Empty;
        result_Final_NotOK = string.Empty;
        /*******************************/
        string WebCall = WebRequestCall(data, (SearchUserIDByUsername_Url_Out + username), SearchUserIDByUsername_Method_Out, SearchUserIDByUsername_ContentType_Out, SearchUserIDByUsername_BasicAuth, out string resultFinal, out string StatusCodeFinal, out string StatusDescriptionFinal, out string resultFinalBad);
        /*******************************/
        StatusCode_Final_SearchUserIDByUsername = StatusCodeFinal;
        StatusDescription_Final_SearchUserIDByUsername = StatusDescriptionFinal;
        result_Final_SearchUserIDByUsername = resultFinal;
        result_Final_NotOK = resultFinalBad;
        /*******************************/
        return WebCall;
    }

    public static string SCIMcheckData_WebRequestCall(string data, string UserID, string Method, out string result_Final_SCIMcheckData, out string StatusCode_Final_SCIMcheckData, out string StatusDescription_Final_SCIMcheckData, out string result_Final_NotOK)
    {
        StatusCode_Final_SCIMcheckData = string.Empty;
        StatusDescription_Final_SCIMcheckData = string.Empty;
        result_Final_SCIMcheckData = string.Empty;
        result_Final_NotOK = string.Empty;
        string WebCall = string.Empty;
        /*******************************/
        if (Method == ConstantsProject.DELETE_METHOD)
        {
            WebCall = WebRequestCall(data, (SCIMcheckData_Url_Out + UserID), Method, SCIMcheckData_ContentType_Out, SCIMcheckData_BasicAuth, out string resultFinal, out string StatusCodeFinal, out string StatusDescriptionFinal, out string resultFinalBad);
            StatusCode_Final_SCIMcheckData = StatusCodeFinal;
            StatusDescription_Final_SCIMcheckData = StatusDescriptionFinal;
            result_Final_SCIMcheckData = resultFinal;
            result_Final_NotOK = resultFinalBad;
        }
        else if (Method == ConstantsProject.PUT_METHOD)
        {
            WebCall = WebRequestCall(data, (SCIMcheckData_Url_Out + UserID), Method, "application/json", SCIMcheckData_BasicAuth, out string resultFinal, out string StatusCodeFinal, out string StatusDescriptionFinal, out string resultFinalBad);
            StatusCode_Final_SCIMcheckData = StatusCodeFinal;
            StatusDescription_Final_SCIMcheckData = StatusDescriptionFinal;
            result_Final_SCIMcheckData = resultFinal;
            result_Final_NotOK = resultFinalBad;
        }
        else
        {
            WebCall = WebRequestCall(data, (SCIMcheckData_Url_Out + UserID), SCIMcheckData_Method_Out, SCIMcheckData_ContentType_Out, SCIMcheckData_BasicAuth, out string resultFinal, out string StatusCodeFinal, out string StatusDescriptionFinal, out string resultFinalBad);
            StatusCode_Final_SCIMcheckData = StatusCodeFinal;
            StatusDescription_Final_SCIMcheckData = StatusDescriptionFinal;
            result_Final_SCIMcheckData = resultFinal;
            result_Final_NotOK = resultFinalBad;
        }
        return WebCall;
    }

    public static string SCIMcheckData_WebRequestCall_All(string data, out string result_Final_SCIMcheckData_All, out string StatusCode_Final_SCIMcheckData_All, out string StatusDescription_Final_SCIMcheckData_All, out string result_Final_NotOK)
    {
        StatusCode_Final_SCIMcheckData_All = string.Empty;
        StatusDescription_Final_SCIMcheckData_All = string.Empty;
        result_Final_SCIMcheckData_All = string.Empty;
        result_Final_NotOK = string.Empty;
        /*******************************/
        string WebCall = WebRequestCall(data, SCIMcheckData_Url_Out, SCIMcheckData_Method_Out, SCIMcheckData_ContentType_Out, SCIMcheckData_BasicAuth, out string resultFinal, out string StatusCodeFinal, out string StatusDescriptionFinal, out string resultFinalBad);
        /*******************************/
        StatusCode_Final_SCIMcheckData_All = StatusCodeFinal;
        StatusDescription_Final_SCIMcheckData_All = StatusDescriptionFinal;
        result_Final_SCIMcheckData_All = resultFinal;
        result_Final_NotOK = resultFinalBad;
        /*******************************/
        return WebCall;
    }

    public static string ValidateCode_WebRequestCall(string data, out string result_Final_ValidateCode, out string StatusCode_Final_ValidateCode, out string StatusDescription_Final_ValidateCode, out string resultFinal_NotOK)
    {
        result_Final_ValidateCode = string.Empty;
        StatusCode_Final_ValidateCode = string.Empty;
        StatusDescription_Final_ValidateCode = string.Empty;
        resultFinal_NotOK = string.Empty;
        /*******************************/
        string WebCall = WebRequestCall(data, ValidateCode_Url_Out, ValidateCode_Method_Out, ValidateCode_ContentType_Out, ValidateCode_BasicAuth, out string resultFinal, out string StatusCodeFinal, out string StatusDescriptionFinal, out string resultFinalBad);
        /*******************************/
        result_Final_ValidateCode = resultFinal;
        StatusCode_Final_ValidateCode = StatusCodeFinal;
        StatusDescription_Final_ValidateCode = StatusDescriptionFinal;
        resultFinal_NotOK = resultFinalBad;
        /*******************************/
        return WebCall;
    }

    public static string ValidateUsername_WebRequestCall(string data, out string result_Final_ValidateUsername, out string StatusCode_Final_ValidateUsername, out string StatusDescription_Final_ValidateUsername, out string resultFinal_NotOK)
    {
        result_Final_ValidateUsername = string.Empty;
        StatusCode_Final_ValidateUsername = string.Empty;
        StatusDescription_Final_ValidateUsername = string.Empty;
        resultFinal_NotOK = string.Empty;
        /*******************************/
        string WebCall = WebRequestCall(data, ValidateUsername_Url_Out, ValidateUsername_Method_Out, ValidateUsername_ContentType_Out, ValidateUsername_BasicAuth, out string resultFinal, out string StatusCodeFinal, out string StatusDescriptionFinal, out string resultFinalBad);
        /*******************************/
        result_Final_ValidateUsername = resultFinal;
        StatusCode_Final_ValidateUsername = StatusCodeFinal;
        StatusDescription_Final_ValidateUsername = StatusDescriptionFinal;
        resultFinal_NotOK = resultFinalBad;
        /*******************************/
        return WebCall;
    }

    public static string ValidateUMCN_WebRequestCall(string data, out string result_Final_ValidateUMCN, out string StatusCode_Final_ValidateUMCN, out string StatusDescription_Final_ValidateUMCN, out string resultFinal_NotOK)
    {
        result_Final_ValidateUMCN = string.Empty;
        StatusCode_Final_ValidateUMCN = string.Empty;
        StatusDescription_Final_ValidateUMCN = string.Empty;
        resultFinal_NotOK = string.Empty;
        /*******************************/
        string WebCall = WebRequestCall(data, ValidateUMCN_Url_Out, ValidateUMCN_Method_Out, ValidateUMCN_ContentType_Out, ValidateUMCN_BasicAuth, out string resultFinal, out string StatusCodeFinal, out string StatusDescriptionFinal, out string resultFinalBad);
        /*******************************/
        result_Final_ValidateUMCN = resultFinal;
        StatusCode_Final_ValidateUMCN = StatusCodeFinal;
        StatusDescription_Final_ValidateUMCN = StatusDescriptionFinal;
        resultFinal_NotOK = resultFinalBad;
        /*******************************/
        return WebCall;
    }

    public static string ExportUserInfoByUsername_WebRequestCall(string data, string username, out string result_Final_ExportUserInfoByUsername, out string StatusCode_Final_ExportUserInfoByUsername, out string StatusDescription_Final_ExportUserInfoByUsername, out string resultFinal_NotOK)
    {
        result_Final_ExportUserInfoByUsername = string.Empty;
        StatusCode_Final_ExportUserInfoByUsername = string.Empty;
        StatusDescription_Final_ExportUserInfoByUsername = string.Empty;
        resultFinal_NotOK = string.Empty;
        /*******************************/
        string WebCall = WebRequestCall(data, (ExportUserInfoByUsername_Url_Out + username), ExportUserInfoByUsername_Method_Out, ExportUserInfoByUsername_ContentType_Out, ExportUserInfoByUsername_BasicAuth, out string resultFinal, out string StatusCodeFinal, out string StatusDescriptionFinal, out string resultFinalBad);
        /*******************************/
        result_Final_ExportUserInfoByUsername = resultFinal;
        StatusCode_Final_ExportUserInfoByUsername = StatusCodeFinal;
        StatusDescription_Final_ExportUserInfoByUsername = StatusDescriptionFinal;
        resultFinal_NotOK = resultFinalBad;
        /*******************************/
        return WebCall;
    }

    public static string SearchUserIDByUMCN_WebRequestCall(string data, string umcn, out string result_Final_SearchUserIDByUMCN, out string StatusCode_Final_SearchUserIDByUMCN, out string StatusDescription_Final_SearchUserIDByUMCN, out string result_Final_NotOK)
    {
        StatusCode_Final_SearchUserIDByUMCN = string.Empty;
        StatusDescription_Final_SearchUserIDByUMCN = string.Empty;
        result_Final_SearchUserIDByUMCN = string.Empty;
        result_Final_NotOK = string.Empty;
        /*******************************/
        string WebCall = WebRequestCall(data, (SearchUserIDByUMCN_Url_Out + umcn), SearchUserIDByUMCN_Method_Out, SearchUserIDByUMCN_ContentType_Out, SearchUserIDByUMCN_BasicAuth, out string resultFinal, out string StatusCodeFinal, out string StatusDescriptionFinal, out string resultFinalBad);
        /*******************************/
        StatusCode_Final_SearchUserIDByUMCN = StatusCodeFinal;
        StatusDescription_Final_SearchUserIDByUMCN = StatusDescriptionFinal;
        result_Final_SearchUserIDByUMCN = resultFinal;
        result_Final_NotOK = resultFinalBad;
        /*******************************/
        return WebCall;
    }

    public static string CreateUsersInBulk_WebRequestCall(string data, out string result_Final_CreateUsersInBulk, out string StatusCode_Final_CreateUsersInBulk, out string StatusDescription_Final_CreateUsersInBulk, out string result_Final_NotOK)
    {
        StatusCode_Final_CreateUsersInBulk = string.Empty;
        StatusDescription_Final_CreateUsersInBulk = string.Empty;
        result_Final_CreateUsersInBulk = string.Empty;
        result_Final_NotOK = string.Empty;
        /*******************************/
        string WebCall = WebRequestCall(data, CreateUsersInBulk_Url_Out, CreateUsersInBulk_Method_Out, CreateUsersInBulk_ContentType_Out, CreateUsersInBulk_BasicAuth, out string resultFinal, out string StatusCodeFinal, out string StatusDescriptionFinal, out string resultFinalBad);
        /*******************************/
        StatusCode_Final_CreateUsersInBulk = StatusCodeFinal;
        StatusDescription_Final_CreateUsersInBulk = StatusDescriptionFinal;
        result_Final_CreateUsersInBulk = resultFinal;
        result_Final_NotOK = resultFinalBad;
        /*******************************/
        return WebCall;
    }


    public static string ExportAuthInfo_WebRequestCall(string data, string username, out string result_Final_ExportAuthInfo, out string StatusCode_Final_ExportAuthInfo, out string StatusDescription_Final_ExportAuthInfo, out string resultFinal_NotOK)
    {
        result_Final_ExportAuthInfo = string.Empty;
        StatusCode_Final_ExportAuthInfo = string.Empty;
        StatusDescription_Final_ExportAuthInfo = string.Empty;
        resultFinal_NotOK = string.Empty;
        /*******************************/
        string WebCall = WebRequestCall(data, (ExportAuthInfo_Url_Out + username), ExportAuthInfo_Method_Out, ExportAuthInfo_ContentType_Out, ExportAuthInfo_BasicAuth, out string resultFinal, out string StatusCodeFinal, out string StatusDescriptionFinal, out string resultFinalBad);
        /*******************************/
        result_Final_ExportAuthInfo = resultFinal;
        StatusCode_Final_ExportAuthInfo = StatusCodeFinal;
        StatusDescription_Final_ExportAuthInfo = StatusDescriptionFinal;
        resultFinal_NotOK = resultFinalBad;
        /*******************************/
        return WebCall;
    }

    public static string WebRequestCall(string data, string apiUrl, string apiMethod, string apiContentType, string apiAuth, out string resultFinal, out string StatusCodeFinal, out string StatusDescriptionFinal, out string result_Final_NotOK)
    {
        StatusCodeFinal = string.Empty;
        StatusDescriptionFinal = string.Empty;
        result_Final_NotOK = string.Empty;
        string result = string.Empty;
        resultFinal = string.Empty;
        try
        {
            log.Info("Start getting result ");
            /////Uvedeno zbog greske:the request was aborted could not create ssl/tls secure channel.
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(AcceptAllCertifications);

            var httpWebRequest = (HttpWebRequest)WebRequest.Create(apiUrl);
            httpWebRequest.Timeout = 300000; //300sec
            httpWebRequest.ContentType = apiContentType;
            httpWebRequest.Method = apiMethod;

            /////Uvedeno zbog greske:the request was aborted could not create ssl/tls secure channel.
            httpWebRequest.ProtocolVersion = HttpVersion.Version10;
            httpWebRequest.PreAuthenticate = true;

            if (apiAuth != string.Empty)
            {
                httpWebRequest.Headers["Authorization"] = "Basic " + Convert.ToBase64String(Encoding.Default.GetBytes(apiAuth));
            }
            
            if (data != string.Empty)
            { 
                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    streamWriter.Write(data);
                }
            }

            try
            {
                log.Info("Before getting response. " + DateTime.Now.ToString("yyyy MM dd HH:mm:ss:FFF"));
                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                log.Info("After getting response. " + DateTime.Now.ToString("yyyy MM dd HH:mm:ss:FFF"));
                GetStatusAndDescriptionCode(httpResponse, out string StatusCode, out string StatusDescription);
                StatusCodeFinal = StatusCode;
                StatusDescriptionFinal = StatusDescription;
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var res = streamReader.ReadToEnd();
                    resultFinal = res;
                    log.Info("Result is " + res);
                }
            }
            catch (WebException ex)
            {
                log.Info("Web exception happened: " + ex.Message);
                using (WebResponse response = ex.Response)
                { 
                    HttpWebResponse httpResponse1 = (HttpWebResponse)response;
                    GetStatusAndDescriptionCode(httpResponse1, out string StatusCode, out string StatusDescription);
                    StatusCodeFinal = StatusCode;
                    StatusDescriptionFinal = StatusDescription;
                    using (var stream = ex.Response.GetResponseStream())
                    using (var reader = new StreamReader(stream))
                    {
                        result = reader.ReadToEnd();
                        result_Final_NotOK = result;
                        log.Info("Web exception message: " + result);
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error("Error getting result: " + ex.Message + " ||| " + ex.InnerException + " ||| " + ex.StackTrace);
                //throw new Exception("Error getting result: " + ex.Message + " ||| " + ex.InnerException + " ||| " + ex.StackTrace);
            }
        }
        catch (Exception ex)
        {
            log.Error("Error in function WebRequestCall. " + ex.Message);
            //throw new Exception("Error in function WebRequestCall. " + ex.Message);
        }
        return result;
    }


    public static bool AcceptAllCertifications(object sender, System.Security.Cryptography.X509Certificates.X509Certificate certification, System.Security.Cryptography.X509Certificates.X509Chain chain, System.Net.Security.SslPolicyErrors sslPolicyErrors)
    {
        return true;
    }

    public static void GetStatusAndDescriptionCode(HttpWebResponse httpResponse, out string StatusCodeFinal, out string StatusDescriptionFinal)
    {
        StatusCodeFinal = string.Empty;
        int StatusCode = Convert.ToInt32(httpResponse.StatusCode);
        StatusCodeFinal = StatusCode.ToString();

        StatusDescriptionFinal = string.Empty;
        string StatusDecription = httpResponse.StatusDescription;
        StatusDescriptionFinal = StatusDecription;
        log.Info("Status code is " + StatusCode);
        log.Info("Status desctiption is " + StatusDecription);
    }

    //public static string GetRegisterUserJson(string item_Username, string item_Realm, string item_Password, string item_Externalid, string item_Givenname, string item_Lastname, string item_Emailaddress, string item_DOB, string item_Placeofbirth, string item_Gender, string item_Streetaddress, string item_City, string item_Postalcode, string item_Country)
    //{
    //    return "{\"user\":{\"username\":\"" + item_Username + "\"," + "\"realm\":\"" + item_Realm + "\"," + "\"password\":\"" + item_Password + "\"," + "\"claims\":[{\"uri\":\"" + string.Empty + "\"," + "\"value\":\"" + item_Externalid + "\"}," + "{\"uri\":\"" + string.Empty + "\"," + "\"value\":\"" + item_Givenname + "\"}," + "{\"uri\":\"" + string.Empty + "\"," + "\"value\":\"" + item_Lastname + "\"}," + "{\"uri\":\"" + string.Empty + "\"," + "\"value\":\"" + item_Emailaddress + "\"}," + "{\"uri\":\"" + string.Empty + "\"," + "\"value\":\"" + item_DOB + "\"}," + "{\"uri\":\"" + string.Empty + "\"," + "\"value\":\"" + item_Placeofbirth + "\"}," + "{\"uri\":\"" + string.Empty + "\"," + "\"value\":\"" + item_Gender + "\"}," + "{\"uri\":\"" + string.Empty + "\"," + "\"value\":\"" + item_Streetaddress + "\"}," + "{\"uri\":\"" + string.Empty + "\"," + "\"value\":\"" + item_City + "\"}," + "{\"uri\":\"" + string.Empty + "\"," + "\"value\":\"" + item_Postalcode + "\"}," + "{\"uri\":\"" + string.Empty + "\"," + "\"value\":\"" + item_Country + "\"}]}," + "\"properties\":[]," + "\"DuplicateHandling\":\"Merge\"," + "\"AuthenticationMethod\":\"PasswordAuthentication\"}";
    //}

    /*VALIDATION*/
    public static bool ValidateDropDown(string SelectedValue, string IDItem, string ddlName, out string ErrorMessage)
    {
        bool returnValue = true;
        ErrorMessage = string.Empty;

        if (SelectedValue == IDItem)
        {
            ErrorMessage = ddlName + " is required field. ";
            returnValue = false;
        }
        else
        {
            returnValue = true;
        }

        return returnValue;
    }


    public static string ParseJsonOneValue(string jsonResponse, string requestText)
    {
        string res = string.Empty;

        // Parse your Result to an Array
        var x = JObject.Parse(jsonResponse);
        var res1 = x[requestText];
        res = res1.ToString();

        return res;
    }

    public static string ParseJsonTwoValues(string jsonResponse, string requestFirstText, string requestSecondText)
    {
        string res = string.Empty;

        // Parse your Result to an Array
        var x = JObject.Parse(jsonResponse);
        var res1 = x[requestFirstText][requestSecondText];
        res = res1.ToString();

        return res;
    }
}
