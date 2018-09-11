using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class LevelEditorControl : MonoBehaviour {

    public string levelName;
    public bool saveLevel;
    public bool loadLevel;

    private void Update()
    {
        if (saveLevel)
        {
            saveLevel = false;
            SaveTexture(levelName, GameControl.singleton.GetTextureInstance());
        }
        if (loadLevel)
        {
            loadLevel = false;
            GameControl.singleton.SetTextureInstance(LoadTexture(levelName));
        }
        
    }

    public void SaveTexture(string fileName, Texture2D texture)
    {
        string path = Application.dataPath + "/levels/" + fileName + ".png";

        File.WriteAllBytes(path, texture.EncodeToPNG());
    }
    public Texture2D LoadTexture(string fileName)
    {

        string path = Application.dataPath + "/levels/" + fileName + ".png";

        byte[] bytes;
        bytes = File.ReadAllBytes(path);
        Texture2D t = new Texture2D(1, 1);
        t.LoadImage(bytes);
        return t;
    }

    public static LevelEditorControl singleton;

    private void Awake()
    {
        singleton = this;
    }
}
