using UnityEngine;

public class OpponentManager : MonoBehaviour
{
    void Start() {

    }
    void Update() {

    }
    public void UpdatePosition(Position pos) {
        transform.position = new Vector3(pos.px, pos.py, pos.pz);
        transform.rotation = Quaternion.Euler(new Vector3(pos.rx, pos.ry, pos.rz));

    }
}