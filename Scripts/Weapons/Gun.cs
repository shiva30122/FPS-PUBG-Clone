using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public enum GunType
    {
        Automatic,
        Burst,
        Shotgun
    }

    [Header("Common Gun Settings")]
    public GunType gunType;

    public string WeaponName;
    public int maxAmmo = 30;
    public string InventoryTagAccess;
    public int currentAmmo;
    public int reloadMagAmmo = 10;
    public float reloadTime = 2f;
    public float shootingDelay = 0.5f;
    public float ReloadAfterDelay= 0.2f;
    public float damageAmount = 10f;
    public bool isPicked = false;
    public bool canShootWhileReloading = true;
    public bool isReloading = false;
    public bool canShoot = true;
    public bool IsShooting;

    private Coroutine reloadCoroutine;
    private Coroutine shootCoroutine;

    [Header("Automatic Shoot Settings")]
    public bool AutomaticAutoShoot = true;

    [Header("Burst & Shotgun Settings")]
    public int shotsPerBurst = 3;
    public float burstShotDelay = 0.1f;
    public float OneByOneMultiSpawnTime = 0; // Control delay between sequential shots
    public bool UseAmmoOncePerShot = false; // Use only 1 ammo for all shots
    public bool MultiPoint = false; // Use multiple points for shooting

    [Header("Bullet Settings")]
    public GameObject bulletPrefab;
    public float bulletSpeed = 20f;
    public float bulletDestroyTime = 2f;
    public float gravity = 8.2f;
    public bool BulletGravity;
    public List<Transform> bulletSpawnPoints; // For shotgun and multi-point burst

    [Header("Sound Settings")]
    public AudioSource weaponSound;
    public AudioClip fireSound;
    public AudioClip reloadSound;
    public AudioClip weaponChangeSound;

    [Header("UI Settings")]
    public UiWeapon uiWeapon;


    private void Start()
    {
        if (weaponSound == null)
        {
            Debug.LogWarning("AudioSource not found! Please add an AudioSource component.");
        }

        if (string.IsNullOrEmpty(InventoryTagAccess))
        {
            Debug.LogWarning("Ammo tag is not set!");
        }

    }

    private void Update()
    {
        maxAmmo = InventoryManager.Instance.GetCurrentAmountByTag(InventoryTagAccess);

        canShoot = !isReloading && currentAmmo > 0;

        if (isReloading && Input.GetButtonDown("Fire1"))
        {
            if (currentAmmo > 0)
            {
                StopReloading();
                StartShooting();
            }
            return;
        }

        if (Input.GetKeyDown(KeyCode.R) && !isReloading && currentAmmo < reloadMagAmmo )
        {
            reloadCoroutine = StartCoroutine(Reload());
        }

        if(Input.GetButtonDown("Fire1") && !AutomaticAutoShoot)
        {
            if (canShoot && !IsShooting)
            {
                StartShooting();
            }
        }
        else if (Input.GetButton("Fire1") && AutomaticAutoShoot)
        {
            if (canShoot && !IsShooting)
            {
                StartShooting();
            }
        }
        else if (Input.GetButtonUp("Fire1"))
        {
            if(IsShooting) return;
            StopShooting();
        }

        if (currentAmmo <= 0 && maxAmmo > 0 && !isReloading)
        {
            if (Input.GetButton("Fire1"))
            {
                StopShooting();
                reloadCoroutine = StartCoroutine(Reload());
            }
            else if (!isReloading)
            {
                reloadCoroutine = StartCoroutine(Reload());
            }
        }
    }

    private void StartShooting()
    {
        if (IsShooting) return;

        if (shootCoroutine != null)
        {
            StopCoroutine(shootCoroutine);
        }

        shootCoroutine = StartCoroutine(Shoot());
    }

    private void StopShooting()
    {
        if (shootCoroutine != null)
        {
            StopCoroutine(shootCoroutine);
            shootCoroutine = null;
        }

        IsShooting = false; // Ensure IsShooting is reset
        canShoot = true; // Reset canShoot to true after stopping shooting
    }

    private IEnumerator Shoot()
    {
        if (IsShooting) yield break; // Avoid multiple shoots
        canShoot = false;
        IsShooting = true;

        PlaySound(fireSound);

        // Add while loop here for continuous shooting logic
        if ( AutomaticAutoShoot )
        {
            while (Input.GetButton("Fire1"))
            {
                if (isReloading || currentAmmo <= 0)
                {
                    StopShooting();
                    yield break;
                }

                if (gunType == GunType.Burst)
                {
                    // Fire burst continuously while Fire1 is pressed
                    yield return StartCoroutine(FireBurst());
                
                }
                else if (gunType == GunType.Shotgun)
                {
                    // Fire shotgun continuously while Fire1 is pressed
                    yield return StartCoroutine(FireShotgun());
                    
                }
                else // Automatic
                {
                    FireSingleShot(bulletSpawnPoints[0].position, bulletSpawnPoints[0].rotation);
                    currentAmmo--;
                    uiWeapon?.UpdateAmmo(currentAmmo, maxAmmo);
                    // yield return new WaitForSeconds(shootingDelay);
                }

                yield return new WaitForSeconds(shootingDelay); // Add delay between shots
            }
        }
        else // ADDED WITHOUT CHAT_GPT !... it so simple to chage and i made in 10 minutes if GHAT_GPT then i cant finesh this logic !...
        {
            if(Input.GetButtonDown("Fire1"))// && !BusterAutomatic || !ShortGunAutomatic || !AutomaticAutoShoot) removed !...
            {
                if (isReloading || currentAmmo <= 0)
                {
                    StopShooting();
                    yield break;
                }

                if (gunType == GunType.Burst)
                {
                    // Fire burst continuously while Fire1 is pressed
                    yield return StartCoroutine(FireBurst());
                    
                }
                else if (gunType == GunType.Shotgun)
                {  
                    // Fire shotgun continuously while Fire1 is pressed 
                    yield return StartCoroutine(FireShotgun());
                    
                }
                else // Automatic
                {
                    FireSingleShot(bulletSpawnPoints[0].position, bulletSpawnPoints[0].rotation);
                    currentAmmo--;
                    uiWeapon?.UpdateAmmo(currentAmmo, maxAmmo);
                    // yield return new WaitForSeconds(shootingDelay);
                }

                yield return new WaitForSeconds(shootingDelay); // Add delay between shots
            }
        }
        canShoot = true;
        IsShooting = false; // Ensure shooting state is reset
    }

    private IEnumerator FireBurst()
    {

        bool ammoUsed = false;

        
        if (MultiPoint)
        {
            int currentIndex = 0;

            for (int i = 0; i < shotsPerBurst; i++)
            {
                if (currentAmmo <= 0) yield break;

                FireSingleShot(bulletSpawnPoints[currentIndex].position, bulletSpawnPoints[currentIndex].rotation);

                if (!UseAmmoOncePerShot)
                {
                    currentAmmo--;
                    uiWeapon?.UpdateAmmo(currentAmmo, maxAmmo);
                }
                else if (!ammoUsed)
                {
                    ammoUsed = true;
                }

                currentIndex = (currentIndex + 1) % bulletSpawnPoints.Count;

                if (OneByOneMultiSpawnTime > 0)
                {
                    yield return new WaitForSeconds(OneByOneMultiSpawnTime);
                }
            }

            if (UseAmmoOncePerShot && ammoUsed)
            {
                currentAmmo--;
                uiWeapon?.UpdateAmmo(currentAmmo, maxAmmo);
            }
        }
        else
        {
            for (int i = 0; i < shotsPerBurst; i++)
            {
                if (currentAmmo <= 0) yield break;

                FireSingleShot(bulletSpawnPoints[0].position, bulletSpawnPoints[0].rotation);

                if (!UseAmmoOncePerShot)
                {
                    currentAmmo--;
                    uiWeapon?.UpdateAmmo(currentAmmo, maxAmmo);
                }
                else if (!ammoUsed)
                {
                    ammoUsed = true;
                }

                yield return new WaitForSeconds(burstShotDelay);
            }

            if (UseAmmoOncePerShot && ammoUsed)
            {
                currentAmmo--;
                uiWeapon?.UpdateAmmo(currentAmmo, maxAmmo);
            }
        }
    }

    private IEnumerator FireShotgun()
    {

        bool ammoUsed = false;

        if (MultiPoint)
        {
            foreach (Transform spawnPoint in bulletSpawnPoints)
            {
                if (currentAmmo <= 0) yield break;

                FireSingleShot(spawnPoint.position, spawnPoint.rotation);

                if (!UseAmmoOncePerShot)
                {
                    currentAmmo--;
                    uiWeapon?.UpdateAmmo(currentAmmo, maxAmmo);
                }
                else if (!ammoUsed)
                {
                    ammoUsed = true;
                }

                if (OneByOneMultiSpawnTime > 0)
                {
                    yield return new WaitForSeconds(OneByOneMultiSpawnTime);
                }
            }

            if (UseAmmoOncePerShot && ammoUsed)
            {
                currentAmmo--;
                uiWeapon?.UpdateAmmo(currentAmmo, maxAmmo);
            }
        }
        else
        {
            for (int i = 0; i < bulletSpawnPoints.Count; i++)
            {
                if (currentAmmo <= 0) yield break;

                FireSingleShot(bulletSpawnPoints[i].position, bulletSpawnPoints[i].rotation);

                if (!UseAmmoOncePerShot)
                {
                    currentAmmo--;
                    uiWeapon?.UpdateAmmo(currentAmmo, maxAmmo);
                }
                else if (!ammoUsed)
                {
                    ammoUsed = true;
                }

                yield return new WaitForSeconds(burstShotDelay);
            }

            if (UseAmmoOncePerShot && ammoUsed)
            {
                currentAmmo--;
                uiWeapon?.UpdateAmmo(currentAmmo, maxAmmo);
            }
        }
    }

    private void FireSingleShot(Vector3 position, Quaternion rotation) // bulltet shoot !...
    {
        GameObject bullet = Instantiate(bulletPrefab, position, rotation);
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        if (rb != null)
        {
            Bullet bulletsetting = bullet.GetOrAddComponent<Bullet>();
            bulletsetting.applyGravity = BulletGravity;
            bulletsetting.gravity = gravity;
            bulletsetting.speed = bulletSpeed;
            bulletsetting.lifetime = bulletDestroyTime;
            bulletsetting.damageAmount = damageAmount;
            //rb.transform.rotation = Quaternion.Euler(0, 0, 0); // Face backward
            rb.AddForce(rotation * Vector3.forward * bulletSpeed, ForceMode.VelocityChange);
            Destroy(bullet, bulletDestroyTime);
        }
    }

