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
        base.OnMessage(msg, client);

        // -------------------------------------------------------------
        // あとは送られてきたメッセージによって何かしたいことを書く
        // -------------------------------------------------------------

        var item = ProtocolMaker.MakeToJson(msg);

        switch (item.msgType)
        {
            case ProtocolType.C2A_RegisterHost:
                var A2C_item_host = ProtocolMaker.Mk_A2C_RegisterHost();

                if (hostObjectId.HasValue)
                {
                    A2C_item_host.boolParam = false;
                }
                else
                {
                    var hostId = GetClientObjectId(client);

                    if (hostId < 0)
                    {
                        A2C_item_host.boolParam = false;
                    }
                    else
                    {
                        hostObjectId = hostId;
                        A2C_item_host.objectId = hostObjectId.Value;
                        A2C_item_host.boolParam = true;
                    }
                }
                msg = ProtocolMaker.SerializeToJson(A2C_item_host);
                SendMessageToClient(msg, client);
                break;
            case ProtocolType.C2A_RegisterClient:
                var A2C_item_client = ProtocolMaker.Mk_A2C_RegisterClient();
                var clientId = GetClientObjectId(client);

                if (clientId < 0)
                {
                    A2C_item_client.boolParam = false;
                }
                else
                {
                    hostObjectId = clientId;
                    A2C_item_client.objectId = hostObjectId.Value;
                    A2C_item_client.boolParam = true;
                }

                msg = ProtocolMaker.SerializeToJson(A2C_item_client);
                // クライアントに受領メッセージを返す
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
