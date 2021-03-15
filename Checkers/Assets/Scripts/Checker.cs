using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Side
{
    White,
    Black
}

public class Checker : MonoBehaviour
{
    private bool _isSimple;
    private bool _isKing;
    [SerializeField] private bool _isWhite;

    public bool IsSimple => _isSimple;

    public bool IsKing => _isKing;

    public bool IsWhite => _isWhite;

    public Checker()
    {
        _isSimple = true;
        _isKing = false;
    }
    public bool IsAbleToMove(Checker[,] board, int x1, int z1, int x2, int z2,bool isWhiteturn)
    {
        if (board[x2, z2] != null)
        {
            return false;
        }
        if (_isSimple || _isKing)
        {
            int stepdelta = 1;
            int beatDelta = 2;
            if(Mathf.Abs(x2-x1)==stepdelta && Mathf.Abs(z2-z1)==stepdelta && _isWhite==isWhiteturn)
                return true;
            if(Mathf.Abs(x2 - x1) == beatDelta && Mathf.Abs(z2 - z1) == beatDelta && _isWhite == isWhiteturn)
            {
                Checker checkerToDelete = board[(x1 + x2) / 2, (z1 + z2) / 2];
                if(checkerToDelete!=null && checkerToDelete._isWhite != _isWhite)
                {
                    return true;
                }
            }
        }
        return false;
    }
}
