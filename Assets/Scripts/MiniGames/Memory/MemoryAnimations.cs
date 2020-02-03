using Moon.Asyncs;
using UnityEngine;

public class MemoryAnimations : MonoBehaviour
{
    [SerializeField] private Animator cameraAnimator;

    public AsyncState StartCutScene()
    {
        return Planner.Chain()
                .AddAction(() => cameraAnimator.SetTrigger("CameraRotate"))
                .AddAwait(IsCutSceneFinished)
            ;
    }

    private void IsCutSceneFinished(AsyncStateInfo state)
    {
        state.IsComplete = cameraAnimator.GetCurrentAnimatorStateInfo(0).IsName("CameraRotate")
            && cameraAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f;
    }
}