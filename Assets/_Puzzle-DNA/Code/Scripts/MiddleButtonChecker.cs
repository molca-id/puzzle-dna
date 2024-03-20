using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MiddleButtonChecker : MonoBehaviour
{
    public GameObject buttonTarget;

    private void Start()
    {
        GetComponent<BoxCollider2D>().size =
            GetComponent<Image>().rectTransform.rect.size;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject == buttonTarget)
        {
            MainMenuHandler.instance.scrollAutomatically = false;
        }
    }
}
