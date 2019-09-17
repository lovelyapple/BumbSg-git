using UnityEngine;

/*
 * TestServer.cs
 * SocketServerを継承、開くポートを指定して、送受信したメッセージを具体的に処理する
 */

public class GameServer : SocketServerBase
{
    static GameServer _instance;
    void Start()
    {
        _instance = this;
        StartServer();
    }
    public static GameServer GetInstance()
    {
        return _instance;
    }
    public static bool IsServerStarted()
    {
        return GetInstance() != null && GetInstance()._listener != null;
    }
#pragma warning disable 0649
    // ポート指定（他で使用していないもの、使用されていたら手元の環境によって変更）
    [SerializeField] private int _port;
    public static string localServerIp;
#pragma warning restore 0649

    public void StartServer()
    {
        // 接続中のIPアドレスを取得
        var ipAddress = GetLocalIPAddress();
        // 指定したポートを開く
        BeginListen(ipAddress, _port);
        localServerIp = ipAddress;
    }
    public void StopServer()
    {
        StopListen();
    }

    // クライアントからメッセージ受信
    protected override void OnMessage(string msg)
    {
        base.OnMessage(msg);

        // -------------------------------------------------------------
        // あとは送られてきたメッセージによって何かしたいことを書く
        // -------------------------------------------------------------

        // クライアントに受領メッセージを返す
        SendMessageToClient(msg + "¥n");

    }
}
