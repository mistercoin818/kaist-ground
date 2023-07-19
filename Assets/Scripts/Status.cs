using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class HPEvent : UnityEngine.Events.UnityEvent<int, int> { }

public class Status : MonoBehaviour
{
    [HideInInspector]
    public HPEvent onHPEvent = new HPEvent();

    [Header("Walk, Run Speed")]
    [SerializeField]
    private float walkSpeed;
    [SerializeField]
    private float runSpeed;

    [Header("HP")]
    [SerializeField]
    private int maxHP = 100;
    private int currentHP;

    public float WalkSpeed => walkSpeed;
    public float RunSpeed => runSpeed;

    public int CurrentHP => currentHP;
    public int MaxHP => maxHP;

    private SceneLoad sceneLoad;

    public void Awake(){
        currentHP = maxHP;
        GameObject socketManagerObject = GameObject.Find("SocketManager");
        sceneLoad = socketManagerObject.GetComponent<SceneLoad>();
    }
    

    public bool DecreaseHP(int damage){
        int previousHP = currentHP;
        sceneLoad.SendWebSocketMessage("shot", damage.ToString());
        
        currentHP = currentHP - damage > 0 ? currentHP - damage : 0;

        onHPEvent.Invoke(previousHP, currentHP);
        
        if(currentHP == 0){
            return true;
        }

        return false;
    }

    public void IncreaseHP(int hp){
        int previousHP = currentHP;

        currentHP = currentHP + hp > maxHP ? maxHP : currentHP + hp;
        sceneLoad.SendWebSocketMessage("shot", (previousHP - currentHP).ToString());

        onHPEvent.Invoke(previousHP, currentHP);
    }
}
