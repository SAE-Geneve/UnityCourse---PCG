using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Timeline.Actions;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

// ReSharper disable once InconsistentNaming
public class BSPGenerator : MonoBehaviour
{

    [SerializeField] private int _startWidth;
    [SerializeField] private int _startHeight;
    [SerializeField] private Vector2Int _dungeonCenter;

    [SerializeField] private Tilemap _map;
    [SerializeField] private List<TileBase> _tiles;
    [SerializeField] private GameObject _startItem;
    [SerializeField] private GameObject _endItem;
    [SerializeField] private GameObject _doorItem;

    [Header("Settings")]
    [SerializeField] private float _maxSize = 20f;
    [SerializeField] private float _cutRange = 20f;
    [SerializeField] private int _borderSize = 2;

    private BSPNode _bspRoot;
    private List<GameObject> _generatedElements = new List<GameObject>();

    public struct Settings
    {
        public float MaxSize;
        public float CutRange;
        public int BorderSize;

        public Settings(float maxSize, float cutRange, int borderSize)
        {
            MaxSize = maxSize;
            CutRange = cutRange;
            BorderSize = borderSize;
        }

    }
    public struct Transform
    {
        public Vector3 Position;
        public Quaternion Rotation;

        public Transform(Vector3 pos, Quaternion rot)
        {
            Position = pos;
            Rotation = rot;
        }
    }

    private readonly List<BSPNode> _BSPLeaves = new List<BSPNode>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Generate();

    }

    public void Generate()
    {
        // Clearance objects
        _BSPLeaves.Clear();
        _map.ClearAllTiles();
        foreach (var go in _generatedElements)
        {
            if (Application.isEditor)
                DestroyImmediate(go);
            else
                Destroy(go);
        }
        _generatedElements.Clear();

        _bspRoot = new BSPNode(_dungeonCenter.x, _dungeonCenter.y, _startWidth, _startHeight, true);
        _bspRoot.Process(_BSPLeaves, new Settings(_maxSize, _cutRange, _borderSize));

        for (int idxRoom = 0; idxRoom < _BSPLeaves.Count; idxRoom++)
        {
            FillTilemap(_BSPLeaves[idxRoom].Room);
        }

        List<BSPGenerator.Transform> doors = SetDoors(_BSPLeaves);
        foreach (BSPGenerator.Transform door in doors)
        {
            AddItem(_doorItem, door.Position);
        }

        /*  TODO : See soon after doors completion
        SetCorridors(out Vector3Int startPosition, out Vector3Int endPosition);

        GameObject generatedItem;

        generatedItem = Instantiate(_startItem, startPosition, Quaternion.identity);
        _generatedElements.Add(generatedItem);

        generatedItem = Instantiate(_endItem, endPosition, Quaternion.identity);
        _generatedElements.Add(generatedItem);
        */
    }
    private void AddItem(GameObject item, Vector3 position)
    {
        GameObject generatedItem;

        generatedItem = Instantiate(item, position, Quaternion.identity);
        _generatedElements.Add(generatedItem);
    }

    private List<BSPGenerator.Transform> SetDoors(List<BSPNode> leaves)
    {

        List<Transform> doors = new List<Transform>();

        foreach (var leaf in leaves)
        {
            BSPGenerator.Transform[] doorPositions =
            {
                new Transform(new Vector3(leaf.Room.center.x, leaf.Room.yMax, 0), Quaternion.Euler(0, 0, 0)),
                new Transform(new Vector3(leaf.Room.xMax, leaf.Room.center.y, 0), Quaternion.Euler(0, 90, 0)),
                new Transform(new Vector3(leaf.Room.center.x, leaf.Room.yMin, 0), Quaternion.Euler(0, 180, 0)),
                new Transform(new Vector3(leaf.Room.xMin, leaf.Room.center.y, 0), Quaternion.Euler(0, 270, 0))
            };

            foreach (BSPGenerator.Transform doorPosition in doorPositions)
            {
                doors.Add(doorPosition);
            }

        }

        return doors;

    }

    private void SetCorridors(out Vector3Int startPosition, out Vector3Int endPosition)
    {

        List<BoundsInt> orderedRooms = _BSPLeaves
            .OrderBy(r => Vector3.Distance(new Vector3(_dungeonCenter.x, _dungeonCenter.y, 0), r.Room.center))
            .Select(r => r.Room)
            .ToList();

        BoundsInt startRoom = orderedRooms.First();
        BoundsInt endRoom = orderedRooms.Last();

        startPosition = new Vector3Int((int)startRoom.center.x, (int)startRoom.center.y, 0);
        endPosition = new Vector3Int((int)endRoom.center.x, (int)endRoom.center.y, 0);

        List<Vector3Int> mainCorridor = new List<Vector3Int>();
        Vector3Int corridorPosition = startPosition;

        Vector3Int corridorDirection = endPosition - startPosition;

        do
        {
            corridorPosition += corridorDirection.x >= 0 ? new Vector3Int(1, 0, 0) : new Vector3Int(-1, 0, 0);
            mainCorridor.Add(corridorPosition);
        } while (corridorPosition.x != endPosition.x);

        do
        {
            corridorPosition += corridorDirection.y >= 0 ? new Vector3Int(0, 1, 0) : new Vector3Int(0, -1, 0);
            mainCorridor.Add(corridorPosition);
        } while (corridorPosition.y != endPosition.y);

        foreach (var tilePos in mainCorridor)
        {
            _map.SetTile(tilePos, GetRandomTile());
        }

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnDrawGizmos()
    {
        if (_bspRoot != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireCube(_bspRoot.Room.center, _bspRoot.Room.size);
        }

        foreach (BSPNode room in _BSPLeaves)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireCube(room.Room.center, room.Room.size);
        }

    }

    private void FillTilemap(BoundsInt bounds)
    {
        // Parcourir le rectangle
        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                _map.SetTile(new Vector3Int(x, y, 0), GetRandomTile());
            }
        }
    }


    // private void AddTile(Vector3Int position, ref int count)
    // {
    //
    //     if (map.HasTile(position))
    //         return;
    //
    //     map.SetTile(position, GetRandomTile());
    //     count++;
    //
    // }
    private TileBase GetRandomTile()
    {
        return _tiles[Random.Range(0, _tiles.Count)];
    }
}
