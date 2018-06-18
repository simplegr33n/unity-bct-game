using UnityEngine;
using UnityEngine.UI;

public class EntityMenu : MonoBehaviour {

    public GameBoard gameBoard;

    public EntityClass SELECTED_GAME_OBJECT;

    public GameObject attackMenu;

    public GameObject inventoryMenu;
    public Transform inventoryContainer;
    public GameObject inventoryItemPrefab;

    public GameObject abilityMenu;
    public Transform abilityContainer;
    public GameObject abilityItemPrefab;



    // Unit Info
    public RawImage profImage;
    public Text unitName;
    public Text unitMaxHP;
    public Text unitHP;
    public Text unitMaxMP;
    public Text unitMP;
    public Image unitMPBar;
    public Image unitHPBar;
    public GameObject entityMenuStatusBar;

    // Buttons
    public Button btnAttack;
    public Button btnAbility;
    public Button btnItems;
    public Button btnDefend;


    private void Awake()
    {
        inventoryMenu.SetActive(false);
        abilityMenu.SetActive(false);

        // ACTIONS MENU SETUP
        btnAttack.onClick.AddListener(AttackButton);
        btnAbility.onClick.AddListener(AbilityButton);
        btnItems.onClick.AddListener(ShowInventoryButton);
        btnDefend.onClick.AddListener(DefendButton);

    }

    // TODO: put in update?
    public void SetSelectedEntity(EntityClass selectedObject)
    {


        // Set new SELECTED_GAME_OBJECT
        SELECTED_GAME_OBJECT = selectedObject;



        if (selectedObject.entityType == "UNIT")
        {

            UnitClass selectedUnit = SELECTED_GAME_OBJECT.GetComponent<UnitClass>();

            // Set AttackMenu
            SetAttackMenu();

            // Update menu with SELECTED_GAME_OBJECT
            profImage.texture = selectedUnit.unitProfileTexture;
            unitName.text = selectedUnit.entityName;
            unitMaxHP.text = (selectedUnit.unitHPMax).ToString();
            unitHP.text = (selectedUnit.unitHP).ToString();
            unitMaxMP.text = (selectedUnit.unitMPMax).ToString();
            unitMP.text = (selectedUnit.unitMP).ToString();

            // Set Entity Menu status bar
            entityMenuStatusBar.GetComponent<EntityMenuStatusBar>().unit = selectedUnit;

            // Fill HP bar
            float REMAINING_HP_RATIO = ((float)selectedUnit.unitHP) / (selectedUnit.unitHPMax);
            unitHPBar.fillAmount = REMAINING_HP_RATIO;

            // Fill MP bar
            float REMAINING_MP_RATIO = ((float)selectedUnit.unitMP) / (selectedUnit.unitMPMax);
            unitMPBar.fillAmount = REMAINING_MP_RATIO;


        }


    }


    // BUTTONS
    void AttackButton()
    {
        SELECTED_GAME_OBJECT.GetComponent<UnitClass>().ShowAttacks();
    }


    void AbilityButton()
    {
        gameBoard.HideIndicatorPlanes();

        foreach (Transform child in abilityContainer)
        {
            Destroy(child.gameObject);
        }

        abilityMenu.SetActive(true);

        GameObject abilityItem = null;
        
        foreach (string ability in SELECTED_GAME_OBJECT.GetComponent<UnitClass>().ABILITY_LIST)
        {

            switch (ability)
            {
                case "Solar Flare":
                    abilityItem = Instantiate(abilityItemPrefab) as GameObject;
                    abilityItem.transform.SetParent(abilityContainer);
                    abilityItem.GetComponentInChildren<Button>().GetComponentInChildren<Text>().text = "Solar Flare (" + SolarFlare.abilityCost + "MP)";

                    if (SELECTED_GAME_OBJECT.GetComponent<UnitClass>().unitMP >= SolarFlare.abilityCost)
                    {
                        abilityItem.GetComponentInChildren<Button>().onClick.AddListener(() =>
                        {
                            AbilityClicked(ability);
                        });
                    }

                    break;

                case "Acid Rain":
                    abilityItem = Instantiate(abilityItemPrefab) as GameObject;
                    abilityItem.transform.SetParent(abilityContainer);
                    abilityItem.GetComponentInChildren<Button>().GetComponentInChildren<Text>().text = ability +" (" + AcidRain.abilityCost + "MP)";

                    if (SELECTED_GAME_OBJECT.GetComponent<UnitClass>().unitMP >= AcidRain.abilityCost)
                    {
                        abilityItem.GetComponentInChildren<Button>().onClick.AddListener(() =>
                        {
                            AbilityClicked(ability);
                        });
                    }

                    break;

                case "Ornithophobia":
                    abilityItem = Instantiate(abilityItemPrefab) as GameObject;
                    abilityItem.transform.SetParent(abilityContainer);
                    abilityItem.GetComponentInChildren<Button>().GetComponentInChildren<Text>().text = ability + " (" + Ornithophobia.abilityCost + "MP)";

                    if (SELECTED_GAME_OBJECT.GetComponent<UnitClass>().unitMP >= Ornithophobia.abilityCost)
                    {
                        abilityItem.GetComponentInChildren<Button>().onClick.AddListener(() =>
                        {
                            AbilityClicked(ability);
                        });
                    }

                    break;

                default:
                    abilityItem = Instantiate(abilityItemPrefab) as GameObject;
                    abilityItem.transform.SetParent(abilityContainer);
                    abilityItem.GetComponentInChildren<Button>().GetComponentInChildren<Text>().text = "db(ENTITY MENU)";

                    break;
            }
        }

    }


