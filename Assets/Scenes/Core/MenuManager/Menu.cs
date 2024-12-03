using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// Add the IAnimationCompleted interface if  you use animator and want to SetActive(fals)
public abstract class Menu : MonoBehaviour // IAnimationCompleted
{
    public MenuClassifier menuClassifier;

    public enum StartingMenuStates
    {
        Ignore,
        Active,
        Disable
    }
    public StartingMenuStates startMenuState = StartingMenuStates.Active;
    public bool resetPosition = true;

    public UnityEvent OnRefreshMenu = new UnityEvent();

    private Animator animator;
    private bool isOpen = false;
    public bool IsOpen {
        get {
            if (animator != null) {
                isOpen = animator.GetBool("IsOpen");
            }
            return isOpen;
        }
        set {
            isOpen = value;
            if (animator != null) {
                if (isOpen == true) {
                    gameObject.SetActive(true);
                }
                animator.SetBool("IsOpen", value);
            } else {
                gameObject.SetActive(value);
            }

            if (value == true) {
                OnRefreshMenu.Invoke();
            }
        }
    }

    public virtual void AnimationCompleted(int animationName) {
        if (isOpen == false) {
            gameObject.SetActive(isOpen);
        }
    }

    public virtual void OnShowMenu(string options = "") {
        IsOpen = true;
    }

    public virtual void OnHideMenu(string options = "") {
        IsOpen = false;
    }

    protected virtual void Awake() {
        if (resetPosition == true) {
            var rectTransform = GetComponent<RectTransform>();
            rectTransform.localPosition = Vector3.zero;
        }
    }

    protected virtual void Start() {
        animator = GetComponent<Animator>();

        if (animator != null) {
            animator.SetBool("IsOpen", IsOpen);
        }

        MenuManagerCore.Instance.AddMenu(this);

        switch (startMenuState) {
            case StartingMenuStates.Active:
                isOpen = true;
                gameObject.SetActive(true);
                break;
            case StartingMenuStates.Disable:
                isOpen = false;
                gameObject.SetActive(false);
                break;
        }
    }

    //protected virtual void OnDestroy() {
    //    MenuManager.Instance.Remove(menuClassifier.menuName);
    //}
}
