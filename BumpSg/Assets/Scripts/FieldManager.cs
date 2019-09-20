using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum GameState
{
    Title = 1,
    Ready = 2,
    Play = 3,
    Goal = 4,
}
public partial class FieldManager : MonoBehaviour
{
    public bool IsInifinityLineMode;
    public GameServer gameServer;
    public SocketClientBase gameClient;
    static FieldManager _instance;
    public static FieldManager GetInstance()
    {
        if (_instance == null)
        {
            return null;
        }

        return _instance;
    }
    public void SetupInstace()
    {
        var fieldRoot = GameObject.Find("FieldRoot");

        if (fieldRoot == null)
        {
            return;
        }

        _instance = fieldRoot.GetComponent<FieldManager>();

        fieldMenu.gameObject.SetActive(true);
    }
    [SerializeField] UIFieldMenu fieldMenu;
    [SerializeField] Camera targetCamera;
    [SerializeField] GameObject linePrefab;
    [SerializeField] LineController creatingLine;
    [SerializeField] LineController selectedLine;
    int _lineLeft = 4;
    public int LineLeft
    {
        get
        {
            if (IsInifinityLineMode)
            {
                return 10;
            }

            if (_lineLeft <= 0)
            {
                return 0;
            }

            return _lineLeft;
        }

        set
        {
            if (IsInifinityLineMode)
            {
                _lineLeft = 10;
            }
            if (value <= 0)
            {
                _lineLeft = 0;
            }
            else
            {
                _lineLeft = value;
            }
        }
    }
    public List<LineController> selfLineList = new List<LineController>();
    [SerializeField] BallController ballPrefab;
    [SerializeField] Vector3 ballStartPosition;
    [SerializeField] BallController ball;
    public BallController Ball { get { return ball; } }

    [SerializeField] float cameraDepth = 5;
    [SerializeField] float lineWidth;
    [SerializeField] float lineDepth;
    Vector3 startPoint;
    [SerializeField] Vector3 lastMousePointGet;
    bool isChargeMode;
    bool nextChargePointIsStart;
    public GameState gameState;
    public GameState requestingState;
    public bool needChangeState;
    public Action<int> onUpdateLineLeftAcount;
    void Awake()
    {
        SetupInstace();
        gameServer.InitializeGameServer();
        gameClient.InitializeGameClient();

        if (targetCamera == null)
        {
            targetCamera = Camera.main;
        }

        UpdateGameState(GameState.Title);
        fieldMenu.Setup(targetCamera);
        fieldMenu.SetupAsTitle();
    }
    void Start()
    {
        var pos = targetCamera.transform.position;
        pos.z = -cameraDepth;
        targetCamera.transform.position = pos;
        targetCameraPos = pos;
    }
    void Update()
    {
        if (!needChangeState && requestingState != gameState)
        {
            needChangeState = true;
            return;
        }

        if (needChangeState)
        {
            UpdateGameState(requestingState);
        }

        switch (gameState)
        {
            case GameState.Title:
                if (ball == null)
                {
                    ball = Instantiate(ballPrefab.gameObject, ballStartPosition, Quaternion.identity).GetComponent<BallController>();
                    ball.OnBallFallingInToHole = OnBallFallInToHole;
                }

                ball.gameObject.SetActive(false);
                break;
            case GameState.Ready:
                break;
            case GameState.Play:
                UpdateInteractive();
                UpdateTargetCameraTransform();
                break;
            case GameState.Goal:
                break;
        }
    }
    public void RequestUpdateGameStateAsync(GameState state)
    {
        requestingState = state;
    }
    public void UpdateGameState(GameState state)
    {
        this.gameState = state;
        requestingState = gameState;
        needChangeState = false;

        switch (gameState)
        {
            case GameState.Title:
                fieldMenu.SetupAsTitle();
                break;
            case GameState.Ready:
                fieldMenu.SetupAsReady();
                break;
            case GameState.Play:
                ball.gameObject.SetActive(true);

                try
                {
                    fieldMenu.SetupAsPlay(ball.gameObject.transform);
                }
                catch
                {
                    Debug.LogError("fieldMenu.SetupAsPlay(ball.gameObject.transform);");
                }
                break;
            case GameState.Goal:
                fieldMenu.SetupAsGoal();
                break;
        }
    }

    void UpdateLineObj(GameObject lineObj, Vector3 start, Vector3 end)
    {
        lineObj.transform.position = (start + end) / 2;
        lineObj.transform.right = (end - start).normalized;
        lineObj.transform.localScale = new Vector3((end - start).magnitude, lineWidth, lineDepth);
    }
    Vector3 GetMouseCameraPoint()
    {
        var ray = targetCamera.ScreenPointToRay(Input.mousePosition);
        var rayTimes = cameraDepth / Mathf.Abs(ray.direction.normalized.z);
        lastMousePointGet = ray.origin + ray.direction * rayTimes;
        lastMousePointGet.z = 0;//最後の調整としてズレ修正
        return lastMousePointGet;
    }
    GameObject CheckIsHoveringLine()
    {
        var ray = targetCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, cameraDepth * 2))
        {
            if (hit.collider.gameObject.name == "HoverArea")
            {
                return hit.collider.transform.parent.gameObject;
            }
        }

        return null;
    }
    public void RemoveLine(LineController line)
    {
        if (selfLineList.Remove(line))
        {
            LineLeft++;

            if (onUpdateLineLeftAcount != null)
            {
                onUpdateLineLeftAcount.Invoke(LineLeft);
            }

            Debug.Log("success remove line");
        }
    }
    public LineController TrySelectOneLine(Vector3 point)
    {
        var lineCnt = selfLineList.Count;

        return null;
    }
    public void OnBallFallInToHole(BallController ballController)
    {
        ball.ResetBall();
        targetCamera.transform.position = new Vector3(0, 0, -cameraDepth);
    }
}
