using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPools : InstanceManager<ObjectPools>
{
    public List<GameObject> activeBarrack,passiveBarrack;
    public List<GameObject> activePowerPlant,passivePowerPlant;
    public List<GameObject> activeBlueSoldier, activeGreenSoldier, activeRedSoldier;
    public List<GameObject> passiveBlueSoldier, passiveGreenSoldier, passiveRedSoldier;
    [SerializeField] GameObject Barrack, powerPlant, blueSoldier, greenSoldier, redSoldier;
    // Start is called before the first frame update
    void Start()
    {
        InstantiateObje(Barrack, passiveBarrack);
        InstantiateObje(powerPlant, passivePowerPlant);
        InstantiateObje(blueSoldier, passiveBlueSoldier);
        InstantiateObje(greenSoldier, passiveGreenSoldier);
        InstantiateObje(redSoldier, passiveRedSoldier);
    }





    void InstantiateObje(GameObject Unit,List<GameObject> passiveList)
    {
        for (int i = 0; i < 30; i++)
        {
            GameObject Build = Instantiate(Unit, transform.position, Quaternion.identity,transform);
            Build.SetActive(false);
            passiveList.Add(Build);
        }
       
    }


}
