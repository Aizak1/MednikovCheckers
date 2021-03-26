using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuSwitcher : MonoBehaviour
{
    [SerializeField] Canvas _startMenu;
    [SerializeField] Canvas _ruleMenu;

    public void SwitchMenu()
    {
        if (_startMenu.enabled)
        {
            _startMenu.enabled = false;
            _ruleMenu.enabled = true;
        }
        else
        {
            _startMenu.enabled = true;
            _ruleMenu.enabled = false;
        }
    }
}
