using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public enum GameState
{
    Started,
    Ended
}
public enum ResultOfGame
{
    WhiteWins,
    BlackWins,
    Draw
}

public class Board : MonoBehaviour
{
    [SerializeField] private GameObject _whitePrefab;
    [SerializeField] private GameObject _blackPrefab;
    [SerializeField] private Material _materialToHighlightForces;
    private Material _initialMaterial;

    private GameState _gameState;
    private Checker[,] _checkers = new Checker[8, 8];
    private List<Checker> _forcedToMoveCheckers;

    private Vector3 _initialCoordinates = Vector3.zero;

    private Checker _selectedChecker;
    private Vector2Int _selectionPosition;

    private Selecter _selecter;
    private RuleValidator _validator;
    private Mover _mover;

    private bool _isWhiteTurn;
    private static int _stepsWithOutKills = 0;
    private bool _hasKilled;

    public GameState GetGameState => _gameState;

    public ResultOfGame Result { get; private set; }

    public void GenerateBoard()
    {
        for (int x = 0; x < 8; x++)
        {
            int initialCoordinate;
            if (x % 2 == 0)
                initialCoordinate = 0;
            else
                initialCoordinate = 1;

            if (!(x > 2 && x < 5))
                for (int z = initialCoordinate; z < 8; z += 2)
                {
                    if (x >= 0 && x < 3)
                        GenerateChecker(_whitePrefab, _initialCoordinates, x, z);
                    else
                        GenerateChecker(_blackPrefab, _initialCoordinates, x, z);
                }

        }

    }

    private void GenerateChecker(GameObject prefab, Vector3 position, int x, int z)
    {
        GameObject piece = Instantiate(prefab, new Vector3(position.x + x, position.y, position.z - z), Quaternion.Euler(-89.98f, 0, 0));
        Checker checker = piece.GetComponent<Checker>();
        _checkers[x, z] = checker;

    }



    private void Start()
    {
        GenerateBoard();
        _isWhiteTurn = true;
        _validator = GetComponent<RuleValidator>();
        _forcedToMoveCheckers = new List<Checker>();
        _selecter = GetComponent<Selecter>();
        _mover = GetComponent<Mover>();
        _initialMaterial = _whitePrefab.GetComponent<Renderer>().sharedMaterial;
        _gameState = GameState.Started;


    }

    private void Update()
    {
        var mouseDownPosition = _selecter.RecordMousePosition();
        if (Input.GetMouseButtonDown(0))
        {
            _forcedToMoveCheckers = SearchForPossibleKills();
            if (_forcedToMoveCheckers.Count != 0)
            {
                ChangeHighlightState(_materialToHighlightForces);
            }

            if(!_validator.OutOfBounds(_checkers, mouseDownPosition.x, mouseDownPosition.y))
            {
                var cell = _selecter.PickACell(_checkers, mouseDownPosition.x, mouseDownPosition.y);

                if (_validator.SelectionValidate(cell, _isWhiteTurn, _forcedToMoveCheckers))
                {
                    _selectedChecker = _selecter.SelectChecker(cell);
                    _selectionPosition = new Vector2Int(mouseDownPosition.x, mouseDownPosition.y);
                }
            }
           
        }


        if (_selectedChecker!=null)
        {
            _mover.UprageCheckDragPosition(_selectedChecker);
        }

        if (Input.GetMouseButtonUp(0))
        {
            ChangeHighlightState(_initialMaterial);
            MakeTurn(_selectionPosition.x, _selectionPosition.y, mouseDownPosition.x, mouseDownPosition.y);
            

        }
        if (_gameState == GameState.Started)
        {
            CheckForEndGameCondition();
        }

    }

    private void ChangeHighlightState(Material material)
    {
        foreach (var item in _forcedToMoveCheckers)
        {
            
            var renderer = item.gameObject.GetComponent<Renderer>();
            renderer.sharedMaterial = material;
        }
    }

