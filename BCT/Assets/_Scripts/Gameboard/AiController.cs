using System.Collections.Generic;
using UnityEngine;

public class AiController : MonoBehaviour {

    private UnitClass controlledUnit;

    private GameBoard gameBoard;

    public bool RUNNING_AI;

    public void ProcessTurnAI(UnitClass unit, GameBoard board)
    {

        controlledUnit = unit;
        gameBoard = board;

        if (!RUNNING_AI && unit != null)
        {
            RUNNING_AI = true;
            StartCoroutine(CheckMovesAI());
        }
        
      
    }

    // Find available move spots
    private IEnumerator<WaitForSeconds> CheckMovesAI()
    {

        yield return new WaitForSeconds(0.1f);
        
        // If died on turn (ie. from Poison) stop processing AI.
        if (controlledUnit == null)
        {
            RUNNING_AI = false;
            gameBoard.turnManager.CoroutineAdvanceTurn("AiController CheckMovesAI");
            yield break;
        }
        
        // ShowMoves() to get list of available move tiles in unit
        // and generate PATH_STRINGS in multitile objects
        controlledUnit.ShowMoves();

        yield return new WaitForSeconds(0.5f);




        Debug.Log("AI available moves count: " + controlledUnit.availableMoveTilesList.Count);

        // Find closest enemy
        UnitClass closestUnit = null;
        float closestEnemyDistance = 999;
        foreach (EntityClass entity in gameBoard.EntityArray)
        {
            if (entity != null 
                && entity.entityType == "UNIT" 
                && entity.GetComponent<UnitClass>().playerTeam != controlledUnit.playerTeam)
            {
                float dist = Vector3.Distance(controlledUnit.transform.position, entity.transform.position);

                if (dist < closestEnemyDistance)
                {
                    closestUnit = entity.GetComponent<UnitClass>();
                    closestEnemyDistance = dist;

                }

            }
        }

        Debug.Log("Closest enemy: " + closestUnit.entityName + ", distance: " + closestEnemyDistance);

        // if tile is adjacent to enemy, attack and break enumerator
        if ((controlledUnit.transform.position.x - closestUnit.transform.position.x == 0
            && ((controlledUnit.transform.position.z - closestUnit.transform.position.z == 1) || (controlledUnit.transform.position.z - closestUnit.transform.position.z == -1)))
            || (controlledUnit.transform.position.z - closestUnit.transform.position.z == 0
            && ((controlledUnit.transform.position.x - closestUnit.transform.position.x == 1) || (controlledUnit.transform.position.x - closestUnit.transform.position.x == -1))))
        {
            Debug.Log("AI FOUND ADJACENT");

            AttackAI(closestUnit.transform.position);

            yield break;

        }


        // Find closest tile to closest enemy
        Tile closestTile = null;
        float closestTileToEnemyDistance = 999;

        foreach (int[] potentialMove in controlledUnit.availableMoveTilesList)
        {


            float dist = Vector2.Distance(new Vector2(closestUnit.transform.position.x, closestUnit.transform.position.z), new Vector2(potentialMove[0], potentialMove[1]));

            // check for possible move tiles for adjacent enemy
            if ((potentialMove[0] - closestUnit.transform.position.x == 0
                && ((potentialMove[1] - closestUnit.transform.position.z == 1) || (potentialMove[1] - closestUnit.transform.position.z == -1))) 
                || (potentialMove[1] - closestUnit.transform.position.z == 0
                && ((potentialMove[0] - closestUnit.transform.position.x == 1) || (potentialMove[0] - closestUnit.transform.position.x == -1))))
            {
                closestTile = gameBoard.TileArray[potentialMove[0], potentialMove[1]];
                closestTileToEnemyDistance = dist;

                
                Debug.Log("AI FOUND ADJACENT");

                // set and break loop if adjacent
                break;
            }

            if (dist < closestTileToEnemyDistance)
            {
                closestTile = gameBoard.TileArray[potentialMove[0], potentialMove[1]];
                closestTileToEnemyDistance = dist;
            }
        }

        Debug.Log("Closest tile to enemy: " + closestTile.transform.position + ", distance: " + closestTileToEnemyDistance);

        Debug.Log("AI Self posit: " + controlledUnit.transform.position + ", closest tile: " + closestTile.transform.position);

        // Move if not already at target pos
        if (!((controlledUnit.transform.position.x == Mathf.Round(closestTile.transform.position.x))
            && (controlledUnit.transform.position.z == Mathf.Round(closestTile.transform.position.z))))
        {
            MoveAI(closestTile.PATH_STRING);
        }


        // Attack if adjacent. Defend if not.
        if ((closestTile.transform.position.x - closestUnit.transform.position.x == 0
            && ((closestTile.transform.position.z - closestUnit.transform.position.z == 1) || (closestTile.transform.position.z - closestUnit.transform.position.z == -1)))
            || (closestTile.transform.position.z - closestUnit.transform.position.z == 0
            && ((closestTile.transform.position.x - closestUnit.transform.position.x == 1) || (closestTile.transform.position.x - closestUnit.transform.position.x == -1))))
        {

            // Ensure height within 1
            if (Mathf.Abs(controlledUnit.transform.position.y - closestUnit.transform.position.y) <= 1)
            {
                AttackAI(closestUnit.transform.position);
            } else
            {
                DefendAI();
            }
            

        } else
        {
            DefendAI();
        }
 

    }

    private void MoveAI(string pathString)
    {

        controlledUnit.QueueAction("MOV|" + pathString);

    }


    private void AttackAI(Vector3 attackPosition)
    {

        controlledUnit.QueueAction("ATK|" + attackPosition.x + "|" + attackPosition.z);

    }

    private void DefendAI()
    {

        controlledUnit.QueueAction("DEF|");

    }


}
