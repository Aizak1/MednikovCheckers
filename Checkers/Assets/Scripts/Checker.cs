using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class Checker : MonoBehaviour
{
  
   
    [SerializeField] private bool _isWhite;
    private const int _simpleStepDelta = 1;
    private const int __simpleBeatDelta = 2;
    private bool _isSimple;
    public bool IsSimple => _isSimple;

    public bool IsWhite => _isWhite;


    public Checker()
    {
        _isSimple = true;
    }
    public bool IsAbleToMove(Checker[,] board,Vector2Int start,Vector2Int final,bool isWhiteturn)
    {
        if (board[final.x, final.y] != null)
        {
            return false;
        }
        if (_isSimple)
        {
            
            if (CheckActionCondition(start,final, isWhiteturn, _simpleStepDelta))
                if (_isWhite && final.x > start.x || (!_isWhite && final.x < start.x))
                    return true;
                else
                    return false;

            if (CheckActionCondition(start,final, isWhiteturn, __simpleBeatDelta))
            {
                Vector2Int deletePosition = (start + final) / 2;
                Checker checkerToDelete = board[deletePosition.x, deletePosition.y];
                if (checkerToDelete != null && checkerToDelete.IsWhite != IsWhite)
                {
                    return true;
                }
            }
        }
        else
        {
            List<Checker> checkersBetweenKingPositions = new List<Checker>();
            if (CheckActionCondition(start,final, isWhiteturn, Mathf.Abs(final.x - start.x)))
            {
                //Из данного вектора используем знаки у координат
                Vector2 direction = ((Vector2)start - final).normalized;
                //Генерируем направление с единичными координатами для построения шага 
                Vector2Int trueDiretion = new Vector2Int((int)(-1 * direction.x / Mathf.Abs(direction.x)), (int)(-1 * direction.y / Mathf.Abs(direction.y)));
                Vector2Int step = start + trueDiretion;
                int stepCounter = 0;
                while (stepCounter != Mathf.Abs(final.x - start.x))
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
                else if (checkersBetweenKingPositions.Count == 1 && checkersBetweenKingPositions.All(x => x.IsWhite != IsWhite))
                {
                    return true;
                }
                else
                    return false;

            }
        }
        return false;
    }

    private bool CheckActionCondition(Vector2Int start,Vector2Int final,bool isWhiteTurn, int conditionDelta)
    {
        return Mathf.Abs(final.x - start.x) == conditionDelta && Mathf.Abs(final.y - start.y) == conditionDelta && _isWhite == isWhiteTurn;
    }

    public bool IsForcedToMove(Checker[,] board, Vector2Int start,bool isWhiteTurn)
    {
        if (_isSimple)
        {
            for (int x = 0; x < board.GetLength(0); x++)
            {
                for (int z = 0; z < board.GetLength(1); z++)
                {
                    Vector2Int final = new Vector2Int(x, z);
                    if (CheckActionCondition(start,final,isWhiteTurn, __simpleBeatDelta) && board[x, z] == null)
                    {
                        Vector2Int deletePosition = (start + final) / 2;
                        Checker checkerToDelete = board[deletePosition.x, deletePosition.y];
                        if (checkerToDelete != null && checkerToDelete.IsWhite != IsWhite)
                        {
                            return true;
                        }
                    }
                }
            }
        }
        else
        {
            int stepsToUp = 7 - start.x;
            int stepsToRight = 7 - start.y;
            int stepsToBottom = Mathf.Abs(0 - start.x);
            int stepsToLeft = Mathf.Abs(0 - start.y);

            int stepsToUpAndRight = stepsToUp < stepsToRight ? stepsToUp : stepsToRight;
            int stepsToBottomAndLeft = stepsToBottom < stepsToLeft ? stepsToBottom : stepsToLeft;
            int steptsToUpAndLeft = stepsToUp < stepsToLeft ? stepsToUp : stepsToLeft;
            int steptsToBottomAndRight = stepsToBottom < stepsToRight ? stepsToBottom : stepsToRight;

            Vector2Int leftBottomStep = new Vector2Int(-1,-1);
            Vector2Int leftUpStep = new Vector2Int(1,-1);
            Vector2Int rightBottomStep = new Vector2Int(-1, 1);
            Vector2Int rightUpStep = new Vector2Int(1, 1);

            //Проверка по всем диагоналям на возможность побить шашку 
            if (!CheckDiagonal(board,start, stepsToUpAndRight, rightUpStep) && !CheckDiagonal(board,start, stepsToBottomAndLeft, leftBottomStep)
                && !CheckDiagonal(board,start, steptsToBottomAndRight, rightBottomStep) && !CheckDiagonal(board,start, steptsToUpAndLeft, leftUpStep))
                return false;
            else
                return true;
        }
        return false;
    }

    private bool CheckDiagonal(Checker[,] board, Vector2Int start, int stepsToDiagonalEnd, Vector2Int directionStep)
    {
       
        bool hasSameColor = false;

        for (int i = 0; i < stepsToDiagonalEnd; i++)
        {
            if (i != 0 && (board[start.x, start.y] != null && board[start.x, start.y].IsWhite == IsWhite))
                hasSameColor = true;
            if (board[start.x, start.y] != null && board[start.x, start.y].IsWhite != IsWhite)
            {
                Vector2Int jVector = start + directionStep;
                for (int j = i; j < stepsToDiagonalEnd; j++)
                {
                    if (board[(start+directionStep).x, (start+directionStep).y] != null)
                        return false;
                    if (board[jVector.x, jVector.y] == null && !hasSameColor)
                        return true;
                    jVector += directionStep;
                }
            }
            start += directionStep;

        }
        return false;
    }
    public void BecomeKing()
    {
        _isSimple = false;
        transform.rotation = Quaternion.Euler(-270, 0, 0);
    }
    
}