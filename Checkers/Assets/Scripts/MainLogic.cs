using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MainLogic : MonoBehaviour
{
    [SerializeField] private GameObject _whitePrefab;
    [SerializeField] private GameObject _blackPrefab;
    [SerializeField] private GameObject _vfx;

    private GameState _gameState;
    /// <summary>
    /// null - пустая клетка, не null - шашка 
    /// </summary>
    private Checker[,] _board = new Checker[8, 8];   
    private Checker _selectedChecker;
    private Vector2Int _selectionPosition;

    private Selecter _selecter;
    private RuleValidator _validator;
    private Mover _mover;
    private History _history;
    private Hinter _hinter;
    private SFX _sfx;

    private bool _isWhiteTurn;
    private static int _stepsWithOutKills = 0;
    private bool _hasKilled;

    public GameState GetGameState => _gameState;

    public ResultOfGame Result { get; private set; }

    private void GenerateBoard()
    {
      
        for (int x = 0; x < 8; x++)
        {
            int initialZCoordinate;
            if (x % 2 == 0)
                initialZCoordinate = 0;
            else
                initialZCoordinate = 1;

            if (!(x > 2 && x < 5))
                for (int z = initialZCoordinate; z < 8; z += 2)
                {
                    if (x >= 0 && x < 3)
                        GenerateChecker(_whitePrefab, x, z);
                    else
                        GenerateChecker(_blackPrefab, x, z);
                }

        }

    }

    private void GenerateChecker(GameObject prefab, int x, int z)
    {
        GameObject checkerGameObject = Instantiate(prefab, new Vector3(x, 0, -z), Quaternion.Euler(-90f, 0, 0));
        Checker checker = checkerGameObject.GetComponent<Checker>();
        _board[x, z] = checker;

    }

    private void Start()
    {
        GenerateBoard();
        _isWhiteTurn = true;
        _validator = GetComponent<RuleValidator>();
        _selecter = GetComponent<Selecter>();
        _mover = GetComponent<Mover>();
        _history = FindObjectOfType<History>();
        _hinter = FindObjectOfType<Hinter>();
        _sfx = FindObjectOfType<SFX>();
        _gameState = GameState.Started;
    }
    private void Update()
    {
        var mouseDownPosition = _selecter.RecordMousePosition();
        if (Input.GetMouseButtonDown(0))
        {
            _validator.SearchForPossibleKills(_board,_isWhiteTurn);
            if (_validator.ForcedToMoveCheckers.Count != 0)
            {
                _hinter.ChangeHighlightState(_validator);
            }
            if (!_validator.OutOfBounds(_board, mouseDownPosition.x, mouseDownPosition.y))
            {
                var cell = _selecter.PickACell(_board, mouseDownPosition.x, mouseDownPosition.y);
                if (_validator.SelectionValidate(cell, _isWhiteTurn))
                {
                    _selectedChecker = _selecter.SelectChecker(cell);
                    _selectionPosition = new Vector2Int(mouseDownPosition.x, mouseDownPosition.y);
                    _sfx.PlayPicKSound();
                }
            }
        }
        if (_selectedChecker != null)
        {
            _mover.UprageCheckDragPosition(_selectedChecker);
        }

        if (Input.GetMouseButtonUp(0))
        {

            _hinter.ChangeHighlightState(_validator);
            MakeTurn(_selectionPosition.x, _selectionPosition.y, mouseDownPosition.x, mouseDownPosition.y);
            _hinter.ShowCurrentTurn(_isWhiteTurn);
        }

        if (_gameState == GameState.Started)
        {
            CheckForEndGameCondition();
        }

    }

    private void CheckForEndGameCondition()
    {
        bool hasWhite = false;
        bool hasBlack = false;
        for (int i = 0; i < _board.GetLength(0); i++)
        {
            for (int j = 0; j < _board.GetLength(1); j++)
            {
                if (_board[i, j] != null && _board[i, j].IsWhite)
                    hasWhite = true;
                if (_board[i, j] != null && !_board[i, j].IsWhite)
                    hasBlack = true;

            }
        }

        if (hasBlack)
        {
            Result = ResultOfGame.BlackWins;
            _gameState = GameState.Ended;
        }
        if (hasWhite)
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
        if (_selectedChecker == null)
            return;

        if (_validator.OutOfBounds(_board, x2, z2))
        {
            if (_selectedChecker != null)
            {
                _mover.Move(_selectedChecker, x1, z1);
            }
            _selecter.Deselect(ref _selectedChecker);
            return;
        }


        if (_selectedChecker.IsAbleToMove(_board, x1, z1, x2, z2, _isWhiteTurn))
        {

            if (Math.Abs(x1 - x2) == 2)
            {
                Vector2Int deletePosition = new Vector2Int((x1 + x2) / 2, (z1 + z2) / 2);
                Checker checkerToDelete = _board[deletePosition.x, deletePosition.y];
                if (checkerToDelete != null)
                {
                    RemoveChecker(deletePosition, checkerToDelete);
                }
            }
            if (Math.Abs(x1 - x2) > 2)
            {
                Vector2 start = new Vector2(x1, z1);
                Vector2 end = new Vector2(x2, z2);
                //Направление с учетом расположения осей и доски
                Vector2 direction = (start - end).normalized;
                //Генерируем направление с единичными координатами для построения шага 
                Vector2Int trueDiretion = new Vector2Int((int)(-1 * direction.x / Mathf.Abs(direction.x)), (int)(-1 * direction.y / Mathf.Abs(direction.y)));
                Vector2Int step = new Vector2Int(x1 + trueDiretion.x, z1 + trueDiretion.y);
                int stepCounter = 0;
                while (stepCounter != Mathf.Abs(x2 - x1))
                {
                    
                    Checker checkerToDelete = _board[step.x, step.y];
                    if (checkerToDelete != null)
                    {
                        RemoveChecker(step, checkerToDelete);
                        break;
                    }
                    step += trueDiretion;
                    stepCounter++;
                }

            }

            if (_validator.ForcedToMoveCheckers.Count != 0 && !_hasKilled)
            {
                _mover.Move(_selectedChecker, x1, z1);
                _selecter.Deselect(ref _selectedChecker);
                return;
            }

            _board[x2, z2] = _selectedChecker;
            _board[x1, z1] = null;
            _mover.Move(_selectedChecker, x2, z2);
           

            EndTurn(x2, z2);

        }
        else
        {
            _mover.Move(_selectedChecker, x1, z1);
            _selecter.Deselect(ref _selectedChecker);
            return;
        }
    }

    private void RemoveChecker(Vector2Int deletePosition, Checker checkerToDelete)
    {
        _board[deletePosition.x, deletePosition.y] = null;
        Destroy(checkerToDelete.gameObject);
        _hasKilled = true;
        Instantiate(_vfx, new Vector3(deletePosition.x, 0, -deletePosition.y), Quaternion.identity);
    }

    private void EndTurn(int x2, int z2)
    {

        if (_selectedChecker != null)
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
        {
            _sfx.PlayKillSound();
            _stepsWithOutKills = 0;
        }
        else
        {
            _sfx.PlayDropSound();
            _stepsWithOutKills++;
        }
          

        if (_validator.SearchForPossibleKills(_board,x2, z2,_isWhiteTurn).Count != 0 && _hasKilled)
            return;

        _history.AddRecord(_selectionPosition.x, _selectionPosition.y, x2, z2, _isWhiteTurn);
        _hasKilled = false;
        _isWhiteTurn = !_isWhiteTurn;
    }
}