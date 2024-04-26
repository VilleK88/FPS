using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowImpactEffect : MonoBehaviour
{
    [SerializeField] Enemy[] enemies;
    private void Start()
    {
        enemies = FindObjectsOfType<Enemy>();
    }
}