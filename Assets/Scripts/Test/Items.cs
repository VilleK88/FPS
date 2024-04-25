using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Items : MonoBehaviour
{
    [SerializeField] TextAsset itemsFile;
    public TestItem[] testItemArray;
    private void OnValidate()
    {
        string[] textLines;
        if(itemsFile != null) // jos k‰ytt‰j‰ on raahannut itemsFile-kohtaan tekstitiedoston
        {
            textLines = itemsFile.text.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            print(textLines);
        }
        else
            textLines = null;
        // k‰yd‰‰n tekstitiedosto l‰pi rivi rivilt‰, ja kutstaan ConvertToTestItem(rivi)
        int linesLength = textLines.Length;
        print(linesLength);
        testItemArray = new TestItem[linesLength];
        for(int i = 0; i < linesLength; i++)
        {
            testItemArray[i] = ConvertToTestItem(textLines[i]);
        }
    }
    TestItem ConvertToTestItem(string row)
    {
        string[] parts = row.Split(","); // esim. CSV (Comma separated values) on usein , tai ;
        return new TestItem
        {
            Name = parts[0], // rivin sis‰ltˆ ensimm‰iseen pilkkuun asti
            Price = int.TryParse(parts[1], out int num) ? num : 0,
            testItemColor = ColorUtility.TryParseHtmlString(parts[2], out Color c)? c:Color.white
        };
    }
}
[Serializable]
public class TestItem
{
    public string Name;
    public int Price;
    public Color testItemColor;
}