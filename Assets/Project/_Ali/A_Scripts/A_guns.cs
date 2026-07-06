using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class A_guns : MonoBehaviour
{
    [Header("Connections")]
    [SerializeField] protected RaycastHit gunRaycastInfo;
    [SerializeField] private Transform raycastStartPoint; // Set your specific origin point here in the Inspector
    [SerializeField] private Transform playerTransform;   // Drag your main Player GameObject here

    [Header("Stats")]
    [SerializeField] protected float gunRange = 100f;
    [SerializeField] protected float gunDamage = 10f;
    [SerializeField] protected float fireRate = 0.5f;
    [SerializeField] protected float bulletSpeed = 50f;
    [SerializeField] protected int gunAmmo = 999;
    [SerializeField] private int maxAmmo = 30;

    [Header("Visuals")]
    [SerializeField] protected ParticleSystem muzzleEffect;
    [SerializeField] protected float muzzleEffectDuration = 0.1f;
    [SerializeField] protected Animator gunAnimator;
    [SerializeField] protected TrailRenderer bulletTrail;
    [SerializeField] protected ParticleSystem impactParticleSystem;
    [SerializeField] protected Transform trailSpawnPoint1;
    [SerializeField] protected Transform trailSpawnPoint2;
    [SerializeField] protected Transform currentTrailSpawnPoint;
    [SerializeField] protected bool isPoint1 = true;

    [Header("Hitscan")]
    [SerializeField] private LayerMask hitLayers;

    // Timer to track when we can shoot again
    protected float nextFireTime;

    // Input tracking
    protected bool attackTrigger;
    private Vector2 mouseScreenPosition;
    private Camera mainCamera;

    public void Start()
    {
        mainCamera = Camera.main;

        // Default to root parent if playerTransform is unassigned
        if (playerTransform == null)
        {
            playerTransform = transform.root;
        }

        // Only assign default fallback if Raycast Start Point is empty
        if (raycastStartPoint == null)
        {
            currentTrailSpawnPoint = isPoint1 ? trailSpawnPoint1 : trailSpawnPoint2;
            raycastStartPoint = currentTrailSpawnPoint;
        }
    }

    public void Update()
    {
        HandleShooting();
    }

    public void OnMousePosition(InputAction.CallbackContext context)
    {
        mouseScreenPosition = context.ReadValue<Vector2>();
    }

    protected virtual void HandleShooting()
    {
        if (attackTrigger && Time.time >= nextFireTime)
        {
            if (gunAmmo <= 0) return;

            // 1. Rotate player towards mouse instantly before shooting
            RotatePlayerToMouseOrthographic();

            // 2. Fallback safety check if raycastStartPoint was completely forgotten
            if (raycastStartPoint == null)
            {
                raycastStartPoint = isPoint1 ? trailSpawnPoint1 : trailSpawnPoint2;
            }

            // 3. Keep visual alternating barrel trails separated
            currentTrailSpawnPoint = isPoint1 ? trailSpawnPoint1 : trailSpawnPoint2;

            // 4. Trigger animations
            gunAnimator.SetTrigger(isPoint1 ? "Fire1" : "Fire2");

            // 5. Fire Hitscan from your custom point
            if (HandleHitScan(out gunRaycastInfo))
            {
                IDamageable damageable = gunRaycastInfo.collider.GetComponent<IDamageable>();
                if (damageable != null)
                {
                    damageable.TakeDamage(gunDamage);
                }

                StartCoroutine(HandleTrail(gunRaycastInfo));
            }
            else
            {
                StartCoroutine(HandleLostTrail());
            }

            // 6. Housekeeping: Deduct ammo, apply cooldown, swap barrel flag
            gunAmmo--;
            nextFireTime = Time.time + fireRate;
            isPoint1 = !isPoint1;
        }
    }

    private void RotatePlayerToMouseOrthographic()
    {
        if (mainCamera == null || playerTransform == null) return;

        // Orthographic ray generation safely projected onto 3D world plane
        Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(new Vector3(mouseScreenPosition.x, mouseScreenPosition.y, mainCamera.nearClipPlane));
        Ray ray = new Ray(mouseWorldPos, mainCamera.transform.forward);
        Plane playerPlane = new Plane(Vector3.up, playerTransform.position);

        if (playerPlane.Raycast(ray, out float hitDistance))
        {
            Vector3 targetPoint = ray.GetPoint(hitDistance);
            Vector3 lookDirection = targetPoint - playerTransform.position;
            lookDirection.y = 0f; // Lock Y axis to prevent vertical tilting

            if (lookDirection != Vector3.zero)
            {
                playerTransform.rotation = Quaternion.LookRotation(lookDirection);
            }
        }
    }

    public void AddAmmo(int amount)
    {
        gunAmmo = Mathf.Min(gunAmmo + amount, maxAmmo);
    }

    protected virtual void Recoil()
    {
        gunAnimator.Play("Shoting");
    }

    protected virtual bool HandleHitScan(out RaycastHit hitInfo)
    {
        // Debug laser line visible in Unity Scene editor tab
        Debug.DrawRay(raycastStartPoint.position, raycastStartPoint.forward * gunRange, Color.red, 2f);

        return Physics.Raycast(raycastStartPoint.position, raycastStartPoint.forward, out hitInfo, gunRange, hitLayers);
    }

    protected virtual IEnumerator HandleTrail(RaycastHit hitInfo)
    {
        TrailRenderer instance = Instantiate(bulletTrail, currentTrailSpawnPoint.position, Quaternion.identity);

        while (Vector3.Distance(instance.transform.position, hitInfo.point) > 0.1f)
        {
            instance.transform.position = Vector3.MoveTowards(
                instance.transform.position,
                hitInfo.point,
                bulletSpeed * Time.deltaTime
            );
            yield return null;
        }

        if (impactParticleSystem != null)
        {
            ParticleSystem impactInstance = Instantiate(impactParticleSystem, hitInfo.point, Quaternion.LookRotation(hitInfo.normal));
            Destroy(impactInstance.gameObject, 2f);
        }

        Destroy(instance.gameObject, instance.time);
    }

    protected virtual IEnumerator HandleLostTrail()
    {
        Vector3 origin = currentTrailSpawnPoint.position;
        Vector3 targetDestination = raycastStartPoint.position + (raycastStartPoint.forward * gunRange);
        TrailRenderer instance = Instantiate(bulletTrail, origin, Quaternion.identity);

        while (Vector3.Distance(instance.transform.position, targetDestination) > 0.1f)
        {
            instance.transform.position = Vector3.MoveTowards(
                instance.transform.position,
                targetDestination,
                bulletSpeed * Time.deltaTime
            );
            yield return null;
        }

        Destroy(instance.gameObject, instance.time);
    }

    public virtual void OnAttack(InputAction.CallbackContext context)
    {
        if (context.started)
            attackTrigger = true;
        else if (context.canceled)
            attackTrigger = false;
    }

    public void OnReload(InputAction.CallbackContext context)
    {
        if (context.started && gunAmmo < maxAmmo)
        {
            gunAnimator.SetTrigger("Reloading");
        }
    }
}