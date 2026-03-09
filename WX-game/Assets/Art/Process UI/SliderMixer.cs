using UnityEngine;
using UnityEngine.UI;

public class SliderMixer : MonoBehaviour
{
    [Header("Slider Settings")]
    public Slider targetSlider;
    
    [Header("Mixing Settings")]
    [Range(0.1f, 10f)]
    public float mixSpeed = 1f;
    [Range(0.01f, 0.5f)]
    public float valueInterval = 0.1f;
    
    private bool increasing = true;
    private float currentValue = 0f;
    
    void Start()
    {
        if (targetSlider == null)
        {
            targetSlider = GetComponent<Slider>();
        }
        
        // 初始化Slider值
        if (targetSlider != null)
        {
            targetSlider.value = currentValue;
        }
    }
    
    void Update()
    {
        if (targetSlider == null) return;
        
        // 更新当前值
        if (increasing)
        {
            currentValue += mixSpeed * valueInterval * Time.deltaTime;
            if (currentValue >= 1f)
            {
                currentValue = 1f;
                increasing = false;
            }
        }
        else
        {
            currentValue -= mixSpeed * valueInterval * Time.deltaTime;
            if (currentValue <= 0f)
            {
                currentValue = 0f;
                increasing = true;
            }
        }
        
        // 应用值到Slider
        targetSlider.value = currentValue;
    }
    
    // 公共方法用于控制搅拌
    public void StartMixing()
    {
        enabled = true;
    }
    
    public void StopMixing()
    {
        enabled = false;
    }
    
    public void SetMixSpeed(float speed)
    {
        mixSpeed = Mathf.Clamp(speed, 0.1f, 10f);
    }
    
    public void SetValueInterval(float interval)
    {
        valueInterval = Mathf.Clamp(interval, 0.01f, 0.5f);
    }
}