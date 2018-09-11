using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Controls the game area by creating a pixel perfect level and manipulating pixel colors.
/// </summary>

public class GameControl : MonoBehaviour
{
    
    public Texture2D levelTexture;
    Texture2D textureInstance;

    public Texture2D GetTextureInstance()
    {
        return textureInstance;
    }
    public void SetTextureInstance(Texture2D t)
    {
        textureInstance = t;
    }

    public SpriteRenderer levelRenderer;

    public float posOffset = 0.01f;
    int maxX;
    int maxY;
    bool addTexture;
    public float editRadius = 6;
    private bool overUIElement;

    Node[,] grid;
    Node currentNode;
    Node previousNode;

    public Transform fillDebugObj;
    public bool addFill;
    public int pixelAmount;
    public int maxPixels;
    float f_t;
    float p_t;

    Unit curUnit;
    public Node spawnNode;
    public Transform spawnTransform;
    public Vector3 spawnPosition;
    private Vector3 mousePos;

    public Color addedTextureColor = Color.green;
    public Color fillColor = Color.magenta;
    public Color shadyCOlor = Color.magenta;
    
    //classes we need to use in our manager.
    public UnitControl unitControl;
    public UiControl uiControl;
    
    public static GameControl singleton;

    public void Awake()
    {
        singleton = this;
    }

    //Builds the level when the game starts.
    private void Start()
    {
        unitControl = UnitControl.singleton;
        uiControl = UiControl.singleton;
        CreateLevel();
        spawnNode = GetNodeFromWorldPos(spawnTransform.position);
        spawnPosition = GetWorldPosFromNode(spawnNode);
    }

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
        levelRenderer.sprite = Sprite.Create(textureInstance, rect, Vector2.zero, 100, 0, SpriteMeshType.FullRect);
    }

    private void Update()
    {
        overUIElement = EventSystem.current.IsPointerOverGameObject();
        GetMousePosition();
        CheckForUnit();
        uiControl.FrameTick();
        HandleUnit();

        if (addFill)
        { 
        //DebugFill();
        }

        HandleFillNodes();
        ClearPixelList();
        BuildNodeList();
        
        if (addTexture)
        {
            textureInstance.Apply();
        }

        //HandleMouseInput();
    }

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

                        //n.isEmpty = true;
                        textureInstance.SetPixel(t_x, t_y, addedTextureColor);

                    }
                }

                addTexture = true;

            }
        }
    }

    void HandleUnit()
    {
        if (curUnit == null)
        {
            return;
        }
        if (Input.GetMouseButtonUp(0))
        {
            if (uiControl.targetAbility == Ability.walker)
            {
                return;
            }
            if (curUnit.curAbility == Ability.walker)
            {
                curUnit.ChangeAbility(uiControl.targetAbility);
               
            }
        }
    }

    void CheckForUnit()
    {
        mousePos.z = 0;

        curUnit = unitControl.GetClosest(mousePos);

        if (curUnit == null)
        {
            uiControl.overUnit = false;
            return;
        }
        uiControl.overUnit = true;
    }

    void GetMousePosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        mousePos = ray.GetPoint(5);
        currentNode = GetNodeFromWorldPos(mousePos);
    }

    List<Node> clearNodes = new List<Node>();
    List<Node> buildNodes = new List<Node>();
    List<FillNode> fillNodes = new List<FillNode>(); 
    
    public void AddNodePossibilitiesForRemoval(List<Node> nlist)
    {
        clearNodes.AddRange(nlist);
    }

    public void ClearPixelList()
    {
        if (clearNodes.Count == 0)
        {
            return;
        }

        Color c = Color.white;
        c.a = 0;

        for (int i = 0; i < clearNodes.Count; i++)
        {
            clearNodes[i].isEmpty = true;
            clearNodes[i].isFiller = false;
            textureInstance.SetPixel(clearNodes[i].x, clearNodes[i].y, c);
        }

        clearNodes.Clear();
        addTexture = true;

    }

    public void AddPossibleNodesToSelection(List<Node> nlist)
    {
        buildNodes.AddRange(nlist);
    }

    void BuildNodeList()
    {

        if (buildNodes.Count == 0)
        {
            return;
        }
        

        for (int i = 0; i < buildNodes.Count; i++)
        {
            buildNodes[i].isEmpty = false;
            textureInstance.SetPixel(buildNodes[i].x, buildNodes[i].y, addedTextureColor);
        }

        buildNodes.Clear();
        addTexture = true;

    }

    void DebugFill()
    {
        if(pixelAmount > maxPixels)
        {
            addFill = false;
            return;
        }
        p_t += Time.deltaTime;

        if (p_t > 0.05f)
        {
            pixelAmount++;
            p_t = 0;
        }
        else
        {
            return;
        }

        Node n = GetNodeFromWorldPos(fillDebugObj.position);
        FillNode f = new FillNode();
        f.x = n.x;
        f.y = n.y;
        fillNodes.Add(f);
        addTexture = true;
    }

    void HandleFillNodes()
    {
        f_t += Time.deltaTime;

        if (f_t > 0.05f)
        {
            f_t = 0;
        }
        else
        {
            return;
        }

        if ( fillNodes.Count == 0)
        {
            return;
        }

        for (int i = 0; i < fillNodes.Count; i++)
        {
            FillNode f = fillNodes[i];
            Node cn = GetNode(f.x, f.y);
            cn.isFiller = true;

            int _y = f.y;
            _y--;

            Node d = GetNode(f.x, _y);

            if (d == null)
            {
                fillNodes.Remove(f);
                continue;
            }

            if (d.isEmpty)
            {
                d.isEmpty = false;
                d.isFiller = true;
                textureInstance.SetPixel(d.x, d.y, fillColor);
                f.y = _y;
                clearNodes.Add(cn);
            }
            else
            {

                Node df = GetNode(f.x - 1, _y);

                if (df == null)
                {
                    fillNodes.Remove(f);
                    continue;
                }

                if (df.isEmpty)
                {
                    textureInstance.SetPixel(df.x, df.y, shadyCOlor);
                    f.y = _y;
                    f.x -= 1;
                    df.isEmpty = false;
                    df.isFiller = true;
                    clearNodes.Add(cn);
                }
                else
                {
                    Node bf = GetNode(f.x + 1, _y);
                    if (bf.isEmpty)
                    {
                        bf.isEmpty = false;
                        bf.isFiller = true;
                        textureInstance.SetPixel(bf.x, bf.y, fillColor);
                        f.y = _y;
                        f.x += 1;
                        clearNodes.Add(cn);
                    
                    }
                    else
                    {
                        f.t++;
                        if (f.t > 15)
                        {
                            Node _cn = GetNode(f.x, f.y);
                            _cn.isFiller = false;
                            fillNodes.Remove(f);
                        }
                    }
                }
            }
        }
    }

    public void AddFillNode(FillNode f)
    {
        fillNodes.Add(f);
    }

    public Node GetNodeFromWorldPos(Vector3 wp)
    {
        int t_x = Mathf.RoundToInt(wp.x / posOffset);
        int t_y = Mathf.RoundToInt(wp.y / posOffset);

        return GetNode(t_x, t_y);
    }

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

    public override bool Equals(object obj)
    {
        var control = obj as GameControl;
        return control != null &&
               base.Equals(obj) &&
               overUIElement == control.overUIElement;
    }
}


public class Node
{
    public int x;
    public int y;
    public bool isEmpty;
    public bool isStopped;
    public bool isFiller;
}

public class FillNode
{
    public int x;
    public int y;
    public int t;
    public bool dropLeft;
}

