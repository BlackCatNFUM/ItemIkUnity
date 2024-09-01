using UnityEngine;

public class Equip : MonoBehaviour
{
    [Header("PRESS 'F' TO PICKUP | PRESS 'G' TO DROP")]
    [SerializeField] [Range(0.0f, 2.0f)] private float rayLength;
    [SerializeField] private Vector3 rayOffset;
    [SerializeField] private LayerMask weaponMask;
    private RaycastHit topRayHitInfo;
    
    
    
    // -------------------------------------------------------
    // YOU HAVE TO CREATE A VARIABLE FOR EACH "gameObject" That
    // IS GOING TO BE A ITEM/WEAPON PARENT (FOR HOLDING/ATTACHING)
    // -------------------------------------------------------
    [SerializeField] private Transform RifleHolderPosition;
    // -------------------------------------------------------

    
    public static bool isHolding;
    
    
    void Start()
    {
        isHolding = false;
    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            EquipFunction();
        }

        if (Input.GetKeyDown(KeyCode.G) && isHolding)
        {
            DropItem();
        }
        RayCastHandler();
    }
    
    
    // CHECK/SCAN FOR THE WEAPON/ITEMS BASED ON THE LAYER AND TAGS
    private void RayCastHandler()
    {
        Ray topRay = new Ray(transform.position + rayOffset, transform.forward);
        Debug.DrawRay(transform.position + rayOffset, transform.forward * rayLength, Color.red);
        Physics.Raycast(topRay, out topRayHitInfo, rayLength, weaponMask);
    }

    private void EquipFunction()
    {
        
        // Check the RayCast & if the player is already Holding Something or not
        if (topRayHitInfo.collider != null && !isHolding)
        {
            
            
            // Get the weapon/obj from the raycast hit
            GameObject item = topRayHitInfo.collider.gameObject;
            
            
            
            // Check the Tag
            // ---------------------------------------------------------------------------------------------------
            // YOU HAVE TO CREATE TAGS FOR YOUR ITEMS/WEAPONS AND EQUIP THEM BASED ON THE TAGS
            // BECAUSE IN THE NORMAL, UR 'RIFLE-HOLDER OBJ' CANT BE FOR A PISTOL, BECAUSE THE SIZE AND THE 
            // GRIPS OF THE WEAPONS AND ITEMS ARE DIFFERENT AND THEY WONT ATTACH CORRECTLY TO THE HAND
            // SO IF YOU WANT TO ATTACH A "FLASHLIGHT" YOU HAVE TO CREATE A TAG FOR THAT AND, TWEAK THE POS
            // OF THAT "FLASHLIGHT" IN THE UNITY, THEN CREATE A LOGIC FOR THAT LIKE THE "TwoHandedRifleEquip" Function
            // ---------------------------------------------------------------------------------------------------
            
            
            
            // In this Scenario: the tags will confirm that how the object/weapon should be hold in hands
            if (item.CompareTag("TwoHandedRifle"))
            {
                TwoHandedRifleEquip(item);
            }
        }
    }
        
    void TwoHandedRifleEquip(GameObject weapon)
    {
        // Parent the weapon to the GunEquip position
        weapon.transform.SetParent(RifleHolderPosition);
        weapon.transform.localPosition = Vector3.zero;
        weapon.transform.localRotation = Quaternion.identity;
        
        
        // Freeze the Rigidbody Constraint rotations and transform of weapon
        FreezeAndTrigger(weapon);
        
        isHolding = true;
    }
    
    
    void FreezeAndTrigger(GameObject item)
    {
        // Freeze the Rigidbody Constraint rotations and transform of weapon
        Rigidbody ItemRigidBody = item.GetComponent<Rigidbody>();
        if (ItemRigidBody != null)
        {
            // Freeze all position and rotation constraints on the Rigidbody
            ItemRigidBody.constraints = RigidbodyConstraints.FreezePosition | RigidbodyConstraints.FreezeRotation;

            // Get the Collider component from the weapon
            Collider ItemCollider = item.GetComponent<Collider>();

            if (ItemCollider != null)
            {
                // Enable the isTrigger property on the Collider
                ItemCollider.isTrigger = true;
            }
        }
    }
    
    private void DropItem()
    {
        // Get the current weapon/item being held
        Transform currentItem = RifleHolderPosition.childCount > 0 ? RifleHolderPosition.GetChild(0) : null;

        if (currentItem != null)
        {
            // Remove the item from the holder (unparent it)
            currentItem.SetParent(null);
            
            // Re-enable physics: unfreeze the Rigidbody constraints and disable isTrigger
            Rigidbody itemRigidBody = currentItem.GetComponent<Rigidbody>();
            if (itemRigidBody != null)
            {
                itemRigidBody.constraints = RigidbodyConstraints.None;  // Unfreeze all constraints
                itemRigidBody.isKinematic = false;  // Enable physics if it was previously set to kinematic

                Collider itemCollider = currentItem.GetComponent<Collider>();
                if (itemCollider != null)
                {
                    itemCollider.isTrigger = false;  // Disable the trigger so the item interacts with the environment
                }
            }

            // Drop the item at the player's current position with a little force
            currentItem.position = transform.position + transform.forward;  // Drop it slightly in front of the player
            itemRigidBody.AddForce(transform.forward * 2f, ForceMode.Impulse);  // Apply force to push it forward
            
            // Mark the player as not holding anything
            isHolding = false;
        }
    }
}
