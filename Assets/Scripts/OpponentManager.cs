using UnityEngine;
using TMPro;

public class OpponentManager : MonoBehaviour
{
    public TextMeshProUGUI textMyHP;
    public TextMeshProUGUI textOpHP;

    public void UpdatePosition(Position pos) {
        transform.position = new Vector3(pos.px, pos.py, pos.pz);
        transform.rotation = Quaternion.Euler(new Vector3(pos.rx, pos.ry, pos.rz));

    }
    public void UpdateMyHP(int hp) {
        // TODO : 여기서 화면에 총 맞는 빨간 효과
        GameObject playerHUDObject = GameObject.Find("PlayerHUD");
        PlayerHUD playerHUD = playerHUDObject.GetComponent<PlayerHUD>();
        playerHUD.UpdateHPHUD(hp +1, hp);

        textMyHP.text = "Player\nHP " + hp.ToString();
    }
    public void UpdateOpHP(int hp) {
        textOpHP.text = "Opponent\nHP " + hp.ToString();
    }
}