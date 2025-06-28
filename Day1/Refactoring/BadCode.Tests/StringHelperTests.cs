using Xunit;
using BadCode;

namespace BadCode.Tests;

public class StringHelperTests
{
    #region Format Method Tests
    
    [Fact]
    public void Format_WithLeadingAndTrailingSpaces_ShouldTrimAndConvertToUpperCase()
    {
        // Arrange
        string input = "  hello world  ";
        
        // Act
        string result = StringHelper.Format(input);
        
        // Assert
        Assert.Equal("HELLO WORLD", result);
    }
    
    [Fact]
    public void Format_WithMixedCase_ShouldConvertToUpperCase()
    {
        // Arrange
        string input = "Hello World";
        
        // Act
        string result = StringHelper.Format(input);
        
        // Assert
        Assert.Equal("HELLO WORLD", result);
    }
    
    [Fact]
    public void Format_WithEmptyString_ShouldReturnEmptyString()
    {
        // Arrange
        string input = "";
        
        // Act
        string result = StringHelper.Format(input);
        
        // Assert
        Assert.Equal("", result);
    }
    
    [Fact]
    public void Format_WithOnlySpaces_ShouldReturnEmptyString()
    {
        // Arrange
        string input = "   ";
        
        // Act
        string result = StringHelper.Format(input);
        
        // Assert
        Assert.Equal("", result);
    }
    
    #endregion
    
    #region Process Method Tests
    
    [Fact]
    public void Process_WithRemoveSpacesTrue_ShouldRemoveAllSpaces()
    {
        // Arrange
        string input = "Hello World Test";
        
        // Act
        string result = StringHelper.Process(input, true);
        
        // Assert
        Assert.Equal("HelloWorldTest", result);
    }
    
    [Fact]
    public void Process_WithRemoveSpacesFalse_ShouldOnlyTrim()
    {
        // Arrange
        string input = "  Hello World Test  ";
        
        // Act
        string result = StringHelper.Process(input, false);
        
        // Assert
        Assert.Equal("Hello World Test", result);
    }
    
    [Fact]
    public void Process_WithNullInput_ShouldReturnEmptyString()
    {
        // Arrange
        string? input = null;
        
        // Act
        string result = StringHelper.Process(input, true);
        
        // Assert
        Assert.Equal("", result);
    }
    
    [Fact]
    public void Process_WithMultipleConsecutiveSpaces_RemoveSpacesTrue_ShouldRemoveAllSpaces()
    {
        // Arrange
        string input = "Hello   World   Test";
        
        // Act
        string result = StringHelper.Process(input, true);
        
        // Assert
        Assert.Equal("HelloWorldTest", result);
    }
    
    #endregion
    
    #region IsPalindrome Method Tests - Sunny Day Scenarios
    
    [Theory]
    [InlineData("racecar", true)]
    [InlineData("madam", true)]
    [InlineData("level", true)]
    [InlineData("noon", true)]
    [InlineData("radar", true)]
    [InlineData("a", true)]
    public void IsPalindrome_WithValidPalindromes_ShouldReturnTrue(string word, bool expected)
    {
        // Act
        bool result = StringHelper.IsPalindrome(word);
        
        // Assert
        Assert.Equal(expected, result);
    }
    
    [Theory]
    [InlineData("hello", false)]
    [InlineData("world", false)]
    [InlineData("test", false)]
    [InlineData("programming", false)]
    public void IsPalindrome_WithNonPalindromes_ShouldReturnFalse(string word, bool expected)
    {
        // Act
        bool result = StringHelper.IsPalindrome(word);
        
        // Assert
        Assert.Equal(expected, result);
    }
    
    [Fact]
    public void IsPalindrome_WithEmptyString_ShouldReturnTrue()
    {
        // Arrange
        string input = "";
        
        // Act
        bool result = StringHelper.IsPalindrome(input);
        
        // Assert
        Assert.True(result); // Empty string is technically a palindrome
    }
    
    #endregion
    
    #region CountWords Method Tests - Sunny Day Scenarios
    
    [Fact]
    public void CountWords_WithSimpleSentence_ShouldReturnCorrectCount()
    {
        // Arrange
        string sentence = "Hello world";
        
        // Act
        int result = StringHelper.CountWords(sentence);
        
        // Assert
        Assert.Equal(2, result);
    }
    
    [Fact]
    public void CountWords_WithSingleWord_ShouldReturnOne()
    {
        // Arrange
        string sentence = "Hello";
        
        // Act
        int result = StringHelper.CountWords(sentence);
        
        // Assert
        Assert.Equal(1, result);
    }
    
    [Fact]
    public void CountWords_WithEmptyString_ShouldReturnZero()
    {
        // Arrange
        string sentence = "";
        
        // Act
        int result = StringHelper.CountWords(sentence);
        
        // Assert
        Assert.Equal(0, result);
    }
    
    [Fact]
    public void CountWords_WithNullString_ShouldReturnZero()
    {
        // Arrange
        string? sentence = null;
        
        // Act
        int result = StringHelper.CountWords(sentence);
        
        // Assert
        Assert.Equal(0, result);
    }
    
    #endregion
    
    #region CountWords Method Tests - Failing Cases (Issues)
    
    [Fact]
    public void CountWords_WithMultipleSpaces_ShouldReturnCorrectCount()
    {
        // Arrange
        string sentence = "Multiple   spaces   between   words";
        int expectedCorrectCount = 4; // Should be 4 words: "Multiple", "spaces", "between", "words"
        
        // Act
        int actualResult = StringHelper.CountWords(sentence);
        
        // Assert - This test SHOULD pass but will FAIL due to the bug
        Assert.Equal(expectedCorrectCount, actualResult);
    }
    
    [Fact]
    public void CountWords_WithLeadingAndTrailingSpaces_ShouldReturnCorrectCount()
    {
        // Arrange
        string sentence = "  hello world  ";
        int expectedCorrectCount = 2; // Should be 2 words: "hello", "world"
        
        // Act
        int actualResult = StringHelper.CountWords(sentence);
        
        // Assert - This test SHOULD pass but will FAIL due to the bug
        Assert.Equal(expectedCorrectCount, actualResult);
    }
    
    [Fact]
    public void CountWords_WithOnlySpaces_ShouldReturnZero()
    {
        // Arrange
        string sentence = "   ";
        int expectedCorrectCount = 0; // Should be 0 words
        
        // Act
        int actualResult = StringHelper.CountWords(sentence);
        
        // Assert - This test SHOULD pass but will FAIL due to the bug
        Assert.Equal(expectedCorrectCount, actualResult);
    }
    
    #endregion
}
