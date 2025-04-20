namespace CaravansCore.Networking;

public interface IClient
{
    public void Receive(Snapshot snapshot);
}