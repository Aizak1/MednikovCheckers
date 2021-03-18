using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mover:MonoBehaviour
{
    public void VisualTransition(Checker checker,int destinationX,int destinationZ)
    {
        checker.gameObject.transform.position = new Vector3(destinationX, 0, -destinationZ);
    }
    public void UprageCheckDragPosition(Checker checker)
    {
        RaycastHit hit;
        float rayLength = 25.0f;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, rayLength, LayerMask.GetMask("Board")))
            checker.transform.position = hit.point + Vector3.up;
    }

}