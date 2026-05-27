using Features.Interactable.Environment;
using Features.Player.Data;
using UnityEngine;
using Zenject;

namespace Features.Player.Interact
{
    public class PlayerPusher : MonoBehaviour
    {
        private MovementSettingsSO _settings;

        [Inject]
        private void Init(MovementSettingsSO settings)
        {
            _settings = settings;
        }

        private void OnControllerColliderHit(ControllerColliderHit hit)
        {
            if (hit.collider.TryGetComponent<GrabbableItem>(out GrabbableItem item))
            {
                if (hit.moveDirection.y < -0.3f) return;

                Vector3 pushDir = new Vector3(hit.moveDirection.x, 0, hit.moveDirection.z);
                var body = item.GetComponent<Rigidbody>();
                if (body != null)
                {
                    body.AddForce(pushDir * _settings.pushForce, ForceMode.VelocityChange);
                }
            }
        }
    }
}