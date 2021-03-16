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
        
        
    }

    public void RestartTheScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
       
    }
}
