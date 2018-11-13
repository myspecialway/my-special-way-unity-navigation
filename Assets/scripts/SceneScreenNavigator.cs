using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneScreenNavigator : MonoBehaviour {

    public string m_BackButtonNavigation = "[Name of Scene To Load]";

    void Start()
    {

    }

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
            SceneManager.LoadScene(sceneName);
        } else {
            Application.Quit();
        }
    }

    public void LoadSceneZero()
    {
        SceneManager.LoadScene(0);
    }
    public void LoadSceneOne()
    {
        SceneManager.LoadScene(1);
    }
}
