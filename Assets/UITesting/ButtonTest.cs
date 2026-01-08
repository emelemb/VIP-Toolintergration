using System;
using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UI;

public class ButtonTest
{
    private Button m_targetButton;
    private string m_sceneForTest;
    private bool m_targetSceneLoaded;
    private string m_expectedScene;
    private string m_buttonName;

    [UnitySetUp]
    public IEnumerator SetUp()
    {
        m_sceneForTest = "MainMenu";
        m_expectedScene = "GameScene";
        m_buttonName = "Start";

        m_targetSceneLoaded = false;

        SceneManager.sceneLoaded += OnSceneLoad;

        SceneManager.LoadScene(m_sceneForTest);

        yield return new WaitForSeconds(1f);
        yield return null;
    }

    [UnityTest]
    public IEnumerator RunTest()
    {
        GameObject buttonObj = GameObject.Find(m_buttonName);
        Assert.IsNotNull(buttonObj, $"Button '{m_buttonName}' not found in scene '{m_sceneForTest}'");

        m_targetButton = buttonObj.GetComponent<Button>();
        Assert.IsNotNull(m_targetButton, $"Button component missing on '{m_buttonName}'");

        m_targetButton.onClick.Invoke();

        float timeout = 5f;
        float timer = 0f;
        while (!m_targetSceneLoaded && timer < timeout)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        Assert.IsTrue(m_targetSceneLoaded,
            $"Target scene '{m_expectedScene}' was not loaded after clicking button '{m_buttonName}'");

        Assert.AreEqual(m_expectedScene, SceneManager.GetActiveScene().name,
            $"Loaded scene '{SceneManager.GetActiveScene().name}' doesn't match expected '{m_expectedScene}'");
    }

    private void OnSceneLoad(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == m_expectedScene)
        {
            m_targetSceneLoaded = true;
        }
    }

    [UnityTearDown]
    public IEnumerator TearDown()
    {
        SceneManager.sceneLoaded -= OnSceneLoad;
        yield return null;
    }
}
