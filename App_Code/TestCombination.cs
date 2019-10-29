using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for TestCombination
/// </summary>
public class TestCombination
{
    public int OrdinalNumber { get; set; }
    public bool ExpectedOutcome { get; set; }
    public List<TestCombinationParameter> ParameterList { get; set; }

    public TestCombination(int ordinalNumber, bool expectedOutcome, List<TestCombinationParameter> parameterList)
    {
        OrdinalNumber = ordinalNumber;
        ExpectedOutcome = expectedOutcome;
        ParameterList = parameterList ?? throw new ArgumentNullException(nameof(parameterList));
    }
}