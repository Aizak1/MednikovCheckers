
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AI : MonoBehaviour
{
    public Move GetRandomMove(Checker[,]board, RuleValidator validator ,bool isWhiteTurn)
    {
        var moves = Move.GetAllMoves(board,validator,isWhiteTurn);
        if (moves.Count != 0)
            return moves[Random.Range(0, moves.Count)];
        else
            return null;
    }
    

}




