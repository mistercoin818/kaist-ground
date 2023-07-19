using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public enum BTNType { New, Continue, Option, Sound, Back, Quit }

public class BtnType : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public BTNType currentType;

    public Transform buttonScale;
    Vector3 defaultScale;

    public CanvasGroup mainGroup;
    public CanvasGroup optionGroup;

    bool isSoundOn = true;

    public void OnBtnClick() {
        switch (currentType) {
        case BTNType.New: // 연습모드
            SceneLoad.LoadSceneHandle("Play", 0);
            break;
        case BTNType.Continue: // 대결모드
            SceneLoad.LoadSceneHandle("P2P", 0);
            break;
        case BTNType.Option:
            CanvasGroupOff(mainGroup);
            CanvasGroupOn(optionGroup);
            break;
        case BTNType.Sound:
            isSoundOn = !isSoundOn;
            break;
        case BTNType.Back:
            CanvasGroupOff(optionGroup);
            CanvasGroupOn(mainGroup);
            break;
        case BTNType.Quit:
            Debug.Log("Quit Game");
            Application.Quit();
            break;
        }
    }

    public void CanvasGroupOn(CanvasGroup cg) {
        cg.alpha = 1f;
        cg.interactable = true;
        cg.blocksRaycasts = true;
    }

    public void CanvasGroupOff(CanvasGroup cg) {
        cg.alpha = 0f;
        cg.interactable = false;
        cg.blocksRaycasts = false;
    }

    public void OnPointerEnter(PointerEventData eventData) {
        buttonScale.localScale = defaultScale * 1.2f;
    }

    public void OnPointerExit(PointerEventData eventData) {
        buttonScale.localScale = defaultScale;
    }

    // Start is called before the first frame update
    void Start()
    {
        defaultScale = buttonScale.localScale;
    }

    // Update is called once per frame
    void Update()
    {
    }
}
