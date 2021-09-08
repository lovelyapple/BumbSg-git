using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class FieldManager : MonoBehaviour
{
    [SerializeField] Vector3 targetCameraPos;
    [SerializeField] float cameraMoveSpeed;
    [SerializeField] float cameraMoveSpeedAcceleration = 0.95f;
    [SerializeField] Vector3 prevMosePosition;

    void UpdateTargetCameraTransform()
    {
        if ((targetCameraPos - targetCamera.transform.position).sqrMagnitude > float.Epsilon)
        {
            targetCamera.transform.position = Vector3.Lerp(targetCamera.transform.position, targetCameraPos, cameraMoveSpeedAcceleration * Time.deltaTime);
        }
    }
    void UpdateInteractive()
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
                    creatingLine = line.GetComponent<LineController>();
                    UpdateLineObj(creatingLine, startPoint, endPoint);
                    selfLineList.Add(creatingLine);
                    DecreaseLineCount();
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
                LineLenghtLeft -= creatingLine.length;
                SocketClientBase.GetInstance().C2A_UpdateLine(SocketClientBase.GetInstance().SelfClientObjectID.Value, creatingLine, true);
                creatingLine = null;
            }

            if (selectedLine != null)
            {
                selectedLine = null;
            }

            isChargeMode = false;
            nextChargePointIsStart = false;
        }
        else if (Input.GetMouseButtonDown(1))
        {
            prevMosePosition = Input.mousePosition;
            targetCameraPos = targetCamera.transform.position;
        }
        else if (Input.GetMouseButton(1))
        {
            var currentPos = Input.mousePosition;

            var offSet = currentPos - prevMosePosition;
            offSet.z = 0;

            if (offSet.sqrMagnitude > float.Epsilon)
            {
                offSet.x = -offSet.x;
                offSet.y = -offSet.y;

                Debug.Log(offSet);
                targetCameraPos += offSet * cameraMoveSpeed * Time.deltaTime;

                if (targetCameraPos.x > 50)
                {
                    targetCameraPos.x = 50;
                }
                else if (targetCameraPos.x < -50)
                {
                    targetCameraPos.x = -50;
                }

                if (targetCameraPos.y > 30)
                {
                    targetCameraPos.y = 30;
                }
                else if (targetCameraPos.y < -30)
                {
                    targetCameraPos.y = -30;
                }
            }

            targetCameraPos.z = -cameraDepth;

            prevMosePosition = currentPos;
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
                        selectedLine.PowerUpOneRound();
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
                creatingLine.Setup(startPoint, endPoint, GetNextSelfLineId(), true);
                UpdateLineObj(creatingLine, startPoint, endPoint);
            }
        }
    }
    void UpdateLineObj(LineController line, Vector3 start, Vector3 end)
    {
        var lineObj = line.gameObject;
        var lenghtLeft = LineLenghtLeft;
        var diff = (end - start);
        var dir = diff.normalized;

        if (diff.magnitude > lenghtLeft)
        {
            end = start + dir * lenghtLeft;
        }

        diff = (end - start);
        if (diff.magnitude > max_line_length)
        {
            end = start + dir * max_line_length;
        }

        diff = (end - start);
        // 残りの分量をリアルタイムに反映
        lenghtLeft = Mathf.Max(lenghtLeft - diff.magnitude, 0);

        if (onUpdateLineLengthLeft != null)
        {
            onUpdateLineLengthLeft(lenghtLeft / max_line_length_total);
        }

        lineObj.transform.position = (start + end) / 2;
        lineObj.transform.right = (end - start).normalized;
        lineObj.transform.localScale = new Vector3((end - start).magnitude, lineWidth, lineDepth);
        line.UpdateStartEnd(start, end);
    }
    public void DecreaseLineCount()
    {
        LineLeft--;

        if (onUpdateLineLeftAcount != null)
        {
            onUpdateLineLeftAcount.Invoke(LineLeft);
        }
    }
    public void AddLineCount()
    {
        LineLeft++;

        if (onUpdateLineLeftAcount != null)
        {
            onUpdateLineLeftAcount.Invoke(LineLeft);
        }
    }
    List<LineController> remoteLineCtrlList = new List<LineController>();
    public void OnRemoteLineCreated(int id, Vector3 pos, Vector3 dir, Vector3 scl)
    {
        Debug.LogError("OnRemoteLineCreated");
        var line = Instantiate(enemyLinePrefab);
        var ctrl = line.GetComponent<LineController>();
        ctrl.lineId = id;
        ctrl.isLocal = false;
        ctrl.transform.position = pos;
        ctrl.transform.eulerAngles = dir;
        ctrl.transform.localScale = scl;
        remoteLineCtrlList.Add(ctrl);
    }
    public void OnRemoteLineDead(int lineId)
    {
        var ctrl = remoteLineCtrlList.Find(x => x.lineId == lineId);

        if (ctrl != null)
        {
            remoteLineCtrlList.Remove(ctrl);
            ctrl.SetLineDead();
        }

        Debug.LogError("OnRemoteLineDead");
    }
}
