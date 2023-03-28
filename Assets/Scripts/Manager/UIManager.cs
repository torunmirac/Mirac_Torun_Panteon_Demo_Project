using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using Unity.VisualScripting;



public class UIManager : InstanceManager<UIManager>
{
    GameManager manager;
    SoldierManager soldierManager;
    ObjectPools objectPools;
    [SerializeField] private OnOffProductionMenu currentOnOffProductionMenu;
    public OnSelectedBuild currentOnSelectedBuild;
    public MoveBuildEnum currentMoveBuildEnum;
    [SerializeField] GameObject warningSoldier, OnProductionMenu, OffProductionMenu, OnItemOptions, OffItemOptions, OnInformationMenu, OffInformationMenu, OnProductSoldierMenu, OffProductSoldierMenu;
    [SerializeField] Transform InstantiatePoint;
    [SerializeField] GameObject Barrack, PowerPlant, blueSoldier, redSoldier, greeSoldier;
    [SerializeField] GameObject ProductionMenu, ItemOptions, InformationMenu, MoveBuildButton, MoveOkeyBuildButton, MoveNoBuildButton, ProductSoldierMenu;
    [SerializeField] GameObject BarrackInfo, PowerPlantInfo, BlueSoldierInfo, GreenSoldierInfo, RedSoldierInfo;
    Vector3 LastPosBuild;
    bool warningBool = true;
    public List<GameObject> ContinueBuild, PassiveButtons, ActiveButtons, InfoButtons;


    private void Awake()
    {
        manager = FindObjectOfType<GameManager>();
        soldierManager = FindObjectOfType<SoldierManager>();
        objectPools = FindObjectOfType<ObjectPools>();
    }
    void Start()
    {
        currentOnSelectedBuild = OnSelectedBuild.None;

    }

    private void Update()
    {
        Physics2D.queriesStartInColliders = true;
    }

    private void OnEnable()
    {
        EventManager.AddHandler(GameEvent.OnSelectBuild, OnSelectBuild);
        EventManager.AddHandler(GameEvent.OnInstantiateBuild, OnInstantiateBuild);
        EventManager.AddHandler(GameEvent.OnDropBuild, OnDropBuild);
        EventManager.AddHandler(GameEvent.OnSelectSoldier, OnSelectSoldier);
        EventManager.AddHandler(GameEvent.OnAddSoldier, OnAddSoldier);
        EventManager.AddHandler(GameEvent.OnAttackBuild, OnAttackBuild);
        EventManager.AddHandler(GameEvent.OnWarningSoldier, OnWarningSoldier);
    }
    private void OnDisable()
    {

        EventManager.RemoveHandler(GameEvent.OnSelectBuild, OnSelectBuild);
        EventManager.RemoveHandler(GameEvent.OnInstantiateBuild, OnInstantiateBuild);
        EventManager.RemoveHandler(GameEvent.OnDropBuild, OnDropBuild);
        EventManager.RemoveHandler(GameEvent.OnSelectSoldier, OnSelectSoldier);
        EventManager.RemoveHandler(GameEvent.OnAddSoldier, OnAddSoldier);
        EventManager.RemoveHandler(GameEvent.OnAttackBuild, OnAttackBuild);
        EventManager.RemoveHandler(GameEvent.OnWarningSoldier, OnWarningSoldier);
    }

    #region Event's Metods

    void OnInstantiateBuild()
    {
        //Invoke("IsControlMoveBuild", .3f);
        StartCoroutine(IsControlMoveBuildDelay());
        IsOffProductionMenu();
        ClickMoveButton();
        if (manager.barrackList.Contains(manager.selectedBuild[0].gameObject))
        {
            currentOnSelectedBuild = OnSelectedBuild.Barrack;
            ItemOptionsControl();
        }
    }

    IEnumerator IsControlMoveBuildDelay()
    {
        yield return new WaitForSeconds(.3f);
        IsControlMoveBuild();

    }

