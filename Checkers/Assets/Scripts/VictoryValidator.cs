using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class VictoryValidator : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]Canvas _endMenu;
    [SerializeField] Text _text;
    Board board;
    void Start()
    {
        board = FindObjectOfType<Board>();
       
    }

    // Update is called once per frame
    void Update()
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
