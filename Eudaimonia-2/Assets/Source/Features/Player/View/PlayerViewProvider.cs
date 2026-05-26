using UnityEngine;

public class PlayerViewProvider : MonoBehaviour
{
    [SerializeField] private float maxCheckDistance = 6f;

    [SerializeField] private LayerMask obstacleLayers;

    public RaycastHit CurrentHit { get; private set; }
    public bool IsHittingSomething { get; private set; }
    public Vector3 LookDirection => startRay.forward;
    private Transform startRay => gameObject.transform;

    public bool IsLookingAtLayer(LayerMask targetLayer, float maxDistance)
    {
        Ray ray = new Ray(startRay.position, startRay.forward);

        LayerMask combinedMask = targetLayer | obstacleLayers;


        if (Physics.Raycast(ray, out RaycastHit hit, maxDistance, combinedMask))
        {
            if ((targetLayer.value & (1 << hit.collider.gameObject.layer)) != 0)
            {
                CurrentHit = hit;
                IsHittingSomething = true;
                return true;
            }
        }

        IsHittingSomething = false;
        return false;
    }

    public bool IsLookingAtLayer(LayerMask targetLayer)
    {
        return IsLookingAtLayer(targetLayer, maxCheckDistance);
    }
}
