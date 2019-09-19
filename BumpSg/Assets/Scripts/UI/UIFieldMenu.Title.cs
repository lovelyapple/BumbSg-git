using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public partial class UIFieldMenu
{
    [SerializeField] Text ipLabel;
    [SerializeField] Text portLabel;
    [SerializeField] InputField ipInputField;
    void UpdateTitle()
    {
        
    }
    public string GetInputingIp()
    {
        return ipInputField.text;
    }
}
