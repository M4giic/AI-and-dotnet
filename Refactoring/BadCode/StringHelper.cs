namespace BadCode;

public static class StringHelper
{
    public static string Format(string input)
    {
        string result = input.Trim();
        
        result = result.ToUpper();
        
        return result;
    }
    
    public static string Process(string text, bool removeSpaces)
    {
        if (text == null)
        {
            return "";
        }
        
        if (removeSpaces)
        {
            string noSpaces = "";
            for (int i = 0; i < text.Length; i++)
            {
                if (text[i] != ' ')
                {
                    noSpaces += text[i];
                }
            }
            return noSpaces;
        }
        else
        {
            return text.Trim();
        }
    }
    
    public static bool IsPalindrome(string word)
    {
        string reversed = "";
        
        for (int i = word.Length - 1; i >= 0; i--)
        {
            reversed += word[i];
        }
        
        if (word == reversed)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    
    public static int CountWords(string sentence)
    {
        Console.WriteLine("Counting words in: " + sentence);
        
        if (string.IsNullOrEmpty(sentence))
        {
            return 0;
        }
        
        string[] words = sentence.Split(' ');
        return words.Length;
    }
}