using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public enum BTNType { New, Continue, Option, Sound, Back, Quit }

public class BtnType : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public BTNType currentType;

    public Transform buttonScale;
    Vector3 defaultScale;

    public CanvasGroup mainGroup;
    public CanvasGroup optionGroup;

    bool isSoundOn = true;
    AudioSource audioSource;
    Button soundButton;
    public TextMeshProUGUI soundButtonText;

    public AudioClip hoverSound;

    // Start is called before the first frame update
    void Start()
    {
        defaultScale = buttonScale.localScale;
        audioSource = GameObject.FindWithTag("MainCamera").GetComponent<AudioSource>();

        // SoundBtn의 Button과 Text(TMP) 찾기
        soundButton = transform.Find("OptionMenu/SoundBtn/Button").GetComponent<Button>();
        
        UpdateSoundButtonText();
    }

    // void Start()
    // {
    //     defaultScale = buttonScale.localScale;
        
    //     // SoundBtn의 Button과 Text(TMP) 찾기
    //     soundButton = transform.Find("Canvas/OptionMenu/SoundBtn/Button").GetComponent<Button>();
    //     soundButtonText = soundButton.GetComponentInChildren<Text>();
        
    //     UpdateSoundButtonText();
    // }


    public void OnBtnClick()
    {
        switch (currentType)
        {
            case BTNType.New:
                SceneLoad.LoadSceneHandle("Play", 0);
                break;
            case BTNType.Continue:
                SceneLoad.LoadSceneHandle("P2P", 0);
                break;
            case BTNType.Option:
                CanvasGroupOff(mainGroup);
                CanvasGroupOn(optionGroup);
                break;
            case BTNType.Sound:
                isSoundOn = !isSoundOn;
                audioSource.enabled = isSoundOn;
                UpdateSoundButtonText();
                break;
            case BTNType.Back:
                CanvasGroupOff(optionGroup);
                CanvasGroupOn(mainGroup);
                break;
            case BTNType.Quit:
                QuitGame();
                break;
        }
    }

    public void QuitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    public void CanvasGroupOn(CanvasGroup cg)
    {
        cg.alpha = 1f;
        cg.interactable = true;
        cg.blocksRaycasts = true;
    }

    public void CanvasGroupOff(CanvasGroup cg)
    {
        cg.alpha = 0f;
        cg.interactable = false;
        cg.blocksRaycasts = false;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        buttonScale.localScale = defaultScale * 1.2f;

        // 마우스 호버 효과음 재생
        if (hoverSound != null)
        {
            audioSource.PlayOneShot(hoverSound);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        buttonScale.localScale = defaultScale;
    }

    private void UpdateSoundButtonText()
    {
        if (isSoundOn)
        {
            soundButtonText.text = "Sound OFF";
        }
        else
        {
            soundButtonText.text = "Sound ON";
        }
    }
}
