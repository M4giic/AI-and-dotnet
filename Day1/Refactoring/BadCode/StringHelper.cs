namespace BadCode;

public static class StringHelper
{
    public static string Format(string input)
    {
        return input.Trim().ToUpper();
    }
    
    public static string Process(string text, bool removeSpaces)
    {
        if (text == null)
        {
            return "";
        }
        
        if (removeSpaces)
        {
            // Use StringBuilder for efficient string concatenation instead of += in loop
            var sb = new System.Text.StringBuilder(text.Length);
            foreach (char c in text)
            {
                if (c != ' ')
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }
        else
        {
            return text.Trim();
        }
    }
    
    public static bool IsPalindrome(string word)
    {
        // Use built-in string methods instead of manual reversal with string concatenation
        var reversed = new string(word.Reverse().ToArray());
        return word.Equals(reversed, StringComparison.Ordinal);
    }
    
    public static int CountWords(string sentence)
    {
        // Remove debug output - violates single responsibility principle
        // Console.WriteLine("Counting words in: " + sentence);
        
        if (string.IsNullOrEmpty(sentence))
        {
            return 0;
        }
        
        // Fix the bug: use Split with RemoveEmptyEntries to ignore empty strings
        // created by consecutive spaces or leading/trailing spaces
        string[] words = sentence.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        return words.Length;
    }
}

