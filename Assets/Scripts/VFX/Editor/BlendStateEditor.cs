using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BlendState), true)]
public class BlendStateEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        GUILayout.Space(20);
        GUILayout.Label("Debugging", EditorStyles.boldLabel);

        BlendState myScript = (BlendState)target;

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
                new GUIContent("Force State", "This attribute is used for on editor debugging and only applies if debugging is set to true. "
                + "Use the State property instead for proper blending."),
                GUILayout.Width(EditorGUIUtility.labelWidth));
            myScript.forceState = EditorGUILayout.IntSlider(myScript.forceState, 0, myScript.StateAmount() - 1);
            GUILayout.EndHorizontal();
            if(GUILayout.Button("Trigger transition"))
            {
                myScript.State = myScript.forceState;
            }


            // override files buttons
            int amount = myScript.StateAmount();
            if(amount > 0)
            {
                EditorGUILayout.HelpBox("The following buttons help to save changes on file while working on the inspector. " +
                    "Be careful, as it discards previous settings.", MessageType.Info);
                GUILayout.BeginHorizontal();
                for(int i = 0; i < amount; i++)
                {
                    if(GUILayout.Button(string.Concat("Override State ", i)))
                    {
                        myScript.OverrideButton(i);
                    }
                }
                GUILayout.EndHorizontal();
            }
        }
    }
}
