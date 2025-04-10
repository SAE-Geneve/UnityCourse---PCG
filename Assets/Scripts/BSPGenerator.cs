using System;
using UnityEngine;

public class BSPGenerator : MonoBehaviour
{

    [SerializeField] private int startWidth;
    [SerializeField] private int startHeight;
    [SerializeField] private Vector2Int startPosition;

    private BSPNode _bspRoot;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _bspRoot = new BSPNode(startPosition.x, startPosition.y, startWidth, startHeight);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDrawGizmos()
    {
        if(_bspRoot != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireCube(_bspRoot.Room.center, _bspRoot.Room.size);
        }
    }
}
