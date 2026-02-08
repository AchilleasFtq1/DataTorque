namespace DataTorque.Api.Services;

public interface IRequestCounter
{
    int Increment();
}

public class RequestCounter : IRequestCounter
{
    private int _count;

    public int Increment() => Interlocked.Increment(ref _count);
}
