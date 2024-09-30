using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VInspector;

public enum E_CastingState
{
    INACTIVE,
    NO_INPUT,
    ELEMENT_SELECTED,
    ON_COOLDOWN,
    DEFAULT,
}

public enum E_Element
{
   FIRE,
   ICE,
   ROCK,
   LIGHTNING,
   EMPTY,
}

public enum E_Template
{
    PROPEL,
    ERUPT,
    OBSTRUCT,
    INFUSE,
    EMPTY,
}
public enum E_AbilityType
{
    DEFAULT,
    PROPEL_FIRE,
    ERUPT_FIRE,
    INFUSE_FIRE,
    PROPEL_LIGHTNING,
    ERUPT_LIGHTNING,
    INFUSE_LIGHTNING,
}

public class SpellSystem : MonoBehaviour
{
    [HideInInspector] public SpellUI spellUI;

    [Tab("Main")]
    [SerializeField] KeyCode SpellKey1 = KeyCode.Q;
    [SerializeField] KeyCode SpellKey2 = KeyCode.W;
    [SerializeField] KeyCode SpellKey3 = KeyCode.E;
    [SerializeField] KeyCode SpellKey4 = KeyCode.R;

    [SerializeField] KeyCode CancelCast = KeyCode.C;
    [SerializeField] KeyCode RepeatCast = KeyCode.Space;

    E_CastingState eCurrentCastingState = E_CastingState.NO_INPUT;
    E_Element eSelectedElement = E_Element.EMPTY;
    E_Template eSelectedTemplate = E_Template.EMPTY;

    E_Element ePreviousElement = E_Element.EMPTY;
    E_Template ePreviousTemplate = E_Template.EMPTY;

    bool bCastingSpell = false;

    [SerializeField] GameObject CurrentElement = null;
    [SerializeField] GameObject CurrentTemplate = null;

    [Tab("References")]
    public Animator anim;


    public GameObject PFPropelFire;
    public GameObject PFPropelLightning;
    public GameObject PFEruptFire;
    public GameObject PFEruptLightning;
    public GameObject PFInfuseFire;
    public GameObject PFInfuseLightning;

    // Teddy was here with the new individual ability PFs!
    [HideInInspector]public Ability PropelFire;
    [HideInInspector]public Ability PropelLightning;
    [HideInInspector]public Ability EruptFire;
    [HideInInspector]public Ability EruptLightning;
    [HideInInspector]public Ability InfuseFire;
    [HideInInspector]public Ability InfuseLightning;

    public List<Ability> upgradeableAbilities;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponentInChildren<Animator>();

        eCurrentCastingState = E_CastingState.NO_INPUT;

        eSelectedElement = E_Element.EMPTY;
        eSelectedTemplate = E_Template.EMPTY;

        ePreviousElement = E_Element.EMPTY;
        ePreviousTemplate = E_Template.EMPTY;

        spellUI = FindFirstObjectByType<SpellUI>();


        PropelFire = Instantiate(PFPropelFire).GetComponent<Ability>();
        PropelLightning = Instantiate(PropelLightning).GetComponent<Ability>();
        EruptFire = Instantiate(EruptFire).GetComponent<Ability>();
        EruptLightning = Instantiate(EruptLightning).GetComponent<Ability>();
        InfuseFire = Instantiate(InfuseFire).GetComponent<Ability>();
        InfuseLightning = Instantiate(InfuseLightning).GetComponent<Ability>();
        


