using UnityEngine;

public class MemoryAnimations : MonoBehaviour
{
    [SerializeField] private Animator cameraAnimator;
    
    public Animator CameraAnimator 
    { 
        get 
        {
            if (cameraAnimator == null)
                Debug.LogError("Camera Animator is null");

            return cameraAnimator; 
        } 
    }
}