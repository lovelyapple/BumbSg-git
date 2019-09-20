using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public partial class UIFieldMenu : MonoBehaviour
{
    [SerializeField] RectTransform fieldMenuTransform;
    [SerializeField] GameObject titleRootObj;
    [SerializeField] GameObject readyRootObj;
    [SerializeField] GameObject playRootObj;
    [SerializeField] GameObject goalRootObj;
    [SerializeField] Text lineLeftLabel;
    [SerializeField] GameObject[] lineLeftObj;
    [SerializeField] GameObject ballDirectionObj;
    [SerializeField] Vector3 ballPanelPos;
    public Transform ballTransform;
    public Camera targetCamera;
    void OnEnable()
    {
        FieldManager.GetInstance().onUpdateLineLeftAcount += UpdateLineLeftAmount;
        UpdateLineLeftAmount(FieldManager.GetInstance().LineLeft);
    }

    void OnDisable()
    {
        if (FieldManager.GetInstance() != null)
        {
            FieldManager.GetInstance().onUpdateLineLeftAcount -= UpdateLineLeftAmount;
        }
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
        titleRootObj.SetActive(true);
        readyRootObj.SetActive(false);
        playRootObj.SetActive(false);
        goalRootObj.SetActive(false);
        SocketClientBase.IsGameHost = false;
    }
    public void SetupAsReady()
    {
        StopServerButtonObj.SetActive(SocketClientBase.IsGameHost);
        titleRootObj.SetActive(false);
        readyRootObj.SetActive(true);
        playRootObj.SetActive(false);
        goalRootObj.SetActive(false);
    }
    public void SetupAsPlay(Transform ballTransform)
    {
        titleRootObj.SetActive(false);
        readyRootObj.SetActive(false);
        playRootObj.SetActive(true);
        goalRootObj.SetActive(false);
        this.ballTransform = ballTransform;
    }
    public void SetupAsGoal()
    {
        titleRootObj.SetActive(false);
        readyRootObj.SetActive(false);
        playRootObj.SetActive(false);
        goalRootObj.SetActive(true);
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

        if(waitForRegisterCoroutine != null)
        {
            StopCoroutine(waitForRegisterCoroutine);
        }
        waitForRegisterCoroutine = null;
    }
}
