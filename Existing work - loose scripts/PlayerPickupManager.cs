using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VInspector;


//Pickup types - feel free to edit
public enum E_PickupType
{
    DEFAULT,
    HEALTH_PICKUP,
    EXPERIENCE_PICKUP,
    UPGRADE_PICKUP,
    BUFF_PICKUP,
    MAGNET_PICKUP,
}

//upgrade types - individual values for each ability
public enum E_PickupUpgradeType
{
    DEFAULT,

    PROPEL_FIRE,
    PROPEL_LIGHTNING,

    ERUPT_FIRE,
    ERUPT_LIGHTNING,

    INFUSE_FIRE,
    INFUSE_LIGHTNING,
}

public enum E_AbilityUpgradeType
{
    DEFAULT,
    DAMAGE = 1,
    RANGE = 2,
    UNIQUE = 3,
}

public enum E_PickupRarity
{
    DEFAULT,
    COMMON,
    UNCOMMON,
    RARE,
}


public class PlayerPickupManager : MonoBehaviour
{
    Player player;
    SpellSystem spellSystem;
    public bool bLeveling = false;

    [SerializeField, ReadOnly] public int iPlayerLevel  = 0;
    [SerializeField, ReadOnly] public int iCurrentXP = 0;
    [SerializeField, ReadOnly] int iTotalXP = 0;   //probably not needed
    [SerializeField] public int iXpRequiredForLevel = 6;
    //the xp needed per level increases by this amount each level up
    [SerializeField] int iXpPerLevelIncrease = 2;



    [SerializeField] int iCommonXPGain = 1;
    [SerializeField] int iUncommonXPGain = 3;
    [SerializeField] int iRareXPGain = 10;

    [SerializeField] float fCommonHealthGain = 30;
    [SerializeField] float fUncommonHealthGain = 60;
    [SerializeField] float fRareHealthGain = 10000;



    private void Awake()
    {
        if(GetComponent<Player>())
        {
            player = GetComponent<Player>();
        }
        if (GetComponent<SpellSystem>())
        {
            spellSystem = GetComponent<SpellSystem>();
        }
    }

    

    public void PickUpThePickup(PickupBase _pickup)
    {
        //switch for the pickup tpe
        switch (_pickup.pickupType)
        {
            case E_PickupType.HEALTH_PICKUP:
                //pik up some health
                PickupHealth(_pickup.pickupRarity);
                break;

            case E_PickupType.EXPERIENCE_PICKUP:
                //Pickup an experience drop
                PickupExperience(_pickup.pickupRarity);
                break;

            case E_PickupType.UPGRADE_PICKUP:
                //upgrade an ability REGARDLESS of current XP
                AbilityUpgradeSequence();
                break;

            case E_PickupType.BUFF_PICKUP:
                break;

            case E_PickupType.MAGNET_PICKUP:
                PickupMagnet();
                break;

            default:
                break;
        }

        //destroy the pickup
        _pickup.PickUp();
    }

    //Health pickup
    void PickupHealth(E_PickupRarity _rarity)
    {
        switch (_rarity)
        {
            case E_PickupRarity.COMMON:
                player.RestoreHealth(fCommonHealthGain);
                break;
            case E_PickupRarity.UNCOMMON:
                player.RestoreHealth(fUncommonHealthGain);
                break;
            case E_PickupRarity.RARE:
                player.RestoreHealth(fRareHealthGain);
                break;
            default:
                break;
        }
        print("hp++ yahoo");
    }

    //experience pickup
    void PickupExperience(E_PickupRarity _rarity)
    {
        //switch for different experience pickup rarities
        switch (_rarity)
        {
            case E_PickupRarity.COMMON:
                iCurrentXP += iCommonXPGain;
                iTotalXP += iCommonXPGain;
                print("xp + " + iCommonXPGain);
                break;
            case E_PickupRarity.UNCOMMON:
                iCurrentXP += iUncommonXPGain;
                iTotalXP += iUncommonXPGain;
                print("xp + " + iUncommonXPGain);
                break;
            case E_PickupRarity.RARE:
                iCurrentXP += iRareXPGain;
                iTotalXP += iRareXPGain;
                print("xp + " + iRareXPGain);
                break;
            default: 
                break;
        }

        UIManager.instance.xpBar.UpdateXPBar();


        CheckXP();
    }

    public void CheckXP()
    {
       
        if(Time.timeScale == 0 || bLeveling)
        {
            return;
        }

        

            //while loop in case you get like 1000 xp and get multiple levels at once
        if (iCurrentXP >= iXpRequiredForLevel)
        {
            bLeveling = true;
            //spend the xp to level up
            iCurrentXP = iCurrentXP - iXpRequiredForLevel;
                    //increase next elvel up threshold
                    iXpRequiredForLevel += iXpPerLevelIncrease;

                    AbilityUpgradeSequence();
                
                



        }


            UIManager.instance.xpBar.UpdateXPBar();

        
    }


    //sequence to upgrade ability - finds ability to upgrade, then prompts the player to choose which path.
    //if all abilities are upgraded, does something else
    void AbilityUpgradeSequence()
    {
        //all abilities are maxed
        if (spellSystem.upgradeableAbilities.Count == 0)
        {
            //endless level character things?
            print("Hey, you have maxed everything! well done. you are now god.");
        }
        else
        {
            //upgrade an ability

           



            //prompting the level up ui to offer 3 upgrade options, returning the option selected
            UIManager.instance.levelUpUI.PromptAbilityUpgrade();

            
            
        }
    }




    


    




    void PickupMagnet()
    {
        foreach (PickupBase pickup in GlobalDropManager.instance.ActivePickups)
        {
            if(pickup.pickupType == E_PickupType.EXPERIENCE_PICKUP )
            {
                pickup.TriggerVacuum();
            }
            
        }
    }

}
