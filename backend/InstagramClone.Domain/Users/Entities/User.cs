using InstagramClone.Domain.Common;
using InstagramClone.Domain.Users.Events;
using InstagramClone.Domain.Users.ValueObjects;

namespace InstagramClone.Domain.Users.Entities
{
    public class User : BaseEntity, IAggregateRoot
    {
        public UserName Username { get; private set; }
        public Email Email { get; private set; }
        public string PasswordHash { get; private set; }
        public string ProfilePictureUrl { get; private set; }
        public string Bio { get; private set; }
        public DateTime CreatedAt { get; private set; }

        // kolekcja relacji “ja obserwuję”
        private readonly List<Follow> _follows = new();
        public IReadOnlyCollection<Follow> Follows => _follows.AsReadOnly();

        // nowa kolekcja relacji “kto mnie obserwuje”
        private readonly List<Follow> _followers = new();
        public IReadOnlyCollection<Follow> Followers => _followers.AsReadOnly();

        // Kolekcja relacji "kogo ja subskrybuje"
        private readonly List<Subscription> _subscriptions = new();
        public IReadOnlyCollection<Subscription> Subscriptions => _subscriptions.AsReadOnly();


        // Kolekcja relacji “kto mnie subskrybuje”
        private readonly List<Subscription> _subscribers = new();
        public IReadOnlyCollection<Subscription> Subscribers => _subscribers.AsReadOnly();

        private User() { }

        private User(UserName username, Email email, string passwordHash)
        {
            Id = Guid.NewGuid();
            Username = username;
            Email = email;
            PasswordHash = passwordHash;
            CreatedAt = DateTime.UtcNow;
            ProfilePictureUrl = string.Empty;
            Bio = string.Empty;
        }

        // Fabryka (zamiast new User(...))
        public static Result<User> Create(string username, string email, string passwordHash)
        {
            if (string.IsNullOrWhiteSpace(passwordHash))
                return Result<User>.Failure("Password hash cannot be empty.");

            var userNameResult = UserName.Create(username);
            var emailResult = Email.Create(email);

            if (userNameResult.IsFailure)
                return Result<User>.Failure(userNameResult.Error);

            if (emailResult.IsFailure)
                return Result<User>.Failure(emailResult.Error);

            var user = new User(userNameResult.Value, emailResult.Value, passwordHash);
            return Result<User>.Success(user);
        }

        public void UpdateProfile(string newBio, string newProfilePictureUrl)
        {
            Bio = newBio?.Trim() ?? string.Empty;
            ProfilePictureUrl = newProfilePictureUrl?.Trim() ?? string.Empty;
        }

        public Result ChangePasswordHash(string newPasswordHash)
        {
            if (string.IsNullOrWhiteSpace(newPasswordHash))
                return Result.Failure("Password hash cannot be empty.");

            PasswordHash = newPasswordHash;
            return Result.Success();
        }


        public Result UpdateProfilePicture(string newUrl)
        {
            if (string.IsNullOrWhiteSpace(newUrl))
                return Result.Failure("Profile picture URL cannot be empty.");

            if (!Uri.IsWellFormedUriString(newUrl, UriKind.Absolute))
                return Result.Failure("Invalid profile picture URL.");

            ProfilePictureUrl = newUrl.Trim();

            return Result.Success();
        }

        // ---------- FOLLOW  -----------------

        /// <summary>
        /// Toworzy nową relacje "ja obserwuje".
        /// </summary>
        public Result NewFollow(Guid targetUserId)
        {
            if (targetUserId == Guid.Empty)
                return Result.Failure("Target user ID cannot be empty.");

            if (Id == targetUserId)
                return Result.Failure("You cannot follow yourself.");

            if (_follows.Any(f => f.FollowingId == targetUserId))
                return Result.Failure("You are already following this user.");

            var followResult = Follow.Create(Id, targetUserId);
            if (followResult.IsFailure)
                return Result.Failure(followResult.Error);

            _follows.Add(followResult.Value);
            AddDomainEvent(new FollowCreatedEvent(Id, targetUserId));
            return Result.Success();
        }

        /// <summary>
        /// Zaprzestanie obserwowania użytkownika.
        /// </summary>    
        public Result UnFollow(Guid targetUserId)
        {
            var follow = _follows.FirstOrDefault(f => f.FollowingId == targetUserId);
            if (follow is null)
                return Result.Failure("You are not following this user.");

            _follows.Remove(follow);
            AddDomainEvent(new FollowDeletedEvent(Id, targetUserId));
            return Result.Success();
        }

        // ---------- SUBSCRIBE / UNSUBSCRIBE -----------------
        /// <summary>
        /// Użytkownik <em>subscriber</em> rozpoczyna subskrypcję autora (<em>this</em>).
        /// </summary>

        public Result Subscribe(Guid subscriberId)
        {
            if (subscriberId == Guid.Empty)
                return Result.Failure("Subscriber ID cannot be empty.");


            if (subscriberId == Id)
                return Result.Failure("You cannot subscribe to yourself.");

            if (_subscriptions.Any(s => s.SubscriberId == subscriberId))
                return Result.Failure("You are already subscribed to this user.");

            Result<Subscription> subscriptionResult = Subscription.Create(subscriberId, Id, 1); // 1 miesięcy

            if (subscriptionResult.IsFailure)
                return Result.Failure(subscriptionResult.Error);

            _subscriptions.Add(subscriptionResult.Value);
            return Result.Success();
        }

        public Result UnSubscribe(Guid subscriberId)
        {
            var subscription = _subscriptions.FirstOrDefault(s => s.SubscriberId == subscriberId);
            if (subscription is null)
                return Result.Failure("You are not subscribed to this user.");

            var result = subscription.CancelAutoRenow();
            if (result.IsFailure)
                return Result.Failure(result.Error);

            return Result.Success();
        }

        public Result RemoveSubscription(Guid subscriberId)
        {
            var subscription = _subscriptions.FirstOrDefault(s => s.SubscriberId == subscriberId);
            if (subscription is null)
                return Result.Failure("You are not subscribed to this user.");

            if (subscription.AutoRenewEnabled == true)
                return Result.Failure("Cannot remove subscription with active auto-renew.");

            _subscriptions.Remove(subscription);
            return Result.Success();
        }

    }
}
