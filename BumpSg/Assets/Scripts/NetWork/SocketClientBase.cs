using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.IO;
using System.Text;
using System;

public partial class SocketClientBase : MonoBehaviour
{
    TcpClient client;
    NetworkStream stream;
    byte[] readbuf = new byte[1024];
    bool isForceStop = false;
    bool isStopReading = false;
    public string serverIp;
    public int serverPort;
    Coroutine streamReadingCoroutine;
    // if (!GameServer.IsServerStarted())
    // {
    //     Debug.Log("There is no server running");
    //     return;
    // }
    //serverIp = GameServer.localServerIp;
    public void ConnectToServer()
    {
        client = new TcpClient(serverIp, serverPort);

        if (client == null)
        {
            return;
        }

        stream = client.GetStream();
        streamReadingCoroutine = StartCoroutine(StartReading());
    }
    void OnDisable()
    {
        if (streamReadingCoroutine != null)
        {
            StopCoroutine(streamReadingCoroutine);
        }
        streamReadingCoroutine = null;
    }
    void OnDestroy()
    {
        if (client != null && client.Client != null)
        {
            client.Client.Close();
            client = null;
        }
    }

    private IEnumerator StartReading()
    {
        Debug.Log("START START");
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
        Debug.Log("ReadCallback " + message);

        var item = ProtocolMaker.MakeToJson(message);

        if (item != null)
        {
            ProtocolMaker.DebugDeserializeProtocol(item);
        }

        isStopReading = false;
    }
    public static byte[] StrToByteArray(string str)
    {
        Encoding encoding = Encoding.UTF8; //or below line
                                           //System.Text.UTF8Encoding  encoding=new System.Text.UTF8Encoding();
        return encoding.GetBytes(str);
    }

}
