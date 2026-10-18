using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Logic
{
    public class TeleporterScript : MonoBehaviour
    {
        [SerializeField] private float spawnHeightOffset = 0.1f;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Teleporter"))
            {
                //shooting ray beneath so it could find a plane
                if (Physics.Raycast(other.transform.position, Vector3.down, out RaycastHit hit, 5f))
                {
                    Collider planeCollider = hit.collider;

                    if (planeCollider == null) return;
                    Bounds b = planeCollider.bounds;
                        
                    var randomPos = new Vector3(//random cords of a plane
                        Random.Range(b.min.x, b.max.x),
                        transform.position.y + spawnHeightOffset,
                        Random.Range(b.min.z, b.max.z)
                    );
                        
                    transform.position = randomPos;
                }
                else
                {
                    Debug.LogWarning("Ray couldn't find a plane");
                }
            }
        }
    }
}