﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public partial class UIFieldMenu : MonoBehaviour
{
    [SerializeField] RectTransform fieldMenuTransform;
    [SerializeField] GameObject ipConfigInfoRootObj;
    [SerializeField] GameObject titleRootObj;
    [SerializeField] GameObject readyRootObj;
    [SerializeField] GameObject playRootObj;
    [SerializeField] GameObject goalRootObj;
    [SerializeField] Text lineLeftLabel;
    [SerializeField] GameObject[] lineLeftObj;
    [SerializeField] Slider lengthLeftSlider;
    [SerializeField] GameObject ballDirectionObj;
    [SerializeField] Vector3 ballPanelPos;
    [SerializeField] bool soloMode;
    public Transform ballTransform;
    public Camera targetCamera;
    void OnEnable()
    {
        FieldManager.GetInstance().onUpdateLineLeftAcount += UpdateLineLeftAmount;
        FieldManager.GetInstance().onUpdateLineLengthLeft += UpdateSliderLeft;
        UpdateLineLeftAmount(FieldManager.GetInstance().LineLeft);
    }

    void OnDisable()
    {
        if (FieldManager.GetInstance() != null)
        {
            FieldManager.GetInstance().onUpdateLineLeftAcount -= UpdateLineLeftAmount;
            FieldManager.GetInstance().onUpdateLineLengthLeft -= UpdateSliderLeft;
        }
    }
    void Start()
    {
        ipInputField.text = PlayerPrefs.GetString("TargetServerIP", "192.168.00.00");
        prevIp = ipInputField.text;
    }
    void Update()
    {
        if (FieldManager.GetInstance() == null)
        {
            return;
        }
        isServerStartedLabel.gameObject.SetActive(GameServer.IsServerStarted());
        switch (FieldManager.GetInstance().gameState)
        {
            case GameState.Title:
                UpdateTitle();
                break;
            case GameState.Ready:
                UpdateReady();
                break;
            case GameState.Play:
                UpdatePlay();
                break;
            case GameState.Goal:
                UpdateResult();
                break;
        }
    }
    public void Setup(Camera targetCamera)
    {
        this.targetCamera = targetCamera;
    }
    public void SetupAsTitle()
    {
        selfReady = false;
        ipLabel.text = GameServer.GetLocalIPAddress();
        portLabel.text = GameServer.GetInstance().Port.ToString();
        ipConfigInfoRootObj.SetActive(true);
        titleRootObj.SetActive(true);
        readyRootObj.SetActive(false);
        playRootObj.SetActive(false);
        goalRootObj.SetActive(false);
        SocketClientBase.IsGameHost = false;
    }
    public void SetupAsReady()
    {
        StopServerButtonObj.SetActive(SocketClientBase.IsGameHost);
        ipConfigInfoRootObj.SetActive(true);
        titleRootObj.SetActive(false);
        readyRootObj.SetActive(true);
        playRootObj.SetActive(false);
        goalRootObj.SetActive(false);
    }
    public void SetupAsPlay(Transform ballTransform)
    {
        Debug.Log("SetupAsPlay");
        ipConfigInfoRootObj.SetActive(false);
        titleRootObj.SetActive(false);
        readyRootObj.SetActive(false);
        playRootObj.SetActive(true);
        goalRootObj.SetActive(false);
        this.ballTransform = ballTransform;
    }
    public void SetupAsGoal()
    {
        ipConfigInfoRootObj.SetActive(false);
        titleRootObj.SetActive(false);
        readyRootObj.SetActive(false);
        playRootObj.SetActive(false);
        goalRootObj.SetActive(true);
        restartButtonObj.SetActive(SocketClientBase.IsGameHost);

        if (SocketClientBase.GetInstance().WinObjectID.Value == SocketClientBase.GetInstance().SelfClientObjectID.Value)
        {
            winLabel.text = "あなたの勝だよ！";
        }
        else
        {
            winLabel.text = "相手の勝だよ！";
        }
    }
    public void OnClickCreateServerInTitle()
    {
        GameServer.GetInstance().StartServer();
        FieldManager.GetInstance().UpdateGameState(GameState.Ready);
        SocketClientBase.IsGameHost = true;
    }
    public void OnClickStartClientInTitle()
    {
        GameServer.GetInstance().StopServer();
        FieldManager.GetInstance().UpdateGameState(GameState.Ready);
        SocketClientBase.IsGameHost = false;

        if (waitForRegisterCoroutine != null)
        {
            StopCoroutine(waitForRegisterCoroutine);
        }

        waitForRegisterCoroutine = null;

        waitForRegisterCoroutine = StartCoroutine(ManualRegisterClientIenumerator());
    }
    public void OnClickStopServerInReady()
    {
        GameServer.GetInstance().StopServer();
        SocketClientBase.GetInstance().StopClient();
        FieldManager.GetInstance().UpdateGameState(GameState.Title);
        SocketClientBase.IsGameHost = false;

        if (waitForRegisterCoroutine != null)
        {
            StopCoroutine(waitForRegisterCoroutine);
        }
        waitForRegisterCoroutine = null;
    }
    public void OnClickStartGameInReady()
    {
        if (!SocketClientBase.IsGameHost || !SocketClientBase.GetInstance().HostClientObjectID.HasValue || !SocketClientBase.GetInstance().GuestClientObjectID.HasValue)
        {
            if (!soloMode)
                return;
        }

        SocketClientBase.GetInstance().C2A_RequestStartGame();
    }
    public void OnClickReplayInResult()
    {
        OnClickStartGameInReady();
    }
}
