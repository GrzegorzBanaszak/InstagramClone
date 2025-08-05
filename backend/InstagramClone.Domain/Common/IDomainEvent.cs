using System;

namespace InstagramClone.Domain.Common;

public interface IDomainEvent
{
    DateTime OccurredOn { get; }
}
