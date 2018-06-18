
public class ManaPotion : ItemClass {

    public static int MP_RESTORE_PERCENTAGE = 25;


    void Awake()
    {
        entityName = "Mana Potion";
    }


    public static int UseManaPotion(UnitClass unit)
    {
        int restoreAmount = (int) ((unit.unitMPMax * MP_RESTORE_PERCENTAGE) / 100);
        if ((unit.unitMP + restoreAmount) > unit.unitMPMax)
        {
            unit.unitMP = unit.unitMPMax;
        } else
        {
            unit.unitMP = unit.unitMP + restoreAmount;
        }

        return restoreAmount;
    }

}
