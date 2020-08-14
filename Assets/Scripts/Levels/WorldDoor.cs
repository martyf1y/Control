using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldDoor : MonoBehaviour
{
    public Sprite[] wBSprite = new Sprite[2];
     SpriteRenderer WBSpriteRenderer;

    public Sprite[] WBSprite { get => wBSprite; set => wBSprite = value; }
    public SpriteRenderer WBSpriteRenderer1 { get => WBSpriteRenderer; set => WBSpriteRenderer = value; }

    // Start is called before the first frame update
    void Start()
    {
        WBSpriteRenderer1 = GetComponent<SpriteRenderer>();
        WBSpriteRenderer1.sprite = WBSprite[0];
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ChangeSprite(int num) => WBSpriteRenderer1.sprite = WBSprite[num];

    public void ChangeSpriteOrder(string sortLayer) => WBSpriteRenderer1.sortingLayerName = sortLayer;
}
