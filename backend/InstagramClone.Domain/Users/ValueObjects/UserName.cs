using InstagramClone.Domain.Common;
using System.Text.RegularExpressions;

namespace InstagramClone.Domain.Users.ValueObjects
{
    public class UserName : ValueObject
    {
        private static readonly Regex ValidPattern = new(@"^[a-zA-Z0-9_.]{3,20}$");

        public string Value { get; }

        private UserName(string value)
        {
            Value = value;
        }

        public static Result<UserName> Create(string value)
        {
            if (value is null)
                return Result<UserName>.Failure("User name cannot be empty."); // zamiast ArgumentNullException

            if (string.IsNullOrWhiteSpace(value))
                return Result<UserName>.Failure("User name cannot be empty.");

            var trimmed = value.Trim();
            if (!ValidPattern.IsMatch(trimmed))
                return Result<UserName>.Failure("User name must be 3-20 characters and contain only letters, numbers, underscores or dots.");

            return Result<UserName>.Success(new UserName(trimmed));
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value.ToLower();
        }

        public override string ToString() => Value;
    }
}
