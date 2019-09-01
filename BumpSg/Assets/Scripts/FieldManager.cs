using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class FieldManager : MonoBehaviour
{
    public static bool IsInifinityLineMode;
    static FieldManager _instance;
    public static FieldManager GetInstance()
    {
        if (_instance == null)
        {
            var fieldRoot = GameObject.Find("FieldRoot");

            _instance = fieldRoot.GetComponent<FieldManager>();
        }

        return _instance;
    }
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
    public Action<int> onUpdateLineLeftAcount;

    void Awake()
    {
        if (targetCamera == null)
        {
            targetCamera = Camera.main;
        }

        if (ball == null)
        {
            ball = Instantiate(ballPrefab, ballStartPosition, Quaternion.identity);
        }

        var pos = targetCamera.transform.position;
        pos.z = -cameraDepth;
        targetCamera.transform.position = pos;
    }
    void Update()
    {

        if (Input.GetMouseButtonDown(0))
        {
            var selectObj = CheckIsHoveringLine();

            // create new
            if (selectObj == null)
            {
                if (LineLeft > 0)
                {
                    startPoint = GetMouseCameraPoint();
                    var endPoint = startPoint + Vector3.up * 0.01f;

                    var line = Instantiate(linePrefab);
                    UpdateLineObj(line, startPoint, endPoint);
                    creatingLine = line.GetComponent<LineController>();
                    selfLineList.Add(creatingLine);
                    LineLeft--;

                    if (onUpdateLineLeftAcount != null)
                    {
                        onUpdateLineLeftAcount.Invoke(LineLeft);
                    }
                }

                isChargeMode = false;
            }
            // selectOne
            else
            {
                selectedLine = selectObj.GetComponent<LineController>();

                // 万が一
                if (selectedLine == null)
                {
                    Destroy(selectObj);
                    return;
                }

                isChargeMode = true;
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            if (creatingLine != null)
            {
                creatingLine = null;
            }

            if (selectedLine != null)
            {
                selectedLine = null;
            }

            isChargeMode = false;
            nextChargePointIsStart = false;
        }
        else
        {
            if (isChargeMode)
            {
                if (selectedLine == null || selectedLine.IsDead)
                {
                    isChargeMode = false;
                    return;
                }

                var endPoint = GetMouseCameraPoint();

                if (nextChargePointIsStart)
                {
                    if (selectedLine.CheckPointRange(endPoint, true))
                    {
                        nextChargePointIsStart = false;
                    }
                }
                else
                {
                    if (selectedLine.CheckPointRange(endPoint, false))
                    {
                        nextChargePointIsStart = true;
                        selectedLine.PowerUpOneRound();
                    }
                }
            }
            else
            {
                if (creatingLine == null || creatingLine.IsDead) return;

                var endPoint = GetMouseCameraPoint();
                UpdateLineObj(creatingLine.gameObject, startPoint, endPoint);
                creatingLine.Setup(startPoint, endPoint);
            }
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
}
