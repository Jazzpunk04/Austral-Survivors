using UnityEngine;

public class SceneChanger : MonoBehaviour
{
    //TEMPORARY FILE, FIX SCENE CHANGER BUG LATER
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void ChangeScene(int index)
    {
        //GameObject.Find("GameManager").GetComponent<PauseManager>().ResumeGame();
        UnityEngine.SceneManagement.SceneManager.LoadScene(index);
    }
}
