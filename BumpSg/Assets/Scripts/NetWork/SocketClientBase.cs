using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.IO;
using System.Text;
using System;

public class SocketClientBase : MonoBehaviour
{
    TcpClient client;
    NetworkStream stream;
    byte[] readbuf = new byte[1024];
    bool isForceStop = false;
    bool isStopReading = false;
    public string serverIp;
    public int serverPort;
    Coroutine streamReadingCoroutine;

    void Start()
    {
        serverIp = GameServer.localServerIp;
        client = new TcpClient(serverIp, serverPort);
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
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            var bytes = StrToByteArray("123456789");


            if (stream.CanWrite)
            {
                Debug.Log("writed");
                stream.Write(bytes, 0, bytes.Length);
                stream.Flush();
            }
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
            yield return new WaitForSeconds(1f);//あんまりしょっちゅうやらないために
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
        isStopReading = false;
    }
    public static byte[] StrToByteArray(string str)
    {
        Encoding encoding = Encoding.UTF8; //or below line
                                           //System.Text.UTF8Encoding  encoding=new System.Text.UTF8Encoding();
        return encoding.GetBytes(str);
    }
}
