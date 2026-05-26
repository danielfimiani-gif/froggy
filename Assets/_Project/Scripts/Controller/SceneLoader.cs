using UnityEngine;
using UnityEngine.SceneManagement;

class SceneLoader : MonoBehaviour {
    public void LoadMainMenu() {
        SceneManager.LoadScene("MainMenu");
    }

    public void LoadGame() {
        SceneManager.LoadScene("GameScene");
    }
}