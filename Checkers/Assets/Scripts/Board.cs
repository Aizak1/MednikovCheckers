using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    [SerializeField] private GameObject _whitePrefab;
    [SerializeField] private GameObject _blackPrebaf;
    private Checker[,] _checkers = new Checker[8, 8];
    private Vector3 _firstblackPos = new Vector3(-3.5f, 0.1f,-3.5f);
    private Vector3 _firstwhitePos = new Vector3(1.5f, 0.1f, -3.5f);

    public void GenerateBoard()
    {
        for (int x = 0; x < 3; x++)
        {
            int initialCoordinate;
            if (x % 2 == 0)
                initialCoordinate = 0;
            else
                initialCoordinate = 1;

            for (int z = initialCoordinate; z < 8; z+=2)
            {
                GenerateChecker(_blackPrebaf,_firstblackPos,x, z);
            }
        }
      
    }

    private void GenerateChecker(GameObject prefab,Vector3 position,int x, int z)
    {
        GameObject piece = Instantiate(prefab, new Vector3(position.x + x, position.y, position.z + z),Quaternion.Euler(-89.98f,0,0),transform);
        _checkers[x, z] = piece.GetComponent<Checker>();
    }

    void Start()
    {
        GenerateBoard();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
