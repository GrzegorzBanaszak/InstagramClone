using System;
using System.Text.RegularExpressions;
using InstagramClone.Domain.Common;

namespace InstagramClone.Domain.Posts.ValueObjects;

public class TagName : ValueObject
{
    static readonly Regex ValidPattern = new(@"^[a-z0-9\-]{1,30}$", RegexOptions.Compiled);
    public string Value { get; }

    private TagName(string value) => Value = value;

    public static Result<TagName> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Result<TagName>.Failure("Tag cannot be empty.");

        value = value.Trim().ToLowerInvariant();
        if (!ValidPattern.IsMatch(value))
            return Result<TagName>.Failure("Tag must be 1–30 chars: lowercase letters, digits or hyphens.");

        return Result<TagName>.Success(new TagName(value));
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;
}
