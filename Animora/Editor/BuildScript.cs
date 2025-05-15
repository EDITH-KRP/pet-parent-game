using UnityEditor;
using UnityEngine;
using System.IO;
using System.Linq;

public class BuildScript
{
    [MenuItem("Build/Build Windows")]
    public static void BuildWindows()
    {
        // Define the build path
        string buildPath = Path.Combine(Path.GetDirectoryName(Application.dataPath), "..", "..", "Build");
        
        // Create the directory if it doesn't exist
        if (!Directory.Exists(buildPath))
        {
            Directory.CreateDirectory(buildPath);
        }
        
        // Define the build target
        string exePath = Path.Combine(buildPath, "Animora.exe");
        
        // Define the scenes to include
        string[] scenes = EditorBuildSettings.scenes
            .Where(scene => scene.enabled)
            .Select(scene => scene.path)
            .ToArray();
            
        if (scenes.Length == 0)
        {
            Debug.LogError("No scenes are enabled in the build settings!");
            return;
        }
        
        // Build the player
        BuildPipeline.BuildPlayer(
            scenes,
            exePath,
            BuildTarget.StandaloneWindows64,
            BuildOptions.None
        );
        
        Debug.Log("Build completed: " + exePath);
    }
}