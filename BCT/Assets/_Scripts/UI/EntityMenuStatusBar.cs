using UnityEngine;
using UnityEngine.UI;

public class EntityMenuStatusBar : MonoBehaviour {

    public static Vector2 offset = new Vector2(-20, 25);

    public UnitClass unit;

    // display bools
    private bool DEFEND_DISPLAY;
    private bool POISON_DISPLAY;

    // Status icons
    public Texture defendIconTexture;
    public Texture poisonIconTexture;

    // Status effect GameObjects
    private GameObject defendIconObject;
    private GameObject poisonIconObject;


    // Update is called once per frame
    void Update () {

        // TODO: somhow have this and basic StatusBar script as one..

        if (unit != null)
        {
            // Update individual status icons
            // DEFEND
            if (defendIconObject == null && unit.STATUS_DEFENDING > 0)
            {
                defendIconObject = Instantiate(unit.gameBoard.statusEffectPrefab) as GameObject;
                defendIconObject.transform.SetParent(transform);

                defendIconObject.GetComponent<RawImage>().texture = defendIconTexture;
            }
            if (defendIconObject != null && unit.STATUS_DEFENDING < 1)
            {
                Destroy(defendIconObject);
            }

            // POISON
            if (poisonIconObject == null && unit.STATUS_POISONED > 0)
            {
                poisonIconObject = Instantiate(unit.gameBoard.statusEffectPrefab) as GameObject;
                poisonIconObject.transform.SetParent(transform);

                poisonIconObject.GetComponent<RawImage>().texture = poisonIconTexture;
            }
            if (poisonIconObject != null && unit.STATUS_POISONED < 1)
            {
                Destroy(poisonIconObject);
            }
        }

    }
}