    private void CheckForEndGameCondition()
    {
        bool hasWhite = false;
        bool hasBlack = false;
        for (int i = 0; i < _checkers.GetLength(0); i++)
        {
            for (int j = 0; j < _checkers.GetLength(1); j++)
            {
                if (_checkers[i, j] != null && _checkers[i, j].IsWhite)
                    hasWhite = true;
                if (_checkers[i, j] != null && !_checkers[i, j].IsWhite)
                    hasBlack = true;

            }
        }
        if (!hasWhite && hasBlack)
        {
            Result = ResultOfGame.BlackWins;
            _gameState = GameState.Ended;
        }

        if (!hasBlack && hasWhite)
        {
            Result = ResultOfGame.WhiteWins;
            _gameState = GameState.Ended;
        }
        if (_stepsWithOutKills == 15)
        {
            Result = ResultOfGame.Draw;
            _gameState = GameState.Ended;
        }

    }

    private void MakeTurn(int x1, int z1, int x2, int z2)
    {
        if (_selectedChecker==null)
             return;
        
        if (_validator.OutOfBounds(_checkers, x2, z2))
        {
            if (_selectedChecker != null)
            {
                _mover.VisualTransition(_selectedChecker, x1, z1);
            }
            _selecter.Deselect(ref _selectedChecker);
            return;
        } 

        
            if (_selectedChecker.IsAbleToMove(_checkers, x1, z1, x2, z2, _isWhiteTurn))
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
                if (Math.Abs(x1 - x2) > 2)
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
                    while (stepCounter != Mathf.Abs(x2 - x1))
                    {
                        Checker checkerToDelete = _checkers[stepX, stepZ];
                        if (checkerToDelete != null)
                        {
                            _checkers[stepX, stepZ] = null;
                            Destroy(checkerToDelete.gameObject);
                            _hasKilled = true;
                            break;
                        }
                        stepX += (int)trueDiretion.x;
                        stepZ += (int)trueDiretion.y;
                        stepCounter++;
                    }
                   
                }

                if (_forcedToMoveCheckers.Count != 0 && !_hasKilled)
                {
                    _selectedChecker.gameObject.transform.position = new Vector3(x1, 0, -z1);
                    _selecter.Deselect(ref _selectedChecker);
                    return;
                }

            _checkers[x2, z2] = _selectedChecker;
            _checkers[x1, z1] = null;
            _mover.VisualTransition(_selectedChecker, x2, z2);

                EndTurn(x2,z2);
               
            }
        else
        {
            _mover.VisualTransition(_selectedChecker, x1, z1);
            _selecter.Deselect(ref _selectedChecker);
             return;
        }
    }



    private void EndTurn(int x2,int z2)
    {
      
        if (_selectedChecker!=null)
        {
            if (_selectedChecker.IsWhite && x2 == 7)
            {
                _selectedChecker.BecomeKing();
            }
            else if (!_selectedChecker.IsWhite && x2 == 0)
            {
                _selectedChecker.BecomeKing();
            }
        }
        _selecter.Deselect(ref _selectedChecker);
        if (_hasKilled)
            _stepsWithOutKills = 0;
        else
            _stepsWithOutKills++;

        if (SearchForPossibleKills(x2, z2).Count != 0 && _hasKilled)
            return;
       
        _hasKilled = false;
        _isWhiteTurn = !_isWhiteTurn;
        
    }

    private List<Checker> SearchForPossibleKills()
    {
        _forcedToMoveCheckers = new List<Checker>();
        for (int i = 0; i < _checkers.GetLength(0); i++)
        {
            for (int j = 0; j < _checkers.GetLength(1); j++)
            {
                if (_checkers[i, j] != null && _checkers[i, j].IsWhite == _isWhiteTurn)
                {
                    if (_checkers[i, j].IsForcedToMove(_checkers, i, j, _isWhiteTurn))
                        _forcedToMoveCheckers.Add(_checkers[i, j]);
                }
            }
        }
        return _forcedToMoveCheckers;
    }

    private List<Checker> SearchForPossibleKills(int x, int z)
    {
        _forcedToMoveCheckers = new List<Checker>();
        if (_checkers[x, z].IsForcedToMove(_checkers, x, z, _isWhiteTurn))
        {
            _forcedToMoveCheckers.Add(_checkers[x, z]);
        }
        return _forcedToMoveCheckers;
    }
}