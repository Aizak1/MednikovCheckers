using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class VictoryValidator : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private Canvas _endMenu;
    [SerializeField] private Text _text;
    private MainLogic board;
    private void Start()
    {
        board = FindObjectOfType<MainLogic>();
       
    }

    // Update is called once per frame
    private void Update()
    {
        if(board.GetGameState == GameState.Ended)
        {
            _endMenu.enabled = true;
            if (board.Result == ResultOfGame.WhiteWins)
                _text.text = "White Wins";
            else if (board.Result == ResultOfGame.BlackWins)
                _text.text = "Black Wins";
            else
                _text.text = "Draw";
        }
        
    }

    public void RestartTheScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
       
    }
}
