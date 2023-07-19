using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private Animation anim;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animation>();
        anim.wrapMode = WrapMode.Loop;

        anim.Play("demo_combat_idle");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
