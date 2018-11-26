using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneScreenNavigator : MonoBehaviour {

    public string m_BackButtonNavigation = "[Name of Scene To Load]";

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            HandleBackButtonPressed();
        }
    }

    public void HandleBackButtonPressed()
    {
        Debug.Log("HandleBackButtonPressed() called.");
        if (SceneManager.GetActiveScene().name != m_BackButtonNavigation)
            LoadScene(m_BackButtonNavigation);
    }

    void LoadScene(string sceneName)
    {
        Debug.Log("LoadScene(" + sceneName + ") called.");
        if (!string.IsNullOrEmpty(sceneName))
        {
            Debug.Log("loading scene");
            SceneManager.LoadScene(sceneName);
        } else {
            Debug.Log("quit app");

            Application.Quit();
        }
    }

    public void LoadSceneZero()
    {
        SceneManager.LoadScene(0);
    }
    public void LoadSceneOne()
    {

        Debug.Log("loading scene ONE");

        SceneManager.LoadScene(1);
    }
    public void LoadSceneTwo()
    {
        Debug.Log("loading scene TWO");

        SceneManager.LoadScene(2);
    }
}
