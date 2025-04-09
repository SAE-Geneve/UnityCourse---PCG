using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class GeneratorPCG : MonoBehaviour
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

    private Vector3Int[] _mooreNeighbours = new[]
    {
        new Vector3Int(0, 1, 0),
        new Vector3Int(1, 1, 0),
        new Vector3Int(1, 0, 0),
        new Vector3Int(1, -1, 0),
        new Vector3Int(0, -1, 0),
        new Vector3Int(-1, -1, 0),
        new Vector3Int(-1, 0, 0),
        new Vector3Int(-1, 1, 0)
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
        Debug.Log("Generate Drunkard");
        yield return GenerateDrunkard();

        Debug.Log("Wait");
        yield return new WaitForSeconds(1);

        Debug.Log("Generate Fill");
        yield return GenerateFillWithLife();

    }

    private IEnumerator GenerateDrunkard()
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

    }

    private IEnumerator GenerateFillWithLife()
    {

        // l’état des cases est évalué 
        // Le nombre de voisin de chaque case est compté
        // Si la case est vivante et qu’elle a 2-3 voisins vivant, elle reste vivante
        // Si la case est morte et qu’elle a 3 voisins vivants, elle devient vivante
        // Sinon elle considérée comme morte
        
        List<Vector3Int> aliveCells = new List<Vector3Int>();
        List<Vector3Int> deadCells = new List<Vector3Int>();

        // Parcourir le rectangle
        for (int x = _barrier.xMin; x < _barrier.xMax; x++)
        {
            for (int y = _barrier.yMin; y < _barrier.yMax; y++)
            {
                //Debug.Log($"x={x} : y={y} : {map.GetTile(new Vector3Int(x, y))}");

                Vector3Int position = new Vector3Int(x, y);

                // check if cell is dead or alive
                bool isAlive = map.HasTile(position);

                // check how many neighbours are dead or alive
                int countAlive = 0;
                foreach (Vector3Int neighbour in _mooreNeighbours)
                {
                    if (IsInBounds(position + neighbour))
                    {
                        TileBase t = map.GetTile(position + neighbour);
                        if (t != null)
                        {
                            // il y a bien une tile a cet emplacement
                            countAlive++;
                        }
                    }
                }

                if (isAlive)
                {
                    
                    
                    if (countAlive > 3 || countAlive < 2)
                    {
                        // Elle meurt
                        deadCells.Add(position);
                    }else
                    {
                        aliveCells.Add(position);
                        // Sinon Elle reste en vie
                    }
                    
                }
                else
                {
                    // Elle est morte
                    if (countAlive == 3)
                    {
                        // Elle devient vivante
                        // map.SetTile(position, GetRandomTile());
                        
                        aliveCells.Add(position);
                    }
                    else
                    {
                        // sinon Elle reste morte
                        deadCells.Add(position);
                    }
                }


                
            }
        }

        foreach (Vector3Int aliveCell in aliveCells)
        {
            if(!map.HasTile(aliveCell)) map.SetTile(aliveCell, GetRandomTile());
            yield return new WaitForSeconds(0.01f);
        }
        foreach (Vector3Int deadCell in deadCells)
        {
            if(map.HasTile(deadCell)) map.SetTile(deadCell, null);
            yield return new WaitForSeconds(0.01f);
        }
        
        // debut boucle
        // Compter les voisins de chaque case
        // si les conditions sont bonnes => on met une nouvelle tile
        // si les conditions sont pas bonnes => on met une nouvelle tile
        // fin boucle

        //  il y a plus rien qui bouge


    }

    private void AddTile(Vector3Int position, ref int count)
    {

        if (map.HasTile(position))
            return;

        map.SetTile(position, GetRandomTile());
        count++;

    }
    private TileBase GetRandomTile()
    {

        return tiles[Random.Range(0, tiles.Count)];
    }
    private bool IsInBounds(Vector3Int position)
    {

        return (position.x >= _barrier.xMin && position.x < _barrier.xMax && position.y >= _barrier.yMin &&
                position.y < _barrier.yMax);

    }
}
