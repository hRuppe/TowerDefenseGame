///////////////////////////////////////////////////////////////////
////  || |\\  ||  ////                                         ////
////  || ||\\ ||  ////     Created by IntenseNation Assets     ////
////  || || \\||  ////     ===============================     ////
////  || ||  \||  ////                                         ////
///////////////////////////////////////////////////////////////////

using UnityEngine;
using TMPro;

namespace IntenseNation.CurrencyManager
{
    public class CurrencyTextManager : MonoBehaviour
    {
        public CurrencyText[] currencyTexts;

        [Range(0, 5)]
        [Tooltip("Adjusts the decimals in the texts, 0 for no deciamls")]
        public int Decimals = 0;

        /// <summary>
        /// Updates the UI of one currency using the Currency Save Name
        /// </summary>
        /// <param name="currencySaveName"></param>
        public void UpdateSpecificUI(string currencySaveName)
        {
            Currency currency = CurrencyManager.instance.GetCurrencyData(currencySaveName); //Get the current currency data from the CurrencyManager
            int currencyNumber = CurrencyManager.instance.Currencies.IndexOf(currency);     //Get the number of the currency from the CurrencyManager currencies

            //Loop through the Texts of the current currency text
            for (int j = 0; j < currencyTexts[currencyNumber].Texts.Length; j++)
            {
                if (currencyTexts[currencyNumber].Texts[j]) //Check if there's a text component assigned
                    currencyTexts[currencyNumber].Texts[j].text = currencyTexts[currencyNumber].TextPrefix + currency.Amount.ToString($"F{Decimals}"); //Set the Text of the current currency text to the proper amount
                                                                                                            //(.ToString("F{Decimals}") adjusts the decimals in the value)
            }
        }

        /// <summary>
        /// Updates all the currencies UI components
        /// </summary>
        public void UpdateAllUI()
        {
            //Loop through the Currencies Texts
            for (int i = 0; i < currencyTexts.Length; i++)
            {
                float currencyAmount = CurrencyManager.instance.GetCurrencyAmount(currencyTexts[i].SaveName); //Get the amount using the CurrencyName in the for loop

                //Then loop through the Texts of the target currency text
                for (int j = 0; j < currencyTexts[i].Texts.Length; j++)
                {
                    if (currencyTexts[i].Texts[j]) //Check if there's a text component assigned
                        currencyTexts[i].Texts[j].text = currencyTexts[i].TextPrefix + currencyAmount.ToString($"F{Decimals}");
                    //Set the Text of the currency text to the proper amount (.ToString("F{Decimals}") adjusts the decimals in the value)
                }
            }
        }
    }


    //Class used by the TextManager only
    [System.Serializable]
    public class CurrencyText
    {
        public string SaveName;
        public string TextPrefix;
        public TextMeshProUGUI[] Texts;
    }
}