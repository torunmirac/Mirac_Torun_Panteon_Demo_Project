using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [SerializeField] private int width, height;

    [SerializeField] private Tile tilePrefab;

    [SerializeField] private Transform cam, parent;

    [SerializeField] private List<GameObject> squaresList;    ///Bu liste başka bir managerde olabilir



    void Start()
    {
        GenerateGrid();
    }

    void GenerateGrid()
    {
        for (float x = 0; x < width; x += .32f)
        {
            for (float y = 0; y < height; y += 0.32f)
            {
                var spawnedTile = Instantiate(tilePrefab, new Vector3(x, y), Quaternion.identity, parent);

                squaresList.Add(spawnedTile.gameObject);
                int index = squaresList.IndexOf(spawnedTile.gameObject);
                var isOffset = (index % 2 == 0);
                spawnedTile.Init(isOffset);
            }
        }
        cam.transform.position = new Vector3(8.04f, 3.63f, -5);
        
    }
}