    void OnSelectBuild()
    {
        if (manager.barrackList.Contains(manager.selectedBuild[0].gameObject))
        {
            currentOnSelectedBuild = OnSelectedBuild.Barrack;
            ItemOptionsControl();
            IsOnItemOptions();
        }

        if (manager.powerplantList.Contains(manager.selectedBuild[0].gameObject))
        {
            currentOnSelectedBuild = OnSelectedBuild.PowerPlant;
            ItemOptionsControl();
            IsOnItemOptions();
        }
    }

    void OnDropBuild()
    {
        IsOffInformationMenu();
        IsOffProductionMenu();
        currentOnSelectedBuild = OnSelectedBuild.None;

    }

    void OnSelectSoldier()
    {
        IsOnItemOptions();
        //ItemOptionsControl();
        ItemOptions.transform.GetChild(0).GetChild(1).gameObject.SetActive(false);
        ItemOptions.transform.GetChild(0).GetChild(2).gameObject.SetActive(false);
        ItemOptions.transform.GetChild(0).GetChild(4).gameObject.SetActive(true);
    }

    void OnAddSoldier()
    {
        MoveBuildButton.SetActive(true);
        //ClickMoveButton();
    }

    void OnAttackBuild()
    {
        IsOffItemOptions();
    }

    void OnWarningSoldier()
    {
        if (warningBool)
        {
            StartCoroutine(IsWarningSoldierIE());
        }
    }

    IEnumerator IsWarningSoldierIE()
    {
        warningSoldier.SetActive(true);
        yield return new WaitForSeconds(3);
        warningSoldier.SetActive(false);
        warningBool = false;
    }

    #endregion

    #region ClickButton

    public void OnClickProductionMenu()
    {
        switch (currentOnOffProductionMenu)
        {
            case OnOffProductionMenu.On:

                IsOffProductionMenu();

                break;

            case OnOffProductionMenu.Off:

                IsOnProductionMenu();

                break;
        }
    }


    public void ClickInstantiateBarrackButton()
    {
        IsInstantiateBuild(objectPools.passiveBarrack, objectPools.activeBarrack, manager.barrackList);


    }

    public void ClickInstantiatePowerPlantButton()
    {

        IsInstantiateBuild(objectPools.passivePowerPlant, objectPools.activePowerPlant, manager.powerplantList);

    }



    public void ClickInfoButton()
    {

        //OnInformationMenü
        IsOnInformationMenu();

    }

    public void ClickProductionSoldierButton()
    {
        IsOnProductSoldierMenu();
        IsOffItemOptions();

    }


    public void ClickCloseInformationMenu()
    {
        //OffInformationMenü
        IsOffInformationMenu();
    }

    public void ClickMoveButton()
    {
        EventManager.Broadcast(GameEvent.OnMove);
        ItemOptions.SetActive(false);
        MoveBuildButton.SetActive(true);
        LastPosBuild = manager.selectedBuild[0].transform.position;

    }

    public void ClickCancelButton()
    {
        EventManager.Broadcast(GameEvent.OnDropBuild);
        EventManager.Broadcast(GameEvent.OnDontAttack);

        IsOffItemOptions();
    }

    public void ClickAttackButton()
    {
        EventManager.Broadcast(GameEvent.OnOkeyAttack);

    }

    public void ClickOkeyMoveBuildButton()
    {
        AstarPath.active.gameObject.SetActive(true);
        //manager.selectedBuild[0].transform.GetChild(0).gameObject.SetActive(true);
        ItemOptions.SetActive(true);
        MoveBuildButton.SetActive(false);
        EventManager.Broadcast(GameEvent.OnDontMove);
        ContinueBuild.Clear();
        IsOffItemOptions();
    }


    public void ClickNoMoveBuildButton()
    {
        AstarPath.active.gameObject.SetActive(true);
        LeanTween.move(manager.selectedBuild[0].gameObject, LastPosBuild, .5f);
        EventManager.Broadcast(GameEvent.OnDontMove);
        ItemOptions.SetActive(true);
        IsOffItemOptions();
        MoveBuildButton.SetActive(false);
        currentMoveBuildEnum = MoveBuildEnum.Green;
        IsControlMoveBuild();
        if (ContinueBuild.Count > 0)
        {
            GameObject build = ContinueBuild[0].GameObject();
            if (manager.barrackList.Contains(build))
            {

                manager.barrackList.Remove(build);

            }
            else
            {
                manager.powerplantList.Remove(build);
            }
            ContinueBuild.Clear();
            Destroy(build);
        }

    }


