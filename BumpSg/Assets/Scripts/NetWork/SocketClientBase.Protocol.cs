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
            Debug.Log("writed RegisterAsHost");
            stream.Write(bytes, 0, bytes.Length);
            stream.Flush();
        }
    }
    void A2C_RegisterAsHost(ProtocolItem item)
    {
        On_A2C_RegisterAsHost.Invoke(item);
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
            Debug.Log("writed RegisterAsClient");
            stream.Write(bytes, 0, bytes.Length);
            stream.Flush();
        }
    }
    void A2C_RegisterAsClient(ProtocolItem item)
    {
        On_A2C_RegisterAsClient.Invoke(item);
    }
}