    void ShowInventoryButton()
    {
        gameBoard.HideIndicatorPlanes();

        foreach (Transform child in inventoryContainer)
        {
            Destroy(child.gameObject);
        }

        inventoryMenu.SetActive(true);

        int healthPotions = 0;
        int manaPotions = 0;

        foreach (string item in gameBoard.PLAYER_INVENTORY)
        {
            switch (item)
            {
                case "Health Potion":
                    healthPotions += 1;
                    break;

                case "Mana Potion":
                    manaPotions += 1;
                    break;
            }
        }

        if (healthPotions > 0)
        {
            GameObject inventoryObject = Instantiate(inventoryItemPrefab) as GameObject;
            inventoryObject.transform.SetParent(inventoryContainer);
            inventoryObject.GetComponentInChildren<Button>().GetComponentInChildren<Text>().text = "Health Potion (" + healthPotions + ")";

            inventoryObject.GetComponentInChildren<Button>().onClick.AddListener(() =>
            {
                ItemClicked("Health Potion");
            });



        }

        if (manaPotions > 0)
        {
            GameObject inventoryObject = Instantiate(inventoryItemPrefab) as GameObject;
            inventoryObject.transform.SetParent(inventoryContainer);
            inventoryObject.GetComponentInChildren<Button>().GetComponentInChildren<Text>().text = "Mana Potion (" + manaPotions + ")";

            inventoryObject.GetComponentInChildren<Button>().onClick.AddListener(() =>
            {
                ItemClicked("Mana Potion");
            });




        }

    }

    void DefendButton()
    {

        SELECTED_GAME_OBJECT.GetComponent<UnitClass>().QueueAction("DEF|");
        gameBoard.turnManager.TURN_INSTRUCTION += "DEF|";
        SetAttackMenu();

    }


    public void SetVisible(bool setTo)
    {

        if (setTo == true)
        {
            gameObject.SetActive(true);
        } else
        {
            gameObject.SetActive(false);
        }
    }

    public void SetAttackMenu()
    {
        // Hide or Show AttackMenu
        if (SELECTED_GAME_OBJECT != null &&
            SELECTED_GAME_OBJECT.GetComponent<UnitClass>().ACTION_READY && 
            (SELECTED_GAME_OBJECT.GetComponent<UnitClass>().playerTeam == gameBoard.PLAYER_TEAM) &&
            !SELECTED_GAME_OBJECT.GetComponent<UnitClass>().IS_MOVING)
        {
            attackMenu.SetActive(true);
        }
        else
        {
            attackMenu.SetActive(false);
            inventoryMenu.SetActive(false);
            abilityMenu.SetActive(false);

        }
    }



    public void HideInventoryAndAbilityMenus()
    {

        inventoryMenu.SetActive(false);
        abilityMenu.SetActive(false);

        gameBoard.RefreshSelectedEntity();

    }

    // On click listener function
    public void ItemClicked(string itemName)
    {
        SELECTED_GAME_OBJECT.GetComponent<UnitClass>().QueueAction("ITM|" + itemName);
        gameBoard.turnManager.TURN_INSTRUCTION += "ITM|" + itemName + "|";

    }

    public void AbilityClicked(string abilityName)
    {
        switch (abilityName)
        {
            case "Solar Flare":
                SolarFlare.SetIndicators(gameBoard, SELECTED_GAME_OBJECT.GetComponent<UnitClass>());
                break;

            case "Acid Rain":
                AcidRain.SetIndicators(gameBoard, SELECTED_GAME_OBJECT.GetComponent<UnitClass>());
                break;

            case "Ornithophobia":
                Ornithophobia.SetIndicators(gameBoard, SELECTED_GAME_OBJECT.GetComponent<UnitClass>());
                break;

            default:
                Debug.Log("error: no such ability");
                break;



        }
       
    }

}
