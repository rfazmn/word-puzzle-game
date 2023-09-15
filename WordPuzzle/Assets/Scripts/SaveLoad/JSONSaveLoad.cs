using UnityEngine;
using System.IO;

public class JSONSaveLoad : ISaveLoad
{
    public SceneData LoadScene(string fullPath)
    {
        TextAsset rawSceneData = Resources.Load<TextAsset>(fullPath);
        if (rawSceneData == null)
            return null;

        SceneData sceneData = Util.Deserialize<SceneData>(rawSceneData.text);
        return sceneData;
    }

    public void SaveScene(string fullPath, SceneData sceneData)
    {
        string jsonData = Util.Serialize(sceneData);
        File.WriteAllText(fullPath, jsonData);
    }
}