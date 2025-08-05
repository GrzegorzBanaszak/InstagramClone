using InstagramClone.Domain.Users.ValueObjects;

namespace InstagramClone.Tests.Domain.Users
{
    public class UserNameTests
    {
        [Theory]
        [InlineData("")]
        [InlineData("  ")]
        [InlineData(null)]
        public void Create_ShouldThrow_WhenUserNameIsNullOrWhitespace(string input)
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => UserName.Create(input));
            Assert.Equal("User name cannot be empty.", exception.Message);
        }

        [Fact]
        public void Create_ShouldSucceed_WhenUserNameIsValid()
        {
            // Arrange
            var input = "john_doe";

            // Act
            var userName = UserName.Create(input);

            // Assert
            Assert.Equal(input, userName.Value);
        }
    }
}
