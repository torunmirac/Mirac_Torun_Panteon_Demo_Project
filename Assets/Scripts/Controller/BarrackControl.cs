using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using Pathfinding;
using TMPro;



public class BarrackControl : AbstractNumber
{
    GameManager manager3;
    SoldierManager soldierManager3;
    UIManager UI3;
    ObjectPools ObjectPoolsBarrack;
    public bool isInside = false, Clickable = true;
    private Vector2 lastPosition;
    public bool MoveAble = false, rightClick = false;
    [SerializeField] bool start = false, select = true;
    [SerializeField] Vector3 startPos;
    [SerializeField] Image healthBar;
    [SerializeField] TextMeshPro healthText;
    [SerializeField] float time, health = 100;
    [SerializeField] List<GameObject> darkGrid, transGrid;
    public List<GameObject> soldierlist;
    [SerializeField] GameObject OtherImage;
    public MoveBuildEnum currentMoveBuildEnum;

    private void Awake()
    {
        manager3 = FindObjectOfType<GameManager>();
        soldierManager3 = FindObjectOfType<SoldierManager>();
        UI3 = FindObjectOfType<UIManager>();
        ObjectPoolsBarrack = FindObjectOfType<ObjectPools>();

    }
    private void Start()
    {
        OtherImage = transform.GetChild(0).gameObject;
        healthText.text = health.ToString();
    }

    void Update()
    {
        MathfClamp(gameObject);

        if (Input.GetMouseButton(0) && start && MoveAble)
        {

            Move(gameObject, startPos, time);

            UI3.IsControlMoveBuild();
        }

        if (manager3.selectedBuild.Count > 0 && !manager3.selectedBuild.Contains(gameObject))
        {
            Clickable = false;

        }
        else
        {
            Clickable = true;

        }

    }

    #region Mouse Event
    void OnMouseDown()
    {
        if (Clickable)
        {
            if (select)
            {
                manager3.selectedBuild.Add(gameObject);
                soldierManager3.targetPos = transform.position;
                select = false;
            }
            lastPosition = transform.position;
            startPos = Input.mousePosition;

            Objects(true);
            start = true;
            EventManager.Broadcast(GameEvent.OnSelectBuild);
        }

    }

    private void OnMouseUp()
    {
        if (Clickable)
        {
            if (isInside)
            {
                transform.position = lastPosition;
            }
            start = false;
            startPos = Vector3.zero;
        }


    }

    void OnMouseOver()
    {
        if (soldierManager3.selectedSoldier.Count > 0)
        {
            if (Input.GetMouseButtonDown(1) && !soldierlist.Contains(soldierManager3.selectedSoldier[0].gameObject) && rightClick)
            {

                Debug.Log("sağ tık");
                GameObject soldier = soldierManager3.selectedSoldier[0].gameObject;
                soldierManager3.targetPos = transform.position;
                EventManager.Broadcast(GameEvent.OnGo);
                Invoke("AttackBarrac", .07f);
               // StartCoroutine(AttackPowerPlant());
                rightClick = false;
            }
        }


    }

    void AttackPowerPlant()
    {
        //yield return new WaitForSeconds(.05f);
        soldierManager3.targetList.Add(gameObject);
        EventManager.Broadcast(GameEvent.OnAttackBuild);

    }

    #endregion

    #region EventManager Voids
    private void OnEnable()
    {
        EventManager.AddHandler(GameEvent.OnInstantiateBuild, OnInstantiateBuild);
        EventManager.AddHandler(GameEvent.OnSelectBuild, OnSelectBuild);
        EventManager.AddHandler(GameEvent.OnDropBuild, OnDropBuild);
        EventManager.AddHandler(GameEvent.OnMove, OnMove);
        EventManager.AddHandler(GameEvent.OnDontMove, OnDontMove);
        EventManager.AddHandler(GameEvent.OnOkeyAttack, OnOkeyAttack);
        EventManager.AddHandler(GameEvent.OnResetGameObject, OnResetGameObject);


    }
    private void OnDisable()
    {
        EventManager.RemoveHandler(GameEvent.OnInstantiateBuild, OnInstantiateBuild);
        EventManager.RemoveHandler(GameEvent.OnSelectBuild, OnSelectBuild);
        EventManager.RemoveHandler(GameEvent.OnDropBuild, OnDropBuild);
        EventManager.RemoveHandler(GameEvent.OnMove, OnMove);
        EventManager.RemoveHandler(GameEvent.OnDontMove, OnDontMove);
        EventManager.RemoveHandler(GameEvent.OnOkeyAttack, OnOkeyAttack);
        EventManager.RemoveHandler(GameEvent.OnResetGameObject, OnResetGameObject);

    }

