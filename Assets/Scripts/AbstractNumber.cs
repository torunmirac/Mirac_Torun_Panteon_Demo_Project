using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static DigitalOpus.MB.MBEditor.MB_BatchPrefabBakerEditorFunctions;

public enum OnOffProductionMenu
{
    On,
    Off
}

public enum OnSelectedBuild
{
    None,
    Barrack,
    PowerPlant,
    SoldierBlue,
    SoldierGreen,
    SoldierRed,
}

public enum MoveBuildEnum
{
    None,
    Green,
    Red
}

public abstract class AbstractNumber : MonoBehaviour
{
    GameManager manager;
    SoldierManager soldierManager;
    UIManager UI;
    Vector3 offset;

    bool hasMoved = false;


    private void Awake()
    {
        manager = FindObjectOfType<GameManager>();
        soldierManager = FindObjectOfType<SoldierManager>();
        UI = FindObjectOfType<UIManager>();
    }

    #region MoveVoids
    public virtual void Move(GameObject Unit, Vector3 startPos, float time)
    {
        offset = Input.mousePosition - startPos;

        if (offset.x < 0 && !hasMoved && Mathf.Abs(offset.x) > Mathf.Abs(offset.y) && Mathf.Abs(offset.x) > 300)
        {
          
            LeanTween.move(Unit, (Unit.transform.position + Vector3.left * .32f), time).setOnComplete(() =>
            {

                hasMoved = false;

            });
            hasMoved = true;


        }

        if (offset.x > 0 && !hasMoved && Mathf.Abs(offset.x) > Mathf.Abs(offset.y) && Mathf.Abs(offset.x) > 300)
        {
            LeanTween.move(Unit, (Unit.transform.position - Vector3.left * .32f), time).setOnComplete(() =>
            {

                hasMoved = false;

            });
            hasMoved = true;


        }

        if (offset.y > 0 && !hasMoved && Mathf.Abs(offset.x) < Mathf.Abs(offset.y) && Mathf.Abs(offset.y) > 300)
        {
            
            LeanTween.move(Unit, (Unit.transform.position + Vector3.up * .32f), time).setOnComplete(() =>
            {

                hasMoved = false;

            });
            hasMoved = true;



        }

        if (offset.y < 0 && !hasMoved && Mathf.Abs(offset.x) < Mathf.Abs(offset.y) && Mathf.Abs(offset.y) > 300)
        {
          
            LeanTween.move(Unit, (Unit.transform.position + Vector3.down * .32f), time).setOnComplete(() =>
            {

                hasMoved = false;

            });
            hasMoved = true;


        }
    }


    public virtual void MathfClamp(GameObject Unit)
    {
    
        Unit.transform.position = new Vector3(Unit.transform.position.x, Mathf.Clamp(Unit.transform.position.y, 1.71f, 6.51f), Unit.transform.position.z);

        
        Unit.transform.position = new Vector3(Mathf.Clamp(Unit.transform.position.x, 2.916f, 13.156f), Unit.transform.position.y, Unit.transform.position.z);

    }

    #endregion

    #region CollisionVoid

    public virtual void CollisionBuild(GameObject unity, GameObject other, ref float healthCount, float damage, Image healthyBar, TextMeshPro health,
        List<GameObject> selectedBuild, List<GameObject> selectedSoldier, List<GameObject> Soldier, List<GameObject> constructedBuilding, List<GameObject> passive, List<GameObject> active)
    {

        healthCount -= damage;
        healthyBar.fillAmount = healthCount * .1f;
        health.text = healthCount.ToString();
        selectedBuild.Clear();
        selectedSoldier.Clear();
        Soldier.Remove(unity);
        constructedBuilding.Remove(unity);
        active.Remove(unity);
        passive.Add(unity);
        //unity.SetActive(false);
        EventManager.Broadcast(GameEvent.OnResetGameObject);

        //Destroy(unity);




    }

    public virtual void collisionStayBuild(GameObject unity, GameObject UI, bool MoveAble, ref bool Inside, Color color)
    {
        if (MoveAble)
        {

            Inside = true;
            unity.transform.GetChild(0).GetChild(0).GetComponent<SpriteRenderer>().color = color;
            UI.GetComponent<UIManager>().currentMoveBuildEnum = MoveBuildEnum.Red;
            UI.GetComponent<UIManager>().IsControlMoveBuild();
        }
    }

    public virtual void collisionExitBuild(GameObject unity, GameObject UI, bool MoveAble, ref bool Inside, Color color)
    {
        if (MoveAble)
        {

            Inside = false;
            unity.transform.GetChild(0).GetChild(0).GetComponent<SpriteRenderer>().color = color;
            UI.GetComponent<UIManager>().currentMoveBuildEnum = MoveBuildEnum.Green;
            UI.GetComponent<UIManager>().IsControlMoveBuild();
        }
    }


    public virtual void CollisionSoldier(GameObject Unit, ref float health, int damage, Image healthBar, TextMeshPro healthText,
        List<GameObject> constructedBuilding, List<GameObject> specialSoldierList, List<GameObject> passive, List<GameObject> active)
    {

        health -= damage;
        healthBar.fillAmount -= damage * .01f;
        healthText.text = health.ToString();
        if (health <= 0)
        {
            constructedBuilding.Remove(Unit);

            specialSoldierList.Remove(Unit);
            EventManager.Broadcast(GameEvent.OnDropBuild);
            active.Remove(Unit);
            passive.Add(Unit);
            //Unit.SetActive(false);
            EventManager.Broadcast(GameEvent.OnResetGameObject);

        }
    }

    #endregion

}
