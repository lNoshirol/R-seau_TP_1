using Unity.Collections;
using UnityEngine;

public class NetMessage
{
    public OpCode Code { get; set; }

    public virtual void Serialize(ref DataStreamWriter writer)
    {

    }
    public virtual void Deserialize(DataStreamReader reader)
    {

    }

    public virtual void ReceivedOnClient(BaseClient client)
    {

    }

    public virtual void ReceivedOnServer(BaseServer server)
    {

    }
}
