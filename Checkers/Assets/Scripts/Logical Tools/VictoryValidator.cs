using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class VictoryValidator : MonoBehaviour
{
    [SerializeField] private Canvas _endMenu;
    [SerializeField] private Text _text;
    private MainLogic _logic;
    private void Start()
    {
        _logic = FindObjectOfType<MainLogic>();
    }

    private void Update()
    {
        if(_logic.GetGameState == GameState.Ended)
        {
            _endMenu.enabled = true;
            if (_logic.Result == ResultOfGame.WhiteWins)
                _text.text = "White Wins";
            else if (_logic.Result == ResultOfGame.BlackWins)
                _text.text = "Black Wins";
            else
                _text.text = "Draw Match";
        }
        
    }

   
}
