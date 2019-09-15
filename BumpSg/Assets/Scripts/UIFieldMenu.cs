using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIFieldMenu : MonoBehaviour
{
    [SerializeField] RectTransform fieldMenuTransform;
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
        if (ballTransform != null && targetCamera != null)
        {
            ballPanelPos = targetCamera.WorldToViewportPoint(ballTransform.position);

            if (ballPanelPos.x < 0 || ballPanelPos.x > 1 || ballPanelPos.y < 0 || ballPanelPos.y > 1)
            {
                ballDirectionObj.SetActive(true);
                var panelViewPortPos = ballPanelPos;
                panelViewPortPos.x = Mathf.Clamp(panelViewPortPos.x, 0, 1f);
                panelViewPortPos.y = Mathf.Clamp(panelViewPortPos.y, 0, 1f);

                var panelViewPos = Vector3.zero;
                panelViewPos.x = fieldMenuTransform.rect.width * panelViewPortPos.x - fieldMenuTransform.rect.width / 2;
                panelViewPos.y = fieldMenuTransform.rect.height * panelViewPortPos.y - fieldMenuTransform.rect.height / 2;

                ballDirectionObj.transform.localPosition = panelViewPos;
            }
            else
            {
                ballDirectionObj.SetActive(false);
            }
        }

    }
    public void Setup(Camera targetCamera)
    {
        this.targetCamera = targetCamera;
    }
    public void SetupAsReady()
    {
        readyRootObj.SetActive(true);
        playRootObj.SetActive(false);
        goalRootObj.SetActive(false);
    }
    public void SetupAsPlay(Transform ballTransform)
    {
        readyRootObj.SetActive(false);
        playRootObj.SetActive(true);
        goalRootObj.SetActive(false);
        this.ballTransform = ballTransform;
    }
    public void SetupAsGoal()
    {
        readyRootObj.SetActive(false);
        playRootObj.SetActive(false);
        goalRootObj.SetActive(true);
    }
    public void OnClickStart()
    {
        FieldManager.GetInstance().UpdateGameState(GameState.Play);
    }
    public void OnClickReStart()
    {
        FieldManager.GetInstance().UpdateGameState(GameState.Ready);
    }
    public void UpdateLineLeftAmount(int left)
    {
        if (FieldManager.GetInstance().IsInifinityLineMode)
        {
            for (int i = 0; i < lineLeftObj.Length; i++)
            {
                lineLeftObj[i].SetActive(true);
            }

            lineLeftLabel.text = "∞";
            return;
        }
        var leftTmp = left;
        for (int i = 0; i < lineLeftObj.Length; i++)
        {
            if (i < left)
            {
                lineLeftObj[i].SetActive(true);
                leftTmp--;
            }
            else
            {
                lineLeftObj[i].SetActive(false);
            }
        }

        if (leftTmp > 0)
        {
            lineLeftLabel.text = "x" + leftTmp;
        }
        else
        {
            lineLeftLabel.text = "";
        }
    }
}
