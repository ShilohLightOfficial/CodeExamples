using System.Collections;
using UnityEngine;
using UnityEngine.UI;

// From a different project of mine, not part of the Doer/IUISpawnable system next to it.
// Every screen needed the same enter/exit animation, so I pulled it into one base class
// instead of copy-pasting the coroutine onto every panel.
public class UICanvas : MonoBehaviour
{
    static readonly int ExitHash = Animator.StringToHash("Exit");
    static readonly int EnterHash = Animator.StringToHash("Enter");

    Animator[] animators;
    private void Awake()
    {
        animators = GetComponentsInChildren<Animator>();
        OnAwake();
    }

    protected virtual void OnAwake()
    {

    }

    public virtual void OnAfterSpawned()
    {

    }
    public virtual void Enter()
    {
        StartCoroutine(EnterRoutine());
    }

    public IEnumerator EnterRoutine()
    {
        foreach (Animator anm in animators) anm.SetBool(ExitHash, false);
        foreach (Animator anm in animators) anm.SetBool(EnterHash, true);

        yield return new WaitForSeconds(0.3f);
    }

    public virtual void Close()
    {
        StartCoroutine(CloseRoutine());
    }

    public IEnumerator CloseRoutine()
    {
        foreach (Animator anm in animators) anm.SetBool(ExitHash, true);
        foreach (Animator anm in animators) anm.SetBool(EnterHash, false);

        yield return new WaitForSeconds(1.5f);
        Destroy(gameObject);
    }

    public void SetAllBtnsEnabledStatusTo(bool isEnabled)
    {
        Button[] btns = GetComponentsInChildren<Button>();
        foreach (Button btn in btns) btn.enabled = isEnabled;
    }
}
