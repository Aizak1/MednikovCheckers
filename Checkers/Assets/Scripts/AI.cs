using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AI : MonoBehaviour
{
    private List<Checker> checkers;
    private int _blackCount;
    private int _whiteCount;
    private void Start()
    {
        RefreshListsofCheckers();
    }

    public void RefreshListsofCheckers()
    {
        checkers = FindObjectsOfType<Checker>().ToList();
    }

 
}
