using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using Pathfinding;
using UnityEngine.UI;
using TMPro;
using static System.Net.WebRequestMethods;
using Unity.Burst.CompilerServices;


public class SoldierControl : AbstractNumber
{
    GameManager managerS;
    SoldierManager soldierManagerS;
    UIManager UIs;
    ObjectPools objectPoolsSoldier;
    Color orgColor;
    public OnSelectedBuild currentOnSelectedSoldier;
    public MoveBuildEnum currentMoveBuildEnum;
    public AIPath aiPath;
    public Vector3 startPos;
    public Vector2 lastPosition;
    [SerializeField] float time, healthCount = 10, healthCountStart, rayDistance;
    public List<GameObject> MyBarrack;
    public bool isInside = false, colliderable = false, clickAble = true;
    [SerializeField] bool start = false, select = true, MoveAble = false, attackBuild = false;
    [SerializeField] Image HealthyBar;
    [SerializeField] TextMeshPro health;
    private Quaternion lastRotation;
    bool attack = false;


    private void Awake()
    {
        managerS = FindObjectOfType<GameManager>();
        soldierManagerS = FindObjectOfType<SoldierManager>();
        UIs = FindObjectOfType<UIManager>();
        objectPoolsSoldier = FindObjectOfType<ObjectPools>();
    }

    void Start()
    {
        healthCountStart = healthCount;
        orgColor = transform.GetChild(1).GetComponent<SpriteRenderer>().color;
        health.text = healthCount.ToString();
        lastRotation = transform.GetChild(2).rotation;
    }

    void Update()
    {
        MathfClamp(gameObject);

        currentOnSelectedSoldier = OnSelectedBuild.SoldierBlue;
        transform.GetChild(2).rotation = lastRotation;
        if (Input.GetMouseButton(0) && start && MoveAble)
        {

            Move(gameObject, startPos, time);
            UIs.IsControlMoveBuild();
        }

        if (soldierManagerS.targetList.Count > 0 && !MyBarrack.Contains(soldierManagerS.targetList[0]) && soldierManagerS.selectedSoldier[0] == gameObject && attackBuild)
        {

            Vector3 target = soldierManagerS.targetList[0].transform.position;
            soldierManagerS.targetList.RemoveAt(0);
            if (aiPath.velocity.magnitude == 0)
            {
                LeanTween.move(gameObject, target, .5f);
            }
        }

        if (managerS.selectedBuild.Count > 0 && !managerS.selectedBuild.Contains(gameObject))
        {
            clickAble = false;

        }
        else
        {
            clickAble = true;

        }


    }
    #region Mouse Event

    void OnMouseDown()
    {
        if (clickAble)
        {
            lastPosition = transform.position;
            startPos = Input.mousePosition;
            start = true;
            if (!MoveAble)
            {
                switch (gameObject.tag)
                {
                    case "SoldierBlue":
                        UIs.currentOnSelectedBuild = OnSelectedBuild.SoldierBlue;

                        break;
                    case "SoldierGreen":
                        UIs.currentOnSelectedBuild = OnSelectedBuild.SoldierGreen;
                        break;
                    case "SoldierRed":
                        UIs.currentOnSelectedBuild = OnSelectedBuild.SoldierRed;
                        break;

                }

                UIs.ItemOptionsControl();
                managerS.selectedBuild.Add(gameObject);
                soldierManagerS.selectedSoldier.Add(gameObject);
                soldierManagerS.unSelectedSoldier.Remove(gameObject);
                EventManager.Broadcast(GameEvent.OnSelectSoldier);
                transform.GetChild(0).gameObject.SetActive(true);

            }
        }
    }






    private void OnMouseUp()
    {
        if (clickAble)
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
        if (!soldierManagerS.selectedSoldier.Contains(gameObject) && attack)
        {

            if (Input.GetMouseButtonDown(1))
            {

                soldierManagerS.targetPos = transform.position;
                EventManager.Broadcast(GameEvent.OnGo);

            }

        }

    }

    #endregion


    #region EventManager Voids

    private void OnEnable()
    {

        EventManager.AddHandler(GameEvent.OnOkeyAttack, OnOkeyAttack);
        EventManager.AddHandler(GameEvent.OnDontAttack, OnDontAttack);
        EventManager.AddHandler(GameEvent.OnDropBuild, OnDropBuild);
        EventManager.AddHandler(GameEvent.OnAddSoldier, OnAddSoldier);
        EventManager.AddHandler(GameEvent.OnDontMove, OnDontMove);
        EventManager.AddHandler(GameEvent.OnAttackBuild, OnAttackBuild);
        EventManager.AddHandler(GameEvent.OnGo, OnGo);
        EventManager.AddHandler(GameEvent.OnResetGameObject, OnResetGameObject);
    }

    private void OnDisable()
    {

        EventManager.RemoveHandler(GameEvent.OnOkeyAttack, OnOkeyAttack);
        EventManager.RemoveHandler(GameEvent.OnDontAttack, OnDontAttack);
        EventManager.RemoveHandler(GameEvent.OnDropBuild, OnDropBuild);
        EventManager.RemoveHandler(GameEvent.OnAddSoldier, OnAddSoldier);
        EventManager.RemoveHandler(GameEvent.OnDontMove, OnDontMove);
        EventManager.RemoveHandler(GameEvent.OnAttackBuild, OnAttackBuild);
        EventManager.RemoveHandler(GameEvent.OnGo, OnGo);
        EventManager.RemoveHandler(GameEvent.OnResetGameObject, OnResetGameObject);
    }



    void OnOkeyAttack()
    {
        attack = true;
        transform.GetChild(0).GetChild(1).gameObject.SetActive(false);
        transform.GetChild(0).GetChild(2).gameObject.SetActive(false);

    }

