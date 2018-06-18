using UnityEngine;

public static class AcidRain {

    private static string abilityName = "Acid Rain";

    public static int abilityCost = 32;

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

    public static void HighlightAOE(GameBoard gameBoard, UnitClass unit, Tile hoveredTile)
    {


        foreach (Tile tile in gameBoard.TileArray)
        {
            if ((tile.roundedPosition.z >= hoveredTile.roundedPosition.z - 1)
                && (tile.roundedPosition.z <= hoveredTile.roundedPosition.z + 1)
                && (tile.roundedPosition.x >= hoveredTile.roundedPosition.x - 1)
                && (tile.roundedPosition.x <= hoveredTile.roundedPosition.x + 1))
            {
                tile.highlightPlane.SetActive(true);
            }
        }


    }

    public static void CastAbility(GameBoard gameBoard, UnitClass unit, Tile hoveredTile)
    {
        Debug.Log(abilityName + " CastAbility()");

        foreach (Tile tile in gameBoard.TileArray)
        {
            if ((tile.roundedPosition.z >= hoveredTile.roundedPosition.z - 1)
                && (tile.roundedPosition.z <= hoveredTile.roundedPosition.z + 1)
                && (tile.roundedPosition.x >= hoveredTile.roundedPosition.x - 1)
                && (tile.roundedPosition.x <= hoveredTile.roundedPosition.x + 1))
            {
                Debug.Log(abilityName + " atk!");

                ExecuteCast(gameBoard, unit, tile);
            }
        }

        // Spend MP
        unit.unitMP -= abilityCost; 

        // Disable unit MOVE_REMAINING and IS_ACTIVE
        unit.MOVE_REMAINING = false;
        unit.ACTION_READY = false;

        // GOTO next turn (Early here incase unit attacks self...)
        gameBoard.turnManager.CoroutineAdvanceTurn("AcidRain");
        gameBoard.turnManager.SetTurnProcessingFalse("AcidRain");
        unit.ACTION_PROCESSING = false;

        // Refresh selected entity
        gameBoard.RefreshSelectedEntity();
    }
 
    
    private static void ExecuteCast(GameBoard gameBoard, UnitClass unit, Tile tile)
    {

        int intX = (int)tile.roundedPosition.x;
        int intZ = (int)tile.roundedPosition.z;

        gameBoard.HideIndicatorPlanes();

        gameBoard.effectDisplayer.CreateEffect(gameBoard.effectDisplayer.acidRain, new Vector3(intX, gameBoard.TileArray[intX, intZ].tile_elevation + 2f, intZ), Quaternion.identity);


        if (gameBoard.EntityArray[intX, intZ] != null && gameBoard.EntityArray[intX, intZ].entityType == "UNIT")
        {

            // Apply Poison for 3 turns, and damage once on onset
            gameBoard.EntityArray[intX, intZ].GetComponent<UnitClass>().STATUS_POISONED = 3;
            gameBoard.EntityArray[intX, intZ].GetComponent<UnitClass>().ApplyPoisonEffect();

        }
    }
}
