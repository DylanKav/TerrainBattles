using System.Collections;
using UnityEngine;

namespace OnTerrainFeatures
{
    public class CleanupObject : MonoBehaviour
    {
        public int TimeToDestroy = 5;
        IEnumerator Start()
        {
            yield return new WaitForSeconds(TimeToDestroy);
            Destroy(this.gameObject);
        }
    }
}
