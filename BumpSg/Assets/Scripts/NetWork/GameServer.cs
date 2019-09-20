using UnityEngine;
using System.IO;
using System.Net;
using System.Net.Sockets;
/*
 * TestServer.cs
 * SocketServerを継承、開くポートを指定して、送受信したメッセージを具体的に処理する
 */

public class GameServer : SocketServerBase
{
#pragma warning disable 0649
    // ポート指定（他で使用していないもの、使用されていたら手元の環境によって変更）
    public string ServerIp;
    public int Port;
#pragma warning restore 0649
    static GameServer _instance;
    public static GameServer GetInstance()
    {
        return _instance;
    }
    public static bool IsServerStarted()
    {
        return GetInstance() != null && GetInstance()._listener != null;
    }
    public void InitializeGameServer()
    {
        _instance = this;
        // 接続中のIPアドレスを取得
        ServerIp = GetLocalIPAddress();
    }
    public void StartServer()
    {

        // 指定したポートを開く
        BeginListen(ServerIp, Port);
    }
    public void StopServer()
    {
        StopListen();
    }

    // クライアントからメッセージ受信
    protected override void OnMessage(string msg, TcpClient client)
    {
        //base.OnMessage(msg, client);

        // -------------------------------------------------------------
        // あとは送られてきたメッセージによって何かしたいことを書く
        // -------------------------------------------------------------
        ProtocolItem item = null;

        try
        {
            item = ProtocolMaker.MakeToJson(msg);
        }
        catch
        {
            Debug.LogError("ProtocolMaker.MakeToJson(msg); failed" + msg);
        }

        ServerDebugLog("OnMessage" + item.msgType.ToString());

        switch (item.msgType)
        {
            case ProtocolType.C2A_RegisterHost:
                hostObjectId = GetClientObjectId(client);
                var host_logined = ProtocolMaker.Mk_A2C_UpdateClientInfo(hostObjectId.Value, -1);
                msg = ProtocolMaker.SerializeToJson(host_logined);
                SendMessageToClientAll(msg);
                break;
            case ProtocolType.C2A_RegisterClient:
                guestObjectId = GetClientObjectId(client);
                var guest_logined = ProtocolMaker.Mk_A2C_UpdateClientInfo(hostObjectId.Value, guestObjectId.Value);
                msg = ProtocolMaker.SerializeToJson(guest_logined);
                SendMessageToClientAll(msg);
                break;


            case ProtocolType.C2A_RequestStartGame:
                var game_start = ProtocolMaker.Mk_A2C_ResponseStartGame();
                msg = ProtocolMaker.SerializeToJson(game_start);
                SendMessageToClientAll(msg);
                break;

            case ProtocolType.C2A_AddForceToBall:
                var add_forec = ProtocolMaker.Mk_A2C_AddForceToBall(item);
                msg = ProtocolMaker.SerializeToJson(add_forec);
                SendMessageToClientAll(msg);
                break;

            default:
                // クライアントに受領メッセージを返す
                SendMessageToClientAll(msg);
                break;
        }

    }

    int GetClientObjectId(TcpClient client)
    {
        foreach (var id in _clients.Keys)
        {
            if (_clients[id] == client)
            {
                return id;
            }
        }

        return -1;
    }
}