    public void ClickBlueSoldierButton()
    {

        InstantiateSoldier(objectPools.passiveBlueSoldier, objectPools.activeBlueSoldier, soldierManager.blueSoldier);

    }



    public void ClickGreenSoldierButton()
    {

        InstantiateSoldier(objectPools.passiveGreenSoldier, objectPools.activeGreenSoldier, soldierManager.greenSoldier);
    }

    public void ClickRedSoldierButton()
    {

        InstantiateSoldier(objectPools.passiveRedSoldier, objectPools.activeRedSoldier, soldierManager.redSoldier);
    }

    public void ClickCloseProductSoldierMenu()
    {
        IsOffProductSoldierMenu();
        EventManager.Broadcast(GameEvent.OnDropBuild);
        EventManager.Broadcast(GameEvent.OnDontAttack);
        IsOffItemOptions();
    }
    #endregion

    #region InstantiateVoids

    void IsInstantiateBuild(List<GameObject> passiveList, List<GameObject> activeList, List<GameObject> specialList)
    {
        GameObject Build = passiveList[0].gameObject;
        Build.transform.position = InstantiatePoint.position;
        Build.SetActive(true);
        activeList.Add(Build);
        passiveList.Remove(Build);
        ContinueBuild.Add(Build);
        specialList.Add(Build);
        manager.constructedBuilding.Add(Build);
        manager.selectedBuild.Add(Build);
        StartCoroutine(InstantaiteColliderRepeat(Build));
        EventManager.Broadcast(GameEvent.OnInstantiateBuild);
    }



    void InstantiateSoldier(List<GameObject> passiveList, List<GameObject> activeList, List<GameObject> soldierList)
    {
        AstarPath.active.gameObject.SetActive(false);
        GameObject ParentBarrack = manager.selectedBuild[0].gameObject;
        GameObject Build = passiveList[0].gameObject;
        Build.transform.position = ParentBarrack.transform.GetChild(1).position;
        Build.SetActive(true);
        activeList.Add(Build);
        passiveList.Remove(Build);
        ParentBarrack.GetComponent<BarrackControl>().soldierlist.Add(Build);
        manager.constructedBuilding.Add(Build);
        manager.selectedBuild.Remove(ParentBarrack);
        ContinueBuild.Add(Build);
        soldierList.Add(Build);
        soldierManager.selectedSoldier.Add(Build);
        manager.selectedBuild.Add(Build);
        IsOffProductSoldierMenu();
        IsOffItemOptions();
        EventManager.Broadcast(GameEvent.OnAddSoldier);
        EventManager.Broadcast(GameEvent.OnWarningSoldier);
        Physics2D.queriesStartInColliders = false;
    }


    IEnumerator InstantaiteColliderRepeat(GameObject Unit)
    {
        Unit.GetComponent<BoxCollider2D>().enabled = false;
        yield return new WaitForSeconds(.3f);
        Unit.GetComponent<BoxCollider2D>().enabled = true;
    }
    #endregion

    #region ControlUI
    public void IsControlMoveBuild()
    {

        switch (currentMoveBuildEnum)
        {
            case MoveBuildEnum.Green:
                MoveOkeyBuildButton.GetComponent<Button>().interactable = true;

                break;
            case MoveBuildEnum.Red:
                MoveOkeyBuildButton.GetComponent<Button>().interactable = false;

                break;
        }
    }



