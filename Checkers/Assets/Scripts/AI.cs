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
   
    public List<Move> GetAllMoves(Checker[,]board,RuleValidator validator, bool isWhiteTurn)
    {
        Checker[,] boardCopy = new Checker[8, 8];
        List<Vector2Int> initialPoses = new List<Vector2Int>();
        List<Move> moves = new List<Move>();
        //в листе чекеров,объекты располагаются в противоположном порядке
        for (int i = 0; i < boardCopy.GetLength(0); i++)
        {
            for (int j = 0; j < boardCopy.GetLength(1); j++)
            {
                if (board[i, j] != null)
                {
                    initialPoses.Add(new Vector2Int(i, j));
                    boardCopy[i, j] = board[i, j];
                }
            }
        }

        validator.SearchForPossibleKills(boardCopy, isWhiteTurn);
        for (int i = 0; i < initialPoses.Count; i++)
        {
            for (int j = 0; j < boardCopy.GetLength(0); j++)
            {
                for (int k = 0; k < boardCopy.GetLength(1); k++)
                {
                    Vector2Int initialPos = initialPoses[i];
                    Vector2Int probableFinalPos = new Vector2Int(j, k);
                    if (validator.ForcedToMoveCheckers.Count == 0)
                    {
                        if (boardCopy[initialPos.x, initialPos.y].IsAbleToMove(boardCopy, initialPos, probableFinalPos, isWhiteTurn))
                        {
                            moves.Add(new Move(boardCopy[initialPos.x, initialPos.y], initialPos, probableFinalPos));
                        }
                    }
                    else
                    {
                        if (boardCopy[initialPos.x, initialPos.y].IsAbleToMove(boardCopy, initialPos, probableFinalPos, isWhiteTurn) && Mathf.Abs(initialPos.x-probableFinalPos.x)>=2)
                        {
                            moves.Add(new Move(boardCopy[initialPos.x, initialPos.y], initialPos, probableFinalPos));
                        }
                    }
                    
                }
               
            }
        }
        return moves;
    }

    public Move GetRandomMove(Checker[,]board,RuleValidator validator ,bool isWhiteTurn)
    {
        var moves = GetAllMoves(board,validator,isWhiteTurn);
        return moves[Random.Range(0, moves.Count)];
    }

 
   
}




