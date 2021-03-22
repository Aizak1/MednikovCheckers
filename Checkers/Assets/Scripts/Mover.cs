using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mover:MonoBehaviour
{
   
    public void Move(Checker checker,Vector2Int destination)
    {
        checker.gameObject.transform.position = new Vector3(destination.x, 0, -destination.y);
    }
    public void UprageCheckerDragPosition(Checker checker)
    {
        RaycastHit hit;
        float rayLength = 25.0f;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, rayLength, LayerMask.GetMask("Board")))
            checker.transform.position = hit.point + Vector3.up;
       
    }

}
