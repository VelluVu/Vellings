using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EditColors : MonoBehaviour {

    public Image img;

    public void Press()
    {
        LevelEditor.singleton.editState = LevelEditor.EditState.paint;
        LevelEditor.singleton.targetColor = img.color;
    }
}
