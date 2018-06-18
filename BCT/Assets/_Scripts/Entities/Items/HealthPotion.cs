
public class HealthPotion : ItemClass {

    public static int HP_RESTORE_PERCENTAGE = 25;

    void Awake()
    {
        entityName = "Health Potion";
    }
    


    public static int UseHealthPotion(UnitClass unit)
    {

        int restoreAmount = (int) ((unit.unitHPMax * HP_RESTORE_PERCENTAGE) / 100);
        if ((unit.unitHP + restoreAmount) > unit.unitHPMax)
        {
            unit.unitHP = unit.unitHPMax;
        } else
        {
            unit.unitHP = unit.unitHP + restoreAmount;
        }
        return restoreAmount;

    }

}
