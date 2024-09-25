using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OSTPlayer : MonoBehaviour
{

    AudioSource source;
    // Start is called before the first frame update
    void Start()
    {
        source = gameObject.AddComponent<AudioSource>();

        source.PlayOneShot((AudioClip)Resources.Load("MGS3"));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
