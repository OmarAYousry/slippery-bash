using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BlendBinaryState), true)]
public class BlendBinaryStateEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        GUILayout.Space(20);
        GUILayout.Label("Debugging", EditorStyles.boldLabel);

        BlendBinaryState myScript = (BlendBinaryState)target;

        // debug toggle
        GUILayout.BeginHorizontal();
        GUILayout.Label(
            new GUIContent("Enable debugging", "Show debug options for setting values or saving properties."),
            GUILayout.Width(EditorGUIUtility.labelWidth));
        myScript.debugging = GUILayout.Toggle(myScript.debugging, "");
        GUILayout.EndHorizontal();

        if(myScript.debugging)
        {
            // override value slider
            GUILayout.BeginHorizontal();
            GUILayout.Label(
                new GUIContent("Override Value", "This attribute is used for on editor debugging and only applies if debugging is set to true. "
                + "Use the Value property instead for proper blending."),
                GUILayout.Width(EditorGUIUtility.labelWidth));
            myScript.overrideValue = EditorGUILayout.Slider(myScript.overrideValue, 0f, 1f);
            GUILayout.EndHorizontal();
            if(GUILayout.Button("Trigger transition"))
            {
                myScript.Value = myScript.overrideValue;
            }


            // override files buttons
            EditorGUILayout.HelpBox("The following buttons help to save changes on file while working on the inspector. " +
                "Be careful, as it discards previous settings.", MessageType.Info);

            GUILayout.BeginHorizontal();
            if(GUILayout.Button("Override State 1"))
            {
                myScript.OverrideButton(true);
            }
            if(GUILayout.Button("Override State 2"))
            {
                myScript.OverrideButton(false);
            }
            GUILayout.EndHorizontal();
        }
    }
}
