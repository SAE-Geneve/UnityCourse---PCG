using UnityEngine;


public class BSPNode
{

    private readonly BoundsInt _room;

    private BSPNode _left;
    private BSPNode _right;

    private bool _verticalSliced;
    
    public BoundsInt Room => _room;
    

    public BSPNode(int x, int y, int width, int height)
    {
        Vector3Int size = new Vector3Int(width, height, 0);
        Vector3Int position = new Vector3Int(x,y,0) - size / 2;

        _room = new BoundsInt(position, size);
    }

    public void Process()
    {
        // Cut into smaller if needed
        // Then process the children
    }

}
