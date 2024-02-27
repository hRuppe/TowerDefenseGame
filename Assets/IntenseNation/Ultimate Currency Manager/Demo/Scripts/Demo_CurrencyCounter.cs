///////////////////////////////////////////////////////////////////
////  || |\\  ||  ////                                         ////
////  || ||\\ ||  ////     Created by IntenseNation Assets     ////
////  || || \\||  ////     ===============================     ////
////  || ||  \||  ////                                         ////
///////////////////////////////////////////////////////////////////

//This script is used by the demo only and not reuired by the core code

using UnityEngine;
using TMPro;
using IntenseNation.CurrencyManager;

public class Demo_CurrencyCounter : MonoBehaviour
{
    public string SaveName;
    public TextMeshProUGUI currencyText;
    public TextMeshProUGUI displayText;

    private void Start()
    {
        UpdateTexts(); //Update the texts
    }

    public void ChangeAmount(bool increase)
    {
        CurrencyManager.instance.UpdateAmount(SaveName, increase ? 10 : -10); //If increase is enabled (+ button) then add 10 points to the currency,
                                                                              //if increase is disabled (- button) then remove 10 points from the currency,
        UpdateTexts(); //Then update the texts with the new points
    }

    void UpdateTexts()
    {
        //Set the currency text to the amount of the currency saved in the Currency Manager
        currencyText.text = CurrencyManager.instance.GetCurrencyAmount(SaveName).ToString();
    }
}
