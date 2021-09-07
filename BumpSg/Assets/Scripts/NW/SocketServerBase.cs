using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

/*
 * SocketServer.cs
 * ソケット通信（サーバ）
 * Unityアプリ内にサーバを立ててメッセージの送受信を行う
 */

public class SocketServerBase : MonoBehaviour
{
    protected TcpListener _listener;
    public int objectIndex;
    public int? hostObjectId;
    public int? guestObjectId;
    public readonly Dictionary<int, TcpClient> _clients = new Dictionary<int, TcpClient>();
    public EndPoint hostEndPoint;

    // ソケット接続準備、待機
    protected void BeginListen(string host, int port)
    {
        ServerDebugLog("ipAddress:" + host + " port:" + port);
        var ip = IPAddress.Parse(host);
        _listener = new TcpListener(ip, port);
        _listener.Start();
        _listener.BeginAcceptSocket(DoAcceptTcpClientCallback, _listener);
        objectIndex = 0;
        hostObjectId = null;
        guestObjectId = null;
    }
    protected void StopListen()
    {
        if (_listener == null)
        {
            return;
        }

        if (_clients.Count != 0)
        {
            foreach (var client in _clients.Values)
            {
                client.Close();
            }
        }

        _clients.Clear();
        _listener.Stop();
        _listener = null;
        objectIndex = 0;
        hostObjectId = null;
        guestObjectId = null;
        ServerDebugLog("Stop Server");
    }
    void AddClient(TcpClient client)
    {
        _clients.Add(objectIndex, client);
        objectIndex++;
    }
    // クライアントからの接続処理
    private void DoAcceptTcpClientCallback(IAsyncResult ar)
    {
        var listener = (TcpListener)ar.AsyncState;
        var client = listener.EndAcceptTcpClient(ar);

        ServerDebugLog("New Connect: " + client.Client.RemoteEndPoint);

        // 接続が確立したら次の人を受け付ける
        listener.BeginAcceptSocket(DoAcceptTcpClientCallback, listener);

        // 今接続した人とのネットワークストリームを取得
        var stream = client.GetStream();
        var reader = new StreamReader(stream, Encoding.UTF8);

        //Add Client
        AddClient(client);

        // 接続が切れるまで送受信を繰り返す
        while (client.Connected)
        {
            // ServerDebugLog("client is connecting " + client.Client.RemoteEndPoint);
            // while (!reader.EndOfStream)
            // {
            //     // 一行分の文字列を受け取る
            //     var str = reader.ReadLine();
            //     OnMessage(str);
            // }

            byte[] bytes = new byte[2048];
            client.Client.Receive(bytes);
            string s = Encoding.UTF8.GetString(bytes);
            OnMessage(s, client);

            // クライアントの接続が切れたら
            if (client.Client.Poll(1000, SelectMode.SelectRead) && (client.Client.Available == 0))
            {
                ServerDebugLog("Disconnect: " + client.Client.RemoteEndPoint);
                client.Close();

                var ids = _clients.Keys.ToArray();
                for (int i = 0; i < ids.Length; i++)
                {
                    if (_clients[ids[i]] == client)
                    {
                        _clients.Remove(ids[i]);
                        break;
                    }
                }

                break;
            }
        }

    }


    // メッセージ受信
    protected virtual void OnMessage(string msg, TcpClient client)
    {
        ServerDebugLog("OnMessage" + msg);
    }

    // クライアントにメッセージ送信
    List<int> removeList = new List<int>();
    protected void SendMessageToClient(string msg, TcpClient client)
    {
        var body = Encoding.UTF8.GetBytes(msg);

        try
        {
            var stream = client.GetStream();
            stream.Write(body, 0, body.Length);
        }
        catch
        {
            ServerDebugLog("Failder send msg");
        }
    }
    protected void SendMessageToClientAll(string msg)
    {
        ServerDebugLog("SendMessae to Clients " + msg);

        if (_clients.Count == 0)
        {
            Debug.LogError("誰も接続せいていない");
            return;
        }
        var body = Encoding.UTF8.GetBytes(msg);

        // 全員に同じメッセージを送る
        foreach (var id in _clients.Keys)
        {
            try
            {
                Debug.LogError("send message to " + _clients[id].Client.RemoteEndPoint);
                var stream = _clients[id].GetStream();
                stream.Write(body, 0, body.Length);
            }
            catch
            {
                Debug.LogError("client lost " + id);
                removeList.Add(id);
            }
        }

        if (removeList.Count != 0)
        {
            bool containHost = false;

            foreach (var id in removeList)
            {
                _clients.Remove(id);

                if (id == hostObjectId)
                {
                    containHost = true;
                }
            }
            removeList.Clear();

            if (containHost)
            {
                StopListen();
            }
        }
    }

    // 終了処理
    protected virtual void OnApplicationQuit()
    {
        StopListen();
    }

    public static string GetLocalIPAddress()
    {
        var host = System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName());
        foreach (var ip in host.AddressList)
        {
            if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
            {
                return ip.ToString();
            }
        }

        throw new System.Exception("No network adapters with an IPv4 address in the system!");
    }
    public static void ServerDebugLog(string str)
    {
        Debug.Log("<color=yellow>ServerLog</color>" + str);
    }
}
