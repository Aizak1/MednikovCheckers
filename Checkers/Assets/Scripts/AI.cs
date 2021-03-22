using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class Move
{
    Checker _selectedChecker;
    Vector2Int _startPosition;
    Vector2Int _finalPosition;

    public Checker SelectedChecker => _selectedChecker;
    public Vector2Int StartPosition => _startPosition;

    public Vector2Int FinalPosition => _finalPosition;

    public Move(Checker _selectedChecker, Vector2Int _startPosition, Vector2Int _finalPosition)
    {
        this._selectedChecker = _selectedChecker;
        this._startPosition = _startPosition;
        this._finalPosition = _finalPosition;
    }
}

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
