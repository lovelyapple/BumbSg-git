using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class FieldManager : MonoBehaviour
{
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
    [SerializeField] LineController currentLine;

    [SerializeField] BallController ballPrefab;
    [SerializeField] Vector3 ballStartPosition;
    [SerializeField] BallController ball;
    public BallController Ball { get { return ball; } }

    [SerializeField] float cameraDepth = 5;
    [SerializeField] float lineWidth;
    [SerializeField] float lineDepth;
    Vector3 startPoint;

    [SerializeField] Vector3 lastMousePointGet;

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
            startPoint = GetMouseCameraPoint();
            var endPoint = startPoint + Vector3.up * 0.01f;

            var line = Instantiate(linePrefab);
            UpdateLineObj(line, startPoint, endPoint);
            currentLine = line.GetComponent<LineController>();
        }
        else if (Input.GetMouseButtonUp(0))
        {
            if (currentLine != null)
            {
                currentLine = null;
            }
        }
        else
        {
            if (currentLine == null || currentLine.IsDead) return;

            var endPoint = GetMouseCameraPoint();
            UpdateLineObj(currentLine.gameObject, startPoint, endPoint);
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
}
