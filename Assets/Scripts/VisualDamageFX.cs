using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class VisualDamageFX : MonoBehaviour
{
    [SerializeField] ParticleSystem smoke;
    [SerializeField] ParticleSystem fire;
    [SerializeField] Transform[] vfxPositions; 

    LocationToDefend locationScript;
    int maxSmoke = 4;
    int smokeCount = 0; 
    int maxFire = 4;
    int fireCount = 0; 

    void Start()
    {
        locationScript = FindObjectOfType<LocationToDefend>();
        if (locationScript == null)
            Debug.Log("LOCATION SCRIPT NOT FOUND"); 
    }

    void Update()
    {
        if (locationScript.GetLocationHealth() <= 25)
        {
            StartVfxInRandomPosition(fire); 
        }
        else if (locationScript.GetLocationHealth() <= 50)
        {
            StartVfxInRandomPosition(smoke);
        }

    }

    void StartVfxInRandomPosition(ParticleSystem vfx)
    {
        int randomIndex = Random.Range(0, vfxPositions.Length);
        Vector3 randPos = vfxPositions[randomIndex].position;

        if (vfx == smoke && smokeCount < maxSmoke)
        {
            ParticleSystem newSmoke = Instantiate(vfx, randPos, gameObject.transform.rotation);
            smokeCount++; 
        }
        else if (vfx == fire && fireCount < maxFire)
        {
            Instantiate(vfx, randPos, gameObject.transform.rotation);
            fireCount++;
        }
    }
}
