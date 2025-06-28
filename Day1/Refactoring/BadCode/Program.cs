// See https://aka.ms/new-console-template for more information

using BadCode;

Console.WriteLine("=== Testing StringHelper Methods ===\n");

// Test Format method
Console.WriteLine("1. Testing Format method:");
string testInput = "  hello world  ";
string formatted = StringHelper.Format(testInput);
Console.WriteLine($"Input: '{testInput}' -> Output: '{formatted}'\n");

// Test Process method with removeSpaces = true
Console.WriteLine("2. Testing Process method (remove spaces):");
string testText = "Hello World Test";
string processedNoSpaces = StringHelper.Process(testText, true);
Console.WriteLine($"Input: '{testText}' -> Output: '{processedNoSpaces}'");

// Test Process method with removeSpaces = false
string processedTrimmed = StringHelper.Process("  " + testText + "  ", false);
Console.WriteLine($"Input: '  {testText}  ' -> Output: '{processedTrimmed}'\n");

// Test Process method with null input
string processedNull = StringHelper.Process(null, true);
Console.WriteLine($"Input: null -> Output: '{processedNull}'\n");

// Test IsPalindrome method
Console.WriteLine("3. Testing IsPalindrome method:");
string[] palindromeTests = { "racecar", "hello", "madam", "test", "level" };
foreach (string word in palindromeTests)
{
    bool isPalindrome = StringHelper.IsPalindrome(word);
    Console.WriteLine($"'{word}' is palindrome: {isPalindrome}");
}
Console.WriteLine();

// Test CountWords method
Console.WriteLine("4. Testing CountWords method:");
string[] sentences = { 
    "Hello world", 
    "This is a test sentence", 
    "", 
    "OneWord",
    "Multiple   spaces   between   words"
};

foreach (string sentence in sentences)
{
    int wordCount = StringHelper.CountWords(sentence);
    Console.WriteLine($"Sentence: '{sentence}' -> Word count: {wordCount}");
}

