using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Selecter:MonoBehaviour
{    
    private Vector3 _cellOffset = new Vector3(0.55f, 0, 0.7f);

    public  Vector2Int RecordMousePosition()
    {
      Vector2Int mouseDownPosition = Vector2Int.zero-Vector2Int.one;
        
      RaycastHit hit;
      float rayLength = 25.0f;
      if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, rayLength, LayerMask.GetMask("Board")))
      {
         mouseDownPosition.x = (int)(hit.point.x + _cellOffset.x);
         mouseDownPosition.y = (int)(Mathf.Abs(hit.point.z) + _cellOffset.z);

       }
       else
       {
         mouseDownPosition.x = -1;
         mouseDownPosition.y = -1;
       }


        return mouseDownPosition;
    }
    public Checker PickACell(Checker[,] _checkers, Vector2Int coodinate)
    {
        return _checkers[coodinate.x, coodinate.y];
    }
    public Checker SelectChecker(Checker checker)
    {
        return checker;
    }
    public void Deselect(ref Checker selectedChecker)
    {
        selectedChecker = null;
    }
}