        //filling list of upgradeable abilities
        upgradeableAbilities = new List<Ability>();
        if(PropelFire.iLevel < PropelFire.iMaxLevel) { upgradeableAbilities.Add(PropelFire);  }
        if(PropelLightning.iLevel < PropelLightning.iMaxLevel) { upgradeableAbilities.Add(PropelLightning);  }
        if(EruptFire.iLevel < EruptFire.iMaxLevel) { upgradeableAbilities.Add(EruptFire);  }
        if(EruptLightning.iLevel < EruptLightning.iMaxLevel) { upgradeableAbilities.Add(EruptLightning);  }
        if(InfuseFire.iLevel < InfuseFire.iMaxLevel) { upgradeableAbilities.Add(InfuseFire);  }
        if(InfuseLightning.iLevel < InfuseLightning.iMaxLevel) { upgradeableAbilities.Add(InfuseLightning);  }
    }

    // Update is called once per frame
    void Update()
    {
        CheckInput();
    }

    public void CheckInput()
    {
        if (Input.GetKeyDown(SpellKey1))
        {
            if(eCurrentCastingState == E_CastingState.NO_INPUT)
            {
                SelectElement(E_Element.FIRE);
            }
            else if (eCurrentCastingState == E_CastingState.ELEMENT_SELECTED) 
            {
                SelectTemplate(E_Template.PROPEL);
            }
        }

        if (Input.GetKeyDown(SpellKey2))
        {
            if (eCurrentCastingState == E_CastingState.NO_INPUT)
            {
                SelectElement(E_Element.LIGHTNING);
            }
            else if (eCurrentCastingState == E_CastingState.ELEMENT_SELECTED)
            {
                SelectTemplate(E_Template.ERUPT);
            }
        }

        if (Input.GetKeyDown(SpellKey3))
        {
            //if (eCurrentCastingState == E_CastingState.NO_INPUT)
            //{
            //    SelectElement(E_Element.ICE);
            //}
            if (eCurrentCastingState == E_CastingState.ELEMENT_SELECTED)
            {
                SelectTemplate(E_Template.INFUSE);
            }
        }

        //if (Input.GetKeyDown(SpellKey4))
        //{
        //    if (eCurrentCastingState == E_CastingState.NO_INPUT)
        //    {
        //        SelectElement(E_Element.ROCK);
        //    }
        //    else if (eCurrentCastingState == E_CastingState.ELEMENT_SELECTED)
        //    {
        //        SelectTemplate(E_Template.OBSTRUCT);
        //    }
        //}
        
        if (Input.GetKeyDown(CancelCast))
        {
            CancelSpell();
        }
        if (Input.GetKeyDown(RepeatCast))
        {
            //
        }
    }

    public void SelectElement(E_Element _element)
    {
        eSelectedElement = _element;

        print("Selected element = " + eSelectedElement);
        //update the UI
        if (spellUI) { spellUI.SelectUIElement(eSelectedElement); }


        eCurrentCastingState = E_CastingState.ELEMENT_SELECTED;
    }

    public void SelectTemplate(E_Template _template)
    {
        eSelectedTemplate = _template;

        CastSpell();
    }

    public void CastSpell()
    {
        //CAST THE SPELL

        // Teddy was here changing how the abilities are cast!
        if (eSelectedTemplate == E_Template.PROPEL)
        {
            if (eSelectedElement == E_Element.LIGHTNING)
            {
                if (PropelLightning) 
                { 
                    PropelLightning.ActivateAbility();

                    // Play sound
                    if (AudioLibrary.instance.sfxLightningProjectile)
                    {
                        AudioLibrary.instance.sfxLightningProjectile.PlayOneShot(AudioLibrary.instance.sfxLightningProjectile.clip);
                    }
                }
            }
            else if (eSelectedElement == E_Element.FIRE)
            {
                if (PropelFire) 
                { 
                    PropelFire.ActivateAbility(); 
                    anim.SetTrigger("fireProj");

                    // Play sound
                    if (AudioLibrary.instance.sfxFireProjectile)
                    {
                        AudioLibrary.instance.sfxFireProjectile.PlayOneShot(AudioLibrary.instance.sfxFireProjectile.clip);
                    }
                }
            }
        }

        if (eSelectedTemplate == E_Template.ERUPT)
        {
            if (eSelectedElement == E_Element.LIGHTNING)
            {
                if (EruptLightning) 
                { 
                    EruptLightning.ActivateAbility();

                    // Play sound
                    if (AudioLibrary.instance.sfxLightningErupt)
                    {
                        AudioLibrary.instance.sfxLightningErupt.PlayOneShot(AudioLibrary.instance.sfxLightningErupt.clip);
                    }
                }
            }
            else if (eSelectedElement == E_Element.FIRE)
            {
                if (EruptFire) 
                { 
                    EruptFire.ActivateAbility();

                    // Play sound
                    if (AudioLibrary.instance.sfxFireErupt)
                    {
                        AudioLibrary.instance.sfxFireErupt.PlayOneShot(AudioLibrary.instance.sfxFireErupt.clip);
                    }
                }
            }
        }

        if (eSelectedTemplate == E_Template.INFUSE)
        {
            if (eSelectedElement == E_Element.LIGHTNING)
            {
                InfuseLightning.ActivateAbility();

                // Play sound
                if (AudioLibrary.instance.sfxFireInfuse)
                {
                    AudioLibrary.instance.sfxFireInfuse.PlayOneShot(AudioLibrary.instance.sfxFireInfuse.clip);
                }
            }
            else if (eSelectedElement == E_Element.FIRE)
            {
                InfuseFire.ActivateAbility();

                // Play sound
                if (AudioLibrary.instance.sfxLightningInfuse)
                {
                    AudioLibrary.instance.sfxLightningInfuse.PlayOneShot(AudioLibrary.instance.sfxLightningInfuse.clip);
                }
            }
        }

        ePreviousElement = eSelectedElement;
        ePreviousTemplate = eSelectedTemplate;

        eSelectedElement = E_Element.EMPTY;
        eSelectedTemplate = E_Template.EMPTY;

        //perhaps no?
        //eCurrentCastingState = E_CastingState.NO_INPUT;
        //
        //if (spellUI) { spellUI.ResetSpellUI(); }

        //COOLDOWN - temporary
        StartCoroutine(SpellCooldown(0.5f));

    }

    //cancels spellcast, resetting element/template
    public void CancelSpell()
    {
        //if the element is selected, but a template isnt hovered
        if(eCurrentCastingState == E_CastingState.ELEMENT_SELECTED)
        {
            eCurrentCastingState = E_CastingState.NO_INPUT;
            eSelectedElement = E_Element.EMPTY;

            //UPDATE UI!
            if (spellUI) { spellUI.ResetSpellUI(); }


            print("cancelled Spell Element");
        }

    }



    public void LevelAbility(E_AbilityType _abilityType, E_AbilityUpgradeType _upgradeType)
    {
        spellUI.UpdateSpellIconUI(_abilityType, _upgradeType);

        switch (_abilityType)
        {
            case E_AbilityType.PROPEL_FIRE:
                PropelFire.LevelGeneral(_upgradeType) ;
                if(PropelFire.iLevel == PropelFire.iMaxLevel)
                {
                    if (upgradeableAbilities.Contains(PropelFire)) { upgradeableAbilities.Remove(PropelFire); }
                
                }
                break;
            case E_AbilityType.ERUPT_FIRE:
                EruptFire.LevelGeneral(_upgradeType);
                if (EruptFire.iLevel == EruptFire.iMaxLevel)
                {
                    if (upgradeableAbilities.Contains(EruptFire)) { upgradeableAbilities.Remove(EruptFire); }

                }
                break;
            case E_AbilityType.INFUSE_FIRE:
                InfuseFire.LevelGeneral(_upgradeType);
                if (InfuseFire.iLevel == InfuseFire.iMaxLevel)
                {
                    if (upgradeableAbilities.Contains(InfuseFire)) { upgradeableAbilities.Remove(InfuseFire); }

                }
                break;
            case E_AbilityType.PROPEL_LIGHTNING:
                PropelLightning.LevelGeneral(_upgradeType);
                if (PropelLightning.iLevel == PropelLightning.iMaxLevel)
                {
                    if (upgradeableAbilities.Contains(PropelLightning)) { upgradeableAbilities.Remove(PropelLightning); }

                }
                break;
            case E_AbilityType.ERUPT_LIGHTNING:
                EruptLightning.LevelGeneral(_upgradeType);
                if (EruptLightning.iLevel == EruptLightning.iMaxLevel)
                {
                    if (upgradeableAbilities.Contains(EruptLightning)) { upgradeableAbilities.Remove(EruptLightning); }

                }
                break;
            case E_AbilityType.INFUSE_LIGHTNING:
                InfuseLightning.LevelGeneral(_upgradeType);
                if (InfuseLightning.iLevel == InfuseLightning.iMaxLevel)
                {
                    if (upgradeableAbilities.Contains(InfuseLightning)) { upgradeableAbilities.Remove(InfuseLightning); }

                }
                break;
            default:
                break;
        }



        Player.instance.playerPickupManager.bLeveling = false;
        Player.instance.playerPickupManager.CheckXP();
    }

    //finds an ability to upgrade from the spellsystem's array of upgradeable abilities
    public E_AbilityType GetRandomAbilityToUpgrade()
    {
        //no ability to upgrade
        if (upgradeableAbilities.Count == 0) { return E_AbilityType.DEFAULT; }

        //min inclusive, max exclusive
        int rando = Random.Range(0, upgradeableAbilities.Count);

        return upgradeableAbilities[rando].abilityType;

    }

    IEnumerator SpellCooldown(float _cooldownTime)
    {

        if (spellUI) { spellUI.ResetSpellUI(); }
        eCurrentCastingState = E_CastingState.ON_COOLDOWN;
       

        float currentTime = _cooldownTime;

        while(currentTime > 0)
        {
            currentTime -= Time.deltaTime;
            if (spellUI) { spellUI.UpdateCooldowFX(currentTime, _cooldownTime); }
            
            yield return null;
        }



        eCurrentCastingState = E_CastingState.NO_INPUT;



    }




}
