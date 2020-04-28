using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{
    //public Sprite sprite;
    [SerializeField]
    private Vector3 rotation = new Vector3(0,0,1);
    private int fadeSpeed = 1;
    Color color;

    // Start is called before the first frame update
    void Start()
    {
        color = this.GetComponent<SpriteRenderer>().material.color;
    }

    // Update is called once per frame
    public void Rotate(float whatWay)
    {
        transform.Rotate(rotation * Time.deltaTime * whatWay);

    }

    public void Update()
    {

    }

    public void FadeOut()
    {
        color.a -= Time.deltaTime * fadeSpeed;
        this.GetComponent<SpriteRenderer>().material.color = color;
        if(color.a <= 0)
        {
            this.gameObject.SetActive(false);
        }
    }
}
