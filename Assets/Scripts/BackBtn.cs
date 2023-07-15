using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;


public class BackBtn : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Transform buttonScale;
    Vector3 defaultScale;

    public void Back() {
        SceneManager.LoadScene("MainMenu");
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
