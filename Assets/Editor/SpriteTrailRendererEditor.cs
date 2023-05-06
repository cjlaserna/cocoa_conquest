using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace SpriteTrailRenderer
{
    [CustomEditor(typeof(SpriteTrailRenderer))]
    public class SpriteTrailRendererEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            // Update the serializedProperty - always do this in the beginning of OnInspectorGUI.
            serializedObject.Update();

            SpriteTrailRenderer trailRenderer = (SpriteTrailRenderer)target;

            if (trailRenderer != null)
            {
                // Get the current sorting layer name and index
                string currentSortingLayerName = trailRenderer._sortingLayerName;
                int currentSortingLayerIndex = trailRenderer._sortingLayerID;

                // Get the sorting layer names and indices
                string[] sortingLayerNames = GetSortingLayerNames();
                int[] sortingLayerIndices = GetSortingLayerIndices();

                // Create the sorting layer dropdown
                int selectedSortingLayerIndex = EditorGUILayout.IntPopup("Sorting Layer", currentSortingLayerIndex, sortingLayerNames, sortingLayerIndices);

                // Get the sorting layer name based on the selected index
                string selectedSortingLayerName = sortingLayerNames[selectedSortingLayerIndex];

                // Set the new sorting layer name and index
                if (selectedSortingLayerName != currentSortingLayerName || selectedSortingLayerIndex != currentSortingLayerIndex)
                {
                    trailRenderer._sortingLayerID = selectedSortingLayerIndex;
                    trailRenderer._sortingLayerName = selectedSortingLayerName;
                }
            }


            // update trail spawn values
            trailRenderer._trailLifetime = EditorGUILayout.FloatField("Trail Lifetime", trailRenderer._trailLifetime);
            trailRenderer._spawnType = (SpawnType)EditorGUILayout.EnumPopup("Trail Spawn Type", trailRenderer._spawnType);

            if (trailRenderer._spawnType == SpawnType.TIME)
            {
                trailRenderer._timeBetweenTrailSpawn = EditorGUILayout.FloatField("Spawn Rate", trailRenderer._timeBetweenTrailSpawn);
            }
            else
            {
                trailRenderer._distanceBetweenTrailSpawn = EditorGUILayout.FloatField("Distance Between Spawns", trailRenderer._distanceBetweenTrailSpawn);
            }

            // update trail scales
            trailRenderer._startScale = EditorGUILayout.Vector2Field("Start Scale", trailRenderer._startScale);
            trailRenderer._endScale = EditorGUILayout.Vector2Field("End Scale", trailRenderer._endScale);

            // update color alpha
            trailRenderer._alphaUpdateOn = EditorGUILayout.Toggle("Alpha Update On", trailRenderer._alphaUpdateOn);
            trailRenderer._maxAlpha = EditorGUILayout.DelayedFloatField("Max Alpha", trailRenderer._maxAlpha);
            trailRenderer._minAlpha = EditorGUILayout.DelayedFloatField("Min Alpha", trailRenderer._minAlpha);

            if (trailRenderer._minAlpha > trailRenderer._maxAlpha)
            {
                trailRenderer._minAlpha = trailRenderer._maxAlpha;
            }


            // color options
            trailRenderer._useSolidColors = EditorGUILayout.Toggle("Change Trail Colors", trailRenderer._useSolidColors);
            if (trailRenderer._useSolidColors)
            {
                trailRenderer._rainbowMode = EditorGUILayout.Toggle("Rainbow Mode", trailRenderer._rainbowMode);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_userSelectedColorPalette"), true);
            }

            // Apply changes to the serializedProperty - always do this in the end of OnInspectorGUI.
            serializedObject.ApplyModifiedProperties();
        }

        // Get the names of all the sorting layers
        private string[] GetSortingLayerNames()
        {
            string[] sortingLayerNames = new string[SortingLayer.layers.Length];

            for (int i = 0; i < SortingLayer.layers.Length; i++)
            {
                sortingLayerNames[i] = SortingLayer.layers[i].name;
            }

            return sortingLayerNames;
        }

        // Get the indices of all the sorting layers
        private int[] GetSortingLayerIndices()
        {
            int[] sortingLayerIndices = new int[SortingLayer.layers.Length];

            for (int i = 0; i < SortingLayer.layers.Length; i++)
            {
                sortingLayerIndices[i] = SortingLayer.layers[i].value;
            }

            return sortingLayerIndices;
        }
    }
}