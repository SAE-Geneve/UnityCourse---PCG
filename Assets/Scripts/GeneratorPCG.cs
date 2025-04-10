using System;
using System.CodeDom.Compiler;
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
    [SerializeField] private int widthMax;

    [Header("Game of life limits")]
    [SerializeField] private int deadLimitMax;
    [SerializeField] private int deadLimitMin;
    [SerializeField] private int aliveLimit;
    [SerializeField] private int fillIterations;

    // private bool _generationDone = true;
    private Coroutine _generationCo = null;

    private readonly Vector2Int[] _directions = new[]
    {
        Vector2Int.up, Vector2Int.right, Vector2Int.down, Vector2Int.left
    };

    private readonly Vector3Int[] _mooreNeighbours = new[]
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
        StartGenerateCoroutines();
    }

    private void OnDrawGizmos()
    {

        SetBarrier();

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(_barrier.center, _barrier.size);
    }

    public void StartGenerateCoroutines()
    {
        if (_generationCo != null)
            StopCoroutine(_generationCo);
        
        _generationCo = StartCoroutine(nameof(GenerateCo));
        
    }
    
    private void GenerateCo()
    {
        map.ClearAllTiles();
        
        Debug.Log("Generate Drunkard");
        GenerateDrunkard();

        Debug.Log("Generate Fill");
        GenerateFillWithLife();


    }    
    private void SetBarrier()
    {

        Vector3Int size = new Vector3Int(widthMax, heightMax, 0);
        // Vector3Int boundsPosition = new Vector3Int(startPosition.x + widhttMax / 2, startPosition.y + heightMax / 2, 0);
        Vector3Int boundsPosition = startPosition - size / 2;

        _barrier = new BoundsInt(boundsPosition, size);
    }



    private void GenerateDrunkard()
    {
        ;
        int tileCount = 0;
        int iterCount = 0;

        Vector3Int position = startPosition;
        AddTile(position, ref tileCount);

        while (tileCount < nbTilesMax && iterCount < iterMax)
        {
            Vector2Int direction = _directions[Random.Range(0, _directions.Length)];
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
            
            Debug.Log($"Tile Count {tileCount} : Iter Count {iterCount}");

        }

    }

    private void GenerateFillWithLife()
    {

        // l’état des cases est évalué 
        // Le nombre de voisin de chaque case est compté
        // Si la case est vivante et qu’elle a 2-3 voisins vivant, elle reste vivante
        // Si la case est morte et qu’elle a 3 voisins vivants, elle devient vivante
        // Sinon elle considérée comme morte
        
        List<Vector3Int> aliveCells = new List<Vector3Int>();
        List<Vector3Int> deadCells = new List<Vector3Int>();

        for(int nbIter = 0; nbIter < fillIterations; nbIter++)
        {
            
            aliveCells.Clear();
            deadCells.Clear();
            
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
                        // if (countAlive > deadLimitMax || countAlive < deadLimitMin)
                        // {
                        //     // Elle meurt
                        //     deadCells.Add(position);
                        // }
                        // else
                        // {
                        //     aliveCells.Add(position);
                        //     // Sinon Elle reste en vie
                        // }
                    }
                    else
                    {
                        // Elle est morte
                        if (countAlive >= aliveLimit + nbIter)
                        {
                            // Elle devient vivante
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
                if (!map.HasTile(aliveCell)) map.SetTile(aliveCell, GetRandomTile());
            }
            foreach (Vector3Int deadCell in deadCells)
            {
                if (map.HasTile(deadCell)) map.SetTile(deadCell, null);
            }

            // debut boucle
            // Compter les voisins de chaque case
            // si les conditions sont bonnes => on met une nouvelle tile
            // si les conditions sont pas bonnes => on met une nouvelle tile
            // fin boucle

            //  il y a plus rien qui bouge
            Debug.Log($"Game of life : Fill iteration {nbIter}");
            
        }
        
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
