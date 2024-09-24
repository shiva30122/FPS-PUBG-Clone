using UnityEngine;
using UnityEngine.UI;

public class UiWeapon : MonoBehaviour
{
    public string weaponName;            // Name of the weapon
    public int maxAmmo;                  // Maximum ammo capacity
    public int magAmmo;                  // Ammo in the magazine
    public int currentAmmo;              // Current ammo count
    public bool isHealthBar;             // If true, the weapon has a durability bar instead of ammo

    [Header("UI References")]
    public Text ammoText;                // Reference to the UI Text displaying ammo
    public Slider durabilitySlider;      // Slider used for health/durability

    private void Start()
    {
        UpdateUI();
    }

    // Function to update ammo in the UI
    public void UpdateAmmo(int newMagAmmo, int newCurrentAmmo)
    {
        magAmmo = newMagAmmo;
        currentAmmo = newCurrentAmmo;
        UpdateUI();
    }

    // Function to update durability for melee or magical weapons
    public void UpdateDurability(int durability)
    {
        if (isHealthBar && durabilitySlider != null)
        {
            durabilitySlider.value = durability;
        }
    }

    // Function to update the UI text or slider
    private void UpdateUI()
    {
        if (isHealthBar)
        {
            durabilitySlider.gameObject.SetActive(true);
            ammoText.gameObject.SetActive(false);
        }
        else
        {
            ammoText.gameObject.SetActive(true);
            ammoText.text = magAmmo + " / " + currentAmmo;
            durabilitySlider.gameObject.SetActive(false);
        }
    }
}
