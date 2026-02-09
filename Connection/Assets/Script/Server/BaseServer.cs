using Unity.Collections;
using Unity.Networking.Transport;
using UnityEditor.MemoryProfiler;
using UnityEngine;

public class BaseServer : MonoBehaviour
{
    public NetworkDriver driver;
    protected NativeList<NetworkConnection> connections;

#if UNITY_EDITOR
    private void Start() { Init(); }
    private void Update() { UpdateServer(); }
    private void OnDestroy() { Shutdown(); }
#endif

    protected virtual void Init()
    {
        driver = NetworkDriver.Create();
        connections = new NativeList<NetworkConnection>(16, Allocator.Persistent);
        var endpoint = NetworkEndpoint.AnyIpv4.WithPort(5000);
        if (driver.Bind(endpoint) != 0)
        {
            Debug.LogError("Failed to bind to port 5000.");
            Destroy(this);
            return;
        }
        driver.Listen();
    }
    protected virtual void Shutdown()
    {
        driver.Dispose();
        connections.Dispose();  
    }
    protected virtual void UpdateServer()
    {
        driver.ScheduleUpdate().Complete();

        CleanUpConnection();
        AcceptNewConnections();
        UpdateMessagePump();

    }
    private void CleanUpConnection()
    {
        for (int i = 0; i < connections.Length; i++)
        {
            if (!connections[i].IsCreated)
            {
                connections.RemoveAtSwapBack(i);
                i--;
            }
        }
    }
    private void AcceptNewConnections()
    {
        NetworkConnection c;
        while ((c = driver.Accept()) != default)
        {
            connections.Add(c);
            Debug.Log("Accepted a connection.");
        }
    }
    protected virtual void UpdateMessagePump()
    {
        DataStreamReader stream;
        for (int i = 0; i < connections.Length; i++)
        {
            NetworkEvent.Type cmd;
            while ((cmd = driver.PopEventForConnection(connections[i], out stream)) != NetworkEvent.Type.Empty)
            {
                if (cmd == NetworkEvent.Type.Data)
                {
                    OnData(stream);
                }
                else if (cmd == NetworkEvent.Type.Disconnect)
                {
                    Debug.Log("Client disconnected.");
                    connections[i] = default(NetworkConnection);
                }
            }
        }
    }
    public virtual void OnData(DataStreamReader stream)
    {
        NetMessage msg = null;
        var opCode = (OpCode)stream.ReadByte();
        switch (opCode)
        {
            case OpCode.CHAT_MESSAGE: msg = new Net_ChatMessage(stream); break;
            default: Debug.LogError("Unknown OpCode received: " + opCode); return;
        }
        msg.ReceivedOnServer(this);
    }
    public virtual void Broadcast(NetMessage msg)
    {
        for (int i = 0; i < connections.Length; i++)
            if (connections[i].IsCreated)
                SendToClent(connections[i], msg);
    }
    public virtual void SendToClent(NetworkConnection connection, NetMessage msg)
    {
        DataStreamWriter writer;
        driver.BeginSend(connection, out writer);
        msg.Serialize(ref writer);
        driver.EndSend(writer);
    }
}