using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
public enum ProtocolType
{
    SimpleMsg,
}
[Serializable]
public class ProtocolItem
{
    public int sendFrom;
    public int sendTo;
    public ProtocolType msgType;
    public int objectId;
    public Vector3 vectorParam_1;
    public Vector3 vectorParam_2;
    public float floatParam_1;
    public float floatParam_2;
    public string stringParam;
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
    public static ProtocolItem MakeToJson(string obj)
    {
        return JsonUtility.FromJson<ProtocolItem>(obj);
    }

    public static string SerializeToJson(ProtocolItem item)
    {
        return JsonUtility.ToJson(item);
    }

    public static void DeserializeProtocol(ProtocolItem item)
    {
        switch (item.msgType)
        {
            case ProtocolType.SimpleMsg:
                Debug.Log("DeserializeProtocol" + item.sendFrom + " " + item.stringParam);
                break;
        }
    }
}
