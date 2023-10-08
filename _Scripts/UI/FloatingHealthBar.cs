using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FloatingHealthBar : MonoBehaviour
{
    private Slider slider;
    [SerializeField] Transform target;
    // Start is called before the first frame update
    void Start()
    {
        target = transform.parent.parent;
        if (target==null)
        {
            Debug.Log("Root is null");
        }
        slider = GetComponent<Slider>();
        if (slider == null)
        {
            // Debug.LogError("slider of health bar from "+target.name+ " is null");
        }
        
    }

    public void SetMax(int max)
    {
        if (slider == null)
        {
            // Debug.LogError("slider of health bar from "+target.name+ " is null");
        }
        else 
        {
            slider.maxValue = max;
            slider.value = max;
        }
        
    }
    public void Set(int current)
    {
        slider.value = current;
    }

    // Update is called once per frame
    void Update()
    {
        transform.localScale = target.localScale; 
    }
}
