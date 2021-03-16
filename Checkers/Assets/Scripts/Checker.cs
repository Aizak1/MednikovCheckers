using System.Collections;
using System.Collections.Generic;
using UnityEngine;



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
            if (CheckActionCondition(x1, z1, x2, z2, isWhiteturn, stepdelta))
                return true;
            if (CheckActionCondition(x1,z1,x2,z2,isWhiteturn,beatDelta))
            {
                Checker checkerToDelete = board[(x1 + x2) / 2, (z1 + z2) / 2];
                if (checkerToDelete != null && checkerToDelete._isWhite != _isWhite)
                {
                    return true;
                }
            }
        }
        return false;
    }

    private bool CheckActionCondition(int x1, int z1, int x2, int z2, bool isWhiteturn, int conditionDelta)
    {
        return Mathf.Abs(x2 - x1) == conditionDelta && Mathf.Abs(z2 - z1) == conditionDelta && _isWhite == isWhiteturn;
    }

    public bool IsForcedToMove(Checker[,]board,int x1,int z1,bool isWhiteTurn,int beatDelta)
    {
        for (int x = 0; x < board.GetLength(0); x++)
        {
            for (int z = 0; z < board.GetLength(1); z++)
            {
                if(CheckActionCondition(x1,z1,x,z,isWhiteTurn,beatDelta) && board[x,z] == null)
                {
                    Checker checkerToDelete = board[(x1 + x) / 2, (z1 + z) / 2];
                    if (checkerToDelete!=null && checkerToDelete.IsWhite != IsWhite)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }
}
