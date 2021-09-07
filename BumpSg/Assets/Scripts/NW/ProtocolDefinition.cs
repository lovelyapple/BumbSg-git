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


    C2A_AddForceToBall = 8,
    A2C_AddForceToBall = 9,

    C2A_UpdateLine = 10,
    A2C_UpdateLine = 11,

    C2A_GameResult = 100,
    A2C_GameResult = 101,
}
[Serializable]
public struct vector_3
{
    public int x;
    public int y;
    public int z;
}
[Serializable]
public class ProtocolItem
{
    public int sendFrom;
    public int sendTo;
    public ProtocolType msgType;
    public int objectId_1;
    public int objectId_2;
    public vector_3 vectorParam_1;
    public vector_3 vectorParam_2;
    public vector_3 vectorParam_3;
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
        item.vectorParam_1 = FormartVector3ToVector_3(vector);
        item.msgType = ProtocolType.Position;
        return item;
    }
    public static vector_3 FormartVector3ToVector_3(Vector3 vector)
    {
        return new vector_3()
        {
            x = (int)Mathf.Floor(vector.x * 1000),
            y = (int)Mathf.Floor(vector.y * 1000),
            z = (int)Mathf.Floor(vector.z * 1000),
        };
    }
    public static Vector3 FormartVector_3ToVector3(vector_3 vector)
    {
        return new Vector3()
        {
            x = vector.x / 1000f,
            y = vector.y / 1000f,
            z = vector.z / 1000f,
        };
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
    // Ball
    //
    public static ProtocolItem Mk_C2A_AddForceToBall(int sendFrom, Vector3 dir, Vector3 pos, Vector3 velocity)
    {
        ProtocolItem item = new ProtocolItem();
        item.sendFrom = sendFrom;
        item.msgType = ProtocolType.C2A_AddForceToBall;
        item.vectorParam_1 = FormartVector3ToVector_3(dir);
        item.vectorParam_2 = FormartVector3ToVector_3(pos);
        item.vectorParam_3 = FormartVector3ToVector_3(velocity);
        // item.vectorParam_1.x = Mathf.Floor(dir.x * 1000) / 1000f;
        // item.vectorParam_1.y = Mathf.Floor(dir.y * 1000) / 1000f;
        // item.vectorParam_1.z = Mathf.Floor(dir.z * 1000) / 1000f;
        // item.vectorParam_2.x = Mathf.Floor(pos.x * 1000) / 1000f;
        // item.vectorParam_2.y = Mathf.Floor(pos.y * 1000) / 1000f;
        // item.vectorParam_2.z = Mathf.Floor(pos.z * 1000) / 1000f;
        return item;
    }
    public static ProtocolItem Mk_A2C_AddForceToBall(ProtocolItem c2a_item)
    {
        c2a_item.msgType = ProtocolType.A2C_AddForceToBall;
        return c2a_item;
    }
    public static ProtocolItem Mk_C2A_UpdateLine(int sendFrom, int id, Vector3 pos, Vector3 dir, Vector3 scale, bool isCreate)
    {
        ProtocolItem item = new ProtocolItem();
        item.sendFrom = sendFrom;
        item.objectId_1 = id;
        item.msgType = ProtocolType.C2A_UpdateLine;
        item.vectorParam_1 = FormartVector3ToVector_3(pos);
        item.vectorParam_2 = FormartVector3ToVector_3(dir);
        item.vectorParam_3 = FormartVector3ToVector_3(scale);
        item.boolParam = isCreate;
        return item;
    }
    public static ProtocolItem Mk_A2C_UpdateLine(ProtocolItem c2a_item)
    {
        c2a_item.msgType = ProtocolType.A2C_UpdateLine;
        return c2a_item;
    }

    //
    // Result
    //

    public static ProtocolItem Mk_C2A_GameResult(int objectId)
    {
        ProtocolItem item = new ProtocolItem();
        item.msgType = ProtocolType.C2A_GameResult;
        item.objectId_1 = objectId;

        return item;
    }
    public static ProtocolItem Mk_A2C_GameResult(ProtocolItem item)
    {
        item.msgType = ProtocolType.A2C_GameResult;

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
