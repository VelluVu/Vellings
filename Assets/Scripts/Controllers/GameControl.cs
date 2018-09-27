using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Controls the game area by creating a pixel perfect level and manipulating pixel colors.
/// </summary>

#pragma warning disable CS0659 // Type overrides Object.Equals(object o) but does not override Object.GetHashCode()
public class GameControl : MonoBehaviour
#pragma warning restore CS0659 // Type overrides Object.Equals(object o) but does not override Object.GetHashCode()
{
    
    public Texture2D levelTexture;
    Texture2D textureInstance;
    Texture2D miniMapInstance;

    public SpriteRenderer levelRenderer;

    public int pixelAmount;
    public int maxPixels;
    public int maxX;
    public int maxY;
    int resetCount;
    
    public bool overUIElement;
    public bool isSpeed;
    public bool addFill;
    bool addTexture;

    public float editRadius = 6f;
    public float posOffset;
    float f_t;
    float p_t;

    Node[,] grid;
    Node currentNode;
    Node previousNode;

    Unit curUnit;

    public Node enemySpawnNod;
    public Vector3 enemySpawnPos;

    public Node spawnNode; 
    public Vector3 spawnPosition;
    private Vector3 mousePos;
    public GameObject enemy;
    public Transform fillDebugObj;
    public bool isAndroid;

    public Color addedTextureColor = Color.green;
    public Color fillColor = Color.magenta;
    public Color shadyCOlor = Color.magenta;
    public Color miniMapColor = Color.yellow;

    UnitControl unitControl;
    UiControl uiControl;
    LevelEditor levelEditor;

    public GameState gameState;
    public static GameControl singleton;

    public void Awake()
    {            
        singleton = this;
      
        /*if (Application.platform == RuntimePlatform.Android)
        {*/
            isAndroid = true;
        //}
    }

    private void Start()
    {
        enemy.SetActive(false);
        resetCount = 0;
        unitControl = UnitControl.singleton;
        //enemyControl = EnemyControl.singleton;
        uiControl = UiControl.singleton;
        levelEditor = LevelEditor.singleton;       
        levelEditor.Init(this);
        ChangeState(GameState.mainMenu);
        levelEditor.FindAllLevels();
       
    }

    //Changes game states

    public void ChangeState(GameState targetState)
    {
        gameState = targetState;

        switch (gameState)
        {
            case GameState.mainMenu:
                uiControl.MainMenuUISetUp();
                unitControl.ResetMapCounters();
               
                break;
            case GameState.levelEditor:
                uiControl.LevelEditorSetUp();
                unitControl.ResetÖutCount();
                InitLevelEditor();
                break;
            case GameState.playGame:
                unitControl.SetMaxUnits(10);
                uiControl.GameUISetUp();
                //enemy.SetActive(true);
                //FindObjectOfType<Enemy>().EnemyPlaceOnNode();
                Init();
                break;
            default:
                break;
        }
    }

    //Custom initialization

    public void Init()
    {      
        CreateLevel();     
        spawnNode = GetNodeFromWorldPos(levelEditor.spawnPoint.transform.position);
        spawnPosition = GetWorldPosFromNode(spawnNode);     
        SetExitPositions();
    }

    public Texture2D GetTextureInstance()
    {
        return textureInstance;
    }

    public void SetTextureInstance(Texture2D texture)
    {
        textureInstance = texture;
    }

    //Initialize level editor

    public void InitLevelEditor()
    {
        textureInstance = new Texture2D(maxX, maxY);
        textureInstance.filterMode = FilterMode.Point;
        miniMapInstance = new Texture2D(maxX, maxY);
        miniMapInstance.filterMode = FilterMode.Point;

        grid = new Node[maxX, maxY];
        Color c = Color.white;
        c.a = 0;

        for (int x = 0; x < maxX; x++)
        {
            for (int y = 0; y < maxY; y++)
            {
                Node n = new Node();
                n.x = x;
                n.y = y;                       
                grid[x, y] = n;
                textureInstance.SetPixel(x, y, c);
                miniMapInstance.SetPixel(x, y, c);
            }
        }
        textureInstance.Apply();
        miniMapInstance.Apply();
        Rect rect = new Rect(0, 0, maxX, maxY);
        levelRenderer.sprite = Sprite.Create(textureInstance, rect, Vector2.zero, 100, 0, SpriteMeshType.FullRect);
        uiControl.miniMapRenderer.sprite = Sprite.Create(miniMapInstance, rect, Vector2.zero, 100, 0, SpriteMeshType.FullRect);
        uiControl.loading.SetActive(false);
    }

    //Build the level apply nodes to each coordinate

    public void CreateLevel()
    {
        LoadLevel();
        maxX = levelTexture.width;
        maxY = levelTexture.height;
        grid = new Node[maxX, maxY];
        textureInstance = new Texture2D(maxX, maxY);
        textureInstance.filterMode = FilterMode.Point;

        miniMapInstance = new Texture2D(maxX, maxY);
        miniMapInstance.filterMode = FilterMode.Point;

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

                Color mmC = miniMapColor;
                if (n.isEmpty)
                {
                    mmC.a = 0;
                }
                miniMapInstance.SetPixel(x, y, mmC);
                grid[x, y] = n;
            }
        }
        textureInstance.Apply();
        miniMapInstance.Apply();
        Rect rect = new Rect(0, 0, maxX, maxY);
        levelRenderer.sprite = Sprite.Create(textureInstance, rect, Vector2.zero, 100, 0, SpriteMeshType.FullRect);
        uiControl.miniMapRenderer.sprite = Sprite.Create(miniMapInstance, rect, Vector2.zero, 100, 0, SpriteMeshType.FullRect);
        uiControl.miniMapRenderer.preserveAspect = true;
    }

    //Loads the level from file and sets spawn + exit positions

    public void LoadLevel()
    {

        LevelFile save = LevelEditor.singleton.LoadFromFile();
        levelTexture = new Texture2D(0, 0);              
        levelTexture.LoadImage(save.levelTexture);

        SaveVector spawn = save.spawnPosition;

        if(spawn != null)
        {
            Vector3 posit = Vector3.zero;
            posit.x = spawn.x;
            posit.y = spawn.y;
            levelEditor.spawnPoint.transform.position = posit;
        }

        SaveVector exit = save.exitPosition;

        if(exit != null)
        {
            Vector3 posexit = Vector3.zero;
            posexit.x = exit.x;
            posexit.y = exit.y;
            levelEditor.exitPoint.transform.position = posexit;
        }

        /*SaveVector eSpawn = save.enemySpawnPosition;

        if(eSpawn != null)
        {
            Vector3 posESpawn = Vector3.zero;
            posESpawn.x = eSpawn.x;
            posESpawn.y = eSpawn.y;
            levelEditor.enemySpawnPoint.transform.position = posESpawn;
        }*/
    }

    //Checks if input has link

    public void LoadTextureFromWWW(string link)
    {
        if ( string.IsNullOrEmpty(link))
        {
            //TODO: Throw invalid link message
            return;
        }

        StartCoroutine(LoadTextureFromWWWTime(link));
    }

    //Starts loading the texture and applies it
    
    IEnumerator LoadTextureFromWWWTime(string url)
    {
        uiControl.loading.SetActive(true);
        WWW www = new WWW(url);
        yield return www;

        if(www.texture == null)
        {
            uiControl.loading.SetActive(false);
            //TODO: add case where the image failed to download
        }
        else
        {          
            textureInstance = www.texture;
            textureInstance.filterMode = FilterMode.Point;
            maxX = textureInstance.width;
            maxY = textureInstance.height;
            Rect rect = new Rect(0, 0, maxX, maxY);
            levelRenderer.sprite = Sprite.Create(textureInstance, rect, Vector2.zero, 100, 0, SpriteMeshType.FullRect);
            uiControl.loading.SetActive(false);
            UpdateMinimap();
            uiControl.isLoading = false;
        }
    }

    //Updates level changes to minimap

    void UpdateMinimap()
    {
        miniMapInstance = new Texture2D(maxX, maxY);
        miniMapInstance.filterMode = FilterMode.Point;

        for (int x = 0; x < maxX; x++)
        {
            for (int y = 0; y < maxY; y++)
            {
                Color c = textureInstance.GetPixel(x, y);
                Color mmC = miniMapColor;
                mmC.a = c.a;
                miniMapInstance.SetPixel(x, y, mmC);
            }
        }
        miniMapInstance.Apply();
        Rect rect = new Rect(0, 0, maxX, maxY);
        uiControl.miniMapRenderer.sprite = Sprite.Create(miniMapInstance, rect, Vector2.zero, 100, 0, SpriteMeshType.FullRect);
        uiControl.miniMapRenderer.preserveAspect = true;
    }


    private void Update()
    {
        overUIElement = EventSystem.current.IsPointerOverGameObject();
        
        uiControl.Tick();

        switch (gameState)
        {
            case GameState.mainMenu:
                break;
            case GameState.levelEditor:             
                GetMousePosition();
                levelEditor.Tick();
                break;
            case GameState.playGame:
                GetMousePosition();
                InGame();              
                break;
            default:
                break;
        }
   
        if (addTexture)
        {
            textureInstance.Apply();
            miniMapInstance.Apply();
        }
    }

    //State where Game is Played

    void InGame()
    {
        float delta = Time.deltaTime;

        float timeScale = 1;
        if (isSpeed)
        {
            timeScale = 2;
        }
        delta *= timeScale;

        //enemyControl.Tick();
        unitControl.Tick(delta, timeScale);
        CheckForUnit();       
        HandleUnit();
        HandleFillNodes(delta);
        ClearPixelList();
        BuildNodeList();

        /*if (addFill)
        {
            DebugFill();
        }*/

    }

    //When Mouse pressed sets pixel color to chosen color and on custom radius

    public void HandleMouseInput(Color targetColor, float targetRadus)
    {
        if (currentNode == null)
        {
            return;
        }
        if (uiControl.isLoading)
        {
            return;
        }
        if (uiControl.isTextureUI)
        {
            return;
        }

        if (Input.GetMouseButton(0))
        {
            if (currentNode != previousNode)
            {
                previousNode = currentNode;         

                Vector3 center = GetWorldPosFromNode(currentNode);
                float radius = targetRadus * posOffset;
                int steps = Mathf.RoundToInt(targetRadus);

                for (int x = -steps; x < steps; x++)
                {
                    for (int y = -steps; y < steps; y++)
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
                        textureInstance.SetPixel(t_x, t_y, targetColor);

                        Color mmC = miniMapColor;
                        mmC.a = targetColor.a;
                        miniMapInstance.SetPixel(t_x, t_y, mmC);
                    }
                }

                addTexture = true;

            }
        }
    }

    //On Mouse click sets tha unit ability to chosen ability

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

    //Checks if the cursor is on any unit

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

    //Finds the cursor position

    void GetMousePosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        mousePos = ray.GetPoint(5);
        currentNode = GetNodeFromWorldPos(mousePos);
    }

    //OurNodeLists

    List<Node> clearNodes = new List<Node>();
    List<Node> buildNodes = new List<Node>();
    List<FillNode> fillNodes = new List<FillNode>(); 
    
    //Add Nodes that is about to be removed to the list

    public void AddNodePossibilitiesForRemoval(List<Node> nlist)
    {
        clearNodes.AddRange(nlist);
    }

    //Clears the list of Nodes

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
            miniMapInstance.SetPixel(clearNodes[i].x, clearNodes[i].y, c);
        }

        clearNodes.Clear();
        addTexture = true;

    }

    //Add Nodes to selection list

    public void AddPossibleNodesToSelection(List<Node> nlist)
    {
        buildNodes.AddRange(nlist);
    }

    //Sets the build ability nodes

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
            miniMapInstance.SetPixel(buildNodes[i].x, buildNodes[i].y, miniMapColor);
        }

        buildNodes.Clear();
        addTexture = true;

    }

    //Only used to test fill pixels

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

    //Calculates fill node movement and sets a color

    void HandleFillNodes(float delta)
    {
        f_t += delta;

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
                miniMapInstance.SetPixel(d.x, d.y, miniMapColor);
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
                    miniMapInstance.SetPixel(df.x, df.y, miniMapColor);
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
                        miniMapInstance.SetPixel(bf.x, bf.y, miniMapColor);
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

    //Add new Node

    public void AddFillNode(FillNode f)
    {
        fillNodes.Add(f);
    }

    //Get Node from vector 

    public Node GetNodeFromWorldPos(Vector3 wp)
    {
        int t_x = Mathf.RoundToInt(wp.x / posOffset);
        int t_y = Mathf.RoundToInt(wp.y / posOffset);

        return GetNode(t_x, t_y);
    }

    //Get Node from x, y

    public Node GetNode(int x, int y)
    {
        if (x < 0 || y < 0 || x > maxX - 1 || y > maxY - 1)
        {
            return null;
        }
        return grid[x, y];
    }

    //Get Position as Vector with offset

    public Vector3 GetWorldPosFromNode(int x, int y)
    {
        Vector3 r = Vector3.zero;
        r.x = x * posOffset;
        r.y = y * posOffset;
        return r;
    }

    //Get Node position with offset

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

    //Button that sets game state to main menu

    public void BackButton()
    {

        ChangeState(GameState.mainMenu);
        unitControl.ClearAll();
        //enemyControl.ClearAllEnemy();
        InitLevelEditor();

    }

    //Set up the exit poition

    public void SetExitPositions()
    {
        Vector3 exitPosition = levelEditor.exitPoint.transform.position;
        Node eNode = GetNodeFromWorldPos(exitPosition);

        for(int x = -1; x < 1; x++)
        {
            for (int y = -4; y < 6; y++)
            {
                int t_x = eNode.x + x;
                int t_y = eNode.y + y;

                Node n = GetNode(t_x, t_y);

                if(n == null)
                {
                    continue;
                }

                n.isExit = true;
            }
        }
    }
    public Node GetCurrentNode()
    {
     
        return this.currentNode;
    }
}

//Each Pixel

public class Node
{
    public int x;
    public int y;
    public bool isEmpty;
    public bool isStopped;
    public bool isFiller;
    public bool isExit;
}

//Each replaced Pixel

public class FillNode
{
    public int x;
    public int y;
    public int t;
    public bool dropLeft;
}

public enum GameState
{
    mainMenu,levelEditor,playGame
}
