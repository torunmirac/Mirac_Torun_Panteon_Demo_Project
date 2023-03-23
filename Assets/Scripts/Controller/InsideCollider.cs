using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InsideCollider : MonoBehaviour
{
    GameManager manager;
    public bool isInside = false;

    private void Awake()
    {
        manager = FindObjectOfType<GameManager>();
    }

    void OnMouseDown()
    {
        manager.selectedBuild.Add(gameObject);
    }

    private void OnMouseUp()
    {
        manager.selectedBuild.Remove(gameObject);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Pos"))
        {
            isInside = true;
            ChangeColor(Color.red);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Pos"))
        {
            isInside = false;
            ChangeColor(Color.green);
        }
    }


    void ChangeColor(Color changeColor)
    {
        transform.GetChild(0).gameObject.SetActive(true);
        int count = transform.GetChild(0).GetChild(1).transform.childCount;
        for (int i = 0; i < count; i++)
        {
            transform.GetChild(0).GetChild(1).GetChild(i).GetComponent<SpriteRenderer>().color = changeColor;
        }
        
    }
}
