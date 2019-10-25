using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for RegisterUserTest
/// </summary>

public class RegisterUserTest
{
    public Int64 RowNumber { get; set; }
    public int TestSessionId { get; set; }
    public string Username { get; set; }
    public string Realm { get; set; }
    public string Password { get; set; }
    public string Externalid { get; set; }
    public string Givenname { get; set; }
    public string Lastname { get; set; }
    public string Emailaddress { get; set; }
    public string DOB { get; set; }
    public string Placeofbirth { get; set; }
    public string Gender { get; set; }
    public string Streetaddress { get; set; }
    public string City { get; set; }
    public string Postalcode { get; set; }
    public string Country { get; set; }

    public RegisterUserTest(Int64 rownnumber, int testsessionid, string username, string realm, string password, string externalid, string givenname, string lastname, 
        string emailaddress, string dob, string placeofbirth , string gender, string streetaddress, string city, string postalcode, string country)
    {
        RowNumber = rownnumber;
        TestSessionId = testsessionid;
        Username = username;
        Realm = realm;
        Password = password;
        Externalid = externalid;
        Givenname = givenname;
        Lastname = lastname;
        Emailaddress = emailaddress;
        DOB = dob;
        Placeofbirth = placeofbirth;
        Gender = gender;
        Streetaddress = streetaddress;
        City = city;
        Postalcode = postalcode;
        Country = country;
    }
}
