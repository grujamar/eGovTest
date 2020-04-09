using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for User
/// </summary>
public class User
{
    public string UserId { get; set; }
    public string Username { get; set; }
    public bool IsDeleted { get; set; }

    public User(string userid, string username, bool isdeleted)
    {
        UserId = userid;
        Username = username;
        IsDeleted = isdeleted;
    }
}