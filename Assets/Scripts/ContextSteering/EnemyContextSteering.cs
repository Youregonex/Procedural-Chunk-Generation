using System.Collections.Generic;
using UnityEngine;

public class EnemyContextSteering : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private List<SteeringBehaviour> _steeringBehaviours;
    [SerializeField] private List<Detector> _detectors;
    [SerializeField] private AIData _aiData;
    [SerializeField] private float _detectionDelay = .05f;

    private void Start()
    {
        InvokeRepeating("PerformDetection", 0, _detectionDelay);
    }

    private void PerformDetection()
    {
        foreach(Detector detector in _detectors)
        {
            detector.Detect(_aiData);
        }

        float[] danger = new float[8];
        float[] interest = new float[8];

        foreach(SteeringBehaviour steeringBehaviour in _steeringBehaviours)
        {
            (danger, interest) = steeringBehaviour.GetSteering(danger, interest, _aiData);
        }
    }
}
