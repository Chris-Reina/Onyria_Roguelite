using System;
using UnityEditor;
using UnityEngine;

public class VersionSettingsWindow : EditorWindow
{
    //TODO: Implement settings as ScriptableObject instead of PlayerPrefs
    
    private VersionSize _versionSizeSelected = VersionSize.None;
    private ProjectDevelopmentStage _developmentStage = ProjectDevelopmentStage.None;
    private ProjectDevelopmentStage _currentDevelopmentStage = ProjectDevelopmentStage.None;
    
    //private int MinorCount => PlayerPrefs.GetInt(VersionUtility.MinorUpdatesKey);
    //private int MediumCount => PlayerPrefs.GetInt(VersionUtility.MediumUpdatesKey);
    //private int MajorCount => PlayerPrefs.GetInt(VersionUtility.MajorUpdatesKey);
    
    [MenuItem("Version/Settings")]
    public static void OpenWindow()
    {
        var window = GetWindow<VersionSettingsWindow>("Version Settings");
        
        window.minSize = new Vector2(300,250);
        window.maxSize = new Vector2(350, 500);
        window.Initialize();
    }

    private void Initialize()
    {
        var nextUpdateData = PlayerPrefs.GetString(VersionUtility.NextUpdate);

        if (string.IsNullOrEmpty(nextUpdateData))
        {
            _versionSizeSelected = VersionSize.None;
        }
        else
        {
            if (!Enum.TryParse(nextUpdateData, true, out _versionSizeSelected))
                Debug.LogError("Development Stage Enumeration could not be Parsed");
        }


        var devStageData = PlayerPrefs.GetString(VersionUtility.DevelopmentStageKey);
        
        if (string.IsNullOrEmpty(devStageData))
        {
            _currentDevelopmentStage = _developmentStage = ProjectDevelopmentStage.None;
        }
        else
        {
            if (!Enum.TryParse(devStageData, true, out _developmentStage))
            {
                Debug.LogError("Development Stage Enumeration could not be Parsed");
                return;
            }

            _currentDevelopmentStage = _developmentStage;
        }
    }

    private void OnGUI()
    {

        GUILayout.Box($"Current Version\n{GetCurrentVersion()}\n\nNext Version\n{GetNextVersion()}",
            new GUIStyle(GUI.skin.box) {stretchWidth = true, alignment = TextAnchor.MiddleCenter});

        
        GUILayout.Space(20);
        GUILayout.Label("Settings", new GUIStyle(GUI.skin.label) {alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold});
        
        
        
        _currentDevelopmentStage = (ProjectDevelopmentStage) EditorGUILayout.EnumPopup("", _currentDevelopmentStage);
        
        GUILayout.BeginHorizontal();

        var buttonStyle = new GUIStyle(GUI.skin.button) {fixedHeight = 45f};
        
        if(GUILayout.Button("Major", buttonStyle)) {_versionSizeSelected = VersionSize.Major;}
        if(GUILayout.Button("Medium", buttonStyle)) {_versionSizeSelected = VersionSize.Medium;}
        if(GUILayout.Button("Minor", buttonStyle)) {_versionSizeSelected = VersionSize.Minor;}
        GUILayout.EndHorizontal();
        

        GUILayout.FlexibleSpace();
        
        if (GUILayout.Button("Save")) SaveSettings();
        GUILayout.Space(4);
    }
    
    private string GetCurrentVersion()
    {
        var minorCount = PlayerPrefs.GetInt(VersionUtility.MinorUpdatesKey);
        var mediumCount = PlayerPrefs.GetInt(VersionUtility.MediumUpdatesKey);
        var majorCount = PlayerPrefs.GetInt(VersionUtility.MajorUpdatesKey);
        
        return $"v{majorCount}.{mediumCount}.{minorCount}";
    }

    private string GetNextVersion()
    {
        var minorCount = PlayerPrefs.GetInt(VersionUtility.MinorUpdatesKey);
        var mediumCount = PlayerPrefs.GetInt(VersionUtility.MediumUpdatesKey);
        var majorCount = PlayerPrefs.GetInt(VersionUtility.MajorUpdatesKey);

        return _versionSizeSelected switch
        {
            VersionSize.None => GetCurrentVersion(),
            VersionSize.Minor => $"v{majorCount}.{mediumCount}.{minorCount+1}",
            VersionSize.Medium => $"v{majorCount}.{mediumCount+1}.{0}",
            VersionSize.Major => $"v{majorCount+1}.{0}.{0}",
            _ => GetCurrentVersion()
        };
    }

    private void SaveSettings()
    {
        PlayerPrefs.SetString(VersionUtility.NextUpdate, _versionSizeSelected.ToString());

        if (_currentDevelopmentStage != _developmentStage)
            PlayerPrefs.SetString(VersionUtility.DevelopmentStageKey, _currentDevelopmentStage.ToString());
        
        Close();
    }
}