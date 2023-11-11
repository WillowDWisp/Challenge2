using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class SetupWindow : EditorWindow {

    [MenuItem("Tools/AI System/Helper")]
    static void Init() {

        float maxWidth = 605f;
        float maxHeight = 652f;

        var window = EditorWindow.GetWindow(typeof(SetupWindow), false, "AI System");
        window.position = new Rect(Screen.width / 3, Screen.height / 6, maxWidth, maxHeight);
        window.maxSize = new Vector2(maxWidth, maxHeight);
        window.minSize = new Vector2(maxWidth, maxHeight);
        window.Show();
    }

    private void OnGUI() {

        float maxWidth = 590f;
        float maxHeight = 30f;
        int numberOfButtons = 2;

        GUILayout.BeginArea(new Rect(0, 0, Screen.width, Screen.height));
        GUILayout.BeginHorizontal();

        GUILayout.BeginVertical();

        GUILayout.Box("AI System Helper", GUILayout.ExpandWidth(true));

        Texture2D logoTexture = (Texture2D)Resources.Load("AI System/Logo");
        GUILayout.Label(logoTexture);

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();

        if (GUILayout.Button("Guide", GUILayout.MaxWidth(maxWidth / numberOfButtons), GUILayout.Height(maxHeight))) {
            Application.OpenURL("https://drive.google.com/file/d/14njzyglUTogbD66Nej9t99Xw8nAQqnJZ/view?usp=sharing");
        }

        if (GUILayout.Button("Feedback", GUILayout.MaxWidth(maxWidth / numberOfButtons), GUILayout.Height(maxHeight))) {
            Debug.Log("Open Asset Store Page");
        }

        //GUILayout.Button("About Us", GUILayout.MaxWidth(maxWidth / numberOfButtons), GUILayout.Height(maxHeight));

        GUILayout.FlexibleSpace();

        GUILayout.EndHorizontal();

        GUILayout.EndVertical();

        GUILayout.EndHorizontal();

        GUILayout.EndArea();



    }

}
