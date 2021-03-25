
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AI : MonoBehaviour
{
    public Move GetRandomMove(Checker[,] board, RuleValidator validator, bool isWhiteTurn)
    {
        var moves = Move.GetAllMoves(board, validator, isWhiteTurn);
        if (moves.Count != 0)
            return moves[UnityEngine.Random.Range(0, moves.Count)];
        else
            return null;

    }
    

    public Tuple<float,Move> Minimax(Checker[,] board,RuleValidator validator,bool isWhiteTurn, int depth,GameState gameState)
    {
        if (gameState == GameState.Ended || depth == 0)
            return Tuple.Create(Evaluate(board,validator,isWhiteTurn),new Move(null,Vector2Int.zero,Vector2Int.zero-Vector2Int.one));

        Move bestMove = null;
        float bestScore = Mathf.NegativeInfinity;
        if (!isWhiteTurn)
            bestScore = Mathf.Infinity;

        foreach (Move move in Move.GetAllMoves(board, validator,isWhiteTurn))
        {
            Checker[,] b = SimulateMove(board,validator, move);
            float currentScore;

            currentScore = Minimax(b, validator, !isWhiteTurn,depth-1,gameState).Item1;

            if (!isWhiteTurn)
            {
                if (currentScore < bestScore)
                {
                    bestScore = currentScore;
                    bestMove = move;
                }
            }
            else
            {
                if (currentScore > bestScore)
                {
                    bestScore = currentScore;
                    bestMove = move;
                }
            }
        }

        return Tuple.Create(bestScore,bestMove);
    }
    private Checker[,] SimulateMove(Checker[,] board,RuleValidator validator,Move move)
    {
        Checker[,] boardCopy = new Checker[8, 8];
        boardCopy = (Checker[,])board.Clone();
        if (validator.HasCheckerToKill(boardCopy, move.StartPosition, move.FinalPosition, out Checker checkerToDelete, out Vector2Int deletePosition))
            boardCopy[deletePosition.x, deletePosition.y] = null;
        boardCopy[move.FinalPosition.x, move.FinalPosition.y] = move.SelectedChecker;
        boardCopy[move.StartPosition.x, move.StartPosition.y] = null;
        return boardCopy;

    }
    private float Evaluate(Checker[,] board, RuleValidator validator, bool isWhiteTurn)
    {
        float whiteScore = 0;
        float blackScore = 0;
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                if (board[i, j] != null && board[i, j].IsWhite)
                    whiteScore++;
                else if (board[i, j] != null && board[i, j].IsWhite == false)
                    blackScore++;
            }
        }
        return whiteScore - blackScore;
    }
}










