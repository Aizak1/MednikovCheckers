using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        if (_isSimple)
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
                Vector2 start = new Vector2(x1, z1);
                Vector2 end = new Vector2(x2, z2);
                Vector2 direction = (start - end).normalized;
                //Ќаправление с учетом расположени€ осей и доски
                Vector2 trueDiretion = new Vector2(-1 * direction.x / Mathf.Abs(direction.x), -1 * direction.y / Mathf.Abs(direction.y));

                int stepX, stepZ;
                int stepCounter = 0;
                stepX = x1 + (int)trueDiretion.x;
                stepZ = z1 + (int)trueDiretion.y;

                while (stepCounter!= Mathf.Abs(x2-x1))
                {
                    if(board[stepX,stepZ] != null)
                    {
                        checkersBetweenKingPositions.Add(board[stepX, stepZ]);
                    }
                    stepX += (int)trueDiretion.x;
                    stepZ += (int)trueDiretion.y;
                    stepCounter++;
                }
                
                if (checkersBetweenKingPositions.Count == 0)
                    return true;
                else if (checkersBetweenKingPositions.Count==1 && checkersBetweenKingPositions.All(x=>x.IsWhite !=_isWhite) )
                {
                    return true;
                }
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