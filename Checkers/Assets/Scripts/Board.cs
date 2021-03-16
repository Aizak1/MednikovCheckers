using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;




public class Board : MonoBehaviour
{
    [SerializeField] private GameObject _whitePrefab;
    [SerializeField] private GameObject _blackPrefab;
    [SerializeField] private float _checkerSpeed;
    private Checker[,] _checkers = new Checker[8, 8];
    private List<Checker> _forcedToMoveCheckers;

    private Vector3 _initialCoordinates = Vector3.zero;
    private Vector3 _boardOffset = new Vector3(0.55f, 0, 0.7f);

    private Vector2 _mouseDownPosition;
    private Checker _selectedChecker;
    private Vector2 _startDragPosition;
    private Vector2 _endDragPosition;

    private bool _isWhiteTurn;
    private bool _hasKilled;

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
        _isWhiteTurn = true;
    }

    private void Update()
    {
        RecordMousePosition();

        int x = (int)_mouseDownPosition.x;
        int z = (int)_mouseDownPosition.y;

        if (Input.GetMouseButtonDown(0))
        {
            SelectChecker(x, z);
        }

        if(_selectedChecker!=null)
        {
            UprageCheckDragPosition(_selectedChecker);
        }

        if (Input.GetMouseButtonUp(0))
        {
            TryMove((int)_startDragPosition.x, (int)_startDragPosition.y, x, z);
        }
    }

    private void TryMove(int x1, int z1, int x2, int z2)
    {
        _forcedToMoveCheckers = SearchForPossibleKills();
        _startDragPosition = new Vector2(x1, z1);
        _endDragPosition = new Vector2(x2, z2);
        _selectedChecker = _checkers[x1, z1];

        if (x2 < 0 || x2 > _checkers.GetLength(0) || z2 < 0 || z2 > _checkers.GetLength(1))
        {
            if (_selectedChecker != null)
            {
                _selectedChecker.gameObject.transform.position = new Vector3(_startDragPosition.x, 0, -_startDragPosition.y);
            }
            Deselect();
            return;
        }
        if (_selectedChecker != null)
        {
           
            if (_selectedChecker.IsAbleToMove(_checkers, x1, z1, x2, z2,_isWhiteTurn))
            {
                if (Math.Abs(x1 - x2) == 2)
                {
                    Checker checkerToDelete = _checkers[(x1 + x2) / 2, (z1 + z2) / 2];
                    if (checkerToDelete != null)
                    {
                        _checkers[(x1 + x2) / 2, (z1 + z2) / 2] = null;
                        Destroy(checkerToDelete.gameObject);
                        _hasKilled = true;
                    }
                }

                if(_forcedToMoveCheckers.Count!=0 && !_hasKilled)
                {
                    _selectedChecker.gameObject.transform.position = new Vector3(_startDragPosition.x, 0, -_startDragPosition.y);
                    Deselect();
                    return;
                }

                _checkers[x2, z2] = _selectedChecker;
                _checkers[x1, z1] = null;
                _selectedChecker.gameObject.transform.position = new Vector3(_endDragPosition.x, 0, -_endDragPosition.y);

                EndTurn();
            }
            else
            {
                _selectedChecker.gameObject.transform.position = new Vector3(_startDragPosition.x, 0, -_startDragPosition.y);
                Deselect();
                return;
            }


        }
            


    }

    

    private void EndTurn()
    {
         Deselect();
        _hasKilled = false;
        _isWhiteTurn = !_isWhiteTurn;
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
        if (selectedChecker != null && selectedChecker.IsWhite == _isWhiteTurn)
        {
            if (_forcedToMoveCheckers.Count == 0)
            {
                _selectedChecker = selectedChecker;
                _startDragPosition = _mouseDownPosition;
            }
            else
            {
                if (_forcedToMoveCheckers.Find(x => x == selectedChecker) == null)
                    return;

                _selectedChecker = selectedChecker;
                _startDragPosition = _mouseDownPosition;
            }
           

        }

    }

    private void Deselect()
    {
        _startDragPosition = Vector2.zero - Vector2.one;
        _selectedChecker = null;
    }

    private void UprageCheckDragPosition(Checker checker)
    {
        RaycastHit hit;
        float rayLength = 25.0f;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, rayLength, LayerMask.GetMask("Board")))
            checker.transform.position = hit.point + Vector3.up;
    }

    private List<Checker> SearchForPossibleKills()
    {
        _forcedToMoveCheckers = new List<Checker>();
        int beatDelta = 2;
        for (int i = 0; i < _checkers.GetLength(0); i++)
        {
            for (int j = 0; j < _checkers.GetLength(1); j++)
            {
                if(_checkers[i,j] != null && _checkers[i,j].IsWhite == _isWhiteTurn)
                {
                    if (_checkers[i, j].IsForcedToMove(_checkers, i, j, _isWhiteTurn, beatDelta))
                        _forcedToMoveCheckers.Add(_checkers[i, j]);
                }
            }
        }
        return _forcedToMoveCheckers;
    }
}
