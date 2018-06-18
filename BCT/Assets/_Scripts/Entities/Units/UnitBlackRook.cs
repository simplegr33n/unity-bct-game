using System.Collections.Generic;

public class UnitBlackRook : UnitClass {

    private void Awake()
    {


        // Set Unit Stats
        entityName = "Black Rook";
        unitHPMax = 163;
        unitHP = 163;
        unitMPMax = 39;
        unitMP = 39;
        unitAtkDamage = 29;
        unitMoveRadius = 8;
        unitCooldownMax = 100;

        ABILITY_LIST = new List<string> { "Ornithophobia" };

    }

}