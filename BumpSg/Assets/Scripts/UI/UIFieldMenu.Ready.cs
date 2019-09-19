using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public partial class UIFieldMenu
{
    [SerializeField] Text isServerStartedLabel;
    Coroutine waitForRegisterCoroutine;
    void UpdateReady()
    {
        isServerStartedLabel.gameObject.SetActive(GameServer.IsServerStarted());

        if (!SocketClientBase.GetInstance().ClientObjectID.HasValue && waitForRegisterCoroutine == null)
        {
            waitForRegisterCoroutine = StartCoroutine(RegisterIenumerator());
        }
    }
    IEnumerator RegisterIenumerator()
    {
        var client = SocketClientBase.GetInstance();

        if (FieldManager.IsHost)
        {
            while (!GameServer.IsServerStarted())
            {
                yield return null;
            }
        }

        client.ConnectToServer(ipLabel.text);

        while (client.Client == null && !client.Client.Connected)
        {
            yield return null;
        }

        if (FieldManager.IsHost)
        {
            SocketClientBase.GetInstance().C2A_RegisterAsHost();
        }
        else
        {
            SocketClientBase.GetInstance().C2A_RegisterAsClient();
        }

        while (!SocketClientBase.GetInstance().ClientObjectID.HasValue)
        {
            yield return null;
        }

        Debug.Log("Got ObjectID " + SocketClientBase.GetInstance().ClientObjectID + " As Host " + FieldManager.IsHost);

        waitForRegisterCoroutine = null;
    }
}
