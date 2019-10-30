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

    public TestSessionRequestsParameters(int testCombinationId, int testSessionId, string requestdata)
    {
        TestCombinationId = testCombinationId;
        TestSessionId = testSessionId;
        RequestData = requestdata;
    }
}