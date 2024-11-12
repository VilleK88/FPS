using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class IgnitionFlame : MonoBehaviour
{
    public ParticleSystem ps;
    private void Start()
    {
        ps = GetComponent<ParticleSystem>();
        ps.Play();
    }
}