using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EditColors : MonoBehaviour {

    public Image img;

    public void Press()
    {
        LevelEditorControl.singleton.editState = LevelEditorControl.EditState.paint;
        LevelEditorControl.singleton.targetColor = img.color;
    }
}
