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

public class Demo_CurrencyConverter : MonoBehaviour
{
    [Header("Required")]
    [Tooltip("Currency where you will pay in")]
    public string From_Currency;

    [Tooltip("The converted currency after payment")]
    public string To_Currency;

    [Tooltip("How much would you want to convert (Pain in the From_Currency")]
    public float Amount;

    [Header("Optional")]
    public TextMeshProUGUI DisplayText;
    public TextMeshProUGUI PriceText;
    public TextMeshProUGUI FeesText;

    ConvertedCurrency convertedCurrency;

    private void Start()
    {
        CalculateData(); //Calculate the Button Data
    }

    //Called using + - buttons (used by the CurrencyConverterWithAmount)
    public void ChangeAmount(int increase)
    {
        if (Amount > 0 || increase > 0)
        {
            Amount += increase; //Add the increase/decrease amount to the overall Amount
            CalculateData(); //Calculate the Button Data
        }
    }

    //Calculates all the data required and updates the texts
    void CalculateData()
    {
        //Get the calculated data
        convertedCurrency = CurrencyManager.instance.CalculateCurrencyConversionData(From_Currency, To_Currency, Amount);

        //Check if each Text is assigned as they are optional
        //If the corresponding Text is assigned then set it to the proper value

        if (FeesText)
            FeesText.text = "Fees: " + convertedCurrency.Fees.ToString("F2");

        if (DisplayText)
            DisplayText.text = $"Get {convertedCurrency.AmountAfterFees:F2} {To_Currency}";

        if (PriceText)
            PriceText.text = $"{Amount:F2} {From_Currency}";
    }

    //Called by the purchase button
    public void Purchase()
    {
        //Exchange the currencies
        CurrencyManager.instance.ExchangeCurrency(From_Currency, To_Currency, Amount, convertedCurrency.AmountAfterFees);
    }
}