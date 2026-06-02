using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

namespace Unity.AI.Assistant.PlayModeTest
{
    [InitializeOnLoad]
    internal static class PlayModeTestRunner
    {
        private const string StateKey = "PlayModeTest.State";
        private const string ResultKey = "PlayModeTest.Result";
        private const string ScriptPathKey = "PlayModeTest.ScriptPath";
        private const string SentinelLog = "PLAY_MODE_TEST_COMPLETE";

        private static readonly int WaitFrames = SessionState.GetInt("PlayModeTest.WaitFrames", 5);

        private static List<string> _capturedLogs = new List<string>();
        private const int MaxCapturedLogs = 50;

        static PlayModeTestRunner()
        {
            string state = SessionState.GetString(StateKey, "Idle");

            switch (state)
            {
                case "WaitingForCompile":
                    EditorApplication.delayCall += () =>
                    {
                        SessionState.SetString(StateKey, "EnteringPlayMode");
                        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
                        EditorApplication.isPlaying = true;
                    };
                    break;

                case "EnteringPlayMode":
                    EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
                    if (EditorApplication.isPlaying)
                    {
                        EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
                        SessionState.SetString(StateKey, "InPlayMode");
                        EditorApplication.update += WaitFramesThenRun;
                    }
                    break;

                case "InPlayMode":
                    if (EditorApplication.isPlaying)
                    {
                        EditorApplication.update += WaitFramesThenRun;
                    }
                    break;

                case "Done":
                    EditorApplication.delayCall += SelfDestruct;
                    break;
            }
        }

        private static void OnPlayModeStateChanged(PlayModeStateChange change)
        {
            if (change == PlayModeStateChange.EnteredPlayMode)
            {
                EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
                SessionState.SetString(StateKey, "InPlayMode");
                EditorApplication.update += WaitFramesThenRun;
            }
        }

        private static int _frameCount = 0;
        private static bool _hasRun = false;

        private static void WaitFramesThenRun()
        {
            _frameCount++;
            if (_frameCount < WaitFrames) return;

            if (_hasRun) return;
            _hasRun = true;
            EditorApplication.update -= WaitFramesThenRun;

            Application.logMessageReceived += OnLogMessage;
            string resultJson;
            try
            {
                resultJson = RunTestLogic();
            }
            catch (System.Exception e)
            {
                Debug.LogError("[PlayModeTest] Test threw exception: " + e);
                resultJson = JsonUtility.ToJson(new TestResult
                {
                    success = false,
                    error = e.Message,
                    logs = _capturedLogs.ToArray()
                });
            }
            finally
            {
                Application.logMessageReceived -= OnLogMessage;
            }

            SessionState.SetString(ResultKey, resultJson);
            SessionState.SetString(StateKey, "Done");
            EditorApplication.isPlaying = false;
        }

        private static void SelfDestruct()
        {
            string scriptPath = SessionState.GetString(ScriptPathKey, "");
            if (!string.IsNullOrEmpty(scriptPath) && AssetDatabase.AssetPathExists(scriptPath))
            {
                AssetDatabase.DeleteAsset(scriptPath);
            }
            SessionState.EraseString(StateKey);
            SessionState.EraseString(ScriptPathKey);
        }

        private static void OnLogMessage(string message, string stackTrace, LogType type)
        {
            if (_capturedLogs.Count >= MaxCapturedLogs) return;
            if (type == LogType.Error || type == LogType.Exception ||
                message.Contains("[Test]") || message.Contains("TEST_RESULT"))
            {
                _capturedLogs.Add("[" + type + "] " + message);
            }
        }

        [System.Serializable]
        private class TestResult
        {
            public bool success;
            public string error;
            public string[] logs;
        }

        private static string RunTestLogic()
        {
            // Ensure correct scene is loaded
            if (SceneManager.GetActiveScene().name != "Map_Manta_Prototype")
            {
                return ErrorResult("Wrong scene: " + SceneManager.GetActiveScene().name);
            }

            var rock = GameObject.Find("SecretEntrance_Rock");
            if (rock == null) return ErrorResult("SecretEntrance_Rock not found");

            var interactionArea = rock.transform.Find("InteractionArea");
            if (interactionArea == null) return ErrorResult("InteractionArea child not found");

            var rockCollider = rock.GetComponent<BoxCollider2D>();
            if (rockCollider == null) return ErrorResult("Rock BoxCollider2D not found");
            if (rockCollider.isTrigger) return ErrorResult("Rock collider is still a trigger");

            var interactionCollider = interactionArea.GetComponent<CircleCollider2D>();
            if (interactionCollider == null) return ErrorResult("InteractionArea CircleCollider2D not found");
            if (!interactionCollider.isTrigger) return ErrorResult("InteractionArea collider is not a trigger");

            var simpleInteraction = interactionArea.GetComponent<SimpleInteraction>();
            if (simpleInteraction == null) return ErrorResult("SimpleInteraction component not found on InteractionArea");

            string prompt = simpleInteraction.GetInteractionPrompt();
            if (prompt != "Presiona E para investigar roca") return ErrorResult("Incorrect prompt: " + prompt);

            var player = GameObject.Find("Player");
            if (player == null) return ErrorResult("Player not found");
            var interactor = player.GetComponentInChildren<PlayerInteractor>();
            if (interactor == null) return ErrorResult("PlayerInteractor not found");

            int playerLayer = player.layer;
            int rockLayer = rock.layer;
            if (Physics2D.GetIgnoreLayerCollision(playerLayer, rockLayer)) return ErrorResult("Player and Rock layers do not collide");

            Debug.Log("[Test] Verification successful: Rock is solid, InteractionArea is trigger, and Prompt is correct.");
            return JsonUtility.ToJson(new TestResult { success = true, logs = _capturedLogs.ToArray() });
        }

        private static string ErrorResult(string msg)
        {
            Debug.LogError("[Test] " + msg);
            return JsonUtility.ToJson(new TestResult { success = false, error = msg, logs = _capturedLogs.ToArray() });
        }
    }
}
