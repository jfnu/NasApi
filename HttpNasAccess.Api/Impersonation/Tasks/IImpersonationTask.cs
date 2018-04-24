namespace HttpNasAccess.Api.Impersonation.Tasks
{
    public interface IImpersonationTask<T>
    {
        T Execute();
    }
}
