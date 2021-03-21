using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RuleValidator:MonoBehaviour
{
    public List<Checker> ForcedToMoveCheckers { get; private set; }

   
    public bool SelectionValidate(Checker selectedChecker, bool _isWhiteTurn)
    {
     
        if (selectedChecker != null && selectedChecker.IsWhite == _isWhiteTurn)
        {
            if (ForcedToMoveCheckers.Count == 0)
            {

                return true;

            }
            else
            {
                foreach (var item in ForcedToMoveCheckers)
                {
                    if (selectedChecker == item)
                    {

                        return true;
                    }
                }

            }
        }
        
        return false;
    }

    public bool OutOfBounds(Checker[,] checkers,int x,int z)
    {
        if (x < 0 || x > checkers.GetLength(0) || z < 0 || z > checkers.GetLength(1))
            return true;
        return false;
    }

    public  List<Checker> SearchForPossibleKills(Checker[,] board, bool isWhiteTurn)
    {
        ForcedToMoveCheckers = new List<Checker>();
        for (int i = 0; i < board.GetLength(0); i++)
        {
            for (int j = 0; j < board.GetLength(1); j++)
            {
                if (board[i, j] != null && board[i, j].IsWhite == isWhiteTurn)
                {
                    if (board[i, j].IsForcedToMove(board, i, j, isWhiteTurn))
                        ForcedToMoveCheckers.Add(board[i, j]);
                }
            }
        }
        return ForcedToMoveCheckers;
    }

    public List<Checker> SearchForPossibleKills(Checker[,] _board,int x, int z,bool isWhiteTurn)
    {
        ForcedToMoveCheckers = new List<Checker>();
        if (_board[x, z].IsForcedToMove(_board, x, z, isWhiteTurn))
        {
            ForcedToMoveCheckers.Add(_board[x, z]);
        }
        return ForcedToMoveCheckers;
    }



}
