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
            targetCamera.transform.position = Vector3.Lerp(targetCamera.transform.position, targetCameraPos, cameraMoveSpeedAcceleration);
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
        else if (Input.GetMouseButtonDown(1))
        {
            prevMosePosition = GetMouseCameraPoint();
            targetCameraPos = targetCamera.transform.position;
        }
        else if (Input.GetMouseButton(1))
        {
            var currentPos = GetMouseCameraPoint();

            var offSet = currentPos - prevMosePosition;
            offSet.z = 0;

            if (offSet.sqrMagnitude > float.Epsilon)
            {
                offSet.x = -offSet.x;
                offSet.y = -offSet.y;
                targetCameraPos += offSet * cameraMoveSpeed * Time.deltaTime;
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
                UpdateLineObj(creatingLine.gameObject, startPoint, endPoint);
                creatingLine.Setup(startPoint, endPoint);
            }
        }


    }
}
