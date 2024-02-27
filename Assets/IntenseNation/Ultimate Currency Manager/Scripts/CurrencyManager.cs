///////////////////////////////////////////////////////////////////
////  || |\\  ||  ////                                         ////
////  || ||\\ ||  ////     Created by IntenseNation Assets     ////
////  || || \\||  ////     ===============================     ////
////  || ||  \||  ////                                         ////
///////////////////////////////////////////////////////////////////

using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

#if INTENSENATION_SAVEANDLOAD
using IntenseNation.UltimateSaveAndLoad;
#endif

namespace IntenseNation.CurrencyManager
{
    public class CurrencyManager : MonoBehaviour
    {
        [Header("Main")]
        public List<Currency> Currencies;

        //The current instance of the Currency Manager, use it when calling any function (CurrencyManager.instance.'function()';)
        public static CurrencyManager instance;
        private CurrencyTextManager currencyTextManager;

        [Header("Currency Converter")]
        [Range(0, 99)]
        [Tooltip("Fees in percentage when converting money to another currency")]
        public float ConversionFeesPercentage = 0;

        private void Awake()
        {
            //This ensures only one prefab is in scene
            if (instance)              //If there's already a musicPlayer in this scene (along this one)
                Destroy(gameObject);      //Then we have to destroy this one since there's already a musicPlayer
            else                       //If there's no musicPlayer in the scene
                instance = this;          //Then assign this prefab to the musicPlayer

            DontDestroyOnLoad(gameObject); //We use this method so that the music player is not destroyed when we change scenes

            SceneManager.sceneLoaded += GetActiveTextManager; //Call the GetActiveTextManager function when a scene finished loading

            for (int i = 0; i < Currencies.Count; i++) //Loop through all the currencies
            {
                //Set the current currency amount to the loaded amount using the current currency save name
                #if INTENSENATION_SAVEANDLOAD
                Currencies[i].Amount = SaveAndLoad.LoadFloat(Currencies[i].SaveName, Currencies[i].Amount);
                #else
                Currencies[i].Amount = PlayerPrefs.GetFloat(Currencies[i].SaveName, Currencies[i].Amount);
                #endif
            }
        }

        /// <summary>
        /// Adds amount to the target currency (to remove amount then use a negative amount (i.e -10))
        /// </summary>
        /// <param name="currencySaveName"></param>
        /// <param name="amount"></param>
        public void UpdateAmount(string currencySaveName, float amount)
        {
            //Loop through all the currencies
            for (int i = 0; i < Currencies.Count; i++)
            {
                //If the current currency save name equals the target currency save name
                if (Currencies[i].SaveName == currencySaveName)
                {
                    Currencies[i].Amount += amount; //Add the amount to the target currency's amount

                    //if this currency doesn't allow dept and the amount is less than 0
                    if (!Currencies[i].Debt && Currencies[i].Amount < 0)
                        Currencies[i].Amount = 0; //Then force the amount to be 0
                                                  //Other than that then this means that debt is allowed so the currency can be a negative value

                    //Save the amount of the target currency
                    #if INTENSENATION_SAVEANDLOAD
                    SaveAndLoad.SaveFloat(currencySaveName, Currencies[i].Amount);
                    #else
                    PlayerPrefs.SetFloat(currencySaveName, Currencies[i].Amount);
                    #endif
                }
            }

            if (currencyTextManager)
                currencyTextManager.UpdateSpecificUI(currencySaveName); //Update all the texts of the target currency

        }

        /// <summary>
        /// Returns the currency amount (float) using a currency saveID
        /// </summary>
        /// <param name="currencySaveName"></param>
        /// <returns></returns>
        public float GetCurrencyAmount(string currencySaveName)
        {
            return GetCurrencyData(currencySaveName).Amount; //Get only the amount of the currency data
        }

        /// <summary>
        /// Returns all the currency data using a currency saveID
        /// </summary>
        /// <param name="currencySaveName"></param>
        /// <returns></returns>
        public Currency GetCurrencyData(string currencySaveName)
        {
            Currency currency = null; //Create a new temporary currency

            //Loop through all the currencies
            for (int i = 0; i < Currencies.Count; i++)
            {
                //If the current currency save name equals the target save name
                if (Currencies[i].SaveName == currencySaveName)
                {
                    currency = Currencies[i]; //Then set the temporary currency to the target currency
                }
            }

            return currency; //Finally return this currency (if the save name doesn't equal any of the currencies save name then the function will return null)
        }

