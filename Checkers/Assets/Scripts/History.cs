using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class History : MonoBehaviour
{
    private Text _fieldText;
    private void Start()
    {
        _fieldText = GetComponent<Text>();
    }

    public void AddRecord(Vector2Int start,Vector2Int final,bool isWhiteTurn)
    {
        string lettersOnBoard = "ABCDEFGH";
       
        if (isWhiteTurn)
            _fieldText.text = $" W - {lettersOnBoard[start.y]}{start.x+1} : {lettersOnBoard[final.y]}{final.x+1} \n" +  _fieldText.text;
        else
            _fieldText.text =  $" B - {lettersOnBoard[start.y]}{start.x + 1} : {lettersOnBoard[final.y]}{final.x + 1} \n" + _fieldText.text;
        
    }

   
}
