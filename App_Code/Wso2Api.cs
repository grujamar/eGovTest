using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Web;
using System.Xml;
using System.Xml.XPath;

/// <summary>
/// Summary description for Wso2Client
/// </summary>
public static class Wso2Api
{
    private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
    public static string SettingsFileCertificate { get; private set; }
    public static string SerialNumber { get; set; }
    public static string usingCertificate { get; set; }
    public static string TestSecureConn_Url { get; set; }
    public static string TestSecureConn_Method { get; set; }

    static Wso2Api()
    {
        SettingsFileCertificate = AppDomain.CurrentDomain.BaseDirectory + "CertificateSettings.xml";
        initializeSetup();
    }

    public static void initializeSetup()
    {
        getCertificateSettings();
    }

    public static void getCertificateSettings()
    {
        try
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(SettingsFileCertificate);
            XPathNavigator navigator = xmlDoc.CreateNavigator();

            navigator.MoveToRoot();
            navigator.MoveToFollowing(System.Xml.XPath.XPathNodeType.Element);//settings

            if (navigator.HasChildren)
            {
                navigator.MoveToFirstChild();//<Certificate>

                do
                {
                    if (navigator.Name == "Certificate")
                    {
                        LoopingThrowCertificateChild(navigator, out string SerialNumber_Out_Final, out string usingCertificate_Out_Final, out string TestSecureConn_Url_Out_Final, out string TestSecureConn_Method_Out_Final);
                        SerialNumber = SerialNumber_Out_Final;
                        usingCertificate = usingCertificate_Out_Final;
                        TestSecureConn_Url = TestSecureConn_Url_Out_Final;
                        TestSecureConn_Method = TestSecureConn_Method_Out_Final;
                        navigator.MoveToFollowing(XPathNodeType.Element);
                    }
                } while (navigator.MoveToNext());
            }
        }
        catch (Exception ex)
        {
            log.Error("Error while reading configuration data from CertificateFile. " + ex.Message);
        }
    }

    public static void LoopingThrowCertificateChild(XPathNavigator navigator, out string SerialNumber_Out, out string usingCertificate_Out, out string TestSecureConn_Url, out string TestSecureConn_Method)
    {
        SerialNumber_Out = string.Empty;
        usingCertificate_Out = string.Empty;
        TestSecureConn_Url = string.Empty;
        TestSecureConn_Method = string.Empty;
        do
        {
            navigator.MoveToFirstChild();
            if (navigator.Name == "serialnumber")
            {
                SerialNumber_Out = navigator.Value;
            }
            navigator.MoveToFollowing(XPathNodeType.Element);
            if (navigator.Name == "usingCertificate")
            {
                usingCertificate_Out = navigator.Value;
            }
            navigator.MoveToFollowing(XPathNodeType.Element);
            if (navigator.Name == "url")
            {
                TestSecureConn_Url = navigator.Value;
            }
            navigator.MoveToFollowing(XPathNodeType.Element);
            if (navigator.Name == "method")
            {
                TestSecureConn_Method = navigator.Value;
            }
            log.Info("Get certificate from settings file : SerialNumber - " + SerialNumber_Out + " usingCertificate - " + usingCertificate_Out + " TestSecureConn_Url - " + TestSecureConn_Url + " TestSecureConn_Method - " + TestSecureConn_Method);
            navigator.MoveToFollowing(XPathNodeType.Element);

            navigator.MoveToParent();

        } while (navigator.MoveToNext());
    }


    public static string WebRequestCall(bool isDocumentsMethod, byte[] formData, string data, string apiUrl, string apiMethod, string apiContentType, string apiAuth, out string resultFinal, out string StatusCodeFinal, out string StatusDescriptionFinal, out string result_Final_NotOK)
    {
        string result = string.Empty;
        StatusCodeFinal = string.Empty;
        StatusDescriptionFinal = string.Empty;
        result_Final_NotOK = string.Empty;
        resultFinal = string.Empty;
        try
        {
            log.Info("Start getting result ");
            /////Uvedeno zbog greske:the request was aborted could not create ssl/tls secure channel.
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(AcceptAllCertifications);

            /****httpWebRequest****/
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(apiUrl);
            httpWebRequest.Timeout = 18000000; //18.000sec = 5hours
            httpWebRequest.ContentType = apiContentType;
            httpWebRequest.Method = apiMethod;

            /////Uvedeno zbog greske:the request was aborted could not create ssl/tls secure channel.
            httpWebRequest.ProtocolVersion = HttpVersion.Version10;
            httpWebRequest.PreAuthenticate = true;

            if (usingCertificate == "1")
            {
                httpWebRequest = (HttpWebRequest)WebRequest.Create(TestSecureConn_Url);
                httpWebRequest.Method = TestSecureConn_Method;
                List<X509Certificate2> certificates = GetCurrentUserCertificates();
                log.Info("List<X509Certificate2> certificates count is " + certificates.Count);

                if (certificates.Count == 0)
                {
                    throw new Exception("No certificates found.");
                }

                foreach (X509Certificate2 certificate in certificates)
                {
                    if (certificate.SerialNumber.Equals(SerialNumber, StringComparison.InvariantCultureIgnoreCase))
                    {
                        httpWebRequest.ClientCertificates.Add(certificate);
                        log.Info("Get certificate from list. Certificate is: " + certificate.SerialNumber);
                        break;
                    }
                }

                if (httpWebRequest.ClientCertificates.Count == 0)
                {
                    throw new Exception("No certificate with wanted serial number found.");
                }
            }

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

            string userAgent = "Someone";

            if (isDocumentsMethod & apiMethod == ConstantsProject.DOCUMENTS_POST_METHOD)
            {
                httpWebRequest.UserAgent = userAgent;
                httpWebRequest.CookieContainer = new CookieContainer();
                httpWebRequest.ContentLength = formData.Length;

                // Send the form data to the request.
                using (Stream requestStream = httpWebRequest.GetRequestStream())
                {
                    requestStream.Write(formData, 0, formData.Length);
                    requestStream.Close();
                }
            }

            try
            {
                log.Info("Before getting response. " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:FFF"));
                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                log.Info("After getting response. " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:FFF"));
                GetStatusAndDescriptionCode(httpResponse, out string StatusCode, out string StatusDescription);
                StatusCodeFinal = StatusCode;
                StatusDescriptionFinal = StatusDescription;
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var res = streamReader.ReadToEnd();
                    resultFinal = res;
                    //log.Info("Result is " + res);
                    /////////////////DOWNLOAD FILES/////////////////
                    if (isDocumentsMethod & apiMethod == ConstantsProject.DOCUMENTS_GET_METHOD)
                    {
                        byte[] bytes = Encoding.ASCII.GetBytes(res);
                        string hrefDocumentsDownloadFinal = System.Configuration.ConfigurationManager.AppSettings["hrefDocumentsDownloadFinal"].ToString();
                        string fileName = (httpResponse.Headers["Content-Disposition"]).ToString();
                        log.Info("filename - " + fileName);
                        string second = fileName.Split(';').Skip(1).FirstOrDefault();
                        string fileNameFinal = After(second, "=");
                        log.Info("fileNameFinal - " + fileNameFinal);
                        File.WriteAllBytes(hrefDocumentsDownloadFinal + fileNameFinal, bytes);
                    }
                    else
                    {
                        log.Info("Result is " + res);
                    }
                    ///////////////////////////////////////////////
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


    public static string After(this string value, string a)
    {
        int posA = value.LastIndexOf(a);
        if (posA == -1)
        {
            return "";
        }
        int adjustedPosA = posA + a.Length;
        if (adjustedPosA >= value.Length)
        {
            return "";
        }
        return value.Substring(adjustedPosA);
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

    public static List<X509Certificate2> GetCurrentUserCertificates()
    {
        List<X509Certificate2> certificates = new List<X509Certificate2>();
        X509Store store = new X509Store(StoreName.My, StoreLocation.LocalMachine);
        store.Open(OpenFlags.ReadOnly);
        log.Info("store.Name is " + store.Name);
        log.Info("store.Certificates.Count is " + store.Certificates.Count);
        foreach (X509Certificate2 certificate in store.Certificates)
        {
            certificates.Add(certificate);
            log.Info("Certificates from store: " + certificate);
        }
        store.Close();
        return certificates;
    }
}