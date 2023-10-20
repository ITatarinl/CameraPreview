#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Editor.Scripts.EditorWindow
{
    public class CameraPreview : UnityEditor.EditorWindow
    {
        private const float MOVE_SPEED = 1f;
        private static bool _isCameraEnabled;

        private Transform _terrainTransform;
        private Quaternion _cameraRotation;
        private Vector3 _cameraPosition;
        private float _previewHeight;
        private float _moveSreed;

        [MenuItem("Level Design/Camera Preview")]
        private static void OpenWindow()
        {
            CameraPreview window = GetWindow<CameraPreview>();
            window.Show();
        }

        private void OnGUI()
        {
            _terrainTransform ??= FindObjectOfType<Terrain>().transform;

            _isCameraEnabled = GUILayout.Toggle(_isCameraEnabled, "Enable Camera"); //SceneView state switcher 
            
            EditorGUILayout.LabelField("Camera speed");
            _moveSreed = EditorGUILayout.FloatField("", _moveSreed);
            if (_moveSreed <= 0)
                _moveSreed = MOVE_SPEED;

            EditorGUILayout.LabelField("Preview Height");
            _previewHeight = EditorGUILayout.FloatField("", _previewHeight);

            EditorGUILayout.LabelField("Camera Rotation");
            _cameraRotation = Quaternion.Euler(EditorGUILayout.Vector3Field("", _cameraRotation.eulerAngles));

            if (!GUILayout.Button("Apply Preview")) return;
            _isCameraEnabled = !_isCameraEnabled;
            SceneView sceneView = SceneView.lastActiveSceneView;
            
            if (sceneView == null) return;
            
            _cameraPosition = sceneView.camera.transform.position;
            _cameraPosition.y = _terrainTransform.position.y + _previewHeight;
            sceneView.LookAtDirect(_cameraPosition, _cameraRotation);
        }
    
        private void OnEnable()
        {
            SceneView.duringSceneGui += DuringSceneGUI;
        }

        private void OnDisable()
        {
            SceneView.duringSceneGui -= DuringSceneGUI;
        }
    
        private void HandleKeyDown(Event currentEvent, SceneView sceneView)
        {
            switch (currentEvent.keyCode)
            {
                case KeyCode.W:
                    MoveCamera(Vector3.forward, sceneView);
                    currentEvent.Use();
                    break;
                case KeyCode.S:
                    MoveCamera(Vector3.back, sceneView);
                    currentEvent.Use();
                    break;
                case KeyCode.A:
                    MoveCamera(Vector3.left, sceneView);
                    currentEvent.Use();
                    break;
                case KeyCode.D:
                    MoveCamera(Vector3.right, sceneView);
                    currentEvent.Use();
                    break;
                case KeyCode.E or KeyCode.Q:
                    MoveCamera(Vector3.zero, sceneView);
                    currentEvent.Use();
                    break;
            }
        }
        private void DuringSceneGUI(SceneView sceneView)
        {
            if(!_isCameraEnabled) return;

            Event currentEvent = Event.current;
            
            switch (currentEvent.type)
            {
                case EventType.MouseDrag or EventType.MouseDown:
                    MoveCamera(Vector3.zero, sceneView);
                    currentEvent.Use();
                    break;
                case EventType.KeyDown:
                    HandleKeyDown(currentEvent, sceneView);
                    break;
            }
        }

        private void MoveCamera(Vector3 direction, SceneView sceneView)
        {
            if(!_isCameraEnabled) return;

            _terrainTransform ??= FindObjectOfType<Terrain>().transform;
            
            _cameraPosition += direction * _moveSreed;
            _cameraPosition.y = _terrainTransform.position.y + _previewHeight;

            sceneView.LookAtDirect(_cameraPosition, _cameraRotation);
            sceneView.Repaint();
        }
    }
}
#endif