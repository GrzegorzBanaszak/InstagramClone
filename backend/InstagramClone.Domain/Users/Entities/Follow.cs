using System;
using InstagramClone.Domain.Common;

namespace InstagramClone.Domain.Users.Entities;

public class Follow : BaseEntity
{
    public Guid FollowerId { get; private set; }      // Kto obserwuje
    public Guid FollowingId { get; private set; }     // Kogo obserwuje
    public DateTime FollowedAt { get; private set; }

    private Follow() { }

    private Follow(Guid followerId, Guid followingId)
    {
        FollowerId = followerId;
        FollowingId = followingId;
        FollowedAt = DateTime.Now;
    }

    public static Result<Follow> Create(Guid followerId, Guid followingId)
    {
        if (followerId == Guid.Empty || followingId == Guid.Empty)
            return Result<Follow>.Failure("Follower and following IDs must be valid.");

        if (followerId == followingId)
            return Result<Follow>.Failure("You cannot follow yourself.");

        return Result<Follow>.Success(new Follow(followerId, followingId));
    }
}
