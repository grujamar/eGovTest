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

public static class ApiUtils
{
    public static string SettingsFile { get; private set; }
    public static string RegisterUser_Url { get; set; }
    public static string RegisterUser_ContentType { get; set; }
    public static string RegisterUser_Method { get; set; }

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
                if (navigator.Name == "RegisterUser")
                {
                    do
                    {
                        navigator.MoveToFirstChild();
                        if (navigator.Name == "url")
                        {
                            RegisterUser_Url = navigator.Value;
                        }
                        navigator.MoveToFollowing(XPathNodeType.Element);
                        if (navigator.Name == "contentType")
                        {
                            RegisterUser_ContentType = navigator.Value;
                        }
                        navigator.MoveToFollowing(XPathNodeType.Element);
                        if (navigator.Name == "method")
                        {
                            RegisterUser_Method = navigator.Value;
                        }
                        log.Info("Get parameters from settings file : URL - " + RegisterUser_Url + " . Content Type - " + RegisterUser_ContentType + " . Method - " + RegisterUser_Method);
                        navigator.MoveToFollowing(XPathNodeType.Element);

                        navigator.MoveToParent();

                    } while (navigator.MoveToNext());
                }
            }
        }
        catch (Exception ex)
        {
            log.Error("Error while reading configuration data.. " + ex.Message);
        }
    }


    public static string RegisterUserWebRequestCall(string data)
    {
        return WebRequestCall(data, RegisterUser_Url, RegisterUser_Method, RegisterUser_ContentType);
    }


    public static string WebRequestCall(string data, string apiUrl, string apiMethod, string apiContentType)
    {
        string result = string.Empty;
        try
        {
            log.Info("Start getting result ");
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(apiUrl);
            httpWebRequest.ContentType = apiContentType;
            httpWebRequest.Method = apiMethod;

            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                log.Info("Json data for Register user " + data);
                streamWriter.Write(data);
            }

            try
            {
                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                GetStatusAndDescriptionCode(httpResponse);
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var res = streamReader.ReadToEnd();
                    log.Info("Result is " + res);
                }
            }
            catch (WebException ex)
            {

                log.Info("Web exception happened: " + ex.Message);
                using (WebResponse response = ex.Response)
                { 
                    HttpWebResponse httpResponse1 = (HttpWebResponse)response;
                    GetStatusAndDescriptionCode(httpResponse1);

                    using (var stream = ex.Response.GetResponseStream())
                    using (var reader = new StreamReader(stream))
                    {
                        result = reader.ReadToEnd();
                        log.Info("Web exception message: " + result);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error getting result: " + ex.Message + " ||| " + ex.InnerException + " ||| " + ex.StackTrace);
            }
        }
        catch (Exception ex)
        {
            log.Error("Error in function WebRequestCall. " + ex.Message);
            throw new Exception("Error in function WebRequestCall. " + ex.Message);
        }
        return result;
    }

    public static void GetStatusAndDescriptionCode(HttpWebResponse httpResponse)
    {
        int StatusCode = Convert.ToInt32(httpResponse.StatusCode);
        string StatusDecription = httpResponse.StatusDescription;
        log.Info("Status code is " + StatusCode);
        log.Info("Status desctiption is " + StatusDecription);
    }

    public static string GetRegisterUserJson(string item_Username, string item_Realm, string item_Password, string item_Externalid, string item_Givenname, string item_Lastname, string item_Emailaddress, string item_DOB, string item_Placeofbirth, string item_Gender, string item_Streetaddress, string item_City, string item_Postalcode, string item_Country)
    {
        return "{\"user\":{\"username\":\"" + item_Username + "\"," + "\"realm\":\"" + item_Realm + "\"," + "\"password\":\"" + item_Password + "\"," + "\"claims\":[{\"uri\":\"" + ConstantsProject.CLAIMS_URI_EXTERNALID + "\"," + "\"value\":\"" + item_Externalid + "\"}," + "{\"uri\":\"" + ConstantsProject.CLAIMS_URI_GIVENNAME + "\"," + "\"value\":\"" + item_Givenname + "\"}," + "{\"uri\":\"" + ConstantsProject.CLAIMS_URI_LASTNAME + "\"," + "\"value\":\"" + item_Lastname + "\"}," + "{\"uri\":\"" + ConstantsProject.CLAIMS_URI_EMAILADDRESS + "\"," + "\"value\":\"" + item_Emailaddress + "\"}," + "{\"uri\":\"" + ConstantsProject.CLAIMS_URI_DOB + "\"," + "\"value\":\"" + item_DOB + "\"}," + "{\"uri\":\"" + ConstantsProject.CLAIMS_URI_PLACEOFBIRTH + "\"," + "\"value\":\"" + item_Placeofbirth + "\"}," + "{\"uri\":\"" + ConstantsProject.CLAIMS_URI_GENDER + "\"," + "\"value\":\"" + item_Gender + "\"}," + "{\"uri\":\"" + ConstantsProject.CLAIMS_URI_STREETADDRESS + "\"," + "\"value\":\"" + item_Streetaddress + "\"}," + "{\"uri\":\"" + ConstantsProject.CLAIMS_URI_CITY + "\"," + "\"value\":\"" + item_City + "\"}," + "{\"uri\":\"" + ConstantsProject.CLAIMS_URI_POSTALCODE + "\"," + "\"value\":\"" + item_Postalcode + "\"}," + "{\"uri\":\"" + ConstantsProject.CLAIMS_URI_COUNTRY + "\"," + "\"value\":\"" + item_Country + "\"}]}," + "\"properties\":[]," + "\"DuplicateHandling\":\"Merge\"," + "\"AuthenticationMethod\":\"PasswordAuthentication\"}";
    }




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
}
