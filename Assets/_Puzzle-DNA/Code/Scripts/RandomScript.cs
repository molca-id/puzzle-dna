using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RandomScript : MonoBehaviour
{
    public List<GameObject> inactives;

    [ContextMenu("Set Range Text Name")]
    public void SetRangeText()
    {
        int i = 0;
        foreach (var item in inactives)
        {
            Debug.Log(i);
            if (item.transform.GetComponentInChildren<TextMeshProUGUI>() == null) continue;

            string index = item.name.Split(' ')[1];
            item.transform.GetComponentInChildren<TextMeshProUGUI>().gameObject.name = "Range Text " + index;
            i++;
        }
    }
}
