using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class FieldManager
{
    void AttachProtocol()
    {
        gameClient.On_A2C_RegisterAsHost = On_A2C_RegisterAsHost;
        gameClient.On_A2C_RegisterAsClient = On_A2C_RegisterAsClient;
    }
    void DettachProtocol()
    {
        gameClient.On_A2C_RegisterAsHost = null;
        gameClient.On_A2C_RegisterAsClient = null;
    }
    void On_A2C_RegisterAsHost(ProtocolItem item)
    {
    }
    void On_A2C_RegisterAsClient(ProtocolItem item)
    {
    }
}
