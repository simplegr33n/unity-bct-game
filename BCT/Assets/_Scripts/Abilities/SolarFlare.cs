using UnityEngine;

public static class SolarFlare {

    private static string abilityName = "Solar Flare";

    public static int abilityCost = 24;

    public static void SetIndicators(GameBoard gameBoard, UnitClass unit)
    {
        gameBoard.HideIndicatorPlanes();

        foreach (Tile tile in gameBoard.TileArray)
        {
            if ((tile.roundedPosition.x == unit.transform.position.x
                && (tile.roundedPosition.z >= unit.transform.position.z - 3)
                && (tile.roundedPosition.z <= unit.transform.position.z + 3)
                && (tile.roundedPosition.z != unit.transform.position.z))
                || (tile.roundedPosition.z == unit.transform.position.z
                && (tile.roundedPosition.x >= unit.transform.position.x - 3))
                && (tile.roundedPosition.x <= unit.transform.position.x + 3)
                && (tile.roundedPosition.x != unit.transform.position.x))
            {
                tile.currentUnit = unit;
                tile.SetAbilityIndicator(abilityName);
            }
        }
    }

    public static void HighlightAOE(GameBoard gameBoard, UnitClass unit, Tile hoveredTile)
    {

        if (hoveredTile.roundedPosition.x > unit.transform.position.x)
        {
            foreach (Tile tile in gameBoard.TileArray)
            {
                if ((Mathf.RoundToInt(tile.roundedPosition.z) == unit.transform.position.z
                    && (tile.roundedPosition.x > unit.transform.position.x)
                    && (tile.roundedPosition.x <= unit.transform.position.x + 3)))
                {
                    tile.highlightPlane.SetActive(true);
                }
            }
        }

        if (hoveredTile.roundedPosition.x < unit.transform.position.x)
        {
            foreach (Tile tile in gameBoard.TileArray)
            {
                if ((Mathf.RoundToInt(tile.roundedPosition.z) == unit.transform.position.z
                    && (tile.roundedPosition.x < unit.transform.position.x)
                    && (tile.roundedPosition.x >= unit.transform.position.x - 3)))
                {
                    tile.highlightPlane.SetActive(true);
                }
            }
        }

        if (hoveredTile.roundedPosition.z > unit.transform.position.z)
        {
            foreach (Tile tile in gameBoard.TileArray)
            {
                if ((Mathf.RoundToInt(tile.roundedPosition.x) == unit.transform.position.x
                    && (tile.roundedPosition.z > unit.transform.position.z)
                    && (tile.roundedPosition.z <= unit.transform.position.z + 3)))
                {
                    tile.highlightPlane.SetActive(true);
                }
            }
        }

        if (hoveredTile.roundedPosition.z < unit.transform.position.z)
        {
            foreach (Tile tile in gameBoard.TileArray)
            {
                if ((Mathf.RoundToInt(tile.roundedPosition.x) == unit.transform.position.x
                    && (tile.roundedPosition.z < unit.transform.position.z)
                    && (tile.roundedPosition.z >= unit.transform.position.z - 3)))
                {
                    tile.highlightPlane.SetActive(true);
                }
            }
        }


    }

    public static void CastAbility(GameBoard gameBoard, UnitClass unit, Tile hoveredTile)
    {
        Debug.Log("SolarFlare CastAbility()");

        if (hoveredTile.roundedPosition.x > unit.transform.position.x)
        {
            foreach (Tile tile in gameBoard.TileArray)
            {
                if ((tile.roundedPosition.z == unit.transform.position.z
                    && (tile.roundedPosition.x > unit.transform.position.x)
                    && (tile.roundedPosition.x <= unit.transform.position.x + 3)))
                {
                    Debug.Log("SolarFlare atk!");

                    ExecuteCast(gameBoard, unit, tile);

                }
            }
        }

        if (hoveredTile.roundedPosition.x < unit.transform.position.x)
        {
            foreach (Tile tile in gameBoard.TileArray)
            {
                if ((tile.roundedPosition.z == unit.transform.position.z
                    && (tile.roundedPosition.x < unit.transform.position.x)
                    && (tile.roundedPosition.x >= unit.transform.position.x - 3)))
                {
                    Debug.Log("SolarFlare atk!");
                    ExecuteCast(gameBoard, unit, tile);

                }
            }
        }

        if (hoveredTile.roundedPosition.z > unit.transform.position.z)
        {
            foreach (Tile tile in gameBoard.TileArray)
            {
                if ((tile.roundedPosition.x == unit.transform.position.x
                    && (tile.roundedPosition.z > unit.transform.position.z)
                    && (tile.roundedPosition.z <= unit.transform.position.z + 3)))
                {
                    Debug.Log("SolarFlare atk!");
                    ExecuteCast(gameBoard, unit, tile);
                }
            }
        }

        if (hoveredTile.roundedPosition.z < unit.transform.position.z)
        {
            foreach (Tile tile in gameBoard.TileArray)
            {
                if ((tile.roundedPosition.x == unit.transform.position.x
                    && (tile.roundedPosition.z < unit.transform.position.z)
                    && (tile.roundedPosition.z >= unit.transform.position.z - 3)))
                {
                    Debug.Log("SolarFlare atk!");
                    ExecuteCast(gameBoard, unit, tile);
                }
            }
        }

        // Spend MP
        unit.unitMP -= abilityCost; 

        // Disable unit MOVE_REMAINING and IS_ACTIVE
        unit.MOVE_REMAINING = false;
        unit.ACTION_READY = false;

        // GOTO next turn (Early here incase unit attacks self...)
        gameBoard.turnManager.CoroutineAdvanceTurn("SolarFlare");
        gameBoard.turnManager.SetTurnProcessingFalse("SolarFlare");
        unit.ACTION_PROCESSING = false;

        // Refresh selected entity
        gameBoard.RefreshSelectedEntity();
    }
 
    
    private static void ExecuteCast(GameBoard gameBoard, UnitClass unit, Tile tile)
    {

        int intX = (int)tile.roundedPosition.x;
        int intZ = (int)tile.roundedPosition.z;

        gameBoard.HideIndicatorPlanes();

        gameBoard.effectDisplayer.CreateEffect(gameBoard.effectDisplayer.solarFlare, new Vector3(intX, gameBoard.TileArray[intX, intZ].tile_elevation + 0.5f, intZ), Quaternion.identity);


        if (gameBoard.EntityArray[intX, intZ] != null && gameBoard.EntityArray[intX, intZ].entityType == "UNIT")
        {
            gameBoard.EntityArray[intX, intZ].GetComponent<UnitClass>().TakeDamage(unit.unitAtkDamage);
            gameBoard.effectDisplayer.CreatePopupText(unit.unitAtkDamage.ToString(), new Vector3(intX, gameBoard.TileArray[intX, intZ].tile_elevation + 1f, intZ), Color.red);

        }
    }
}
