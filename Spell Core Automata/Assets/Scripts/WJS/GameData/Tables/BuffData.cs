using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WJS
{
    public static class BuffData
    {
        public static Dictionary<string,BuffModel> data = new Dictionary<string, BuffModel>();

        public static void Initialize()
        {
            data = new Dictionary<string, BuffModel>()
            {
                //怪物出生buff
                { "Resurrect", new BuffModel("Resurrect", "复苏中","","Body",
                    new string[]{"Passive"}, 0, 1, 0f,
                    "ResurrectOnCreate", new object[0],  //occur
                    "ResurrectOnRemoved", new object[0],  //remove
                    "", new object[0],  //tick
                    "", new object[0],  //cast
                    "", new object[0],  //hit
                    "", new object[0],  //hurt
                    "", new object[0],  //kill
                    "", new object[0],  //dead
                    new ChaControlState(false,false,false),
                    null
                )}
            };
        }
    }
}
