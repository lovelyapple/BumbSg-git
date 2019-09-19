using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class SocketClientBase
{
    //
    // Hostと登録
    //
    public Action<ProtocolItem> On_A2C_RegisterAsHost = (item) => { };
    public void C2A_RegisterAsHost()
    {
        var Item = ProtocolMaker.Mk_C2A_RegisterHost();
        var json = ProtocolMaker.SerializeToJson(Item);
        var bytes = StrToByteArray(json);

        if (stream.CanWrite)
        {
            ClientBaseDebugLog("writed RegisterAsHost");
            stream.Write(bytes, 0, bytes.Length);
            stream.Flush();
        }
    }
    void A2C_RegisterAsHost(ProtocolItem item)
    {
        ClientBaseDebugLog("A2C_RegisterAsHost id " + item.objectId + " result " + item.boolParam);
        if (item.boolParam)
        {
            IsGameHost = true;
            ClientObjectID = item.objectId;

            if (On_A2C_RegisterAsHost != null)
                On_A2C_RegisterAsHost.Invoke(item);
        }
    }
    //
    // Clientとして登録
    //
    public Action<ProtocolItem> On_A2C_RegisterAsClient = (item) => { };
    public void C2A_RegisterAsClient()
    {
        var Item = ProtocolMaker.Mk_C2A_RegisterClient();
        var json = ProtocolMaker.SerializeToJson(Item);
        var bytes = StrToByteArray(json);

        if (stream.CanWrite)
        {
            ClientBaseDebugLog("writed RegisterAsClient");
            stream.Write(bytes, 0, bytes.Length);
            stream.Flush();
        }
    }
    void A2C_RegisterAsClient(ProtocolItem item)
    {
        ClientBaseDebugLog("A2C_RegisterAsClient id " + item.objectId + " result " + item.boolParam);
        if (item.boolParam)
        {
            IsGameHost = false;
            ClientObjectID = item.objectId;

            if (On_A2C_RegisterAsClient != null)
                On_A2C_RegisterAsClient.Invoke(item);
        }
    }
}
