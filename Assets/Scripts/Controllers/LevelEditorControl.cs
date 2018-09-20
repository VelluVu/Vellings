using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System;

public class LevelEditorControl : MonoBehaviour {

    public string levelName;    
    public int maxX;
    public int maxY;
    public float paintSize;

    public EditState editState;
    public Color targetColor;
    public GameObject levelTemplate;
    public GameObject spawnPoint;
    public GameObject exitPoint;
    public List<string> availableLevels = new List<string>();
    List<GameObject> levelObjs = new List<GameObject>();
    GameControl gc;

    

    public static LevelEditorControl singleton;

    private void Awake()
    {
        singleton = this;
    }

    public enum EditState
    {
        paint,remove,spawnpoint,exitpoint
    }

    public void Init(GameControl g)
    {
        gc = g;

       
    }  

    public void AddAllLevels()
    {
        UiControl ui = UiControl.singleton;

        for (int i = 0; i < availableLevels.Count; i++)
        {
            GameObject g = Instantiate(levelTemplate);
            g.transform.SetParent(ui.levelGrid);
            g.SetActive(true);
            g.GetComponentInChildren<UnityEngine.UI.Text>().text = availableLevels[i];
            UILevelButton u = g.GetComponentInChildren<UILevelButton>();
            u.targetLevel = availableLevels[i];
            levelObjs.Add(g);
        }
    }

    public void ClearObjs()
    {
        for (int i = 0; i < levelObjs.Count; i++)
        {
            if (levelObjs[i])
            {
                Destroy(levelObjs[i]);
            }
        }
        levelObjs.Clear();
        availableLevels.Clear();
    }

    public void Tick()
    {
        Paint();
    }

    void Paint()
    {
        switch (editState)
        {
            case EditState.paint:
                gc.HandleMouseInput(targetColor, paintSize);
                break;
            case EditState.remove:
                gc.HandleMouseInput(targetColor, paintSize);
                break;
            case EditState.spawnpoint:
                PaintSpawnExitPoint(true);
                break;
            case EditState.exitpoint:
                PaintSpawnExitPoint(false);
                break;
            default:
                break;
        }     
    }  

    void PaintSpawnExitPoint(bool isSpawn)
    {
        if (Input.GetMouseButtonUp(0))
        {

            Node curNode = gc.GetCurrentNode();

            if (curNode == null)
            {
                return;
            }

            GameObject gameObj = exitPoint;

            if (isSpawn)
            {
                gameObj = spawnPoint;
            }

            gameObj.transform.position = gc.GetWorldPosFromNode(curNode);
           
        }
    }

    public void SaveLevel()
    {

        //SaveTexture(levelName, gc.GetTextureInstance());
        SaveToFile();
        FindAllLevels();
    }

    void SaveToFile()
    {
        LevelFile save = new LevelFile();
        save.levelTexture = GameControl.singleton.GetTextureInstance().EncodeToPNG();
        save.spawnPosition = new CreateVector();
        save.spawnPosition.x = spawnPoint.transform.position.x;
        save.spawnPosition.y = spawnPoint.transform.position.y;

        save.exitPosition = new CreateVector();
        save.exitPosition.x = exitPoint.transform.position.x;
        save.exitPosition.y = exitPoint.transform.position.y;
        
        string saveLocation = SaveLocation();
        saveLocation += levelName;

        IFormatter formatter = new BinaryFormatter();
        Stream stream = new FileStream(saveLocation, FileMode.Create, FileAccess.Write, FileShare.None);
        formatter.Serialize(stream, save);
        stream.Close();
    }

    string SaveLocation()
    {
        string saveLocation = Application.dataPath + "/Levels/";
        if(!Directory.Exists(saveLocation))
        {
            Directory.CreateDirectory(saveLocation);
        }
        return saveLocation;
    }

    public LevelFile LoadFromFile()
    {

        LevelFile saveFile = null;
        string targetName = SaveLocation();
        targetName += levelName;
        
        if (!File.Exists(targetName))
        {
            Debug.Log(levelName + "Not found");
        
        }
        else
        {
            IFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(targetName, FileMode.Open);
            LevelFile save = (LevelFile)formatter.Deserialize(stream);
            saveFile = save;
            stream.Close();
        }

        return saveFile;
    }

    public Texture2D LoadLevelTextureAs(string fileName)
    {
        string path = Application.dataPath + "/Levels/" + fileName + ".png";

        byte[] bytes;
        bytes = File.ReadAllBytes(path);
        Texture2D t = new Texture2D(1, 1);
        t.LoadImage(bytes);
        return t;
    }

    public void FindAllLevels()
    {
        ClearObjs();
        LoadAllLevels();
        AddAllLevels();
    }

    public void LoadAllLevels()
    {
        DirectoryInfo dirInfo = new DirectoryInfo(SaveLocation());
        FileInfo[] fileInfo = dirInfo.GetFiles();

        foreach  (FileInfo f in fileInfo)
        {
            string[] readName = f.Name.Split(new string[] { "." }, StringSplitOptions.RemoveEmptyEntries);
            if(readName.Length == 1)
            {
                availableLevels.Add(readName[0]);
            }

        }
    }

    public void LoadLevel()
    {
        gc.CreateLevel();
        
        //LoadLevelTexture(levelName);
    }
 
    /*public void SaveTexture(string fileName, Texture2D texture)
    {
        string path = Application.dataPath + "/Levels/" + fileName + ".png";

        File.WriteAllBytes(path, texture.EncodeToPNG());
    }
    */

    /*public void LoadLevelTexture(string fileName)
    {

        string path = Application.dataPath + "/Levels/" + fileName + ".png";

        byte[] bytes;
        bytes = File.ReadAllBytes(path);
        GameControl.singleton.GetTextureInstance().LoadImage(bytes);

    }
    */

}

[System.Serializable]
public class LevelFile
{
    public byte[] levelTexture;
    public CreateVector spawnPosition;
    public CreateVector exitPosition;
}

[System.Serializable]
public class CreateVector
{
    public float x;
    public float y;
}
