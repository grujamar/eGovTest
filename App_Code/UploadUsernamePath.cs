using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for UploadUsernamePath
/// </summary>
public class UploadUsernamePath
{
    public string Username { get; set; }
    public string Path { get; set; }

    public UploadUsernamePath(string username, string path)
    {
        Username = username;
        Path = path;
    }
}