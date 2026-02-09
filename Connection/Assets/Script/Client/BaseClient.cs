using UnityEngine;
using Unity.Networking.Transport;
using Unity.Collections;
using TMPro;

public class BaseClient : MonoBehaviour
{
    public NetworkDriver driver;
    protected NetworkConnection connection;
    public TextMeshProUGUI chatText;

    public string playerName;

#if UNITY_EDITOR
    private void Start() { Init(); }
    private void Update() { UpdateServer(); }
    private void OnDestroy() { Shutdown(); }
#endif

    protected virtual void Init()
    {
        driver = NetworkDriver.Create();
        connection = default;

        var endpoint = NetworkEndpoint.LoopbackIpv4.WithPort(5000);
        connection = driver.Connect(endpoint);
    }
    protected virtual void Shutdown()
    {
        driver.Dispose();
    }
    protected virtual void UpdateServer()
    {
        driver.ScheduleUpdate().Complete();
        CheckAlive();

        UpdateMessagePump();

    }
    private void CheckAlive()
    {
        if (!connection.IsCreated)
        {
            Debug.Log("Something went wrong, lost connecton to server");
        }
    }
    protected virtual void UpdateMessagePump()
    {
        DataStreamReader stream;

        NetworkEvent.Type cmd;
        while ((cmd = connection.PopEvent(driver, out stream)) != NetworkEvent.Type.Empty)
        {
            if (cmd == NetworkEvent.Type.Connect)
            {
                Debug.Log("We are now connected to the server.");
            }
            else if (cmd == NetworkEvent.Type.Data)
            {
                OnData(stream);
            }
             else if (cmd == NetworkEvent.Type.Disconnect)
            {
                Debug.Log("Client got disconnected from server");
                connection = default;
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
        msg.ReceivedOnClient(this);
    }
    public virtual void SendToServer(NetMessage msg)
    {
        DataStreamWriter writer;
        driver.BeginSend(connection, out writer);
        msg.Serialize(ref writer);
        driver.EndSend(writer);
    }
}