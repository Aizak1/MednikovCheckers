using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AI : MonoBehaviour
{
    private List<Checker> whiteCheckers;
    private List<Checker> blackCheckers;
    private int _blackCount;
    private int _whiteCount;
    private void Start()
    {
        RefreshListsofCheckers();
    }

    public void RefreshListsofCheckers()
    {
        whiteCheckers = FindObjectsOfType<Checker>().Where(x => x.IsWhite).ToList();
        blackCheckers = FindObjectsOfType<Checker>().Where(x => x.IsWhite == false).ToList();
    }

    public void CalculateCount()
    {
        foreach (var item in whiteCheckers)
        {
            if (item.IsSimple)
                _whiteCount += 10;
            else
                _whiteCount += 100;
        }
        foreach (var item in blackCheckers)
        {
            if (item.IsSimple)
                _blackCount += 10;
            else
                _whiteCount += 100;
        }
    }
    public int Evaluate()
    {
        CalculateCount();
        return _whiteCount - _blackCount;
    } 

    public Tuple<int,Vector2Int> Minimax(Vector2Int position,int depth,bool isWhiteTurn,GameState gameState)
    {
        if(depth == 0 || gameState == GameState.Ended)
        {
            return Tuple.Create(Evaluate(),position);
        }
        else
        {
            if (isWhiteTurn)
            {
                int maxEval = int.MinValue;
                Vector2Int bestPosition = Vector2Int.zero - Vector2Int.one;
                foreach (var move in GetAllMoves(position, isWhiteTurn))
                {
                    int evaluation = Minimax(move, depth - 1, false, gameState).Item1;
                    maxEval = Mathf.Max(maxEval, evaluation);
                    if (maxEval == evaluation)
                        bestPosition = move;
                   
                }
                return Tuple.Create(maxEval, bestPosition);
            }
            else
            {
                int minEval = int.MaxValue;
                Vector2Int bestPosition = Vector2Int.zero - Vector2Int.one;
                foreach (var move in GetAllMoves(position, !isWhiteTurn))
                {
                    int evaluation = Minimax(move, depth - 1, true, gameState).Item1;
                    minEval = Mathf.Min(minEval, evaluation);
                    if (minEval == evaluation)
                        bestPosition = move;
                    
                }
                return Tuple.Create(minEval, bestPosition);
            }
        }
        
    }

    private List<Vector2Int> GetAllMoves(Vector2Int position, bool isWhiteTurn)
    {
        throw new NotImplementedException();
    }
}
