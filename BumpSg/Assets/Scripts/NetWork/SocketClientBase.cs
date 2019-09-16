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

    public string serverIp;
    public int serverPort;

    void Start()
    {
        serverIp = GameServer.localServerIp;
        client = new TcpClient(serverIp, serverPort);
        stream = client.GetStream();
    }
    void OnDestroy()
    {
        client.Client.Close();
        client = null;
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            var bytes = StrToByteArray("Hellow World");

            stream.Write(bytes, 0, bytes.Length);
            stream.Flush();
        }
    }
    public static byte[] StrToByteArray(string str)
    {
        Encoding encoding = Encoding.UTF8; //or below line
                                           //System.Text.UTF8Encoding  encoding=new System.Text.UTF8Encoding();
        return encoding.GetBytes(str);
    }
}
