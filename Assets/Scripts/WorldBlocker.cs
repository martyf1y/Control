using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldBlocker : MonoBehaviour
{
    public static WorldBlocker instance;
    public Sprite[] WBSprite = new Sprite[5];
    private SpriteRenderer WBSpriteRenderer;

    // Start is called before the first frame update
    void Start()
    {
        WBSpriteRenderer = GetComponent<SpriteRenderer>();
        WBSpriteRenderer.sprite = WBSprite[0];
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void changeSprite(int num)
    {
        WBSpriteRenderer.sprite = WBSprite[num];
    }
}
