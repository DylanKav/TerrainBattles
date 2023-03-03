using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

namespace OnTerrainFeatures
{
    [System.Serializable] public class DestructableObject : MonoBehaviour
    {
        [HideInInspector] public bool isDisappearOnDestroy;
        [HideInInspector] public GameObject DestroyedPrefab;
        [HideInInspector] public int TimeToClear = 5;

/*
        private void Start()
        {
            Destroy();
        }
*/
        public void Destroy()
        {
            if (isDisappearOnDestroy)
            {
                Destroy(this.gameObject);
                return;
            }
            if (!Application.isPlaying) return;
            var destroyedFX = Instantiate(DestroyedPrefab);
            var transform1 = this.transform;
            destroyedFX.transform.position = transform1.position;
            destroyedFX.transform.rotation = transform1.rotation;
            var cleanup = destroyedFX.AddComponent<CleanupObject>();
            cleanup.TimeToDestroy = TimeToClear;
            Destroy(this.gameObject);
        }

        #region Editor
#if UNITY_EDITOR
        [CustomEditor(typeof(DestructableObject))]
        public class MyEditorClass : Editor
        {
            public override void OnInspectorGUI()
            {
                // If we call base the default inspector will get drawn too.
                // Remove this line if you don't want that to happen.
                base.OnInspectorGUI();

                DestructableObject DestructionScript = target as DestructableObject;

                if (DestructionScript == null) return;
                
                serializedObject.FindProperty("isDisappearOnDestroy").boolValue = EditorGUILayout.Toggle("Disappear on Destroy?", DestructionScript.isDisappearOnDestroy);

                if (!DestructionScript.isDisappearOnDestroy)
                {
                    serializedObject.FindProperty("DestroyedPrefab").objectReferenceValue = EditorGUILayout.ObjectField("DestroyedPrefab:", DestructionScript.DestroyedPrefab, typeof(GameObject), true) as GameObject;
                    serializedObject.FindProperty("TimeToClear").intValue = EditorGUILayout.IntField("Time to clear:", DestructionScript.TimeToClear);

                }

                serializedObject.ApplyModifiedProperties();
            }
        }
#endif
        #endregion
    }
}

