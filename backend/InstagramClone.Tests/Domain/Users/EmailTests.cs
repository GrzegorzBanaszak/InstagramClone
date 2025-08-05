using System;
using InstagramClone.Domain.Users.ValueObjects;

namespace InstagramClone.Tests.Domain.Users;

public class EmailTests
{
    [Fact]
    public void Email_should_not_be()
    {
        var input = "test@wp.pl";

        var email = Email.Create(input);

        Assert.Equal(input, email.Value);
    }
}