    void OnInstantiateBuild()
    {
        if (manager3.selectedBuild.Contains(gameObject))
        {
            for (int i = 0; i < manager3.barrackList.Count; i++)
            {
                if (!manager3.selectedBuild.Contains(manager3.barrackList[i]))
                {
                    manager3.barrackList[i].GetComponent<BarrackControl>().Clickable = false;

                }
            }
            transform.GetChild(0).gameObject.SetActive(true);
            OnMove();
            UI3.IsControlMoveBuild();
        }

    }


    void OnSelectBuild()
    {
        for (int i = 0; i < manager3.barrackList.Count; i++)
        {
            if (!manager3.selectedBuild.Contains(manager3.barrackList[i]))
            {
                manager3.barrackList[i].GetComponent<BarrackControl>().Clickable = false;
            }
        }

    }

    void OnDropBuild()
    {
        if (manager3.selectedBuild.Contains(gameObject))
        {
            manager3.selectedBuild.Remove(gameObject);
        }
        Objects(false);
        OnDontMove();
        rightClick = false;
    }

    void OnMove()
    {

        if (manager3.selectedBuild.Contains(gameObject))
        {
            MoveAble = true;
            transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
        }

    }

    void OnDontMove()
    {
        AstarPath.active.Scan();
        MoveAble = false;
        transform.GetChild(0).gameObject.SetActive(false);
        transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
        manager3.selectedBuild.Clear();
        foreach (var item in manager3.barrackList)
        {
            item.GetComponent<BarrackControl>().Clickable = true;

        }
        select = true;
        ChangeColor(Color.green);
    }

    void OnOkeyAttack()
    {
        rightClick = true;
    }

    void OnAttack()
    {
        rightClick = true;
    }

    void OnResetGameObject()
    {

        if (ObjectPoolsBarrack.passiveBarrack.Contains(gameObject))
        {
            health = 100;
            healthBar.fillAmount = 1;
            healthText.text = health.ToString();
            gameObject.SetActive(false);
        }
    }

    #endregion

    #region Collision Event
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (manager3.constructedBuilding.Contains(other.gameObject) && manager3.selectedBuild.Contains(gameObject))
        {
            isInside = true;
            ChangeColor(Color.red);
            UI3.currentMoveBuildEnum = MoveBuildEnum.Red;

        }
        if (!soldierlist.Contains(other.gameObject) && currentMoveBuildEnum != MoveBuildEnum.Red)
        {
            switch (other.gameObject.tag)
            {
                case "SoldierBlue":
                    CollisionSoldier(gameObject, ref health, 2, healthBar, healthText, manager3.barrackList, manager3.constructedBuilding, ObjectPoolsBarrack.passiveBarrack, ObjectPoolsBarrack.activeBarrack);


                    break;
                case "SoldierGreen":
                    CollisionSoldier(gameObject, ref health, 5, healthBar, healthText, manager3.barrackList, manager3.constructedBuilding, ObjectPoolsBarrack.passiveBarrack, ObjectPoolsBarrack.activeBarrack);


                    break;
                case "SoldierRed":
                    CollisionSoldier(gameObject, ref health, 10, healthBar, healthText, manager3.barrackList, manager3.constructedBuilding, ObjectPoolsBarrack.passiveBarrack, ObjectPoolsBarrack.activeBarrack);

                    break;
            }
        }


    }

    private void OnCollisionExit2D(Collision2D other)
    {
        if (manager3.constructedBuilding.Contains(other.gameObject) && manager3.selectedBuild.Contains(gameObject))
        {
            isInside = false;
            ChangeColor(Color.green);
            UI3.currentMoveBuildEnum = MoveBuildEnum.Green;
        }
    }


    #endregion


    #region OtherVoids

    void ChangeColor(Color changeColor)
    {

        for (int i = 0; i < darkGrid.Count; i++)
        {
            SpriteRenderer spriteRenderer1 = darkGrid[i].GetComponent<SpriteRenderer>();
            SpriteRenderer spriteRenderer2 = transGrid[i].GetComponent<SpriteRenderer>();

            Color colordark = changeColor;
            Color colortrans = changeColor;

            colordark.a = 0.8f;
            colortrans.a = 0.5f;

            spriteRenderer1.color = colordark;
            spriteRenderer2.color = colortrans;

        }
    }

    void Objects(bool active)
    {
        OtherImage.SetActive(active);


    }

    #endregion



}
