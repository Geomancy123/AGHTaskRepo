using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelUpUI : MonoBehaviour
{
    SpellUI spellUI;

    public bool bUpgradesEnabled = true;

    public bool bSelecting = false;

    int iCurrentSelectionIndex = -1;
    [SerializeField] GameObject LevelUpScreen;


    [SerializeField] GameObject DamageSocketPF;
    [SerializeField] GameObject RangeSocketPF;
    [SerializeField] GameObject UniqueSocketPF;

    [SerializeField] List<GameObject> SpellPositions;
    [SerializeField] List<GameObject> SocketPositions;
    [SerializeField] List<TextMeshProUGUI> SocketTexts;


    List<GameObject> SpellImages = new List<GameObject>() { null, null, null};
    List<GameObject> SocketImages = new List<GameObject>() { null, null, null };


    [SerializeField] Button UpgradeButton;
    [SerializeField] TextMeshProUGUI UpgradeButtonText;


    List<E_AbilityType> AbilitiesToUpgrade;
    List<E_AbilityUpgradeType> UpgradeTypes;





    private void Start()
    {
        LevelUpScreen.SetActive(false);
        UpgradeButton.GetComponent<Button>().interactable = false;

        spellUI = Player.instance.playerSpellSystem.spellUI;


        



    }




    //prompts the player to upgrade an ability
    public void PromptAbilityUpgrade()
    {
        bSelecting = false;
        iCurrentSelectionIndex = -1;

        if (!bUpgradesEnabled)
        {
            //doesnt upgrade if upgrades are disabled
            ReturnUpgradeSelection();
        }

        LevelUpScreen.SetActive(true);

        //rolling placeholder variables
        E_AbilityType phType = E_AbilityType.DEFAULT;
        E_AbilityUpgradeType phUpType = E_AbilityUpgradeType.DEFAULT;




        //rolls random ubilities and upgrades
        AbilitiesToUpgrade = new List<E_AbilityType>(); ;
        UpgradeTypes = new List<E_AbilityUpgradeType>(); ;


        //1
        AbilitiesToUpgrade.Add(Player.instance.playerSpellSystem.GetRandomAbilityToUpgrade());
        UpgradeTypes.Add(GetRandomUpgrade(AbilitiesToUpgrade[0]));


        //2
        int rounds = 0;
        while(rounds < 2)
        {
            phType = Player.instance.playerSpellSystem.GetRandomAbilityToUpgrade();
            phUpType = GetRandomUpgrade(phType);

            if(phType == AbilitiesToUpgrade[0] && phUpType == UpgradeTypes[0])
            {
                rounds++;
            }
            else
            {
                break;
            }
        }
        AbilitiesToUpgrade.Add(phType);
        UpgradeTypes.Add(phUpType);

        //3
        rounds = 0;
        while (rounds < 4)
        {
            phType = Player.instance.playerSpellSystem.GetRandomAbilityToUpgrade();
            phUpType = GetRandomUpgrade(phType);

            if ((phType == AbilitiesToUpgrade[0] && phUpType == UpgradeTypes[0] )|| (phType == AbilitiesToUpgrade[1] && phUpType == UpgradeTypes[1]))
            {
                rounds++;
            }
            else
            {
                break;
            }
        }
        AbilitiesToUpgrade.Add(phType);
        UpgradeTypes.Add(phUpType);






        //Set the upgrade UI

        for (int i = 0; i < 3; i++)
        {

            //ABILITIES

            switch (AbilitiesToUpgrade[i])
            {
                case E_AbilityType.PROPEL_FIRE:
                    SpellImages[i] = Instantiate(Player.instance.playerSpellSystem.spellUI.PropelButton_Fire, SpellPositions[i].transform);
                    break;
                case E_AbilityType.ERUPT_FIRE:
                    SpellImages[i] = Instantiate(Player.instance.playerSpellSystem.spellUI.EruptButton_Fire, SpellPositions[i].transform);
                    break;
                case E_AbilityType.INFUSE_FIRE:
                    SpellImages[i] = Instantiate(Player.instance.playerSpellSystem.spellUI.InfuseButton_Fire, SpellPositions[i].transform);
                    break;
                case E_AbilityType.PROPEL_LIGHTNING:
                    SpellImages[i] = Instantiate(Player.instance.playerSpellSystem.spellUI.PropelButton_Lightning, SpellPositions[i].transform);
                    break;
                case E_AbilityType.ERUPT_LIGHTNING:
                    SpellImages[i] = Instantiate(Player.instance.playerSpellSystem.spellUI.EruptButton_Lightning, SpellPositions[i].transform);
                    break;
                case E_AbilityType.INFUSE_LIGHTNING:
                    SpellImages[i] = Instantiate(Player.instance.playerSpellSystem.spellUI.InfuseButton_Lightning, SpellPositions[i].transform);
                    break;
                default:
                    break;
                    
            };


            SpellImages[i].transform.localScale = new Vector3(1.5f, 1.5f, 1.5f); 
            SpellImages[i].transform.position = SpellPositions[i].transform.position;
            SpellImages[i].SetActive(true);
            int parameter = i;
            SpellImages[i].GetComponent<Button>().onClick.AddListener(delegate { MakeSelection(parameter); });


            //UPGRADE TYPES / SOCKETS

            switch (UpgradeTypes[i])
            {
                case E_AbilityUpgradeType.DAMAGE:
                    SocketImages[i] = Instantiate(DamageSocketPF, SocketPositions[i].transform);

                    if(AbilitiesToUpgrade[i] == E_AbilityType.INFUSE_LIGHTNING)
                    {
                        SocketTexts[i].SetText("+ Speed");
                    }
                    else
                    {
                        SocketTexts[i].SetText("+ Damage");
                    }
                    
                    break;
                case E_AbilityUpgradeType.RANGE:
                    SocketImages[i] = Instantiate(RangeSocketPF, SocketPositions[i].transform);

                    if (AbilitiesToUpgrade[i] == E_AbilityType.INFUSE_LIGHTNING)
                    {
                        SocketTexts[i].SetText("+ Dodge");
                    }
                    else
                    {
                        SocketTexts[i].SetText("+ Range");
                    }

                    
                    break;
                case E_AbilityUpgradeType.UNIQUE:
                    SocketImages[i] = Instantiate(UniqueSocketPF, SocketPositions[i].transform);

                    if (AbilitiesToUpgrade[i] == E_AbilityType.PROPEL_FIRE)
                    {
                        SocketTexts[i].SetText("+ Pierce");
                    }
                    else if (AbilitiesToUpgrade[i] == E_AbilityType.PROPEL_LIGHTNING)
                    {
                        SocketTexts[i].SetText("+ Spread");
                    }
                    if (AbilitiesToUpgrade[i] == E_AbilityType.ERUPT_FIRE)
                    {
                        SocketTexts[i].SetText("+ Burn");
                    }
                    else if (AbilitiesToUpgrade[i] == E_AbilityType.ERUPT_LIGHTNING)
                    {
                        SocketTexts[i].SetText("+ Slow");
                    }

                    break;
                default:
                    break;

            };


            //SocketImages[i].transform.localScale *= 1.5f;
            SocketImages[i].transform.position = SocketImages[i].transform.position;


        }

        Time.timeScale = 0.0f;
    }

    



    public void MakeSelection(int _selectionIndex)
    {

        SpellImages[0].transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
        SpellImages[1].transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
        SpellImages[2].transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);

        SpellImages[_selectionIndex].transform.localScale = new Vector3(1.9f, 1.9f, 1.9f);



        iCurrentSelectionIndex = _selectionIndex;

        UpgradeButtonText.SetText("CONFIRM");
        UpgradeButton.GetComponent<Button>().interactable = true;
       
    }

    public void ReturnUpgradeSelection()
    {
        UpgradeButtonText.SetText("CHOOSE ONE");

        print(iCurrentSelectionIndex);
        if (iCurrentSelectionIndex >= 0 && iCurrentSelectionIndex < 3)
        {
            Player.instance.playerPickupManager.iPlayerLevel++;
            print(AbilitiesToUpgrade[iCurrentSelectionIndex]);
            print(UpgradeTypes[iCurrentSelectionIndex]);
            Player.instance.playerSpellSystem.LevelAbility(AbilitiesToUpgrade[iCurrentSelectionIndex], UpgradeTypes[iCurrentSelectionIndex]);
        }

        

        Destroy(SpellImages[0]);
        Destroy(SpellImages[1]);
        Destroy(SpellImages[2]);

        Destroy(SocketImages[0]);
        Destroy(SocketImages[1]);
        Destroy(SocketImages[2]);



        LevelUpScreen.SetActive(false);
        bSelecting = false;
        UpgradeButton.GetComponent<Button>().interactable = false;
        Time.timeScale = 1.0f;
    }



    
    //gets a random upgrade shard
    E_AbilityUpgradeType GetRandomUpgrade(E_AbilityType _aType)
    {
        //1 = damage, 2 = range, 3 = unique
        int rand = 0; 

        
        
        if(_aType == E_AbilityType.INFUSE_FIRE || _aType == E_AbilityType.INFUSE_LIGHTNING)
        {
            rand = Random.Range(1, 3);
        }
        else
        {
            rand = Random.Range(1, 4);
        }

        


        return (E_AbilityUpgradeType)rand;
    }
}
