using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FieldMenu : MonoBehaviour
{
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
    public void UpdateLineLeftAmount(int left)
    {
        if (FieldManager.IsInifinityLineMode)
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
