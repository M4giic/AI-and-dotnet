using System;
using System.Text.Json;

namespace Tools.Agent;

public static class JsonHelper
{
    // Helper to extract JSON from text that might have additional content
    public static string ExtractJson(string content)
    {
        // Try to find JSON content between curly braces or square brackets
        int startIndex = content.IndexOf('[');
        if (startIndex < 0)
        {
            startIndex = content.IndexOf('{');
        }
        
        if (startIndex < 0)
        {
            throw new JsonException("Could not find start of JSON content");
        }
        
        int endIndex;
        if (content[startIndex] == '[')
        {
            endIndex = FindMatchingClosingBracket(content, startIndex, '[', ']');
        }
        else
        {
            endIndex = FindMatchingClosingBracket(content, startIndex, '{', '}');
        }
        
        if (endIndex < 0)
        {
            throw new JsonException("Could not find end of JSON content");
        }
        
        return content.Substring(startIndex, endIndex - startIndex + 1);
    }
    
    public static int FindMatchingClosingBracket(string text, int openPosition, char openChar, char closeChar)
    {
        int depth = 1;
        for (int i = openPosition + 1; i < text.Length; i++)
        {
            if (text[i] == openChar)
            {
                depth++;
            }
            else if (text[i] == closeChar)
            {
                depth--;
                if (depth == 0)
                {
                    return i;
                }
            }
        }
        return -1; // No matching bracket found
    }
}
