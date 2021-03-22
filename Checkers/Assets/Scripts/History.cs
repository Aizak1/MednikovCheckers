using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class History : MonoBehaviour
{
    private Text _fieldText;
    public Stack<Tuple<Vector2Int, Vector2Int,bool>> StepsHistory { get; private set; }
    private void Start()
    {
        _fieldText = GetComponent<Text>();
        StepsHistory = new Stack<Tuple<Vector2Int, Vector2Int, bool>>();
    }

    public void AddRecord(Vector2Int start,Vector2Int final,bool isWhiteTurn)
    {
        string lettersOnBoard = "ABCDEFGH";
        StepsHistory.Push(Tuple.Create(start, final,isWhiteTurn));
        if (isWhiteTurn)
            _fieldText.text = $" W - {lettersOnBoard[start.y]}{start.x+1} : {lettersOnBoard[final.y]}{final.x+1} \n" +  _fieldText.text;
        else
            _fieldText.text =  $" B - {lettersOnBoard[start.y]}{start.x + 1} : {lettersOnBoard[final.y]}{final.x + 1} \n" + _fieldText.text;
        
    }

    public Tuple<Vector2Int,Vector2Int,bool> RemoveStepFromHistory()
    {
        var words = Regex.Split(_fieldText.text, @"\n+");
        _fieldText.text = "";
        for (int i = 1; i < words.Length; i++)
        {
            _fieldText.text = words[i] + _fieldText.text;
        }
        if (StepsHistory.Count != 0)
            return StepsHistory.Pop();
        else
            return null;
    }
}
