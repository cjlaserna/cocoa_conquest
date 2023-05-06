using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowIndicator : MonoBehaviour
{
    public enum IndicatorType { Dash, Dodge };
    // Update is called once per frame

    [SerializeField] private GameObject[] uiPrefabs;
    private GameObject indicator;

    public void Show(float destroyDelay, IndicatorType type)
    {
        // Instantiate the object as a child of the current object
        indicator = Instantiate(uiPrefabs[(int)type], transform.position, Quaternion.identity, transform);
        Animator animation = indicator.GetComponent<Animator>();

        animation.SetFloat("speed", 1 / destroyDelay);
        animation.Play("indicatorAnim");

        Destroy(indicator, destroyDelay);
    }
}
