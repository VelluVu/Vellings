using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UILevelButton : MonoBehaviour {

    public string targetLevel;
	
    public void Press()
    {
        LevelEditor.singleton.levelName = targetLevel;
    }
}
