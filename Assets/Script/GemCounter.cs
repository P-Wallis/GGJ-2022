using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GemCounter : MonoBehaviour
{
    public static GemCounter _;
    private void Awake()
    {
        if (_ == null)
            _ = this;
    }

    public TextMeshProUGUI text;
    private int count;
    public int Count { get { return count; } }

    public void IncrementCount()
    {
        count++;
        UpdateCountText();
    }

    public void DecrementCount()
    {
        count--;
        UpdateCountText();
    }

    public void setCount(int newCount)
    {
        count = newCount;
        UpdateCountText();
    }

    private void UpdateCountText()
    {
        text.text = "Gems Collected: " + count;
    }
}
