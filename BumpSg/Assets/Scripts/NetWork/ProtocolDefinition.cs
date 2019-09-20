using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
public enum ProtocolType
{
    SimpleMsg = 1,
    Position = 2,
    C2A_RegisterHost = 3,
    C2A_RegisterClient = 4,
    A2C_UpdateClientInfo = 5,

    C2A_RequestStartGame = 6,
    A2C_ResponseStartGame = 7,
}
[Serializable]
public class ProtocolItem
{
    public int sendFrom;
    public int sendTo;
    public ProtocolType msgType;
    public int objectId_1;
    public int objectId_2;
    public Vector3 vectorParam_1;
    public Vector3 vectorParam_2;
    public float floatParam_1;
    public float floatParam_2;
    public string stringParam;
    public bool boolParam;
}
public class ProtocolMaker
{
    public static ProtocolItem Mk_SimpleMsg(int sendFrom, string msg)
    {
        ProtocolItem item = new ProtocolItem();
        item.sendFrom = sendFrom;
        item.stringParam = msg;
        item.msgType = ProtocolType.SimpleMsg;
        return item;
    }
    public static ProtocolItem Mk_Position(int sendFrom, Vector3 vector)
    {
        ProtocolItem item = new ProtocolItem();
        item.sendFrom = sendFrom;
        item.vectorParam_1 = vector;
        item.msgType = ProtocolType.Position;
        return item;
    }

    //
    //　ログイン
    //
    public static ProtocolItem Mk_C2A_RegisterHost()
    {
        ProtocolItem item = new ProtocolItem();
        item.msgType = ProtocolType.C2A_RegisterHost;
        return item;
    }
    public static ProtocolItem Mk_C2A_RegisterClient()
    {
        ProtocolItem item = new ProtocolItem();
        item.msgType = ProtocolType.C2A_RegisterClient;
        return item;
    }
    public static ProtocolItem Mk_A2C_UpdateClientInfo(int hostId, int guestId)
    {
        ProtocolItem item = new ProtocolItem();
        item.msgType = ProtocolType.A2C_UpdateClientInfo;
        item.objectId_1 = hostId;
        item.objectId_2 = guestId;
        return item;
    }

    //
    // ゲームスタート
    //
    public static ProtocolItem Mk_C2A_RequestStartGame()
    {
        ProtocolItem item = new ProtocolItem();
        item.msgType = ProtocolType.C2A_RequestStartGame;
        return item;
    }
    public static ProtocolItem Mk_A2C_ResponseStartGame()
    {
        ProtocolItem item = new ProtocolItem();
        item.msgType = ProtocolType.A2C_ResponseStartGame;
        return item;
    }
    //
    // ヘルパー
    //
    public static ProtocolItem MakeToJson(string obj)
    {
        return JsonUtility.FromJson<ProtocolItem>(obj);
    }

    public static string SerializeToJson(ProtocolItem item)
    {
        return JsonUtility.ToJson(item);
    }

    public static void DebugDeserializeProtocol(ProtocolItem item)
    {
        switch (item.msgType)
        {
            case ProtocolType.SimpleMsg:
                Debug.Log("DeserializeProtocol" + item.sendFrom + " " + item.stringParam);
                break;
            case ProtocolType.Position:
                Debug.Log("DeserializeProtocol" + item.sendFrom + " " + item.vectorParam_1);
                break;

        }
    }
}
