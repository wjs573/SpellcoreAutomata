using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WJS;

public class RuneSlot
{
    public RuneObj runeObj;
    public RuneSlot(RuneModel runeModel)
    {
        runeObj = new RuneObj() { runeModel = runeModel };
    }
}
