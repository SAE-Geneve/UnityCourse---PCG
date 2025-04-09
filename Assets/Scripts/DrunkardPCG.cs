using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class DrunkardPCG : MonoBehaviour
{

    [SerializeField] private Tilemap map;
    [SerializeField] private List<TileBase> tiles;
    [SerializeField] private Vector3Int startPosition = Vector3Int.zero;

    [Header("Settings")]
    [SerializeField] private int lMin;
    [SerializeField] private int lMax;
    [SerializeField] private int iterMax;
    [SerializeField] private int nbTilesMax;
    [SerializeField] private int heightMax;
    [SerializeField] private int widhttMax;




    private Vector2Int[] _directions = new[]
    {
        Vector2Int.up, Vector2Int.right, Vector2Int.down, Vector2Int.left
    };

    private BoundsInt _barrier;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SetBarrier();

        StartCoroutine("Generate");
    }

    private void OnDrawGizmos()
    {

        SetBarrier();

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(_barrier.center, _barrier.size);
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void SetBarrier()
    {

        Vector3Int size = new Vector3Int(widhttMax, heightMax, 0);
        // Vector3Int boundsPosition = new Vector3Int(startPosition.x + widhttMax / 2, startPosition.y + heightMax / 2, 0);
        Vector3Int boundsPosition = startPosition - size / 2;

        _barrier = new BoundsInt(boundsPosition, size);
    }

    private IEnumerator Generate()
    {
        Vector2Int direction = Vector2Int.zero;
        Vector3Int position = Vector3Int.zero;

        int tileCount = 0;
        int iterCount = 0;

        AddTile(startPosition, ref tileCount);

        while (tileCount < nbTilesMax && iterCount < iterMax)
        {
            direction = _directions[Random.Range(0, _directions.Length)];
            int currentPathLength = Random.Range(lMin, lMax);

            Vector3Int futurePosition = position + currentPathLength * new Vector3Int(direction.x, direction.y, 0);

            if (IsInBounds(futurePosition))
            {
                for (int i = 0; i < currentPathLength; i++)
                {
                     position += new Vector3Int(direction.x, direction.y, 0);
                    AddTile(position, ref tileCount);
                }

                iterCount++;

            }
            
            yield return new WaitForSeconds(0.01f);
            Debug.Log($"Tile Count {tileCount} : Iter Count {iterCount}");

        }


        //     
        //     [SerializeField] private int iterMax;
        // [SerializeField] private int nbTilesMax;
    }
    private void AddTile(Vector3Int position, ref int count)
    {

        if (map.HasTile(position))
            return;

        map.SetTile(position, tiles[Random.Range(0, tiles.Count)]);
        count++;

    }

    private bool IsInBounds(Vector3Int position)
    {

        return (position.x >= _barrier.xMin && position.x < _barrier.xMax && position.y >= _barrier.yMin &&
                position.y < _barrier.yMax);

    }
}