    public void ItemOptionsControl()
    {
        if (currentOnSelectedBuild != OnSelectedBuild.None)
        {

            IsOnItemOptions();

            for (int i = 0; i < InfoButtons.Count; i++)
            {
                InfoButtons[i].SetActive(false);
            }
        }

        switch (currentOnSelectedBuild)
        {
            case OnSelectedBuild.None:
                IsOffItemOptions();
                currentOnSelectedBuild = OnSelectedBuild.None;

                break;

            case OnSelectedBuild.Barrack:

                IsChangeInfo(BarrackInfo, PowerPlantInfo, BlueSoldierInfo, RedSoldierInfo, GreenSoldierInfo);
                ItemOptions.transform.GetChild(0).GetChild(1).GetComponent<Button>().interactable = true;
                break;

            case OnSelectedBuild.PowerPlant:
                IsChangeInfo(PowerPlantInfo, BarrackInfo, BlueSoldierInfo, RedSoldierInfo, GreenSoldierInfo);
                ItemOptions.transform.GetChild(0).GetChild(1).GetComponent<Button>().interactable = false;

                break;

            case OnSelectedBuild.SoldierBlue:

                IsChangeInfo(BlueSoldierInfo, BarrackInfo, PowerPlantInfo, RedSoldierInfo, GreenSoldierInfo);
                ItemOptions.transform.GetChild(0).GetChild(1).GetComponent<Button>().interactable = false;
                break;
            case OnSelectedBuild.SoldierGreen:

                IsChangeInfo(GreenSoldierInfo, BarrackInfo, BlueSoldierInfo, RedSoldierInfo, PowerPlantInfo);
                ItemOptions.transform.GetChild(0).GetChild(1).GetComponent<Button>().interactable = false;
                break;
            case OnSelectedBuild.SoldierRed:

                IsChangeInfo(RedSoldierInfo, BarrackInfo, BlueSoldierInfo, PowerPlantInfo, GreenSoldierInfo);
                ItemOptions.transform.GetChild(0).GetChild(1).GetComponent<Button>().interactable = false;
                break;
        }
    }

    #endregion

    #region On/Off Panels

    ///ProductionMenu
    ///
    void IsOnProductionMenu()
    {
        ProductionMenu.transform.DOMove(OnProductionMenu.transform.position, 0.5f).SetEase(Ease.InCubic).OnComplete(() =>
        {
            currentOnOffProductionMenu = OnOffProductionMenu.On;
        });
    }

    void IsOffProductionMenu()
    {
        ProductionMenu.transform.DOMove(OffProductionMenu.transform.position, 0.5f).SetEase(Ease.InCubic).OnComplete(() =>
        {
            currentOnOffProductionMenu = OnOffProductionMenu.Off;
        });
    }



    ///InformationMenü   

    void IsOnInformationMenu()
    {
        LeanTween.move(InformationMenu, OnInformationMenu.transform.position, .5f).setEaseInCubic();

    }

    void IsOffInformationMenu()
    {
        LeanTween.move(InformationMenu, OffInformationMenu.transform.position, .5f).setEaseInCubic();
        ProductionMenu.transform.GetChild(0).GetComponent<Button>().interactable = true;
    }


    //ObjectInfo

    void IsChangeInfo(GameObject active, GameObject passive1, GameObject passive2, GameObject passive3, GameObject passive4)
    {
        active.SetActive(true);
        passive1.SetActive(false);
        passive2.SetActive(false);
        passive3.SetActive(false);
        passive4.SetActive(false);
    }


    //ItemOptions

    void IsOnItemOptions()
    {

        ItemOptions.transform.DOMove(OnItemOptions.transform.position, 0.5f).SetEase(Ease.InCubic);
        ProductionMenu.transform.GetChild(0).GetComponent<Button>().interactable = false;
    }

    void IsOffItemOptions()
    {
        ItemOptions.transform.DOMove(OffItemOptions.transform.position, 0.5f).SetEase(Ease.InCubic).OnComplete(() =>
        {
            ItemOptions.transform.GetChild(0).GetChild(1).gameObject.SetActive(true);
            ItemOptions.transform.GetChild(0).GetChild(2).gameObject.SetActive(true);
            ItemOptions.transform.GetChild(0).GetChild(4).gameObject.SetActive(false);
        });
        ProductionMenu.transform.GetChild(0).GetComponent<Button>().interactable = true;
    }

    //SoldierMenu
    void IsOnProductSoldierMenu()
    {
        ProductSoldierMenu.transform.DOMove(OnProductSoldierMenu.transform.position, 0.5f).SetEase(Ease.InCubic);
    }

    void IsOffProductSoldierMenu()
    {
        ProductSoldierMenu.transform.DOMove(OffProductSoldierMenu.transform.position, 0.5f).SetEase(Ease.InCubic);
    }
    #endregion


   










}
