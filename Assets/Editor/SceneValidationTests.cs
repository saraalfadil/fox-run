using NUnit.Framework;
using UnityEngine;
using UnityEditor.SceneManagement;

public class SceneValidationTests
{
    [Test]
    public void Level1_HasRequiredGameObjects()
    {
        EditorSceneManager.OpenScene("Assets/Scenes/Level1.unity");

        Assert.IsNotNull(GameObject.Find("Player"), "Player not found in Level1");
        Assert.IsNotNull(GameObject.Find("==== UI ===="), "UI not found in Level1");
        Assert.IsNotNull(GameObject.Find("Main Camera"), "Main Camera not found in Level1");
    }

    [Test]
    public void Level2_HasRequiredGameObjects()
    {
        EditorSceneManager.OpenScene("Assets/Scenes/Level2.unity");

        Assert.IsNotNull(GameObject.Find("Player"), "Player not found in Level2");
        Assert.IsNotNull(GameObject.Find("==== UI ===="), "UI not found in Level2");
        Assert.IsNotNull(GameObject.Find("Main Camera"), "Main Camera not found in Level2");
    }
}
