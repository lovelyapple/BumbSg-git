using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class UIFieldMenu
{
    void UpdatePlay()
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
            lineLeftLabel.text = "+" + leftTmp;
        }
        else
        {
            lineLeftLabel.text = "";
        }
    }
}
