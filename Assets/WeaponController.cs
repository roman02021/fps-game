using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponController : MonoBehaviour
{
    private InputAction shootAction;

    [SerializeField] float range = 50f;
    [SerializeField] float damage = 10f;
    [SerializeField] float fireRate = 10f;
    public GameObject projectile;
    public AudioSource audioSource;
    public AudioClip laserShootSound;
    public ParticleSystem laserImpactParticles;

    Coroutine shootingCoroutine;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake() 
    {
        shootAction = InputSystem.actions.FindAction("Attack");


        shootAction.started += _ => StartShooting();
        shootAction.canceled += _ => StopShooting();
    }

    public IEnumerator Shoot()
    {


       

        while (true) 
        {
            Vector3 pos = transform.position;
            Vector3 direction = transform.forward;
            Vector3 spawnPos = pos + direction * 2;
            print("SHOOTED");
            GameObject projectileObject = Instantiate(projectile, spawnPos, transform.rotation);

            audioSource.PlayOneShot(laserShootSound);

            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.forward, out hit, range))
            {

                print("HIT OBJECT: " + hit.collider.name + "\nHIT POS: " + hit.transform.position);
                Instantiate(laserImpactParticles, hit.point + (hit.normal * .01f), Quaternion.FromToRotation(Vector3.up, hit.normal));
            }

            

            yield return new WaitForSeconds(1 / fireRate);
        }

    }

    void StartShooting()
    {
        shootingCoroutine = StartCoroutine(Shoot());
    }

    void StopShooting()
    {
        StopCoroutine(shootingCoroutine);
    }



    void OnEnable()
    {
        shootAction.Enable();
    }

    void OnDisable() 
    {
        shootAction.Disable();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
