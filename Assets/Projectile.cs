using UnityEngine;

public class Projectile : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    [SerializeField] private Rigidbody rb;
    [SerializeField] private float projectileSpeed = 40f;
    public ParticleSystem laserImpactParticles;
    void Start()
    {
        //print("PROJECTILE forward:" + transform.forward + "\nROTATION: " + transform.rotation);

        rb.AddForce(transform.forward * projectileSpeed);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        Destroy(this.gameObject);
        //var instantiatedParticle = Instantiate(laserImpactParticles, collision.GetContact(0).point, Quaternion.identity);
        //print(collision.gameObject.name + "\nSPAWNING PARTICLE AT: " + collision.GetContact(0).thisCollider.po);
        //Debug.DrawRay(new Vector3(0,0,0), collision.transform.position, Color.white);
        //instantiatedParticle.Play();
    }
}