private IEnumerator Reload()
{

    if (maxAmmo < 0)
    {
        yield break;
    }

    if (isReloading) 
    {
        yield break;
    }

    isReloading = true;

    Debug.Log("Reloading...");

    if (weaponSound != null && reloadSound != null)
    {
        weaponSound.PlayOneShot(reloadSound);
    }

    // Initial wait before starting the reload process
    yield return new WaitForSeconds(reloadTime);

    // Check if there's ammo to reload
    if (maxAmmo > 0)
    {
        // Get current available ammo from the inventory
        int availableAmmo = InventoryManager.Instance.GetCurrentAmountByTag(InventoryTagAccess);

        // Calculate how much ammo can be reloaded
        int availableToReload = reloadMagAmmo - currentAmmo;
        int ammoToReload = Mathf.Min(availableToReload, availableAmmo, maxAmmo);

    
        currentAmmo += ammoToReload;
        maxAmmo -= ammoToReload;

        // Use ammo from inventory
        InventoryManager.Instance.UseCurrentAmountByTag(InventoryTagAccess, ammoToReload);

        // Update UI
        uiWeapon?.UpdateAmmo(currentAmmo, maxAmmo);
        Debug.Log($"Reloaded {ammoToReload} ammo.");
    }
    else
    {
        Debug.Log("No ammo available to reload!");
    }
    

    isReloading = false;
    Debug.Log("Reloading stopped !..");

    // Allow shooting if the fire button is pressed
    if (Input.GetButton("Fire1") && canShoot && currentAmmo > 0)
    {
        StartShooting();
    }
}


    private void StopReloading()
    {
        if (reloadCoroutine != null)
        {
            StopCoroutine(reloadCoroutine);
            reloadCoroutine = null;
        }
        isReloading = false;
    }

    private void PlaySound(AudioClip clip)
    {
        if (weaponSound && clip)
        {
            weaponSound.clip = clip;
            weaponSound.Play();
        }
    }
}
