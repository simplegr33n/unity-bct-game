using System.Collections.Generic;

public class UnitSunMan : UnitClass
{

private void Awake()
    {

  
        // Set Unit Stats
        entityName = "Sun Man";
        unitHPMax = 333;
        unitHP = 333;
        unitMPMax = 76;
        unitMP = 76;
        unitAtkDamage = 54;
        unitMoveRadius = 7;
        unitCooldownMax = 90;

        ABILITY_LIST = new List<string> {"Solar Flare"};


    }

    

}
