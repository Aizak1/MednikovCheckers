using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RuleValidator:MonoBehaviour
{

    public bool SelectionIsValid(Checker selectedChecker, bool _isWhiteTurn,List<Checker> forcedToMoveCheckers)
    {
        if (selectedChecker == null || selectedChecker.IsWhite != _isWhiteTurn)
            return false;
        
        if (forcedToMoveCheckers.Count == 0)
            return true;
        else
            foreach (var item in forcedToMoveCheckers)
            {
                if (selectedChecker == item)
                    return true;
            }

        return false;
    }

    public bool CellIsOutOfBounds(Checker[,] checkers,Vector2Int coordinate)
    {
        if (coordinate.x < 0 || coordinate.x >= checkers.GetLength(0) || coordinate.y < 0 || coordinate.y >= checkers.GetLength(1))
            return true;
        return false;
    }
    
    public List<Checker> SearchForPossibleKills(Checker[,] board, bool isWhiteTurn)
    {
        List<Checker> forcedToMoveCheckers = new List<Checker>();
        for (int i = 0; i < board.GetLength(0); i++)
        {
            for (int j = 0; j < board.GetLength(1); j++)
            {
                if (board[i, j] != null && board[i, j].IsWhite == isWhiteTurn)
                {
                    if (board[i, j].IsForcedToMove(board,new Vector2Int(i,j), isWhiteTurn))
                        forcedToMoveCheckers.Add(board[i, j]);
                }
            }
        }
        return forcedToMoveCheckers;

    }
  
    public List<Checker> SearchForPossibleKills(Checker[,] _board,Vector2Int coodinates,bool isWhiteTurn)
    {
        List<Checker> forcedToMoveCheckers = new List<Checker>();
        if (_board[coodinates.x, coodinates.y].IsForcedToMove(_board,coodinates, isWhiteTurn))
        {
            forcedToMoveCheckers.Add(_board[coodinates.x, coodinates.y]);
        }
        return forcedToMoveCheckers;
      
    }

    public bool HasCheckerToKill(Checker[,] _board,Vector2Int start,Vector2Int final,out Checker checkerToDelete,out Vector2Int deletePosition)
    {
        checkerToDelete = null;
        deletePosition = Vector2Int.zero - Vector2Int.one;
        Vector2Int step = Checker.CalculateDirectionalStep(start, final);
        //Инкрементируем вектор,чтобы не проверять начальную клетку
        Vector2Int startStep = start + step;
        int stepCounter = 0;
        while (stepCounter != Mathf.Abs(final.x - start.x))
        {
            if (_board[startStep.x, startStep.y] != null)
            {
                checkerToDelete = _board[startStep.x, startStep.y];
                deletePosition = startStep;
                return true;
            }
            startStep += step;
            stepCounter++;
        }
        return false;
    }



}
