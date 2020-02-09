using Moon.Asyncs;
using UnityEngine;

public class MemoryAnimations : MonoBehaviour
{
    [SerializeField] private Animator cameraAnimator;
    public AsyncState StartCutScene()
    {
        return Planner.Chain()
                .AddTimeout(1f)
                .AddAwait(IsIntroCutSceneFinished)
            ;
    }

    public AsyncState EndCutScene()
    {
        return Planner.Chain()
                .AddAction(() => cameraAnimator.SetTrigger("CameraEnd"))
                .AddAwait(IsOutroCutSceneFinished)
            ;
    }

    private void IsIntroCutSceneFinished(AsyncStateInfo state)
    {
        state.IsComplete = cameraAnimator.GetCurrentAnimatorStateInfo(0).IsName("Camera_Start")
            && cameraAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f;
    }

    private void IsOutroCutSceneFinished(AsyncStateInfo state)
    {
        state.IsComplete = cameraAnimator.GetCurrentAnimatorStateInfo(0).IsName("Camera_End")
            && cameraAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f;
    }
}