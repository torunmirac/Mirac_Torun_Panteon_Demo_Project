using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public class SoldierManager : InstanceManager<SoldierManager>
{
    public static SoldierManager instance; 
    public List<GameObject> selectedSoldier, unSelectedSoldier, blueSoldier, greenSoldier, redSoldier,
        targetList;

    public Vector3 targetPos;
    // Start is called before the first frame update
    void Start()
    {
        instance=this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
