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
    private List<Checker> _checkers;
    private int _blackCount;
    private int _whiteCount;
   
    public List<Move> GetAllMoves(RuleValidator validator, bool isWhiteTurn)
    {
        _checkers = FindObjectsOfType<Checker>().ToList();

        Checker[,] boardCopy = new Checker[8, 8];
        Vector2Int[] initialPoses = new Vector2Int[_checkers.Count];
        List<Move> moves = new List<Move>();
        //в листе чекеров,объекты располагаются в противоположном порядке
        int posCounter = 0;
        for (int i = _checkers.Count-1; i>=0; i--)
        {
            initialPoses[posCounter] = new Vector2Int((int)_checkers[i].transform.position.x, -(int)_checkers[i].transform.position.z);
            boardCopy[initialPoses[posCounter].x, initialPoses[posCounter].y] = _checkers[i];
            posCounter++;
        }

       
        for (int i = 0; i < initialPoses.Length; i++)
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

    public Move GetRandomMove(RuleValidator validator ,bool isWhiteTurn)
    {
        var moves = GetAllMoves(validator,isWhiteTurn);
        return moves[Random.Range(0, moves.Count)];
    }


 
}
