using System;
using System.Collections.Generic;

public class BooleanParser
{
    public List<string> Parse(List<string> tokens, out List<(string Operator, string Token)> booleanExpressions)
    {
        booleanExpressions = new List<(string Operator, string Token)>();
        var finalTokens = new List<string>();

        string currentOperator = "AND"; // Default operator

        foreach (var token in tokens)
        {
            if (token.Equals("AND", StringComparison.OrdinalIgnoreCase) ||
                token.Equals("OR", StringComparison.OrdinalIgnoreCase) ||
                token.Equals("NOT", StringComparison.OrdinalIgnoreCase))
            {
                currentOperator = token.ToUpper();
            }
            else
            {
                booleanExpressions.Add((currentOperator, token));
                finalTokens.Add(token);
            }
        }

        return finalTokens;
    }
}