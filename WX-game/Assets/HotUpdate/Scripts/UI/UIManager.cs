using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YooAsset;

public class UIManager : BaseSingleton<UIManager>
{
    // 缓存已加载的面板
    private Dictionary<string, GameObject> panels = new Dictionary<string, GameObject>();
    // 面板层级栈，栈顶为当前可交互的面板
    private Stack<string> panelStack = new Stack<string>();

    // 每次获取当前场景的 Canvas
    private Transform UIRoot
    {
        get
        {
            Canvas canvas = FindObjectOfType<Canvas>();
            return canvas != null ? canvas.transform : transform;
        }
    }

    /// <summary>
    /// 打开面板：压栈，旧顶层失去交互，新面板获得交互
    /// </summary>
    public void ShowPanel(string panelName)
    {
        if (panels.TryGetValue(panelName, out GameObject panel))
        {
            // 旧顶层失去交互
            if (panelStack.Count > 0)
                SetPanelInteractable(panelStack.Peek(), false);

            panel.SetActive(true);
            panelStack = RemoveFromStack(panelStack, panelName);
            panelStack.Push(panelName);
            SetPanelInteractable(panelName, true);
            return;
        }
        StartCoroutine(LoadPanel(panelName));
    }

    private IEnumerator LoadPanel(string panelName)
    {
        AssetHandle handle = GameRoot.Instance.Package.LoadAssetAsync<GameObject>(panelName);
        yield return handle;

        if (handle.AssetObject == null)
        {
            Debug.LogError($"[UIManager] Panel 加载失败: {panelName}");
            yield break;
        }

        GameObject go = handle.InstantiateSync(UIRoot);
        go.name = panelName;
        panels[panelName] = go;

        EnsureCanvasGroup(go);

        // 压栈前把当前顶层（旧面板）设为不可交互
        // （ShowPanel 里设的那次可能因为协程延迟而顺序有误，这里再确认一次）
        if (panelStack.Count > 0)
            SetPanelInteractable(panelStack.Peek(), false);

        panelStack.Push(panelName);
        SetPanelInteractable(panelName, true);
    }

    /// <summary>
    /// 关闭栈顶面板，激活下一层面板的交互
    /// </summary>
    public void CloseTopPanel()
    {
        if (panelStack.Count == 0) return;

        string topName = panelStack.Pop();
        if (panels.TryGetValue(topName, out GameObject top))
            top.SetActive(false);

        // 激活新顶层
        if (panelStack.Count > 0)
        {
            string nextName = panelStack.Peek();
            if (panels.TryGetValue(nextName, out GameObject next))
            {
                next.SetActive(true);
                SetPanelInteractable(nextName, true);
            }
        }
    }

    /// <summary>
    /// 关闭指定面板：若是栈顶则同 CloseTopPanel，否则从栈中移除
    /// </summary>
    public void HidePanel(string panelName)
    {
        if (panelStack.Count > 0 && panelStack.Peek() == panelName)
        {
            CloseTopPanel();
            return;
        }

        panelStack = RemoveFromStack(panelStack, panelName);
        if (panels.TryGetValue(panelName, out GameObject panel))
            panel.SetActive(false);
    }

    /// <summary>销毁并移除指定 Panel</summary>
    public void RemovePanel(string panelName)
    {
        panelStack = RemoveFromStack(panelStack, panelName);
        if (panels.TryGetValue(panelName, out GameObject panel))
        {
            Destroy(panel);
            panels.Remove(panelName);
        }
        // 激活新顶层
        if (panelStack.Count > 0)
            SetPanelInteractable(panelStack.Peek(), true);
    }

    /// <summary>获取已缓存的 Panel</summary>
    public GameObject GetPanel(string panelName)
    {
        panels.TryGetValue(panelName, out GameObject panel);
        return panel;
    }

    /// <summary>隐藏所有 Panel 并清空栈</summary>
    public void HideAllPanels()
    {
        foreach (var panel in panels.Values)
            panel.SetActive(false);
        panelStack.Clear();
    }

    /// <summary>清除切换场景后的旧缓存（场景切换时调用）</summary>
    public void ClearPanelCache()
    {
        panels.Clear();
        panelStack.Clear();
    }

    // ===================== 内部工具 =====================

    private void EnsureCanvasGroup(GameObject go)
    {
        if (go.GetComponent<CanvasGroup>() == null)
            go.AddComponent<CanvasGroup>();
    }

    private void SetPanelInteractable(string panelName, bool interactable)
    {
        if (!panels.TryGetValue(panelName, out GameObject go)) return;
        EnsureCanvasGroup(go);
        CanvasGroup cg = go.GetComponent<CanvasGroup>();
        cg.interactable = interactable;
        cg.blocksRaycasts = true; // 始终拦截射线，防止点击穿透到下层
    }

    // Stack 不支持直接删除中间元素，重建一个不含目标的栈
    private Stack<string> RemoveFromStack(Stack<string> stack, string target)
    {
        var list = new List<string>(stack);
        list.Reverse(); // Stack 枚举是从顶到底，Reverse 后变底到顶
        list.Remove(target);
        var newStack = new Stack<string>();
        foreach (var item in list)
            newStack.Push(item);
        return newStack;
    }
}

