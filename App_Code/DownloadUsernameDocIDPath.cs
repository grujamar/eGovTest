using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for DownloadUsernameDocIDPath
/// </summary>
public class DownloadUsernameDocID
{
    public string Username { get; set; }
    public string DocumentId { get; set; }

    public DownloadUsernameDocID(string username, string documentid)
    {
        Username = username;
        DocumentId = documentid;
    }
}