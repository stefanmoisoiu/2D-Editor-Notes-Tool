#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class Notes : EditorWindow
{
    public VisualTreeAsset layout;
    private ScriptableLevelNotes _levelNotes;
    private ScriptableNote _selectedNote;
    
    private Action<ScriptableLevelNotes> _onLevelNoteSelected;
    private Action<ScriptableNote> _onNoteSelected;
    
    [MenuItem("Window/Notes")]
    public static void ShowWindow()
    {
        GetWindow<Notes>("Notes Tool");
    }
    public void CreateGUI()
    {
        layout.CloneTree(rootVisualElement);
        VisualElement root = rootVisualElement;
        // select _levelNotes
        ObjectField levelNotesField = rootVisualElement.Q<ObjectField>("LevelNotes");
        
        string selectedLevelNotes = PlayerPrefs.GetString("SelectedLevelNotes");
        if (!string.IsNullOrEmpty(selectedLevelNotes) && levelNotesField != null)
        {
            _levelNotes = AssetDatabase.LoadAssetAtPath<ScriptableLevelNotes>(selectedLevelNotes);
            levelNotesField.value = _levelNotes;
        }
        
        levelNotesField.RegisterValueChangedCallback(evt =>
        {
            _levelNotes = (ScriptableLevelNotes)evt.newValue;
            PlayerPrefs.SetString("SelectedLevelNotes", AssetDatabase.GetAssetPath(_levelNotes));
            Debug.Log("Selected " + _levelNotes);
            _onLevelNoteSelected?.Invoke(_levelNotes);
        });
        
        Button createNewNoteButton = rootVisualElement.Q<Button>("AddNote");
        createNewNoteButton.clicked += CreateNewNote;
        createNewNoteButton.SetEnabled(ShowCreateNewNoteButton());
        _onNoteSelected += _ => createNewNoteButton.SetEnabled(ShowCreateNewNoteButton());
        _onLevelNoteSelected += _ => createNewNoteButton.SetEnabled(ShowCreateNewNoteButton());
        
        Button deleteSelectedNoteButton = rootVisualElement.Q<Button>("RemoveNote");
        deleteSelectedNoteButton.clicked += DeleteSelectedNote;
        deleteSelectedNoteButton.SetEnabled(ShowDeleteSelectedNoteButton());
        _onNoteSelected += _ => deleteSelectedNoteButton.SetEnabled(ShowDeleteSelectedNoteButton());
        _onLevelNoteSelected += _ => deleteSelectedNoteButton.SetEnabled(ShowDeleteSelectedNoteButton());
    }
    private bool ShowCreateNewNoteButton()
    {
        if (_levelNotes == null) return false;
        if (_selectedNote != null) return false;
        return true;
    }
    private bool ShowDeleteSelectedNoteButton()
    {
        if (_levelNotes == null) return false;
        if (_selectedNote == null) return false;
        return true;
    }
    
    private void CreateNewNote()
    {
        if (_levelNotes == null) return;
        if (_selectedNote != null) return;
        
        ScriptableNote newNote = CreateInstance<ScriptableNote>();
        string noteName = $"Note {_levelNotes.notes.Length}";
        newNote.name = noteName;
        newNote.SetWorldCenterPosition(SceneView.lastActiveSceneView.camera.transform.position);
        newNote.SetNoteText(noteName);
        
        string savePath = AssetDatabase.GetAssetPath(_levelNotes);
        savePath = savePath.Substring(0, savePath.LastIndexOf('/'));
        savePath = $"{savePath}/{noteName}.asset";
        
        Array.Resize(ref _levelNotes.notes, _levelNotes.notes.Length + 1);
        _levelNotes.notes[^1] = newNote;
        _selectedNote = newNote;
        _onNoteSelected?.Invoke(newNote);
        
        AssetDatabase.CreateAsset(newNote, savePath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
    private void DeleteSelectedNote()
    {
        if (_levelNotes == null) return;
        if (_selectedNote == null) return;
        
        _levelNotes.notes = Array.FindAll(_levelNotes.notes, note => note != _selectedNote);
        AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(_selectedNote));
        _selectedNote = null;
        _onNoteSelected?.Invoke(null);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    private void OnEnable() {
        SceneView.duringSceneGui += DisplayNotes;
        SceneView.duringSceneGui += HandleSelection;
    }

    private void OnDisable() {
        SceneView.duringSceneGui -= DisplayNotes;
        SceneView.duringSceneGui -= HandleSelection;
    }

    private void HandleSelection(SceneView sceneView)
    {
        if(Event.current.type != EventType.MouseDown) return;
        if(Event.current.button != 0) return;

        bool hasSelectedNote = MousePositionIntersectsNote(out ScriptableNote[] intersectingNotes);
        if (hasSelectedNote)
        {
            _selectedNote = intersectingNotes[0];
            _onNoteSelected?.Invoke(_selectedNote);
        }
        else
        {
            _selectedNote = null;
            _onNoteSelected?.Invoke(null);
        }
    }
    private void DisplayNotes(SceneView sceneView)
    {
        if (_levelNotes == null) return;
        
        Handles.BeginGUI();
        
        bool isHoveringNote = MousePositionIntersectsNote(out ScriptableNote[] hoveredNotes);
        
        foreach (ScriptableNote note in _levelNotes.notes)
        {
            Handles.zTest = UnityEngine.Rendering.CompareFunction.Always;
            
            ScriptableNote.State state = ScriptableNote.State.Normal;
            if(_selectedNote == note) state = ScriptableNote.State.Selected;
            else if(isHoveringNote && hoveredNotes[0] == note) state = ScriptableNote.State.Hovered;
            
            note.DrawMain(state,sceneView);
        }
        
        if(_selectedNote != null) _selectedNote.DrawOptions();
        
        Handles.EndGUI();
    }
    private bool MousePositionIntersectsNote(out ScriptableNote[] intersectingNotes)
    {
        intersectingNotes = null;
        Vector2 mousePosition = Event.current.mousePosition;
        Vector2 worldPosition = HandleUtility.GUIPointToWorldRay(mousePosition).origin;
        return IntersectsWithNote(worldPosition, out intersectingNotes);
    }
    private bool IntersectsWithNote(Vector2 worldPosition, out ScriptableNote[] intersectingNotes)
    {
        List<ScriptableNote> intersectingNotesList = new();
        if(_levelNotes == null || _levelNotes.notes.Length == 0)
        {
            intersectingNotes = intersectingNotesList.ToArray();
            return false;
        }
        foreach (ScriptableNote note in _levelNotes.notes)
        {
            if(!note.WorldTextRect.Contains(worldPosition)) continue;
            intersectingNotesList.Add(note);
        }
        intersectingNotes = intersectingNotesList.ToArray();
        return intersectingNotes.Length > 0;
    }
}
#endif
