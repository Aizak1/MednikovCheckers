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
    private  int _stepsWithoutKills = 0;
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

            if ((x <= 2 || x >= 5))
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
            TryToSelectChecker(mouseDownPosition);

        if (_selectedChecker != null)
            _mover.UprageCheckerDragPosition(_selectedChecker);

        if (Input.GetMouseButtonUp(0))
        {
            _hinter.ChangeHighlightState(_validator.SearchForPossibleKills(_board,_isWhiteTurn));
             TryMakeTurn(_selectionPosition, mouseDownPosition);
            if (_secondPlayer == SecondPlayer.Human)
                _hinter.ShowCurrentTurn(_isWhiteTurn);
        }
        if (_gameState == GameState.Started)
            CheckForEndGameCondition();

        if (!_isWhiteTurn && _secondPlayer == SecondPlayer.AI)
            AiMakeTurn();
       
    }

   

    private void TryToSelectChecker(Vector2Int mouseDownPosition)
    {
        var forcedToMoveCheckers = _validator.SearchForPossibleKills(_board, _isWhiteTurn);
        if (forcedToMoveCheckers.Count != 0)
        {
            _hinter.ChangeHighlightState(forcedToMoveCheckers);
        }
        if (_validator.CellIsOutOfBounds(_board, mouseDownPosition))
            return;

        var cell = _selecter.PickACell(_board, mouseDownPosition);
        if (_validator.SelectionIsValid(cell, _isWhiteTurn, forcedToMoveCheckers))
        {
            _selectedChecker = cell;
            _selectionPosition = mouseDownPosition;
            _sfx.PlayPicKSound();
        }
        
    }

    private void CheckForEndGameCondition()
    {
        var moves = Move.GetAllMoves(_board, _validator, _isWhiteTurn);
        if(_isWhiteTurn && moves.Count == 0)
        {
            Result = ResultOfGame.BlackWins;
            _gameState = GameState.Ended;
        }
        if(!_isWhiteTurn && moves.Count == 0)
        {
            Result = ResultOfGame.WhiteWins;
            _gameState = GameState.Ended;
        }
        //15 ходов без боя только дамками,считаются ничьей
        if (_stepsWithoutKills == 15)
        {
            Result = ResultOfGame.Draw;
            _gameState = GameState.Ended;
           
        }

    }

    private void TryMakeTurn(Vector2Int start, Vector2Int final)
    {
        if (_selectedChecker == null)
            return;

        if (_validator.CellIsOutOfBounds(_board, final) || !_selectedChecker.IsAbleToMove(_board, start, final, _isWhiteTurn))
        {
            _mover.VisualReplace(_selectedChecker, start);
            _selecter.Deselect(ref _selectedChecker);
            return;
        }

        if (Math.Abs(start.x - final.x) >= 2)
        {
            if (_validator.HasCheckerToKill(_board, start, final, out Checker checkerToDelete, out Vector2Int deletePosition))
                RemoveChecker(deletePosition, checkerToDelete);
        }

        if (_validator.SearchForPossibleKills(_board, _isWhiteTurn).Count != 0 && !_hasKilled)
        {
            _mover.VisualReplace(_selectedChecker, start);
            _selecter.Deselect(ref _selectedChecker);
            return;
        }
        _mover.VisualReplace(_selectedChecker, final);
        EndTurn(start, final);
    }


    private void EndTurn(Vector2Int start, Vector2Int final)
    {
        MakeTurn(start, final);
        if (_validator.SearchForPossibleKills(_board, final, _isWhiteTurn).Count != 0 && _hasKilled)
            return;
        SwitchTurn();
    }

    private void RemoveChecker(Vector2Int deletePosition, Checker checkerToDelete)
    {
        _board[deletePosition.x, deletePosition.y] = null;
        Destroy(checkerToDelete.gameObject);
        _hasKilled = true;
        Instantiate(_vfx, new Vector3(deletePosition.x, 0, -deletePosition.y), Quaternion.identity);
    }

    private void MakeTurn(Vector2Int start,Vector2Int final)
    {
        _board[final.x, final.y] = _selectedChecker;
        _board[start.x, start.y] = null;

        if (_selectedChecker.IsWhite && final.x == 7)
        {
            _selectedChecker.BecomeKing();
        }
        else if (!_selectedChecker.IsWhite && final.x == 0)
        {
            _selectedChecker.BecomeKing();
        }

        if (_hasKilled)
        {
            _sfx.PlayKillSound();
            _stepsWithoutKills = 0;
        }
        else
        {
            _sfx.PlayDropSound();
            if(!_selectedChecker.IsSimple)
                _stepsWithoutKills++;
        }
        _selecter.Deselect(ref _selectedChecker);
        _history.AddRecord(_selectionPosition, final, _isWhiteTurn);
        
        
    }

    private void SwitchTurn()
    {
        _hasKilled = false;
        _isWhiteTurn = !_isWhiteTurn;
    }
    private void AiMakeTurn()
    {
        // 3 - оптимальная глубина для корректного и быстрого расчета хода ИИ
        Move bestMove = _ai.Minimax(_board, _validator, _isWhiteTurn, 3, _gameState).Item2;
        if (bestMove == null)
        {
            //В случае если нет выигрышного хода,выбирает случайный
            bestMove = _ai.GetRandomMove(_board, _validator, _isWhiteTurn);
            if (bestMove == null)
                return;
        }
        _selectedChecker = bestMove.SelectedChecker;
         _selectionPosition = bestMove.StartPosition;
         TryMakeTurn(bestMove.StartPosition, bestMove.FinalPosition); 
            
    }




}