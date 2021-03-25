
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AI : MonoBehaviour
{
    private Checker[,] boardCopy = new Checker[8, 8];
    private void Start()
    {
        var allCheckers = FindObjectsOfType<Checker>();
        allCheckers.Reverse();
        for (int i = 0; i < allCheckers.Length; i++)
        {
            boardCopy[(int)allCheckers[i].transform.position.x, -(int)allCheckers[i].transform.position.z] = allCheckers[i];
        }
    }
    public Move GetRandomMove(Checker[,]board, RuleValidator validator ,bool isWhiteTurn)
    {
        var moves = Move.GetAllMoves(board,validator,isWhiteTurn);
        if (moves.Count != 0)
            return moves[Random.Range(0, moves.Count)];
        else
            return null;
    }
    

}




