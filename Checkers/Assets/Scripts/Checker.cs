using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class Checker : MonoBehaviour
{
    private bool _isSimple;
    private bool _isKing;
    private const int _simplestepDelta = 1;
    private const int _simplebeatDelta = 2;
    [SerializeField] private bool _isWhite;

    public bool IsSimple => _isSimple;

    public bool IsKing => _isKing;

    public bool IsWhite => _isWhite;


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
            if (CheckActionCondition(x1, z1, x2, z2, isWhiteturn, _simplestepDelta))
                if (_isWhite && x2 > x1)
                    return true;
                else if (!_isWhite && x2 < x1)
                    return true;
                else
                    return false;

            if (CheckActionCondition(x1, z1, x2, z2, isWhiteturn, _simplebeatDelta))
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
                //направление в зависимости от расположение осей и доски
                Vector2Int trueDiretion = new Vector2Int((int)(-1 * direction.x / Mathf.Abs(direction.x)), (int)(-1 * direction.y / Mathf.Abs(direction.y)));
                Vector2Int step = new Vector2Int(x1 + trueDiretion.x, z1 + trueDiretion.y);
                
                int stepCounter = 0;
               

                while (stepCounter != Mathf.Abs(x2 - x1))
                {
                    if (board[step.x, step.y] != null)
                    {
                        checkersBetweenKingPositions.Add(board[step.x, step.y]);
                    }
                    step += trueDiretion;
                    stepCounter++;
                }

                if (checkersBetweenKingPositions.Count == 0)
                    return true;
                else if (checkersBetweenKingPositions.Count == 1 && checkersBetweenKingPositions.All(x => x.IsWhite != _isWhite))
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

    public bool IsForcedToMove(Checker[,] board, int x1, int z1, bool isWhiteTurn)
    {
        if (_isSimple)
        {
            for (int x = 0; x < board.GetLength(0); x++)
            {
                for (int z = 0; z < board.GetLength(1); z++)
                {

                    if (CheckActionCondition(x1, z1, x, z, isWhiteTurn, _simplebeatDelta) && board[x, z] == null)
                    {
                        Checker checkerToDelete = board[(x1 + x) / 2, (z1 + z) / 2];
                        if (checkerToDelete != null && checkerToDelete.IsWhite != IsWhite)
                        {
                            return true;
                        }
                    }


                }

            }
        }
        if (_isKing)
        {

            int stepsToUp = 7 - x1;
            int stepsToRight = 7 - z1;
            int stepsToBottom = Mathf.Abs(0 - x1);
            int stepsToLeft = Mathf.Abs(0 - z1);

            int stepsToUpAndRight = stepsToUp < stepsToRight ? stepsToUp : stepsToRight;
            int stepsToBottomAndLeft = stepsToBottom < stepsToLeft ? stepsToBottom : stepsToLeft;
            int steptsToUpAndLeft = stepsToUp < stepsToLeft ? stepsToUp : stepsToLeft;
            int steptsToBottomAndRight = stepsToBottom < stepsToRight ? stepsToBottom : stepsToRight;

            Vector3Int leftBottomStep = new Vector3Int(-1, 0, -1);
            Vector3Int leftUpStep = new Vector3Int(1, 0, -1);
            Vector3Int rightBottomStep = new Vector3Int(-1, 0, 1);
            Vector3Int rightUpStep = new Vector3Int(1, 0, 1);


            if (!CheckDiagonal(board, x1, z1, stepsToUpAndRight, rightUpStep) && !CheckDiagonal(board, x1, z1, stepsToBottomAndLeft, leftBottomStep)
                && !CheckDiagonal(board, x1, z1, steptsToBottomAndRight, rightBottomStep) && !CheckDiagonal(board, x1, z1, steptsToUpAndLeft, leftUpStep))
                return false;
            else
                return true;


        }



        return false;
    }

    private bool CheckDiagonal(Checker[,] board, int x1, int z1, int stepsToDiagonalEnd, Vector3Int directionStep)
    {
        int stepX = x1;
        int stepZ = z1;
        bool hasSameColor = false;


        for (int i = 0; i < stepsToDiagonalEnd; i++)
        {
            if (i != 0 && (board[stepX, stepZ] != null && board[stepX, stepZ].IsWhite == _isWhite))
                hasSameColor = true;
            if (board[stepX, stepZ] != null && board[stepX, stepZ].IsWhite != _isWhite)
            {
                int jx = stepX + directionStep.x;
                int jz = stepZ + directionStep.z;
                for (int j = i; j < stepsToDiagonalEnd; j++)
                {
                    if (board[stepX + directionStep.x, stepZ + directionStep.z] != null)
                        return false;
                    if (board[jx, jz] == null && !hasSameColor)
                        return true;
                    jx += directionStep.x;
                    jz += directionStep.z;
                }
            }
            stepX += directionStep.x;
            stepZ += directionStep.z;

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