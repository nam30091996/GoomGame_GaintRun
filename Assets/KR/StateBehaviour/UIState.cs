using System.Collections;
using System.Collections.Generic;
using Arbor;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using System.Threading.Tasks;
using System;

public class UIState : StateBehaviour
{

    [SerializeField]
    private GameObject m_uiRoot;

    [SerializeField]
    public Button m_nextStateBtn;


    private CanvasGroup m_canvasGroup;


    [SerializeField]
    protected StateLink m_nextState;

    async UniTask Fade(float from, float to, float duration = 1f, Action endCallback = null)
    {
        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime / duration;
            m_canvasGroup.alpha = Mathf.Lerp(from, to, t);
            await UniTask.WaitForEndOfFrame();
            // yield return new WaitForEndOfFrame();
        }
        endCallback?.Invoke();
    }

    public override void OnStateAwake()
    {
        m_canvasGroup = m_uiRoot.AddComponent<CanvasGroup>();
        m_nextStateBtn.onClick.AddListener(()=> Transition(m_nextState));
    }
    public override void OnStateBegin()
    {
        In();
    }

    public override void OnStateEnd()
    {
        Out();
    }
    public void In()
    {
        m_uiRoot.SetActive(true);
        Fade(0, 1f).Forget();
    }
    public void Out()
    {
        Fade(1, 0f, 0.1f, () =>
        {
            m_uiRoot.SetActive(false);

        }).Forget();
    }
}
