using System;
using InstagramClone.Domain.Common;

namespace InstagramClone.Domain.Users.Entities;

public class Subscription : BaseEntity
{
    public Guid SubscriberId { get; private set; }   // Ten, kto subskrybuje
    public Guid CreatorId { get; private set; }   // Ten, kogo subskrybuje
    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }

    /// <summary>
    /// Czy subskrypcja będzie się automatycznie odnawiać po zakończeniu?
    /// </summary>
    public bool AutoRenewEnabled { get; private set; }

    /// <summary>
    /// O ile miesięcy przedłużać przy każdym odnowieniu.
    /// </summary>
    public int RenewalPeriodMonths { get; private set; }

    // Dla EF Core
    private Subscription() { }

    private Subscription(Guid subscriberId, Guid creatorId, int months)
    {
        Id = Guid.NewGuid();
        SubscriberId = subscriberId;
        CreatorId = creatorId;
        StartDate = DateTime.UtcNow;
        EndDate = StartDate.AddMonths(months);
        RenewalPeriodMonths = months;
        AutoRenewEnabled = true;
    }


    public static Result<Subscription> Create(Guid subscriberId, Guid creatorId, int months)
    {
        if (subscriberId == Guid.Empty || creatorId == Guid.Empty)
            return Result<Subscription>.Failure("SubscriberId and CreatorId must be provided.");

        if (subscriberId == creatorId)
            return Result<Subscription>.Failure("Cannot subscribe to yourself.");

        if (months <= 0)
            return Result<Subscription>.Failure("Subscription duration must be at least one month.");

        return Result<Subscription>.Success(new Subscription(subscriberId, creatorId, months));
    }

    /// <summary>
    /// Wyłącza automatyczne odnawianie – bieżąca subskrypcja pozostaje aktywna do EndDate.
    /// </summary>
    public Result CancelAutoRenow()
    {
        if (!AutoRenewEnabled)
            return Result.Failure("Auto-renew is disabled.");

        AutoRenewEnabled = false;
        return Result.Success();
    }

    /// <summary>
    /// Próbuje odnowić subskrypcję, jeśli jest aktywna i włączone AutoRenew, a czas EndDate minął.
    /// </summary>
    public Result RenewIfDue(DateTime now)
    {
        if (!AutoRenewEnabled)
            return Result.Failure("Auto-renew is disabled.");

        if (now < EndDate)
            return Result.Failure("Subscription is not yet due for renewal.");

        EndDate = EndDate.AddMonths(RenewalPeriodMonths);
        return Result.Success();
    }

    /// <summary>
    /// Czy subskrypcja jest aktywna (w tym spierz bieżącej lub odnowionej sesji).
    /// </summary>
    public bool IsActive(DateTime now) => now < EndDate;
}
