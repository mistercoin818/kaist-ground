// using UnityEngine;
// using UnityEngine.UI;
// using UnityEngine.SceneManagement;
// using System.Collections;

// public class GameOverUI : MonoBehaviour
// {
//     public Text countdownText;
//     private float countdownTime = 10f;
//     private bool canContinue = false;

//     private void Start()
//     {
//         StartCoroutine(CountdownCoroutine());
//     }

//     private IEnumerator CountdownCoroutine()
//     {
//         while (countdownTime > 0f)
//         {
//             countdownText.text = "Time left: " + Mathf.Round(countdownTime).ToString();
//             yield return new WaitForSeconds(1f);
//             countdownTime--;
//         }

//         canContinue = true;
//         countdownText.text = "Press 'Continue' to play again or 'Exit' to quit.";
//     }

//     public void ContinueGame()
//     {
//         if (canContinue)
//         {
//             // 게임을 계속 진행할 때 필요한 로직 추가 가능
//             SceneManager.LoadScene("Play"); // 게임 재시작을 위해 적절한 씬으로 전환
//         }
//     }

//     public void ExitGame()
//     {
//         SceneManager.LoadScene("MainMenu"); // 메인 메뉴로 돌아가기 위해 적절한 씬으로 전환
//     }
// }
