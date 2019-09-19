﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public partial class UIFieldMenu
{
    [SerializeField] Text isServerStartedLabel;
    [SerializeField] Text gameStartButtonLabel;
    Coroutine waitForRegisterCoroutine;
    bool selfReady;
    void UpdateReady()
    {

        if (FieldManager.IsHost && !SocketClientBase.GetInstance().SelfClientObjectID.HasValue && waitForRegisterCoroutine == null)
        {
            waitForRegisterCoroutine = StartCoroutine(AutoRegisterHostIenumerator());
        }

        if (FieldManager.IsHost)
        {
            if(selfReady)
            {
                gameStartButtonLabel.text = SocketClientBase.GetInstance().EnemyClientObjectID.HasValue ? "Start" : "WaitForPlayers";
            }
            else
            {
                gameStartButtonLabel.text =  "Crea";
            }
            
        }
        else
        {
            gameStartButtonLabel.text = SocketClientBase.GetInstance().EnemyClientObjectID.HasValue ? "Start" : "WaitForPlayers";
        }
    }
    IEnumerator AutoRegisterHostIenumerator()
    {
        selfReady = false;
        var client = SocketClientBase.GetInstance();

        Debug.Log("AutoRegisterHostIenumerator as Host ");


        while (!GameServer.IsServerStarted())
        {
            yield return null;
        }

        Debug.Log("AutoRegisterHostIenumerator IsServerStarted ");
        yield return new WaitForSeconds(0.1f);


        client.ConnectToServer(ipLabel.text);

        while (client.Client == null && !client.Client.Connected)
        {
            yield return null;
        }

        Debug.Log("AutoRegisterHostIenumerator Client.Connected ");
        yield return new WaitForSeconds(0.1f);


        SocketClientBase.GetInstance().C2A_RegisterAsHost();


        while (!SocketClientBase.GetInstance().SelfClientObjectID.HasValue)
        {
            yield return null;
        }

        Debug.Log("Got ObjectID " + SocketClientBase.GetInstance().SelfClientObjectID + " As Host");

        waitForRegisterCoroutine = null;
        selfReady = true;
    }
    IEnumerator ManualRegisterClientIenumerator()
    {
        selfReady = false;
        var client = SocketClientBase.GetInstance();
        Debug.Log("ManualRegisterClientIenumerator as Client ");

        client.ConnectToServer(ipInputField.text);

        while (client.Client == null && !client.Client.Connected)
        {
            yield return null;
        }

        yield return new WaitForSeconds(0.1f);

        Debug.Log("ManualRegisterClientIenumerator Client.Connected ");

        SocketClientBase.GetInstance().C2A_RegisterAsClient();

        while (!SocketClientBase.GetInstance().SelfClientObjectID.HasValue)
        {
            yield return null;
        }

        Debug.Log("Got ObjectID " + SocketClientBase.GetInstance().SelfClientObjectID + " As Client");
        waitForRegisterCoroutine = null;
        selfReady = true;
    }
}