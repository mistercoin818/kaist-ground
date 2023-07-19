using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public enum buttonType {Main, Quit}

public class GameoverBtn : MonoBehaviour
{
    public buttonType current;

    public Transform buttonScale;
    Vector3 defaultScale;

    public CanvasGroup mainGroup;
    AudioSource audioSource;
    public AudioClip hoverSound;
    // Start is called before the first frame update
    void Start()
    {
        defaultScale = buttonScale.localScale;
    }

    public void OnBtnClick(){
        switch(current){
            case buttonType.Main:
                SceneLoad.LoadSceneHandle("MainMenu", 0);
                break;
            case buttonType.Quit:
                QuitGame();
                break;
        }
    }

    public void QuitGame(){
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    public void OnPointerEnter(PointerEventData eventData){
        buttonScale.localScale = defaultScale * 1.2f;

        // 마우스 호버 효과음 재생
        if(hoverSound != null){
            audioSource.PlayOneShot(hoverSound);
        }
    }

    public void OnPointerExit(PointerEventData eventData){
        buttonScale.localScale = defaultScale;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
