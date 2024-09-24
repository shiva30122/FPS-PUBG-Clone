using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Pick_Up_ : MonoBehaviour
{
    #region Public Variables

    [Header("Weapon Settings")]
    public LayerMask weaponLayerMask;
    public float pickupRange = 2f;
    public GameObject currentWeapon;

    [Header("Weapon Slots")]
    public List<GameObject> gunSlots;    // List to hold slots for guns (e.g., right hand, left hand)
    public List<GameObject> pistolSlots;  // List to hold slots for pistols (e.g., right leg, left leg)
    public List<GameObject> meleeSlots;   // List to hold slots for melee weapons (e.g., back, belt)

    public bool isPicked = false;

    #endregion

    public Text PickUPInfoText;

    void Update()
    {
        HandleWeaponPickup();
        HandleWeaponDrop();
        HandleWeaponSwitch();
    }

    #region Weapon Pickup, Drop, and Switch Region

    private void HandleWeaponPickup()
    {
        RaycastHit Info;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out Info, pickupRange, weaponLayerMask))
        {

            Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * pickupRange, Color.green, 2f);

            PickupItem PicakableInfo = Info.collider.GetComponent<PickupItem>();
            Gun Guns = Info.collider.GetComponent<Gun>();
            if(PicakableInfo)
            {
                PickUPInfoText.text = "  "+PicakableInfo.itemData.ItemName + " -> " + PicakableInfo.itemData.CurrentAmount;
            }
            else if (Guns)
            {
                PickUPInfoText.text = Guns.WeaponName + "  Ammo -> " + Guns.currentAmmo;
            }
            else 
            {
                PickUPInfoText.text = "";   
            }
        
        }
        else 
        {
            PickUPInfoText.text = "";   
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, pickupRange, weaponLayerMask))
            {
                GameObject weapon = hit.collider.gameObject;
                Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * pickupRange, Color.green, 2f);
                Debug.Log("Weapon detected: " + weapon.name);

                if (weapon.CompareTag("Gun"))
                {
                    TryPickupWeapon(weapon, gunSlots);
                }
                else if (weapon.CompareTag("Pistol"))
                {
                    TryPickupWeapon(weapon, pistolSlots);
                }
                else if (weapon.CompareTag("Melee"))
                {
                    TryPickupWeapon(weapon, meleeSlots);
                }


                else if (weapon.CompareTag("Pickable"))
                {
                    // Attempt to get the PickupItem component from the hit object
                    PickupItem item = weapon.GetComponent<PickupItem>();

                    // Check if PickupItem component exists
                    if (item != null)
                    {
                        Debug.Log("Pickable item found: " + weapon.name);
                        // Call Pickup() method to process the item
                        item.Pickup();
                        print("PICKUPS!>>>>...");
                    }
                    
             
                }
                
            }
            else
            {
                Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * pickupRange, Color.red, 2f);
                Debug.Log("No weapon or Pickables -> detected within range.");
            }
        }
    }

    private void TryPickupWeapon(GameObject weapon, List<GameObject> weaponSlots)
    {
        bool slotFound = false;

        if (currentWeapon)
        {
            if (currentWeapon == weapon)
            {
                return; // enven thoug it collidig it prevent repicking or drop !...
            }
        }

        // Check for an empty slot
        foreach (var slot in weaponSlots)
        {
            if (slot.transform.childCount == 0) // Slot is empty
            {
                EquipWeapon(weapon, slot);
                slotFound = true;
                break;
                
            }
        }

        // If no empty slot, drop the current weapon and pick the new one
        if (!slotFound)
        {
            // Drop the current weapon
            DropWeapon();

            // Equip the new weapon in the first available slot
            foreach (var slot in weaponSlots)
            {
                if (slot.transform.childCount == 0) // Slot is now empty after dropping
                {
                    EquipWeapon(weapon, slot);
                    break;
                }
            }
        }
    }

    private void EquipWeapon(GameObject weapon, GameObject slot)
    {
        // If there's already a weapon equipped, disable its script
        if (currentWeapon != null)
        {
            if (currentWeapon == weapon) 
            {
                EnableWeaponScript(currentWeapon) ;
                return; // prevent to don't konw , but it will be use full !...
            }
            DisableWeaponScript(currentWeapon);
        }
        

        // Store the original scale
        Vector3 originalScale = weapon.transform.localScale;

        print(" before " + originalScale );

        // Set the new weapon as the current weapon
        currentWeapon = weapon;
        currentWeapon.transform.SetParent(slot.transform, false);
        currentWeapon.transform.localPosition = Vector3.zero;
        currentWeapon.transform.localRotation = Quaternion.identity;

        // Restore the original scale
        //currentWeapon.transform.localScale = originalScale;
        print(" After " + originalScale );

        // Enable the weapon's script
        EnableWeaponScript(currentWeapon);
        isPicked = true;

        ChangeDropOrPick(currentWeapon,true);

    }

    private void HandleWeaponDrop()
    {
        if (Input.GetKeyDown(KeyCode.X) && isPicked)
        {
            DropWeapon();
        }
    }

    private void DropWeapon()
    {
        if (currentWeapon != null)
        {
            Debug.Log("Dropping weapon: " + currentWeapon.name);
            DisableWeaponScript(currentWeapon);

            // Store the original scale
            Vector3 originalScale = currentWeapon.transform.localScale;

            print(" Before " + originalScale );

            currentWeapon.transform.SetParent(null, false);
            currentWeapon.transform.position = transform.position + transform.forward * 1.5f;

            // Restore the original scale
            //currentWeapon.transform.localScale = originalScale;
            print(" After " + originalScale );
            
            ChangeDropOrPick(currentWeapon,false);

            currentWeapon = null;
            isPicked = false;
            
        }
    }

    private void HandleWeaponSwitch()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SwitchToNextAvailableWeapon(gunSlots);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SwitchToNextAvailableWeapon(pistolSlots);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SwitchToNextAvailableWeapon(meleeSlots);
        }
    }

    private void SwitchToNextAvailableWeapon(List<GameObject> slots)
    {   
        
        if(currentWeapon)
        {
            DisableWeaponScript(currentWeapon);
            currentWeapon = null;
        }

        foreach (GameObject slot in slots)
        {


            if (slot.transform.childCount > 0)
            {
                GameObject weapon = slot.transform.GetChild(0).gameObject;
                Debug.LogWarning(" SWith is happen !! but ??? " + weapon.name );
                Debug.LogWarning(" AAAAAAAAAAAA ");

                if(currentWeapon)break;

                currentWeapon = weapon;
                isPicked = true;

                EnableWeaponScript(currentWeapon);
                

                // if (!weapon.activeInHierarchy)
                // {
                //     EquipWeapon(weapon, slot);
                //     Debug.LogWarning(" AAAAAAAAAAAA ");
                //     break;
                // }
            }
        }
    }

    private void EnableWeaponScript(GameObject weapon)
    {
        var gunScript = weapon.GetComponent<Gun>();
        if (gunScript != null)
        {
            gunScript.enabled = true;
            Collider coll = weapon.GetComponent<Collider>();
            coll.enabled = false;
            gunScript.isPicked = true;
        }

        // Enable scripts for other weapon types if needed
        // For example:
        // var meleeScript = weapon.GetComponent<Melee>();
        // if (meleeScript != null)
        // {
        //     meleeScript.enabled = true;
        // }
    }

    private void DisableWeaponScript(GameObject weapon)
    {
        var gunScript = weapon.GetComponent<Gun>();
        if (gunScript != null)
        {
            gunScript.enabled = false;
            Collider coll = weapon.GetComponent<Collider>();
            coll.enabled = true;         
            gunScript.isPicked = true;
   
        }

        // Disable scripts for other weapon types if needed
        // For example:
        // var meleeScript = weapon.GetComponent<Melee>();
        // if (meleeScript != null)
        // {
        //     meleeScript.enabled = false;
        // }
    }

    #endregion


    private void ChangeDropOrPick(GameObject CurrentWeapon , bool SetState)
    {
        Rigidbody getRB = CurrentWeapon.GetComponent<Rigidbody>();
        if (getRB)
        {
            if(SetState) getRB.isKinematic = true; else getRB.isKinematic = false;
        }
    }



}
