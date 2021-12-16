using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mineExplode : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject explosionPrefab;
    GameObject explosionInstance;
    public AudioClip boomSound;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision other) {
        if (other.gameObject.tag == "Player" || other.gameObject.tag == "8Ball" || other.gameObject.tag == "fakePlayer") {
            explode();
        }
    }

    public void explode() {
        float power = 1000;
        float radius = 3 * (float)globalVariables.levelScale;
        Vector3 explosionPos = transform.position;
        Collider[] colliders = Physics.OverlapSphere(explosionPos, radius);
        foreach (Collider hit in colliders)
        {
            Rigidbody rb = hit.GetComponent<Rigidbody>();

            if (rb != null)
                rb.AddExplosionForce(power * rb.mass, explosionPos, radius, 0f);
        }
        explosionInstance = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        //audioSource.Play();

        // 5 TIMES AS LOUD
        AudioSource.PlayClipAtPoint(boomSound, transform.position);
        AudioSource.PlayClipAtPoint(boomSound, transform.position);
        AudioSource.PlayClipAtPoint(boomSound, transform.position);
        AudioSource.PlayClipAtPoint(boomSound, transform.position);
        AudioSource.PlayClipAtPoint(boomSound, transform.position);

        Destroy(gameObject);
    }



}
