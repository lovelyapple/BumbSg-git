using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public partial class UIFieldMenu
{
    [SerializeField] Text ipLabel;
    [SerializeField] Text portLabel;
    [SerializeField] InputField ipInputField;
    [SerializeField] string prevIp;
    void UpdateTitle()
    {

    }
    public string GetInputingIp()
    {
        if (prevIp != ipInputField.text)
        {
            PlayerPrefs.SetString("TargetServerIP", ipInputField.text);
            prevIp = ipInputField.text;
        }
        return ipInputField.text;
    }
}
