using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameStarter : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameObject.Find("Continue").GetComponent<Button>().interactable = GameStateManager.GetSavedGame(); 
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void StartGame(int index)
    {
        GameStateManager.StartNewGame(index);
    }

    public void ContinueGame(int fallbackSceneIndex)
    {
        GameStateManager.ContinueSavedGame(fallbackSceneIndex);
    }

    public void ContinueGame()
    {
        GameStateManager.ContinueSavedGame();
    }

}
