using UnityEngine;
using UnityEngine.UI;

public class ImageFillMixer : MonoBehaviour
{
    [Header("Image Settings")]
    public Image targetImage;
    
    [Header("Mixing Settings")]
    [Range(0.1f, 10f)]
    public float mixSpeed = 1f;
    [Range(0.01f, 0.5f)]
    public float fillInterval = 0.1f;
    
    private bool increasing = true;
    private float currentFill = 0f;
    private float timer = 0f;
    
    void Start()
    {
        if (targetImage == null)
        {
            targetImage = GetComponent<Image>();
        }
        
        // 确保Image类型为Filled
        if (targetImage != null)
        {
            targetImage.type = Image.Type.Filled;
            targetImage.fillAmount = currentFill;
        }
    }
    
    void Update()
    {
        if (targetImage == null) return;
        
        // 使用计时器控制间隔变化
        timer += Time.deltaTime * mixSpeed;
        
        if (timer >= fillInterval)
        {
            // 按照固定间隔变化
            if (increasing)
            {
                currentFill += fillInterval;
                if (currentFill >= 1f)
                {
                    currentFill = 1f;
                    increasing = false;
                }
            }
            else
            {
                currentFill -= fillInterval;
                if (currentFill <= 0f)
                {
                    currentFill = 0f;
                    increasing = true;
                }
            }
            
            // 应用填充值到Image
            targetImage.fillAmount = currentFill;
            
            // 重置计时器
            timer = 0f;
        }
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
    
    public void SetFillInterval(float interval)
    {
        fillInterval = Mathf.Clamp(interval, 0.01f, 0.5f);
    }
    
    public void SetFillMethod(Image.FillMethod method)
    {
        if (targetImage != null)
        {
            targetImage.fillMethod = method;
        }
    }
}