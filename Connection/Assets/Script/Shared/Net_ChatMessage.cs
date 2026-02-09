using Unity.Collections;
using UnityEngine;
using System;

public class Net_ChatMessage : NetMessage
{
    public FixedString128Bytes ChatMessage { set; get;}
    
    public Net_ChatMessage()
    {
        Code = OpCode.CHAT_MESSAGE;
    }
    public Net_ChatMessage(DataStreamReader reader)
    {
        Code = OpCode.CHAT_MESSAGE;
        Deserialize(reader);
    }
    public Net_ChatMessage(string chatMessage)
    {
        Code = OpCode.CHAT_MESSAGE;
        ChatMessage = chatMessage;
    }

    public override void Serialize(ref DataStreamWriter writer)
    {
        writer.WriteByte((byte)Code);
        writer.WriteFixedString128(ChatMessage);
    }

    public override void Deserialize(DataStreamReader reader)
    {
        ChatMessage = reader.ReadFixedString128();
    }

    public override void ReceivedOnClient(BaseClient client)
    {
        Debug.Log($"CLIENT::{ChatMessage}");
        DateTime now = DateTime.Now;
        client.chatText.text += $"\n{now.Hour}:{now.Minute}:{now.Second} :: {ChatMessage}";

    }

    public override void ReceivedOnServer(BaseServer server)
    {
        Debug.Log($"SERVER::{ChatMessage}");
        server.Broadcast(this);
    }
}