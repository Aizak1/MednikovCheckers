using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RuleValidator : MonoBehaviour
{
    public bool SelectionValidate(Checker selectedChecker, bool _isWhiteTurn, List<Checker> _forcedToMoveCheckers)
    {
     
        if (selectedChecker != null && selectedChecker.IsWhite == _isWhiteTurn)
        {
            if (_forcedToMoveCheckers.Count == 0)
            {

                return true;

            }
            else
            {
                foreach (var item in _forcedToMoveCheckers)
                {
                    if (selectedChecker == item)
                    {

                        return true;
                    }
                }

            }
        }
        
        return false;
    }

    public bool OutOfBounds(Checker[,] checkers,int x,int z)
    {
        if (x < 0 || x > checkers.GetLength(0) || z < 0 || z > checkers.GetLength(1))
            return true;
        return false;
    }

    
    
}
