using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for TestCombinationParameter
/// </summary>
public class TestCombinationParameter
{
    public int ParameterOrdinalNumber { get; set; }
    public string ParameterName { get; set; }
    public string ParameterValue { get; set; }

    public TestCombinationParameter(int parameterOrdinalNumber, string parameterName, string parameterValue)
    {
        ParameterOrdinalNumber = parameterOrdinalNumber;
        ParameterName = parameterName ?? throw new ArgumentNullException(nameof(parameterName));
        ParameterValue = parameterValue ?? throw new ArgumentNullException(nameof(parameterValue));
    }
}