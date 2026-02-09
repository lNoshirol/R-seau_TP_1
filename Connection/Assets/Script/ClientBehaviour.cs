using System.Text;
using TMPro;
using Unity.Collections;
using Unity.Networking.Transport;
using UnityEngine;

public class ClientBehaviour : MonoBehaviour
{
    NetworkDriver m_Driver;
    NetworkConnection m_Connection;

    public TextMeshProUGUI TheText;

    void Start()
    {
        m_Driver = NetworkDriver.Create();

        var endpoint = NetworkEndpoint.LoopbackIpv4.WithPort(8778);
        m_Connection = m_Driver.Connect(endpoint);
        m_Driver.ScheduleUpdate().Complete();
    }

    void OnDestroy()
    {
        m_Driver.Dispose();
    }

    void Update()
    {
        //Tuto documentation
        return;

        m_Driver.ScheduleUpdate().Complete();

        if (!m_Connection.IsCreated)
        {
            return;
        }

        Unity.Collections.DataStreamReader stream;
        NetworkEvent.Type cmd;


        while ((cmd = m_Connection.PopEvent(m_Driver, out stream)) != NetworkEvent.Type.Empty)
        {

            if (cmd == NetworkEvent.Type.Connect)
            {
                Debug.Log("We are now connected to the server.");

                byte[] bytes = Encoding.UTF8.GetBytes("EVOIE D'N TRUC");

                m_Driver.BeginSend(m_Connection, out var writer);

                writer.WriteBytes(bytes);
                m_Driver.EndSend(writer);
            }
            else if (cmd == NetworkEvent.Type.Data)
            {
                uint value = stream.ReadUInt();
                Debug.Log($"Got the value {value} back from the server.");

                m_Connection.Disconnect(m_Driver);
                m_Connection = default;
            }
            else if (cmd == NetworkEvent.Type.Disconnect)
            {
                Debug.Log("Client got disconnected from server.");
                m_Connection = default;
            }
        }
    }

    public void SendText(string text)
    {
        m_Driver = NetworkDriver.Create();

        var endpoint = NetworkEndpoint.LoopbackIpv4.WithPort(7778);
        m_Connection = m_Driver.Connect(endpoint);

        m_Driver.ScheduleUpdate().Complete();

        if (!m_Connection.IsCreated)
        {
            Debug.Log("ça se chie dessus");
            return;
        }

        Debug.Log("ENVOIIIIIIIIIIIIIIIIIIIIE");


        Unity.Collections.DataStreamReader stream;
        NetworkEvent.Type cmd;

        while ((cmd = m_Connection.PopEvent(m_Driver, out stream)) != NetworkEvent.Type.Empty)
        {
            if (cmd == NetworkEvent.Type.Connect)
            {
                Debug.Log("We are now connected to the server.");
                m_Driver.BeginSend(m_Connection, out var writer);

                byte[] bytes = Encoding.UTF8.GetBytes(text);

                writer.WriteBytes(bytes);
                m_Driver.EndSend(writer);
            }
            else if (cmd == NetworkEvent.Type.Data)
            {
                uint value = stream.ReadUInt();
                Debug.Log($"Got the value {value} back from the server.");

                TheText.text = "Nique bien ta mere";

                m_Connection.Disconnect(m_Driver);
                m_Connection = default;
            }
            else if (cmd == NetworkEvent.Type.Disconnect)
            {
                Debug.Log("Client got disconnected from server.");
                m_Connection = default;
            }
        }

        Debug.Log("The End ?...");
    }
}