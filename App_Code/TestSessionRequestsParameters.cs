using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for TestSessionRequestsParameters
/// </summary>
public class TestSessionRequestsParameters
{
    public int TestCombinationId { get; set; }
    public int TestSessionId { get; set; }
    public string RequestData { get; set; }
    public string BeforeStep { get; set; }
    public string AfterStep { get; set; }

    public TestSessionRequestsParameters(int testCombinationId, int testSessionId, string requestdata, string beforestep, string afterstep)
    {
        TestCombinationId = testCombinationId;
        TestSessionId = testSessionId;
        RequestData = requestdata;
        BeforeStep = beforestep;
        AfterStep = afterstep;
    }
}