using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pauser : MonoBehaviour
{
    [SerializeField] private Canvas _canvas;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (_canvas.enabled)
                _canvas.enabled = false;
            else
                _canvas.enabled = true;
        }
           
    }
}
