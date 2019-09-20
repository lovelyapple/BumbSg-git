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
    void A2C_UpdateClientInfo(ProtocolItem item)
    {
        ClientBaseDebugLog("A2C_UpdateClientInfo id " + item.objectId_1 + " result " + item.objectId_2);
        HostClientObjectID = item.objectId_1;
        GuestClientObjectID = item.objectId_2 >= 0? item.objectId_2 : new int?();

        if(IsGameHost)
        {
            SelfClientObjectID = HostClientObjectID;
        }
        else
        {
            SelfClientObjectID = GuestClientObjectID;
        }
    }

    public void C2A_RequestStartGame()
    {
        var Item = ProtocolMaker.Mk_C2A_RequestStartGame();
        var json = ProtocolMaker.SerializeToJson(Item);
        var bytes = StrToByteArray(json);

        if (stream.CanWrite)
        {
            ClientBaseDebugLog("writed C2A_RequestStartGame");
            stream.Write(bytes, 0, bytes.Length);
            stream.Flush();
        }
    }
    public void A2C_ResponseStartGame()
    {
        FieldManager.GetInstance().RequestUpdateGameStateAsync(GameState.Play);
    }
}
