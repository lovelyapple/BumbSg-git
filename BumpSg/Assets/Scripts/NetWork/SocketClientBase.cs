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
    public bool IsGameHost;
    public int? SelfClientObjectID;
    public int? EnemyClientObjectID;
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
        EnemyClientObjectID = null;

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
        EnemyClientObjectID = null;

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

        var item = ProtocolMaker.MakeToJson(message);

        if (item != null)
        {
            ProtocolMaker.DebugDeserializeProtocol(item);

            switch (item.msgType)
            {
                case ProtocolType.A2C_RegisterHost:
                    A2C_RegisterAsHost(item);
                    break;
                case ProtocolType.A2C_RegisterClient:
                    A2C_RegisterAsClient(item);
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
