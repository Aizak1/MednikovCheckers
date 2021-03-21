using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class History : MonoBehaviour
{
    private Text _fieldText;
   
    private void Start()
    {
        _fieldText = GetComponent<Text>();
    }

    public void AddRecord(int x1,int z1,int x2,int z2,bool isWhiteTurn)
    {
        string lettersOnBoard = "ABCDEFGH";
        if (isWhiteTurn)
            _fieldText.text = $" W - {lettersOnBoard[z1]}{x1+1} : {lettersOnBoard[z2]}{x2+1} \n" +  _fieldText.text;
        else
            _fieldText.text =  $" B - {lettersOnBoard[z1]}{x1+1} : {lettersOnBoard[z2]}{x2+1} \n" + _fieldText.text;
        

    }
}
