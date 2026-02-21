using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WJS
{
    ///<summary>
    ///AoeModel
    ///</summary>
    public class AoEData
    {
        public static Dictionary<string, AoeModel> data = new Dictionary<string, AoeModel>();

        public static void Initialize()
        {
            data = new Dictionary<string, AoeModel>()
            {
                
            };
        }
    }
}

