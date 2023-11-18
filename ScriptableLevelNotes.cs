#if UNITY_EDITOR
using UnityEngine;

[CreateAssetMenu(fileName = "Level Notes", menuName = "Notes/Level Notes", order = 1)]
public class ScriptableLevelNotes : ScriptableObject
{
    public ScriptableNote[] notes;
}
#endif