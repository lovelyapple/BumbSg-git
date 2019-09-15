using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIFieldMenu : MonoBehaviour
{
    [SerializeField] GameObject readyRootObj;
    [SerializeField] GameObject playRootObj;
    [SerializeField] GameObject goalRootObj;
    [SerializeField] Text lineLeftLabel;
    [SerializeField] GameObject[] lineLeftObj;
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
    public void SetupAsReady()
    {
        readyRootObj.SetActive(true);
        playRootObj.SetActive(false);
        goalRootObj.SetActive(false);
    }
    public void SetupAsPlay()
    {
        readyRootObj.SetActive(false);
        playRootObj.SetActive(true);
        goalRootObj.SetActive(false);
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
