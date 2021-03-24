using System.Collections.Generic;
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
    public static List<Move> GetAllMoves(Checker[,] board, RuleValidator validator, bool isWhiteTurn)
    {
        List<Vector2Int> initialPoses = new List<Vector2Int>();
        List<Move> moves = new List<Move>();
        //в листе чекеров,объекты располагаются в противоположном порядке
        for (int i = 0; i < board.GetLength(0); i++)
        {
            for (int j = 0; j < board.GetLength(1); j++)
            {
                if (board[i, j] != null)
                    initialPoses.Add(new Vector2Int(i, j));   
            }
        }

       var forcedToMoveCheckers = validator.SearchForPossibleKills(board, isWhiteTurn);
        for (int i = 0; i < initialPoses.Count; i++)
        {
            for (int j = 0; j < board.GetLength(0); j++)
            {
                for (int k = 0; k < board.GetLength(1); k++)
                {
                    Vector2Int initialPos = initialPoses[i];
                    Vector2Int probableFinalPos = new Vector2Int(j, k);
                    if (forcedToMoveCheckers.Count == 0)
                    {
                        if (board[initialPos.x, initialPos.y].IsAbleToMove(board, initialPos, probableFinalPos, isWhiteTurn))
                        {
                            moves.Add(new Move(board[initialPos.x, initialPos.y], initialPos, probableFinalPos));
                        }
                    }
                    else
                    {
                        if (board[initialPos.x, initialPos.y].IsAbleToMove(board, initialPos, probableFinalPos, isWhiteTurn) && Mathf.Abs(initialPos.x - probableFinalPos.x) >= 2)
                        {
                            if (validator.HasCheckerToKill(board,initialPos,probableFinalPos, out Checker checkerToDelete,out Vector2Int deletePosition))
                                moves.Add(new Move(board[initialPos.x, initialPos.y], initialPos, probableFinalPos));
                        }
                    }

                }

            }
        }
        return moves;
    }
}




