using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MongoDB.Driver;
using MongoDB.Bson;
public class ListCollectionNames : MonoBehaviour
{
    Connection conn;
    private void Awake()
    {
        conn = new Connection("scoreDB", "scoreCollection");
    }
    async void Start()
    {
        List<string> collections = await conn.db.ListCollectionNames().ToListAsync();
        foreach (string coll in collections)
            print(coll);
        //print(string.Join(",", collections));
    }
}