using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
public class A_guns : MonoBehaviour
{
    [Header("Connections")]
    [SerializeField] protected RaycastHit gunRaycastInfo;
    [SerializeField] private Transform RaycastStartPoint;


    [Header("Stats")]
    [SerializeField] protected float gunRange = 100f;
    [SerializeField] protected float gunDamage = 10f;
    [SerializeField] protected float fireRate = 0.5f;
    [SerializeField] protected float bulletSpeed = 50f;
    [SerializeField] protected int gunAmmo = 999;
    [SerializeField] private int maxAmmo;


    [Header("Visuals")]
    [SerializeField] protected ParticleSystem muzzleEffect;
    [SerializeField] protected float muzzleEffectDuration = 0.1f;
    [SerializeField] protected Animator gunAnimator;
    [SerializeField] protected TrailRenderer bulletTrail;
    [SerializeField] protected ParticleSystem impactParticleSystem;
    [SerializeField] protected Transform trailSpawnPoint1;
    [SerializeField] protected Transform trailSpawnPoint2;
    [SerializeField] protected Transform currentTrailSpawnPoint;
    [SerializeField] protected bool isPoint1;

    [Header("hitscan")]
    [SerializeField] LayerMask hitLayers;

   

    // Timer to track when we can shoot again
    protected float nextFireTime;


    //input button
    protected bool attackTrigger;



    public void Start()
    {

    }

    public void Update()
    {
        HandleShooting();
    }



    public void AddAmmo(int amount)
    {
        gunAmmo += amount;
    }

    protected virtual void HandleShooting()
    {
        if (attackTrigger && nextFireTime <= Time.time)
        {
           // shotFeedBack.PlayFeedbacks();
            if (gunAmmo <= 0)
            {
            }
            else
            {

                if (isPoint1)
                {
                    gunAnimator.SetTrigger("Shooting1");
                    RaycastStartPoint = trailSpawnPoint1;
                }
                else
                {
                    gunAnimator.SetTrigger("Shooting2");
                    RaycastStartPoint = trailSpawnPoint2;

                }
                if (HandleHitScan(out gunRaycastInfo))
                {
                    IDamageable damageable = gunRaycastInfo.collider.GetComponent<IDamageable>();

                    if (damageable != null)
                    {
                        damageable.TakeDamage(gunDamage);
                    }
                    StartCoroutine(HandleTrail(gunRaycastInfo));
                    if (isPoint1)
                    {
                        currentTrailSpawnPoint = trailSpawnPoint2;
                        isPoint1 = false;
                    }
                    else if (!isPoint1)
                    {
                        currentTrailSpawnPoint = trailSpawnPoint1;
                        isPoint1 = true;
                    }
                }
                else
                {
                    StartCoroutine(HandleLostTrail());// if we didnt hit anything in the range of the gun 
                    if (isPoint1)
                    {
                        currentTrailSpawnPoint = trailSpawnPoint2;
                        isPoint1 = false;
                    }
                    else if (!isPoint1)
                    {
                        currentTrailSpawnPoint = trailSpawnPoint1;
                        isPoint1 = true;
                    }
                }
                gunAmmo--;
                nextFireTime = Time.time + fireRate;
            }
        }

    }



    // protected virtual IEnumerator Flashmuzzle()
    // {
    //   muzzleEffect.SetActive(true);
    //   yield return new WaitForSeconds(muzzleEffectDuration);
    //   muzzleEffect.SetActive(false);

    //  }


    protected virtual void Recoil()
    {
        gunAnimator.Play("Shoting");

    }


    protected virtual bool HandleHitScan(out RaycastHit hitInfo)
    {

        Debug.DrawRay(RaycastStartPoint.position, RaycastStartPoint.forward * gunRange, Color.red, 2f);// just so we can see the line
        if (Physics.Raycast(RaycastStartPoint.position, RaycastStartPoint.forward, out hitInfo, gunRange, hitLayers))
        {
            return true;
        }
        return false;
    }


    protected virtual IEnumerator HandleTrail(RaycastHit gunRaycasthitInfo)
    {
        TrailRenderer instance = Instantiate(bulletTrail, currentTrailSpawnPoint.position, Quaternion.identity);
        while (Vector3.Distance(instance.transform.position, gunRaycasthitInfo.point) > 0.1f)
        {
            instance.transform.position = Vector3.MoveTowards(
                instance.transform.position,
                gunRaycasthitInfo.point,
                bulletSpeed * Time.deltaTime

                );
            yield return null;// wait for the next frame and redo the while again 

        }
        ParticleSystem instanceofParticleSystem = Instantiate(impactParticleSystem, gunRaycasthitInfo.point, Quaternion.LookRotation(gunRaycastInfo.normal));
        Destroy(instanceofParticleSystem.gameObject, 2f);
        Destroy(instance.gameObject, instance.time);
    }


    protected virtual IEnumerator HandleLostTrail()
    {
        Vector3 longetPointYouCanGetToInRange = RaycastStartPoint.transform.position + (RaycastStartPoint.forward * gunRange);
        TrailRenderer instance = Instantiate(bulletTrail, currentTrailSpawnPoint.position, Quaternion.identity);
        while (Vector3.Distance(instance.transform.position, longetPointYouCanGetToInRange) > 0.1f)
        {
            instance.transform.position = Vector3.MoveTowards(
                instance.transform.position,
                longetPointYouCanGetToInRange,
                bulletSpeed * Time.deltaTime
                );
            yield return null;
        }

        Destroy(instance.gameObject, instance.time);
    }

    public virtual void OnAttack(InputAction.CallbackContext context)
    {

        if (context.started)
        {
            attackTrigger = true;

        }
        else if (context.canceled)
        {
            attackTrigger = false;
        }
    }


    public void OnReload(InputAction.CallbackContext context)
    {
        if (context.started && gunAmmo < maxAmmo)
        {
            gunAnimator.SetTrigger("Reloading");

        }
    }
}
