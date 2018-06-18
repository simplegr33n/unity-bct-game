using UnityEngine;

public static class Ornithophobia {

    private static string abilityName = "Ornithophobia";

    public static int abilityCost = 18;

    public static void SetIndicators(GameBoard gameBoard, UnitClass unit)
    {
        gameBoard.HideIndicatorPlanes();

        foreach (Tile tile in gameBoard.TileArray)
        {
            if ((tile.roundedPosition.z >= unit.transform.position.z - 3)
                && (tile.roundedPosition.z <= unit.transform.position.z + 3)
                && (tile.roundedPosition.x >= unit.transform.position.x - 3)
                && (tile.roundedPosition.x <= unit.transform.position.x + 3))
            {
                tile.currentUnit = unit;
                tile.SetAbilityIndicator(abilityName);
            }
        }
    }

    public static void HighlightAOE(GameBoard gameBoard, UnitClass unit, Tile tile)
    {
        tile.highlightPlane.SetActive(true);
    }

    public static void CastAbility(GameBoard gameBoard, UnitClass unit, Tile tile)
    {
        Debug.Log(abilityName + " CastAbility()");


        ExecuteCast(gameBoard, unit, tile);
  

        // Spend MP
        unit.unitMP -= abilityCost; 

        // Disable unit MOVE_REMAINING and IS_ACTIVE
        unit.MOVE_REMAINING = false;
        unit.ACTION_READY = false;

        // GOTO next turn (Early here incase unit attacks self...)
        gameBoard.turnManager.CoroutineAdvanceTurn("Ornithophobia");
        gameBoard.turnManager.SetTurnProcessingFalse("Ornithophobia");
        unit.ACTION_PROCESSING = false;

        // Refresh selected entity
        gameBoard.RefreshSelectedEntity();
    }
 
    
    private static void ExecuteCast(GameBoard gameBoard, UnitClass unit, Tile tile)
    {

        int intX = (int) tile.roundedPosition.x;
        int intZ = (int) tile.roundedPosition.z;

        gameBoard.HideIndicatorPlanes();

        gameBoard.effectDisplayer.CreateEffect(gameBoard.effectDisplayer.ornithophobia, new Vector3(intX, gameBoard.TileArray[intX, intZ].tile_elevation + 1f, intZ), Quaternion.identity);


        if (gameBoard.EntityArray[intX, intZ] != null && gameBoard.EntityArray[intX, intZ].entityType == "UNIT")
        {

            // Calculate damage
            int dmgCalculation = unit.unitAtkDamage + (int)(unit.unitAtkDamage / 2);

            gameBoard.EntityArray[intX, intZ].GetComponent<UnitClass>().TakeDamage(dmgCalculation);
            gameBoard.effectDisplayer.CreatePopupText(""+dmgCalculation, new Vector3(intX, gameBoard.TileArray[intX, intZ].tile_elevation + .5f, intZ), Color.red);

        }
    }
}
