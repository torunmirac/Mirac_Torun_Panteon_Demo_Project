using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    SoldierManager soldierManager;
    [SerializeField] private Color baseColor, offsetColor;
    [SerializeField] private SpriteRenderer render;
    [SerializeField] private GameObject highlight;
    bool attack = false;

    private void Awake()
    {
        soldierManager = FindObjectOfType<SoldierManager>();
    }

    public void Init(bool isOffset)
    {
        render.color = isOffset ? offsetColor : baseColor;
    }

    private void Update()
    {

    }

    private void OnEnable()
    {
        EventManager.AddHandler(GameEvent.OnOkeyAttack, OnOkeyAttack);
        EventManager.AddHandler(GameEvent.OnDontAttack, OnDontAttack);
    }

    private void OnDisable()
    {
        EventManager.RemoveHandler(GameEvent.OnOkeyAttack, OnOkeyAttack);
        EventManager.RemoveHandler(GameEvent.OnDontAttack, OnDontAttack);
    }

    void OnOkeyAttack()
    {
        attack = true;
    }

    void OnDontAttack()
    {
        attack = false;
    }

    void OnMouseOver()
    {
        if (attack)
        {
          
            if (Input.GetMouseButtonDown(1))
            {
                
                soldierManager.targetPos = transform.position;

                
                EventManager.Broadcast(GameEvent.OnGo);
                //EventManager.Broadcast(GameEvent.OnAttack);
            }

        }

    }

    void OnMouseEnter()
    {
        highlight.SetActive(true);
    }

    void OnMouseExit()
    {

        highlight.SetActive(false);
    }
}