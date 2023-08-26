using System.Collections;
using UnityEngine;

namespace Util
{
    public class Movement : MonoBehaviour
    {
        [SerializeField] private float movementSpeed;
        [SerializeField] private Vector3 movementAxis;
        [SerializeField] private Vector3 minPoint;
        [SerializeField] private Vector3 maxPoint;

        
        private void Start()
        {
            StartCoroutine(MovementRoutine());
        }

        private IEnumerator MovementRoutine()
        {
            while (gameObject.activeInHierarchy)
            {
                Vector3 pos = transform.position + Random.insideUnitSphere.normalized * movementSpeed;
                pos.x = Mathf.Clamp(pos.x, minPoint.x, maxPoint.x) * movementAxis.x;
                pos.y = Mathf.Clamp(pos.y, minPoint.y, maxPoint.y) * movementAxis.y;
                pos.z = Mathf.Clamp(pos.z, minPoint.z, maxPoint.z) * movementAxis.z;
                transform.position = pos;
                yield return null;
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(minPoint, 0.5f);
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(maxPoint, 0.5f);
        }
    }
}