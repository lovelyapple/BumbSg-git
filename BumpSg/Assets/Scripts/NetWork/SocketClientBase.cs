using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.IO;
using System.Text;
using System;

public partial class SocketClientBase : MonoBehaviour
{
    static SocketClientBase _instance;
    public TcpClient Client { get; private set; }
    NetworkStream stream;
    byte[] readbuf = new byte[1024];
    bool isForceStop = false;
    bool isStopReading = false;
    public static bool IsGameHost;
    public int? SelfClientObjectID;
    public int? HostClientObjectID;
    public int? GuestClientObjectID;
    public int? WinObjectID;
    public string serverIp;
    public int serverPort;
    Coroutine streamReadingCoroutine;
    // if (!GameServer.IsServerStarted())
    // {
    //     Debug.Log("There is no server running");
    //     return;
    // }
    //serverIp = GameServer.localServerIp;
    public void InitializeGameClient()
    {
        _instance = this;
    }
    public static SocketClientBase GetInstance()
    {
        return _instance;
    }
    public void ConnectToServer(string ip)
    {
        serverIp = ip;
        SelfClientObjectID = null;
        HostClientObjectID = null;
        GuestClientObjectID = null;
        WinObjectID = null;

        ClientBaseDebugLog("Try to connect to ip " + ip + " port " + serverPort);
        Client = new TcpClient(serverIp, serverPort);

        if (Client == null)
        {
            return;
        }

        stream = Client.GetStream();
        streamReadingCoroutine = StartCoroutine(StartReading());
    }
    void OnDestroy()
    {
        StopClient();
    }
    public void StopClient()
    {
        if (streamReadingCoroutine != null)
        {
            StopCoroutine(streamReadingCoroutine);
        }

        streamReadingCoroutine = null;

        if (Client != null && Client.Client != null)
        {
            Client.Client.Close();
        }

        SelfClientObjectID = null;
        HostClientObjectID = null;
        GuestClientObjectID = null;
        WinObjectID = null;

        ClientBaseDebugLog("Client stoped");
    }

    private IEnumerator StartReading()
    {
        ClientBaseDebugLog("START Reading!");
        readbuf = new byte[1024];

        while (!isForceStop)
        {
            if (!isStopReading)
            {
                StartCoroutine(ReadMessage());
            }
            yield return new WaitForSeconds(0.01f);//あんまりしょっちゅうやらないために
        }
    }
    //常駐
    private IEnumerator ReadMessage()
    {
        // 非同期で待ち受けする
        stream.BeginRead(readbuf, 0, readbuf.Length, new AsyncCallback(ReadCallback), null);
        isStopReading = true;
        yield return null;
    }

    public void ReadCallback(IAsyncResult ar)
    {
        Encoding enc = Encoding.UTF8;
        int bytes = stream.EndRead(ar);
        string message = enc.GetString(readbuf, 0, bytes);
        message = message.Replace("\r", "").Replace("\n", "");
        ClientBaseDebugLog("ReadCallback " + message);

        ProtocolItem item = null;

        try
        {
            item = JsonUtility.FromJson<ProtocolItem>(message);
        }
        catch
        {
            ClientBaseDebugLog("JsonUtility.FromJso Failed");
        }

        if (item != null)
        {
            // ProtocolMaker.DebugDeserializeProtocol(item);
            switch ((ProtocolType)item.msgType)
            {
                case ProtocolType.A2C_UpdateClientInfo:
                    A2C_UpdateClientInfo(item);
                    break;
                case ProtocolType.A2C_ResponseStartGame:
                    A2C_ResponseStartGame();
                    break;
                case ProtocolType.A2C_AddForceToBall:
                    A2C_AddForceToBall(item);
                    break;
                case ProtocolType.A2C_GameResult:
                    A2C_GameResult(item);
                    break;
            }
        }

        isStopReading = false;
    }
    public static byte[] StrToByteArray(string str)
    {
        Encoding encoding = Encoding.UTF8; //or below line
                                           //System.Text.UTF8Encoding  encoding=new System.Text.UTF8Encoding();
        return encoding.GetBytes(str);
    }

    void ClientBaseDebugLog(string str)
    {
        Debug.Log("<color=white>ClientLog</color>" + str);
    }
}
