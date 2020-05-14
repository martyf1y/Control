using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldDoor : MonoBehaviour
{
    public Sprite[] WBSprite = new Sprite[5];
    private SpriteRenderer WBSpriteRenderer;

    // Start is called before the first frame update
    void Start()
    {
        WBSpriteRenderer = GetComponent<SpriteRenderer>();
        WBSpriteRenderer.sprite = WBSprite[0];
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangeSprite(int num)
    {
        WBSpriteRenderer.sprite = WBSprite[num];
    }
}
