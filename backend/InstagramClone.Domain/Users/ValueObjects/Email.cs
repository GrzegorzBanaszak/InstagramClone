using System;
using System.Text.RegularExpressions;
using InstagramClone.Domain.Common;

namespace InstagramClone.Domain.Users.ValueObjects;

public class Email : ValueObject
{
    private static readonly Regex EmailRegex = new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled);

    public string Value { get; }

    private Email(string value)
    {
        Value = value;
    }

    public static Result<Email> Create(string value)
    {
        if (value is null)
            return Result<Email>.Failure("Email cannot be null.");

        if (string.IsNullOrWhiteSpace(value))
            return Result<Email>.Failure("Email cannot be empty.");

        if (!EmailRegex.IsMatch(value))
            return Result<Email>.Failure("Invalid email format.");

        return Result<Email>.Success(new Email(value.ToLowerInvariant())); // normalizujemy
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;
}
