using TMPro;
using Unity.Collections;
using UnityEngine;

public class ChatMessageTest : MonoBehaviour
{
    public TextMeshProUGUI text;
    public BaseClient client;

    public void OnSubmitClick()
    {
        Net_ChatMessage msg = new Net_ChatMessage(text.text);
        client.SendToServer(msg);
    }

    public void ChangeName()
    {
        client.playerName = text.text;
    }
}
