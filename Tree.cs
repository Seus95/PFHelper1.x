using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tree : MonoBehaviour
{
    private Color tmp;
    void Awake()
    {
        tmp = GetComponent<SpriteRenderer>().material.color;
    }
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.tag == "Player")
        {
            tmp.a = .4f;
            GetComponent<SpriteRenderer>().material.color = tmp;
        }

    }
    private void OnTriggerExit2D(Collider2D col)
    {
        if (col.tag == "Player")
        {
            tmp.a = 1f;
            GetComponent<SpriteRenderer>().material.color = tmp;
        }
    }
}
