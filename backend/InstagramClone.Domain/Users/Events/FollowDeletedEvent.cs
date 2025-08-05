using System;
using InstagramClone.Domain.Common;

namespace InstagramClone.Domain.Users.Events;

public class FollowDeletedEvent : IDomainEvent
{
    public Guid FollowerId { get; }
    public Guid FollowingId { get; }
    public DateTime OccurredOn { get; } = DateTime.UtcNow;

    public FollowDeletedEvent(Guid followerId, Guid followingId)
    {
        FollowerId = followerId;
        FollowingId = followingId;
    }
}
