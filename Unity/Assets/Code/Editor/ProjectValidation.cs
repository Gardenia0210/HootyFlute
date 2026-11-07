using System;
using System.IO;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class ProjectValidation
{
    public static void ValidateAndBuild()
    {
        int checkedCases = 0;
        string[] invalid = { "1", "HF1,1,500,500,500,0,1,1", "HF1,9,500,500,500,1,1,1",
            "HF1,1,1024,500,500,1,1,1", "HF1,1,500,500,500,1,1,0", "", "HF1,1,500,500,500,2,1,1" };
        foreach (string line in invalid)
        {
            if (InstrumentFrame.TryParse(line, out _)) throw new Exception("Invalid telemetry accepted: " + line);
            checkedCases++;
        }
        for (int note = 0; note <= 8; note++)
        {
            string line = "HF1," + note + ",500,500,500," + (note > 0 ? "1,1,1" : "0,0,1");
            if (!InstrumentFrame.TryParse(line, out InstrumentFrame frame) || frame.Note != note)
                throw new Exception("Valid telemetry rejected.");
            checkedCases++;
        }
        string[] scenes = Array.ConvertAll(EditorBuildSettings.scenes, scene => scene.path);
        foreach (string path in scenes)
        {
            var scene = EditorSceneManager.OpenScene(path, OpenSceneMode.Single);
            foreach (GameObject root in scene.GetRootGameObjects())
                foreach (Transform child in root.GetComponentsInChildren<Transform>(true))
                    if (GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(child.gameObject) > 0)
                        throw new Exception("Missing script: " + path + "/" + child.name);
        }
        SongDefinition song = AssetDatabase.LoadAssetAtPath<SongDefinition>("Assets/Data/Songs/TwinkleTwinkle.asset");
        if (song == null || song.notes.Length != 14) throw new Exception("Song asset failed to load.");
        Debug.Log("HOOTY_VALIDATION: " + checkedCases + " protocol cases; four scenes without missing scripts; song asset loaded.");
        string output = Environment.GetEnvironmentVariable("HOOTY_BUILD_PATH");
        if (string.IsNullOrEmpty(output)) throw new Exception("Set HOOTY_BUILD_PATH outside Assets.");
        BuildReport report = BuildPipeline.BuildPlayer(scenes, Path.Combine(output, "HootyFlute.exe"),
            BuildTarget.StandaloneWindows64, BuildOptions.None);
        if (report.summary.result != BuildResult.Succeeded) throw new Exception("Desktop build failed: " + report.summary.result);
        Debug.Log("HOOTY_BUILD: succeeded, " + report.summary.totalSize + " bytes.");
    }
}