        /// <summary>
        /// Calculates the data of the Currency Conversion (ConvertedAmount, AmountAfterFees, Fees)
        /// </summary>
        /// <param name="From_Currency"></param>
        /// <param name="To_Currency"></param>
        /// <param name="Amount"></param>
        /// <returns></returns>
        public ConvertedCurrency CalculateCurrencyConversionData(string From_Currency, string To_Currency, float Amount)
        {
            float primaryRatio = GetCurrencyData(From_Currency).Ratio; //The From_Currency Ratio
            float targetRatio = GetCurrencyData(To_Currency).Ratio;    //The To_Currency Ratio
            ConvertedCurrency currency = new ConvertedCurrency(); //Create a new ConvertedCurrency variable to hold the data

            if (primaryRatio >= targetRatio) //If the Ratio of the From_Currency is greater than / equal the To_Currency (To_Currrency is more worth than the From_Currency)
                currency.ConvertedAmount = Amount / (primaryRatio / targetRatio); //Divide the Amount by the proper Ratio (the greater/the smaller value) the set it to the ExpectedAmount
            else
                currency.ConvertedAmount = Amount * (targetRatio / primaryRatio); //Multiply the Amount by the proper Ratio (the greater/the smaller value) the set it to the ExpectedAmount
                                                                                  //Multiplication is used to increase the value

            currency.Fees = currency.ConvertedAmount * ConversionFeesPercentage / 100; //Get the amount of the fees only

            if (ConversionFeesPercentage > 0) //If there's a conversion fee
                currency.AmountAfterFees = currency.ConvertedAmount * (100 - ConversionFeesPercentage) / 100; //Then get the remaining value of the amount (i.e if conversionFees are 10 then get the other 90% of the amount)
            else
                currency.AmountAfterFees = currency.ConvertedAmount; //If no conversion fee, then just set the amountAfterFees the same as the ExpectedAmount value

            return currency; //Finally return the currency data
        }

        /// <summary>
        /// Exchanges given Amount from a currency to another (Converted Amount is calculated automatically)
        /// </summary>
        /// <param name="From_Currency"></param>
        /// <param name="To_Currency"></param>
        /// <param name="Amount"></param>
        public void ExchangeCurrency(string From_Currency, string To_Currency, float Amount)
        {
            ExchangeCurrency(From_Currency, To_Currency, Amount, CalculateCurrencyConversionData(From_Currency, To_Currency, Amount).ConvertedAmount);
        }

        /// <summary>
        /// Exchanges given Amount from a currency to another using the ConvertedAmount
        /// </summary>
        /// <param name="From_Currency"></param>
        /// <param name="To_Currency"></param>
        /// <param name="Amount"></param>
        /// <param name="ConvertedAmount"></param>
        public void ExchangeCurrency(string From_Currency, string To_Currency, float Amount, float ConvertedAmount)
        {
            //Check if there's enough money to convert the Required Amount
            //Or if the currency has debt enabled (therefor, the money will be converted all the time and the From_Currency will be in negative value
            if (GetCurrencyAmount(From_Currency) >= Amount || GetCurrencyData(From_Currency).Debt)
            {
                UpdateAmount(From_Currency, -Amount);      //Remove the amount from the From_Currency
                UpdateAmount(To_Currency, ConvertedAmount); //Add the Converted Amount to the To_Currency
            }
        }

        /// <summary>
        /// Searches for the TextManager in scene
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="mode"></param>
        void GetActiveTextManager(Scene scene, LoadSceneMode mode)
        {
            //Get the CurrencyTextManager from the scene (if it's not available then the variable will be null)
            currencyTextManager = FindObjectOfType<CurrencyTextManager>();

            //If the currencyTextManager is not null
            if (currencyTextManager)
                currencyTextManager.UpdateAllUI(); //Update all the UI in the TextManager
        }
    }
}