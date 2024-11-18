using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalReferences : MonoBehaviour
{
    public static GlobalReferences Instance { get; set; }
    public GameObject bulletImpactStoneEffect;
    public GameObject bulletImpactSandEffect;
    public GameObject bulletImpactMetalEffect;
    public GameObject bulletImpactFleshSmallEffect;
    public GameObject bloodSplatterEffectPrefab;
    public GameObject ignitionFlame;
    public GameObject explosionEffect;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
    }
}