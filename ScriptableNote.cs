#if UNITY_EDITOR
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "Note", menuName = "Notes/Note", order = 1)]
public class ScriptableNote : ScriptableObject
{
    [TextArea(15, 20)]
    [SerializeField] private string note = "Note";
    [FoldoutGroup("Note Rect")] [SerializeField] private Vector2 worldCenterPosition;
    [FoldoutGroup("Note Style")] [SerializeField] private Color textColor = Color.white;
    [FoldoutGroup("Note Style")] [SerializeField] private float textFontSize = 12;
    [FoldoutGroup("Note Style")] [SerializeField] private GUIStyle textStyle;
    
    public void SetWorldCenterPosition(Vector2 position) => worldCenterPosition = position;
    public void SetNoteText(string text) => note = text;

    public Rect WorldTextRect { get; private set; }

    public enum State
    {
        Normal,Hovered,Selected
    }
    
    [Button("Reset Text Style")]
    private void ResetTextStyle()
    {
        textStyle = new GUIStyle();
        textStyle.normal.textColor = textColor;
        textStyle.fontStyle = FontStyle.Bold;
        textStyle.alignment = TextAnchor.MiddleCenter;
    }
    public void DrawMain(State currentState,SceneView sceneView)
    {
        if(textStyle == null) ResetTextStyle();
        float zoom = sceneView.camera.orthographicSize;
        float size = textFontSize * 20 / zoom;
        textStyle.fontSize = Mathf.Max((int)size, 1);
        GUIContent content = new (note);

        Handles.color = textColor;
        Handles.Label(worldCenterPosition,content, textStyle);
        
        Vector2 rectSizeInScreen = textStyle.CalcSize(content);
        Vector2 rectSizeInWorld = ConvertScreenSizeToWorldSize(rectSizeInScreen, sceneView.camera);
        
        Vector2 adjustedScreenPosition = worldCenterPosition - rectSizeInWorld / 2;
        WorldTextRect = new Rect(adjustedScreenPosition, rectSizeInWorld);

        switch (currentState)
        {
            case State.Hovered:
                Handles.DrawSolidRectangleWithOutline(WorldTextRect, Color.clear, Color.green);
                break;
            case State.Selected:
                Handles.DrawSolidRectangleWithOutline(WorldTextRect, Color.clear, Color.red);
                break;
        }
    }
    public void DrawOptions()
    {
        EditorGUI.BeginChangeCheck();
        Vector3 newWorldCenterPosition = Handles.PositionHandle(worldCenterPosition, Quaternion.identity);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(this, "Change Position");
            worldCenterPosition = newWorldCenterPosition;
        }
        
        GUILayout.BeginArea(new Rect(10, 10, 200, 200));
        
        EditorGUI.BeginChangeCheck();
        string newText = EditorGUILayout.TextArea(note);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(this, "Change Text");
            note = newText;
        }
        
        EditorGUI.BeginChangeCheck();
        Color newColor = EditorGUILayout.ColorField("Label Color", textColor);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(this, "Change Color");
            textColor = newColor;
            textStyle.normal.textColor = textColor;
        }
        
        EditorGUI.BeginChangeCheck();
        
        float newFontSize = EditorGUILayout.Slider("Font Size", textFontSize, 1, 50);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(this, "Change Font Size");
            textFontSize = newFontSize;
            textStyle.fontSize = (int) textFontSize;
        }
        
        GUILayout.EndArea();
    }
    private Vector2 ConvertScreenSizeToWorldSize(Vector2 screenSize, Camera camera)
    {
        // Check if the camera is orthographic
        if (!camera.orthographic) return new Vector2(0, 0);
        // The orthographic size is the half-height of the camera view in world space
        float worldHeight = camera.orthographicSize * 2.0f;
        float worldWidth = worldHeight * camera.aspect;

        // Get the screen height and width in pixels
        float screenHeight = Screen.height;
        float screenWidth = Screen.width;

        // Calculate the world space size
        float worldSizeWidth = (screenSize.x / screenWidth) * worldWidth;
        float worldSizeHeight = (screenSize.y / screenHeight) * worldHeight;

        return new Vector2(worldSizeWidth, worldSizeHeight);
    }
}
#endif