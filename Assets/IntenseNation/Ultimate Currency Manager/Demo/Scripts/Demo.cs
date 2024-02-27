///////////////////////////////////////////////////////////////////
////  || |\\  ||  ////                                         ////
////  || ||\\ ||  ////     Created by IntenseNation Assets     ////
////  || || \\||  ////     ===============================     ////
////  || ||  \||  ////                                         ////
///////////////////////////////////////////////////////////////////

//This script is used by the demo only and not reuired by the core code

using System.Collections.Generic;
using UnityEngine;
using IntenseNation.CurrencyManager;

public class Demo : MonoBehaviour
{
    public List<Currency> Currencies;

    public GameObject CurrencyPrefab;
    public List<GameObject> spawnedPrefabs;
    public Transform buttonsLayout;

    private void Awake()
    {
        Currencies = CurrencyManager.instance.Currencies; //Get the Currencies from the active Currency Manager

        for (int i = 0; i < Currencies.Count; i++) //Loop through each Currency
        {
            GameObject spawnedPrefab = Instantiate(CurrencyPrefab, buttonsLayout); //Spawn a new Prefab in the buttons layout 
            spawnedPrefab.GetComponent<Demo_CurrencyCounter>().SaveName = Currencies[i].SaveName; //Set the SaveName text in the spawned prefab to the current currency save name
            spawnedPrefab.GetComponent<Demo_CurrencyCounter>().displayText.text = Currencies[i].SaveName; //Set the DisplayText text in the spawned prefab to the current currency display name
        }
    }
}
