using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneEsc : MonoBehaviour
{
    private bool isPaused = false;
    private string originalScene;

    private void Start()
    {
        originalScene = SceneManager.GetActiveScene().name;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    private void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f; // 게임 일시 정지
        SceneManager.LoadScene("Pause", LoadSceneMode.Additive); // Pause 메뉴 Scene 로드
    }

    private void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f; // 게임 재개
        SceneManager.UnloadSceneAsync("Pause"); // Pause 메뉴 Scene 언로드
    }

    public void ReturnToOriginalScene()
    {
        isPaused = false;
        Time.timeScale = 1f; // 게임 재개
        SceneManager.LoadScene(originalScene); // 원래 Scene으로 돌아가기
    }
}
