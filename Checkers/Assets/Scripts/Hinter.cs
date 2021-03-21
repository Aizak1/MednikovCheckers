using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Hinter : MonoBehaviour
{
    [SerializeField] private GameObject _whiteDecorative;
    [SerializeField] private GameObject _blackDecorative;
    [SerializeField] private Vector3 _whiteWaterSurface;
    [SerializeField] private Vector3 _blackWaterSurface;
    [SerializeField] private Material _materialToHighlightForces;
    private Material _initialMaterial;


    private Vector3 _initialWhitePosition;
    private Vector3 _initialBlackPosition;
    [SerializeField] private float _speed;
    private void Start()
    {
        _initialWhitePosition = _whiteDecorative.transform.position;
        _initialBlackPosition = _blackDecorative.transform.position;
        _initialMaterial = _whiteDecorative.GetComponent<Renderer>().sharedMaterial;
        StartCoroutine(FirstTurn());
    }
    
    public void ShowCurrentTurn(bool isWhiteTurn)
    {
        StartCoroutine(ShowDecorativeInWater(isWhiteTurn));
    }
    private IEnumerator FirstTurn()
    {
        while (_whiteDecorative.transform.position!=_whiteWaterSurface)
        {
            MoveDecorativeObject(_whiteDecorative, _whiteWaterSurface);
            yield return null;
        }
        
    }
    private IEnumerator ShowDecorativeInWater(bool isWhiteTurn)
    {
        if (isWhiteTurn)
        {
            while (_whiteDecorative.transform.position != _whiteWaterSurface)
            {
                MoveDecorativeObject(_whiteDecorative, _whiteWaterSurface);
                MoveDecorativeObject(_blackDecorative, _initialBlackPosition);
                yield return null;
            }
        }
        else
        {
            while(_whiteDecorative.transform.position != _initialWhitePosition)
            {
                MoveDecorativeObject(_whiteDecorative, _initialWhitePosition);
                MoveDecorativeObject(_blackDecorative, _blackWaterSurface);
                yield return null;
            }
            
        }    
    }

    private void MoveDecorativeObject(GameObject decorativeobject,Vector3 direction)
    {
        decorativeobject.transform.position = Vector3.MoveTowards(decorativeobject.transform.position, direction, _speed * Time.deltaTime);
    }
    public void ChangeHighlightState(List<Checker> forcedToMoveCheckers)
    {
        foreach (var item in forcedToMoveCheckers)
        {

            var renderer = item.gameObject.GetComponent<Renderer>();
            if (renderer.sharedMaterial  == _initialMaterial)
                renderer.sharedMaterial = _materialToHighlightForces;
            else
                renderer.sharedMaterial = _initialMaterial;
        }
    }
   





}
