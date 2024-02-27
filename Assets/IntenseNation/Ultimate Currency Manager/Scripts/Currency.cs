///////////////////////////////////////////////////////////////////
////  || |\\  ||  ////                                         ////
////  || ||\\ ||  ////     Created by IntenseNation Assets     ////
////  || || \\||  ////     ===============================     ////
////  || ||  \||  ////                                         ////
///////////////////////////////////////////////////////////////////

//This class holds the currency data

using UnityEngine;

namespace IntenseNation.CurrencyManager
{
    [System.Serializable]
    public class Currency
    {
        [Tooltip("(Required) Save ID, must be set only once to work properly during save/load")]
        public string SaveName;

        [Tooltip("If enabled, the currency can be a negative value, if disabled the currency won't go below 0")]
        public bool Debt;

        [Tooltip("You can change the default value so that it will be added the first time the game is started")]
        public float Amount;

        [Min(0.05f)]
        [Tooltip("Used by code to determine the ratio between currencies (i.e, 2 Coins = 1 Diamond)")]
        public float Ratio = 1;
    }
}