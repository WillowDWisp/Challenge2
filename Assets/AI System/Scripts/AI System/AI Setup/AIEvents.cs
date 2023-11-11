
namespace jcsilva.AISystem {
    /// <summary>
    /// All Events related to the AI System
    /// You can add more events after the comment in the enum.
    /// </summary>
    public enum AIEvents {
        NoLongerIdle,
        ReachedDestination,
        SeePlayer,
        LostPlayer,
        OutOfRange,
        InRange,
        PlayerNotFound,
        // Insert new events bellow this comment
        ReachedLastKnownPosition,
    }
}