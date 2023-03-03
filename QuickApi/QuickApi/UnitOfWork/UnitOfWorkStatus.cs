namespace QuickApi.UnitOfWork
{
    public enum UnitOfWorkStatus
    {
        UnCreated,
        Created,
        Committed,
        Rollbacked,
        Disposed
    }
}