using System.Collections.Generic;
using UnityEngine;


// ReSharper disable once InconsistentNaming
public class BSPNode
{

    private BoundsInt _room;

    private BSPNode _left;
    private BSPNode _right;

    private readonly bool _verticalSliced; // True : Vertical cut, False : Horizontal cut

    // ReSharper disable once ConvertToAutoPropertyWhenPossible
    public BoundsInt Room => _room;


    public BSPNode(int centerX, int centerY, int width, int height, bool slice)
    {
        Vector3Int size = new Vector3Int(width, height, 0);
        Vector3Int position = new Vector3Int(centerX, centerY, 0) - size / 2;

        _room = new BoundsInt(position, size);
        _verticalSliced = slice;
    }

    private BSPNode(BoundsInt bounds, bool slice)
    {
        _room = bounds;
        _verticalSliced = slice;
    }

    public void Process(List<BSPNode> leaves, BSPGenerator.Settings settings)
    {

        if (_room.size.x * _room.size.y <= settings.MaxSize)
        {
            Vector3Int newSize = _room.size - new Vector3Int(settings.BorderSize, settings.BorderSize, 0);
            _room.position = new Vector3Int(
                Mathf.RoundToInt(_room.center.x - 0.5f * (float)newSize.x),
                Mathf.RoundToInt(_room.center.y - 0.5f * (float)newSize.y),
                Mathf.RoundToInt(_room.center.z - 0.5f * (float)newSize.z)
            );
            _room.size = newSize;
            leaves.Add(this);
            return;
        }

        float ratio = Random.Range(0.5f - settings.CutRange, 0.5f + settings.CutRange);

        // Cut into smaller if needed
        BoundsInt left = new BoundsInt(_room.position, _room.size);
        BoundsInt right = new BoundsInt(_room.position, _room.size);
        Vector3 cutPoint = _room.position + new Vector3(ratio * _room.size.x, ratio * _room.size.y, ratio * _room.size.z);

        if (_verticalSliced)
        {
            left.xMax = Mathf.RoundToInt(cutPoint.x);
            right.xMin = Mathf.RoundToInt(cutPoint.x);
        }
        else
        {
            left.yMax = Mathf.RoundToInt(cutPoint.y);
            right.yMin = Mathf.RoundToInt(cutPoint.y);
        }

        // Construct the tree
        _left = new BSPNode(left, !_verticalSliced);
        _right = new BSPNode(right, !_verticalSliced);

        _left.Process(leaves, settings);
        _right.Process(leaves, settings);

        // Examine the criterias
        // Then process the children
        // if (_left.Room.size.x * _left.Room.size.y <= maxSize)
        // {
        //     rooms.Add(left);
        // }
        // else
        // {
        //     _left.Process(rooms, maxSize);
        // }
        //
        // if (_right.Room.size.x * _right.Room.size.y <= maxSize)
        // {
        //     rooms.Add(right);
        // }
        // else
        // {
        //     _right.Process(rooms, maxSize);
        // }


    }

}
