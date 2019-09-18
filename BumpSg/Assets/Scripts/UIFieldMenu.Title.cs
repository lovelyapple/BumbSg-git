using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public partial class UIFieldMenu
{
    [SerializeField] Text ipLabel;
    [SerializeField] Text isServerStartedLabel;
    [SerializeField] InputField ipInputField;
    void UpdateTitle()
    {
        isServerStartedLabel.gameObject.SetActive(GameServer.IsServerStarted());
    }
    public string GetInputingIp()
    {
        return ipInputField.text;
    }
}
