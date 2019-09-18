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

        switch (FieldManager.GetInstance().gameState)
        {
            case GameState.Title:
                UpdateTitle();
                break;
            case GameState.Ready:
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
        ipLabel.text = GameServer.GetLocalIPAddress();
        titleRootObj.SetActive(true);
        readyRootObj.SetActive(false);
        playRootObj.SetActive(false);
        goalRootObj.SetActive(false);
        FieldManager.IsHost = false;
    }
    public void SetupAsReady()
    {
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
        FieldManager.IsHost = true;
    }
    public void OnClickStartClientInTitle()
    {
        FieldManager.IsHost = false;
    }
    public void OnClickStopServerInReady()
    {
        GameServer.GetInstance().StopServer();
    }
}
