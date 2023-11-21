using UnityEngine;
using UnityEngine.Events;

public class AnimationManager : MonoBehaviour
{
        [SerializeField] private Animator animator;
        [SerializeField] private UnityEvent onTrigger = new UnityEvent();
        [SerializeField] private string freezeFrameClipName = "FreezeFrame";
        
        private string _isStateUncancellable = "IsStateUncancellable";
        
        public Animator Animator => animator;

        public bool IsStateUncancellable
        {
                get => animator.GetBool(_isStateUncancellable);
        }

        public void SetTrigger(string trigger)
        {
                if (IsStateUncancellable)
                        return;
                
                ForceTrigger(trigger);
        }


        public void SetTrigger(int id)
        {
                if (IsStateUncancellable)
                        return;
                
                ForceTrigger(id);
        }
        
        public void ForceTrigger(string trigger)
        {
                TriggerEvent();
                ResumeAnimator();
                animator.SetTrigger(trigger);
        }

        public void ForceTrigger(int id)
        {
                TriggerEvent();
                ResumeAnimator();
                animator.SetTrigger(id);
        }
        
        private void ResumeAnimator()
        {
                animator.speed = 1;
                SetBool(_isStateUncancellable, false);
        }

        public void SetBool(string boolName, bool value)
        {
                animator.SetBool(boolName, value);
        }
        
        public void SetBool(int id, bool value)
        {
                animator.SetBool(id, value);
        }
        
        public void SetInteger(string intName, int value)
        {
                animator.SetInteger(intName, value);
        }
        
        public void SetInteger(int id, int value)
        {
                animator.SetInteger(id, value);
        }
        
        public void SetFloat(string floatName, float value)
        {
                animator.SetFloat(name, value);
        }
        
        public void SetFloat(int id, float value)
        {
                animator.SetFloat(id, value);
        }

        public void ResetTrigger(int id)
        {
                animator.ResetTrigger(id);
        }
        
        public void ResetTrigger(string triggerName)
        {
                animator.ResetTrigger(triggerName);
        }

        private void TriggerEvent()
        {
                onTrigger?.Invoke();
        }
}