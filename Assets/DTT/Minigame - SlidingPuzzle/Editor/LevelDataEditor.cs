#if UNITY_EDITOR

using DTT.PublishingTools;
using UnityEditor;

namespace DTT.MiniGame.SlidingPuzzle.Editor
{
    /// <summary>
    /// Custom editor for the <see cref="LevelData"/> asset.
    /// </summary>
    [CustomEditor(typeof(LevelData)), DTTHeader("dtt.minigame-slidingpuzzle")]
    public class LevelDataEditor : DTTInspector
    {
        /// <summary>
        /// Draws the default inspector.
        /// </summary>
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            SerializedProperty gametype = serializedObject.FindProperty("_gameType");
            SerializedProperty image = serializedObject.FindProperty("_tilesTexture");
            SerializedProperty size = serializedObject.FindProperty("_size");
            SerializedProperty scrambleCount = serializedObject.FindProperty("_scrambleCount");
            
            EditorGUILayout.PropertyField(gametype);
            if (gametype.intValue == 0)
                EditorGUILayout.PropertyField(image);

            EditorGUILayout.PropertyField(size);
            EditorGUILayout.PropertyField(scrambleCount);

            serializedObject.ApplyModifiedProperties();
        }
    }
}

#endif