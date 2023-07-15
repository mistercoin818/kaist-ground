using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneLoad : MonoBehaviour
{
    public Slider progressbar;
    public Text loadtext;
    private static string loadScene;
    private static int loadType;

    public static void LoadSceneHandle(string _name, int _loadType) {
        loadScene = _name;
        loadType = _loadType;
        SceneManager.LoadScene("Loading");
    }

    IEnumerator LoadScene() {
        yield return null;
        AsyncOperation operation = SceneManager.LoadSceneAsync("Play");
        operation.allowSceneActivation = false;


        while(!operation.isDone) {
            if (loadType == 0) {

                Debug.Log("새 게임");
            }
            else if (loadType == 1) {
                Debug.Log("헌 게임");
            }


            yield return null;
            if (progressbar.value < 0.9f) {
                progressbar.value = Mathf.MoveTowards(progressbar.value, 0.9f, Time.deltaTime);
            }
            if (operation.progress >= 0.9f) {
                progressbar.value = Mathf.MoveTowards(progressbar.value, 1f, Time.deltaTime);
            }

            if (progressbar.value >= 1f) {
                loadtext.text = "Press SpaceBar";
            }

            if  (Input.GetKeyDown(KeyCode.Space) && progressbar.value >= 1f && operation.progress >= 0.9f) {
                operation.allowSceneActivation = true;
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(LoadScene());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
