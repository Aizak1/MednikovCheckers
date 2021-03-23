using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
//Игра расположена в плоскости XZ .X - положительно направлена,Z - отрицательно 
public class MainLogic : MonoBehaviour
{
    [SerializeField] private GameObject _whitePrefab;
    [SerializeField] private GameObject _blackPrefab;
    [SerializeField] private GameObject _vfx;
    [SerializeField] private SecondPlayer _secondPlayer;

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
    private AI _ai;
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
        _validator = FindObjectOfType<RuleValidator>();
        _selecter = FindObjectOfType<Selecter>();
        _mover = FindObjectOfType<Mover>();
        _history = FindObjectOfType<History>();
        _hinter = FindObjectOfType<Hinter>();
        _sfx = FindObjectOfType<SFX>();
        _ai = FindObjectOfType<AI>();
        _gameState = GameState.Started;
    }
    private void Update()
    {
        var mouseDownPosition = _selecter.RecordMousePosition();
        if (Input.GetMouseButtonDown(0))
        {
            TryToSelectChecker(mouseDownPosition);
        }
        if (_selectedChecker != null)
        {
            _mover.UprageCheckerDragPosition(_selectedChecker);
        }

        if (Input.GetMouseButtonUp(0))
        {
            _hinter.ChangeHighlightState(_validator.ForcedToMoveCheckers);
             if(_secondPlayer == SecondPlayer.Human)
             {
                TryMakeTurn(_selectionPosition, mouseDownPosition);
             }
            else
            {
                if (_isWhiteTurn)
                {
                    TryMakeTurn(_selectionPosition, mouseDownPosition);
                    if (!_isWhiteTurn)
                    {
                        AiTurn();
                    }
                }
              
            }
            _hinter.ShowCurrentTurn(_isWhiteTurn);
        }

        if (_gameState == GameState.Started)
        {
            CheckForEndGameCondition();
        }

    }

   

    private void TryToSelectChecker(Vector2Int mouseDownPosition)
    {
        _validator.SearchForPossibleKills(_board, _isWhiteTurn);
        if (_validator.ForcedToMoveCheckers.Count != 0)
        {
            _hinter.ChangeHighlightState(_validator.ForcedToMoveCheckers);
        }
        if (_validator.CellIsOutOfBounds(_board, mouseDownPosition))
            return;

        var cell = _selecter.PickACell(_board, mouseDownPosition);
        if (_validator.SelectionValidate(cell, _isWhiteTurn))
        {
            _selectedChecker = _selecter.SelectChecker(cell);
            _selectionPosition = mouseDownPosition;
            _sfx.PlayPicKSound();
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

    private void TryMakeTurn(Vector2Int start, Vector2Int final)
    {
        if (_selectedChecker == null)
            return;

        if (_validator.CellIsOutOfBounds(_board,final))
        {
              _mover.Move(_selectedChecker, start);
            _selecter.Deselect(ref _selectedChecker);
            return;
        }

        if (_selectedChecker.IsAbleToMove(_board,start,final, _isWhiteTurn))
        {
            if (Math.Abs(start.x - final.x) >= 2)
            {
                Vector2Int step = Checker.CalculateDirectiobalStep(start, final);
                //Инкрементируем вектор,чтобы не проверять начальную клетку
                Vector2Int startStep = start + step;
                int stepCounter = 0;
                while (stepCounter != Mathf.Abs(final.x - start.x))
                {

                    Checker checkerToDelete = _board[startStep.x, startStep.y];
                    if (checkerToDelete != null)
                    {
                        RemoveChecker(startStep, checkerToDelete);
                        break;
                    }
                    startStep += step;
                    stepCounter++;
                }
            }
            if (_validator.ForcedToMoveCheckers.Count != 0 && !_hasKilled)
            {
                _mover.Move(_selectedChecker, start);
                 _selecter.Deselect(ref _selectedChecker);
                  return;
            }

            _board[final.x, final.y] = _selectedChecker;
            _board[start.x, start.y] = null;
            _mover.Move(_selectedChecker,final);

            EndTurn(final);

            _validator.SearchForPossibleKills(_board, final, _isWhiteTurn);
            if (_validator.ForcedToMoveCheckers.Count != 0 && _hasKilled)
                return;
            SwitchTurn();
        }
        else
        {
            _mover.Move(_selectedChecker, start);
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

    private void EndTurn(Vector2Int final)
    {

        if (_selectedChecker.IsWhite && final.x == 7)
        {
            _selectedChecker.BecomeKing();
        }
        else if (!_selectedChecker.IsWhite && final.x == 0)
        {
            _selectedChecker.BecomeKing();
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
        _history.AddRecord(_selectionPosition, final, _isWhiteTurn);
        
        
    }

    private void SwitchTurn()
    {
        _hasKilled = false;
        _isWhiteTurn = !_isWhiteTurn;
    }
    private void AiTurn()
    {
        while (!_isWhiteTurn)
        {
            var move = _ai.GetRandomMove(_board,_validator, _isWhiteTurn);
            _selectedChecker = move.SelectedChecker;
            _selectionPosition = move.StartPosition;
            TryMakeTurn(move.StartPosition, move.FinalPosition);
        }
    }




}