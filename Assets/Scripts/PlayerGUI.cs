using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGUI : MonoBehaviour
{

    private SpriteRenderer sprRndr;

    void Start()
    {
        sprRndr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (Player.instance.PlayInteract)
            sprRndr.color = Color.green;
        else
            sprRndr.color = Color.red;
    }
}
