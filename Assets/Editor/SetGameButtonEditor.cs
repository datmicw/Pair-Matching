using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
[CustomEditor(typeof(SetGameButton))]
[CanEditMultipleObjects]
[System.Serializable]
public class SetGameButtonEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        SetGameButton myScript = target as SetGameButton;
        switch (myScript.ButtonType)
        {
            case SetGameButton.EButtonType.PairsNumbertn:
                myScript.PairsNumber =
                    (GameSetting.EPairNumber)EditorGUILayout.EnumPopup("Pairs Number", myScript.PairsNumber);
                break;
            case SetGameButton.EButtonType.PuzzleCategoryBtn:
                myScript.PuzzleCategory =
                    (GameSetting.EPuzzleCategory)EditorGUILayout.EnumPopup("Puzzle Category", myScript.PuzzleCategory);
                break;
        }
        if(GUI.changed)
        {
            EditorUtility.SetDirty(target);
        }
    }
}
