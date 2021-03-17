using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Checker : MonoBehaviour
{
    private bool _isSimple;
    private bool _isKing;
    private const int _stepDelta = 1;
    private const int _beatDelta = 2;
    [SerializeField] private bool _isWhite;

    public bool IsSimple => _isSimple;

    public bool IsKing => _isKing;

    public bool IsWhite => _isWhite;

    public int StepDelta => _stepDelta;

    public int BeatDelta => _beatDelta;

    public Checker()
    {
        _isSimple = true;
        _isKing = false;
    }
    public bool IsAbleToMove(Checker[,] board, int x1, int z1, int x2, int z2, bool isWhiteturn)
    {
        if (board[x2, z2] != null)
        {
            return false;
        }
        if (_isSimple || _isKing)
        {
            if (CheckActionCondition(x1, z1, x2, z2, isWhiteturn, _stepDelta))
                if (_isWhite && x2 > x1)
                    return true;
                else if (!_isWhite && x2 < x1)
                    return true;
                else
                    return false;
            if (CheckActionCondition(x1, z1, x2, z2, isWhiteturn, _beatDelta))
            {
                Checker checkerToDelete = board[(x1 + x2) / 2, (z1 + z2) / 2];
                if (checkerToDelete != null && checkerToDelete._isWhite != _isWhite)
                {
                    return true;
                }
            }
        }
        if (_isKing)
        {
            List<Checker> checkersBetweenKingPositions = new List<Checker>();
            if (CheckActionCondition(x1, z1, x2, z2, isWhiteturn, Mathf.Abs(x2 - x1)))
            {
                for (int x = x1 + 2; x < x2; x += 2)
                {
                    for (int z = z1 + 2; z < z2; z += 2)
                    {
                        Checker checkerToDelete = board[(x1 + x) / 2, (z1 + z) / 2];
                        if (checkerToDelete != null && checkerToDelete._isWhite != _isWhite)
                        {
                            checkersBetweenKingPositions.Add(checkerToDelete);
                        }
                        x1++;
                        z1++;
                    }
                }
                if (checkersBetweenKingPositions.Count <= 1)
                    return true;
                else
                    return false;

            }


        }
        return false;
    }

    private bool CheckActionCondition(int x1, int z1, int x2, int z2, bool isWhiteTurn, int conditionDelta)
    {
        return Mathf.Abs(x2 - x1) == conditionDelta && Mathf.Abs(z2 - z1) == conditionDelta && _isWhite == isWhiteTurn;
    }

    public bool IsForcedToMove(Checker[,] board, int x1, int z1, bool isWhiteTurn, int beatDelta)
    {

        for (int x = 0; x < board.GetLength(0); x++)
        {
            for (int z = 0; z < board.GetLength(1); z++)
            {

                if (CheckActionCondition(x1, z1, x, z, isWhiteTurn, beatDelta) && board[x, z] == null)
                {
                    Checker checkerToDelete = board[(x1 + x) / 2, (z1 + z) / 2];
                    if (checkerToDelete != null && checkerToDelete.IsWhite != IsWhite)
                    {
                        return true;
                    }
                }


            }

        }
        return false;
    }
    public void BecomeKing()
    {
        _isSimple = false;
        _isKing = true;
        transform.rotation = Quaternion.Euler(-270, 0, 0);
    }
}