using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    [SerializeField] private GameObject _whitePrefab;
    [SerializeField] private GameObject _blackPrefab;
    [SerializeField] private float _checkerSpeed;
    private Checker[,] _checkers = new Checker[8, 8];

    private Vector3 _initialCoordinates = Vector3.zero;
    private Vector3 _boardOffset = new Vector3(0.55f, 0, 0.7f);

    private Vector2 _mouseDownPosition;
    private Checker _selectedChecker;
    private Vector2 _startDragPosition;
    private Vector2 _endDragPosition;

    public void GenerateBoard()
    {
        for (int x = 0; x < 8; x++)
        {
            int initialCoordinate;
            if (x % 2 == 0)
                initialCoordinate = 0;
            else
                initialCoordinate = 1;

               if(!(x>2 && x<5))
                for (int z = initialCoordinate; z < 8; z += 2)
                {
                    if (x >= 0 && x < 3)
                        GenerateChecker(_whitePrefab, _initialCoordinates, x, z);
                    else 
                        GenerateChecker(_blackPrefab, _initialCoordinates, x, z);
                }
            
           

          
        }
      
    }

    private void GenerateChecker(GameObject prefab,Vector3 position,int x, int z)
    {
        GameObject piece = Instantiate(prefab, new Vector3(position.x + x, position.y, position.z - z),Quaternion.Euler(-89.98f,0,0));
        Checker checker = piece.GetComponent<Checker>();
        _checkers[x, z] = checker;
        
    }

   

    private void Start()
    {
        GenerateBoard();
    }

    // Update is called once per frame
    private void Update()
    {
        RecordMousePosition();

        int x = (int)_mouseDownPosition.x;
        int z = (int)_mouseDownPosition.y;

        if (Input.GetMouseButtonDown(0))
        {
            SelectChecker(x, z);
            

        }
        if (Input.GetMouseButtonUp(0))
        {
            TryMove((int)_startDragPosition.x, (int)_startDragPosition.y, x, z);
        }
    }

    private void TryMove(int x1, int z1, int x2, int z2)
    {
        _startDragPosition = new Vector2(x1, z1);
        _endDragPosition = new Vector2(x2, z2);
        _selectedChecker = _checkers[x1, z1];
       
        _selectedChecker.gameObject.transform.position = new Vector3(_endDragPosition.x, 0, -_endDragPosition.y);


    }

    private void RecordMousePosition()
    {
        if (Camera.main)
        {
            RaycastHit hit;
            float rayLength = 25.0f;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition) ,out hit, rayLength, LayerMask.GetMask("Board")))
            {
                _mouseDownPosition.x = (int)(hit.point.x + _boardOffset.x);
                _mouseDownPosition.y = (int)(Math.Abs(hit.point.z) + _boardOffset.z);
                
            }
            else
            {
                _mouseDownPosition.x = -1;
                _mouseDownPosition.y = -1;
            }
            
        }
    }

    private void SelectChecker(int x, int z)
    {
        if (x < 0 || x > _checkers.GetLength(0) || z < 0 || z > _checkers.GetLength(1))
            return;

        Checker selectedChecker = _checkers[x, z];
        if (selectedChecker != null)
        {
            _selectedChecker = selectedChecker;
            _startDragPosition = _mouseDownPosition;
           
            
        }
       
            

       
    }
}
