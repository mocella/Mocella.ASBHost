namespace Mocella.AsbHost.Services;

public interface IMocellaService<T> : IMocellaService
{
    public T Add(T addEvent);
    public T Update(T updateEvent);
    public T Delete(T deleteEvent);
}

// here to help DI registration
public interface IMocellaService {

}