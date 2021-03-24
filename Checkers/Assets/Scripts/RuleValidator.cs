using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RuleValidator:MonoBehaviour
{
    public List<Checker> ForcedToMoveCheckers { get; private set; }

    private void Start()
    {
        ForcedToMoveCheckers = new List<Checker>();
    }
    public bool SelectionIsValid(Checker selectedChecker, bool _isWhiteTurn)
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

    public bool CellIsOutOfBounds(Checker[,] checkers,Vector2Int coordinate)
    {
        if (coordinate.x < 0 || coordinate.x >= checkers.GetLength(0) || coordinate.y < 0 || coordinate.y >= checkers.GetLength(1))
            return true;
        return false;
    }

    public  void SearchForPossibleKills(Checker[,] board, bool isWhiteTurn)
    {
        ForcedToMoveCheckers = new List<Checker>();
        for (int i = 0; i < board.GetLength(0); i++)
        {
            for (int j = 0; j < board.GetLength(1); j++)
            {
                if (board[i, j] != null && board[i, j].IsWhite == isWhiteTurn)
                {
                    if (board[i, j].IsForcedToMove(board,new Vector2Int(i,j), isWhiteTurn))
                        ForcedToMoveCheckers.Add(board[i, j]);
                }
            }
        }
        
    }

    public void SearchForPossibleKills(Checker[,] _board,Vector2Int coodinates,bool isWhiteTurn)
    {
        ForcedToMoveCheckers = new List<Checker>();
        if (_board[coodinates.x, coodinates.y].IsForcedToMove(_board,coodinates, isWhiteTurn))
        {
            ForcedToMoveCheckers.Add(_board[coodinates.x, coodinates.y]);
        }
      
    }



}
