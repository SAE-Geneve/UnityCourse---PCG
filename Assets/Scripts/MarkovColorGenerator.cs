using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class MarkovColorGenerator : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _sprite;
    [SerializeField] private int _nbChances = 20;
    
    private MarkovColorChain _colorChain = new MarkovColorChain();
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        for (int idxObject = 0; idxObject < _nbChances; idxObject++)
        {
            Maximilien(_colorChain.GetNextColor(), new Vector3(idxObject, 0, 0));
        }
    }

    private void Maximilien(Color color, Vector3 position)
    {
        SpriteRenderer sr = Instantiate(_sprite, position, quaternion.identity);
        sr.color = color;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

public class MarkovColorChain
{
    private MarkovColorNode _currentNode;
    
    public MarkovColorChain()
    {
        MarkovColorNode less_dark = new MarkovColorNode(new Color(20/(float)255,63/(float)255,96/(float)255));
        MarkovColorNode bright = new MarkovColorNode(new Color(86/(float)255, 162/(float)255, 232/(float)255));
        MarkovColorNode less_bright = new MarkovColorNode(new Color(34/(float)255, 115/(float)255, 180/(float)255));
        MarkovColorNode dark = new MarkovColorNode(new Color(18/(float)255, 30/(float)255, 39/(float)255));

        less_dark.Attach(bright, 0.9f);
        less_dark.Attach(less_dark, 0.1f);
        
        bright.Attach(bright, 0.1f);
        bright.Attach(less_dark, 0.4f);
        bright.Attach(less_bright, 0.3f);
        bright.Attach(dark, 0.2f);
        
        less_bright.Attach(dark, 1f);
        
        dark.Attach(less_dark, 0.99f);
        dark.Attach(dark, 0.01f);

        // Arbitrary choice
        // ??? TODO Get a random node
        _currentNode = bright;

    }

    public Color GetNextColor()
    {
        // Switch to next node
        _currentNode = _currentNode.GetNextNode();
        Debug.Log($"New node : {_currentNode}");
        return _currentNode.Color;
    }
}

public struct MarkovColorNode
{

    struct Link{
        public MarkovColorNode Node;
        public float Chance;
    }
    
    private Color _color;
    private List<Link> _links;

    public Color Color => _color;

    public MarkovColorNode(Color color)
    {
        _color = color;
        _links = new List<Link>();
    }
    
    public void Attach(MarkovColorNode node, float chance)
    {
        Link newLink = new Link();
        newLink.Node = node;
        newLink.Chance = chance;
        
        _links.Add(newLink);
    }

    public MarkovColorNode GetNextNode()
    {
        float x = Random.Range(0f, 1f);
        
        float maxRange = 0;
        foreach (Link link in _links)
        {
            maxRange += link.Chance;
            if (x < maxRange)
            {
                return link.Node;
            }
        }

        return this;

    }

    public override string ToString()
    {
        return $"Color : {_color}";
    }

}


