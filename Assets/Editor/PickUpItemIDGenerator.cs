using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ItemPickup))]
public class PickUpItemIDGenerator : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        ItemPickup script = (ItemPickup)target;

        if (GUILayout.Button("Generate ID"))
            script.GenerateID();
    }
}
