using UnityEngine;


/// <summary>
/// Controls the game area by creating a pixel perfect level and manipulating pixel colors.
/// </summary>

public class GameControl : MonoBehaviour
{
    //Declaring texture objects for level
    public Texture2D levelTexture;
    Texture2D textureInstance;
    public SpriteRenderer levelRenderer;

    //Declaring variables for max coordinates of area and offset of pixels.
    public float posOffset = 0.01f;
    int maxX;
    int maxY;

    //Declaring array of Nodes "pixel" we use to generate level matrix.
    Node[,] grid;

    //Declaring Nodes for comparing the values.
    Node currentNode;
    Node previousNode;

    //Declaring Vector3 for mouse position.
    Vector3 mousePos;

    //Eraser
    public float editRadius = 6;

    //Spawning units
    public Node spawnNode;
    public Transform spawnTransform;
    public Vector3 spawnPosition;

    public UnitControl unit;


    //Builds the level when the game starts.
    private void Start()
    {
        CreateLevel();
        spawnNode = GetNodeFromWorldPos(spawnTransform.position);
        spawnPosition = GetWorldPosFromNode(spawnNode);
        unit.Init(this);
    }

    /// <summary>
    /// This function that generates the level.
    /// 
    /// Initializes max X coordinate by texture width.
    /// Y coordinate with texture height,
    /// Builds the "GRID" Node object array and limits maximum coordinates.
    /// At the sametime builds Texture2D object and changes it filter config to Point.
    /// After this builds Node for each x and y value starting from 0 -> max X and max Y.
    /// and sets the Node coordinate to current coordinates.
    /// Then Creates Color which is initialized by the current color of texture coordinates.
    /// then sets textureInstance pixel coordinates and color.
    /// Also sets node isEmpty to be equal color zero.
    /// Grid array x and y is equal to node values.
    /// after looping through every coordinate / matrix.
    /// applies the textureInstance, renders the map.
    /// </summary>
    void CreateLevel()
    {
        maxX = levelTexture.width;
        maxY = levelTexture.height;
        grid = new Node[maxX, maxY];
        textureInstance = new Texture2D(maxX, maxY);
        textureInstance.filterMode = FilterMode.Point;

        for (int x = 0; x < maxX; x++)
        {
            for (int y = 0; y < maxY; y++)
            {
                Node n = new Node();
                n.x = x;
                n.y = y;

                Color c = levelTexture.GetPixel(x, y);

                textureInstance.SetPixel(x, y, c);

                n.isEmpty = (c.a == 0);

                grid[x, y] = n;
            }
        }
        textureInstance.Apply();
        Rect rect = new Rect(0, 0, maxX, maxY);
        levelRenderer.sprite = Sprite.Create(textureInstance, rect, Vector2.zero);
    }

    //Goes through these functions every "second".
    private void Update()
    {
        GetMousePosition();
        HandleMouseInput();
    }

    /// <summary>
    /// This Function starts by checking if current node is null, will return instantly,
    /// If it's not null it and left mouse button is pressed
    /// and if current node is not previous node
    /// it makes previous node to a current node.
    /// Intializes Color c to color white then makes it transparent == 0.
    /// Loops x and y size -3 to less than 3 "size of eraser" and
    /// makes current "pixel" node transparent unless it's null "already transparent".
    /// after loop aplies the change to the level texture.
    /// </summary>
    void HandleMouseInput()
    {
        if (currentNode == null)
        {
            return;
        }
        if (Input.GetMouseButton(0))
        {
            if (currentNode != previousNode)
            {
                previousNode = currentNode;

                Color c = Color.white;
                c.a = 0;

                Vector3 center = GetWorldPosFromNode(currentNode);
                float radius = editRadius * posOffset;

                for (int x = -6; x < 6; x++)
                {
                    for (int y = -6; y < 6; y++)
                    {
                        int t_x = x + currentNode.x;
                        int t_y = y + currentNode.y;

                        float d = Vector3.Distance(center, GetWorldPosFromNode(t_x, t_y));
                        if (d > radius)
                        {
                            continue;
                        }

                        Node n = GetNode(t_x, t_y);
                        if (n == null)
                        {
                            continue;
                        }

                        n.isEmpty = true;
                        textureInstance.SetPixel(t_x, t_y, c);

                    }
                }

                textureInstance.Apply();

            }
        }
    }

    /// <summary>
    /// This Function initializes a ray as a point on the main camera screen area 
    /// which takes the mouseposition as input.
    /// Sets the mousePos to ray point
    /// Current node is set to be the mouseposition from GetNodeFromWorldPosition function.
    /// </summary>
    void GetMousePosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        mousePos = ray.GetPoint(5);
        currentNode = GetNodeFromWorldPos(mousePos);
    }

    /// <summary>
    /// This function takes Vector3 as parameter which we call wp "worldposition"
    /// then it intializes int target X "t_x" coordinate 
    /// to Rounded int from calculation wp.x coordinate divided by position offset.
    /// Does the same for target Y "t_y" coordinate.
    /// Then returns the node target coordinates.
    /// </summary>
    /// <param name="wp"></param>
    /// <returns></returns>
    public Node GetNodeFromWorldPos(Vector3 wp)
    {
        int t_x = Mathf.RoundToInt(wp.x / posOffset);
        int t_y = Mathf.RoundToInt(wp.y / posOffset);

        return GetNode(t_x, t_y);
    }

    /// <summary>
    /// This Function returns the Node array coordinates unless
    /// Coordinate is less than 0 or higher than maximum texture size.
    /// "To not break array bounds"
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public Node GetNode(int x, int y)
    {
        if (x < 0 || y < 0 || x > maxX - 1 || y > maxY - 1)
        {
            return null;
        }
        return grid[x, y];
    }

    public Vector3 GetWorldPosFromNode(int x, int y)
    {
        Vector3 r = Vector3.zero;
        r.x = x * posOffset;
        r.y = y * posOffset;
        return r;
    }

    public Vector3 GetWorldPosFromNode(Node n)
    {
        if (n == null)
        {
            return -Vector3.one;
        }

        Vector3 r = Vector3.zero;
        r.x = n.x * posOffset;
        r.y = n.y * posOffset;
        return r;
    }

}

/// <summary>
/// Class for One Pixel "Node" of game area.
/// </summary>

public class Node
{
    public int x;
    public int y;
    public bool isEmpty;
}

