using System.Collections.Generic;

public class UnitCloud : UnitClass {


    private void Awake()
    {

  
        // Set Unit Stats
        entityName = "Cloud";
        unitHPMax = 227;
        unitHP = 227;
        unitMPMax = 63;
        unitMP = 63;
        unitAtkDamage = 57;
        unitMoveRadius = 7;
        unitCooldownMax = 100;

        ABILITY_LIST = new List<string> { "Acid Rain" };

    }

}
