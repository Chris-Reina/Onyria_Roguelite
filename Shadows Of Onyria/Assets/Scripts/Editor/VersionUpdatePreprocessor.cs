using System;
using UnityEngine;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor;

/// <summary>
/// Will update version before building the executable based on settings in the "Version" menu item.
/// </summary>
public class VersionUpdatePreprocessor : IPreprocessBuildWithReport
{
    public int callbackOrder => 0;

    public void OnPreprocessBuild(BuildReport report)
    {
        var minorUpdates = PlayerPrefs.GetInt(VersionUtility.MinorUpdatesKey);
        var mediumUpdates = PlayerPrefs.GetInt(VersionUtility.MediumUpdatesKey);
        var majorUpdates = PlayerPrefs.GetInt(VersionUtility.MajorUpdatesKey);
        var devStageData = PlayerPrefs.GetString(VersionUtility.DevelopmentStageKey);
        
        var nextUpdateData = PlayerPrefs.GetString(VersionUtility.NextUpdate);
        var nextUpdate = VersionSize.Minor;

        if (!string.IsNullOrEmpty(nextUpdateData))
        {
            if (!Enum.TryParse(nextUpdateData, true, out nextUpdate))
                Debug.LogError("Development Stage Enumeration could not be Parsed");
        }

        switch (nextUpdate)
        {
            case VersionSize.Minor:
                minorUpdates += 1;
                break;
            case VersionSize.Medium:
                minorUpdates = 0;
                mediumUpdates += 1;
                break;
            case VersionSize.Major:
                minorUpdates = 0;
                mediumUpdates = 0;
                majorUpdates += 1;
                break;
        }

        var version = 
            $"({devStageData}) v{majorUpdates}.{mediumUpdates}.{minorUpdates}".Replace("(None) ", "");

        PlayerSettings.bundleVersion = version;
        
        PlayerPrefs.SetInt(VersionUtility.MinorUpdatesKey, minorUpdates);
        PlayerPrefs.SetInt(VersionUtility.MediumUpdatesKey, mediumUpdates);
        PlayerPrefs.SetInt(VersionUtility.MajorUpdatesKey, majorUpdates);
    }
}