    void OnDontAttack()
    {
        attack = false;
    }

    void OnGo()
    {
        if (soldierManagerS.selectedSoldier.Contains(gameObject))
        {
            aiPath.destination = soldierManagerS.targetPos;
            soldierManagerS.targetList.Clear();
            attackBuild = false;
        }
    }

    void OnAttackBuild()
    {
        attackBuild = true;
    }

    void OnDropBuild()
    {
        managerS.selectedBuild.Remove(gameObject);
        soldierManagerS.selectedSoldier.Remove(gameObject);
        transform.GetChild(0).gameObject.SetActive(false);
        colliderable = true;
    }

    void OnAddSoldier()
    {
        if (soldierManagerS.selectedSoldier[0].gameObject == gameObject)
        {
            transform.GetChild(0).gameObject.SetActive(true);
            transform.GetChild(0).GetChild(1).gameObject.SetActive(true);
            colliderable = false;
            MoveAble = true;
        }
    }

    void OnDontMove()
    {
        clickAble = true;
        MoveAble = false;
        colliderable = true;
        transform.GetChild(0).gameObject.SetActive(false);
        transform.GetChild(0).GetChild(1).gameObject.SetActive(false);
        managerS.selectedBuild.Clear();
        soldierManagerS.selectedSoldier.Clear();
    }

    void OnResetGameObject()
    {
        if (objectPoolsSoldier.passiveBlueSoldier.Contains(gameObject) || objectPoolsSoldier.passiveGreenSoldier.Contains(gameObject) || objectPoolsSoldier.passiveRedSoldier.Contains(gameObject))
        {
            healthCount = healthCountStart;
            health.text = healthCountStart.ToString();
            HealthyBar.fillAmount = 1;
            gameObject.SetActive(false);
            attackBuild = false;
        }

    }
    #endregion





    #region Collision Event

    private void OnCollisionStay2D(Collision2D other)
    {
        if (managerS.constructedBuilding.Contains(other.gameObject) && managerS.selectedBuild.Contains(gameObject))
        {
            collisionStayBuild(gameObject, UIs.gameObject, MoveAble, ref isInside, Color.red);

        }
        if (other.gameObject.CompareTag("Soldier") && other.gameObject != transform.GetChild(1).gameObject)
        {
            collisionStayBuild(gameObject, UIs.gameObject, MoveAble, ref isInside, Color.red);

        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {

        if (managerS.constructedBuilding.Contains(other.gameObject) && managerS.selectedBuild.Contains(gameObject) && colliderable && currentMoveBuildEnum != MoveBuildEnum.Red)
        {
            switch (gameObject.tag)
            {
                case "SoldierBlue":
                    CollisionBuild(gameObject, other.gameObject, ref healthCount, 2, HealthyBar, health, managerS.selectedBuild, soldierManagerS.selectedSoldier, soldierManagerS.blueSoldier, managerS.constructedBuilding,
                        objectPoolsSoldier.passiveBlueSoldier, objectPoolsSoldier.activeBlueSoldier);
                    break;
                case "SoldierGreen":
                    CollisionBuild(gameObject, other.gameObject, ref healthCount, 5, HealthyBar, health, managerS.selectedBuild, soldierManagerS.selectedSoldier, soldierManagerS.greenSoldier, managerS.constructedBuilding,
                        objectPoolsSoldier.passiveBlueSoldier, objectPoolsSoldier.activeBlueSoldier);
                    break;
                case "SoldierRed":
                    CollisionBuild(gameObject, other.gameObject, ref healthCount, 10, HealthyBar, health, managerS.selectedBuild, soldierManagerS.selectedSoldier, soldierManagerS.redSoldier, managerS.constructedBuilding,
                        objectPoolsSoldier.passiveBlueSoldier, objectPoolsSoldier.activeBlueSoldier);
                    break;
            }
        }

        if (other.gameObject.CompareTag("Soldier") && transform.GetChild(1).gameObject != other.gameObject && currentMoveBuildEnum != MoveBuildEnum.Red)
        {
            switch (gameObject.tag)
            {
                case "SoldierBlue":
                    CollisionSoldier(gameObject, ref healthCount, 2, HealthyBar, health, managerS.constructedBuilding, soldierManagerS.blueSoldier, objectPoolsSoldier.passiveBlueSoldier, objectPoolsSoldier.activeBlueSoldier);


                    break;
                case "SoldierGreen":

                    CollisionSoldier(gameObject, ref healthCount, 5, HealthyBar, health, managerS.constructedBuilding, soldierManagerS.greenSoldier, objectPoolsSoldier.passiveGreenSoldier, objectPoolsSoldier.activeGreenSoldier);


                    break;
                case "SoldierRed":
                    CollisionSoldier(gameObject, ref healthCount, 10, HealthyBar, health, managerS.constructedBuilding, soldierManagerS.redSoldier, objectPoolsSoldier.passiveRedSoldier, objectPoolsSoldier.activeRedSoldier);

                    break;

            }
        }


    }



    private void OnCollisionExit2D(Collision2D other)
    {

        if (managerS.constructedBuilding.Contains(other.gameObject) && managerS.selectedBuild.Contains(gameObject))
        {
            collisionExitBuild(gameObject, UIs.gameObject, MoveAble, ref isInside, orgColor);


        }
        if (other.gameObject.CompareTag("Soldier") && other.gameObject != transform.GetChild(1).gameObject)
        {
            collisionExitBuild(gameObject, UIs.gameObject, MoveAble, ref isInside, orgColor);

        }

    }
    #endregion
}

