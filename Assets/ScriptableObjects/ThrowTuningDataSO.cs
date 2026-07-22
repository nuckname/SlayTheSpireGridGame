using UnityEngine;

[CreateAssetMenu(fileName = "ThrowTuningData", menuName = "Throw Tuning Data")]
public class ThrowTuningDataSO : ScriptableObject
{
    [Header("Throw Tuning Parameters")]
    public float forceMinSpeed = 25f;
    public float forceMaxSpeed = 65f;
    public float totalMinTime = 0.3f;
    public float totalMaxTime = 1.5f;
    [Tooltip("Extra hold time after the minimum draw duration before launch power starts tapering off")]
    public float powerFalloffDelay = 0.4f;
    
    [Header("Position Limits")]
    public float minZ = 10f;
    public float maxZ = 55f;
    
    [Header("Animation Scales")]
    public float pullbackYScale = 0.15f;
    public float pullbackZScale = 0.2f;
    public float oppositeXScale = 2.0f;
    public AnimationCurve swingCurve;
    
    [Header("Tolerances")]
    public float drawTimeTolerance = 0.5f;
    public float drawPositionTolerance = 0.5f;
    public float jitterTolerance = 0.05f;
}