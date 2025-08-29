using System.Collections.Generic;
using System.Text;
using TMPro;
using Unity.Profiling;
using UnityEngine;

namespace DebugToolkit.Profiling
{
    public class Metrics : MonoBehaviour
    {
        [Header("UI elements")]
        [SerializeField] private TextMeshProUGUI fpsText;
        [SerializeField] private TextMeshProUGUI threadText;
        [SerializeField] private TextMeshProUGUI renderText;
        [SerializeField] private TextMeshProUGUI memoryText;
        [SerializeField] private TextMeshProUGUI audioText;

        [Header("Params")]
        [SerializeField] private float updateDeltaTime = 4;

        [Header("Graphs")]
        [SerializeField] private Graph mainThreadGraph;

        private ProfilerRecorder _mainThreadTimeRecorder;
        private ProfilerRecorder _batchesRecorder;
        private ProfilerRecorder _setPassCallsRecorder;
        private ProfilerRecorder _drawCallsRecorder;
        private ProfilerRecorder _trisRecorder;
        private ProfilerRecorder _vertsRecorder;
        private ProfilerRecorder _CPUmainThreadRecorder;
        private ProfilerRecorder _shadowCastersRecorder;
        private ProfilerRecorder _visibleSkinnedMeshesCountRecorder;

        private ProfilerRecorder _totalUsedMemoryRecorder;
        private ProfilerRecorder _systemUsedMemoryRecorder;
        private ProfilerRecorder _videoUsedMemoryRecorder;
        private ProfilerRecorder _audioUsedMemoryRecorder;
        private ProfilerRecorder _gcMemoryRecorder;

        private bool _isActive;

        private void Awake()
        {
            _isActive = true;

            _mainThreadTimeRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Internal, "Main Thread", 15);
            _batchesRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Render, "Batches Count");
            _setPassCallsRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Render, "SetPass Calls Count");
            _drawCallsRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Render, "Draw Calls Count");
            _trisRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Render, "Triangles Count");
            _vertsRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Render, "Vertices Count");
            _CPUmainThreadRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Render, "CPU Main Thread Frame Time"); 
            _shadowCastersRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Render, "Shadow Casters Count"); 
            _visibleSkinnedMeshesCountRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Render, "Visible Skinned Meshes Count");

            _totalUsedMemoryRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Memory, "Total Used Memory");
            _systemUsedMemoryRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Memory, "System Used Memory");
            _videoUsedMemoryRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Memory, "Video Used Memory");
            _audioUsedMemoryRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Memory, "Audio Used Memory");
            _gcMemoryRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Memory, "GC Used Memory");

            UpdateUI();
        }

        public async void Dispose()
        {
            _isActive = false;

            await Awaitable.WaitForSecondsAsync(1 / updateDeltaTime);
            gameObject.SetActive(false);

            _mainThreadTimeRecorder.Dispose();
            _batchesRecorder.Dispose();
            _setPassCallsRecorder.Dispose();
            _drawCallsRecorder.Dispose();
            _trisRecorder.Dispose();
            _vertsRecorder.Dispose();
            _CPUmainThreadRecorder.Dispose();
            _shadowCastersRecorder.Dispose();
            _visibleSkinnedMeshesCountRecorder.Dispose();

            _totalUsedMemoryRecorder.Dispose();
            _systemUsedMemoryRecorder.Dispose();
            _videoUsedMemoryRecorder.Dispose();
            _audioUsedMemoryRecorder.Dispose();
            _gcMemoryRecorder.Dispose();

            Destroy(gameObject);
        }

        private async void UpdateUI()
        {
            StringBuilder builder = new StringBuilder();
            while (_isActive)
            {
                await Awaitable.WaitForSecondsAsync(1 / updateDeltaTime);

                //FPS
                fpsText.text =
                    $"{GetFPS():F1} FPS ({GetRecorderFrameAverage(_mainThreadTimeRecorder) * (1e-6f):F1}ms)";
                //CPU
                threadText.text =
                    $"CPU: main <b><color=white>{_mainThreadTimeRecorder.LastValue * (1e-6f):F1}</color></b>ms render thread <color=white>{_CPUmainThreadRecorder.LastValue * (1e-6f):F1}</color>ms";

                builder.Clear();
                //Rendering
                builder.AppendLine(
                    $"Batches <b><color=white>{_batchesRecorder.LastValue}</color></b>");
                builder.AppendLine(
                    $"Tris: <color=white>{ToKilo(_trisRecorder.LastValue)}</color> Verts: <color=white>{ToKilo(_vertsRecorder.LastValue)}</color>");
                builder.AppendLine(
                    $"SetPass calls: <color=white>{_setPassCallsRecorder.LastValue}</color> Draw calls: <color=white>{_drawCallsRecorder.LastValue}</color>");
                builder.AppendLine(
                    $"Shadow casters: <color=white>{_shadowCastersRecorder.LastValue}</color>");
                builder.AppendLine(
                    $"Visible skinned meshes: <color=white>{_visibleSkinnedMeshesCountRecorder.LastValue}</color>");
                renderText.text = builder.ToString();

                builder.Clear();
                //Memory
                builder.AppendLine(
                    $"Total: <color=white>{_totalUsedMemoryRecorder.LastValue / (1024 * 1024)}</color> MB");
                builder.AppendLine(
                    $"System: <color=white>{_systemUsedMemoryRecorder.LastValue / (1024 * 1024)}</color> MB");
                builder.AppendLine(
                    $"Video: <color=white>{_videoUsedMemoryRecorder.LastValue / (1024 * 1024)}</color> MB");
                builder.AppendLine(
                    $"Audio: <color=white>{_audioUsedMemoryRecorder.LastValue / (1024 * 1024)}</color> MB");
                builder.AppendLine(
                    $"GC: <color=white>{_gcMemoryRecorder.LastValue / (1024 * 1024)}</color> MB");
                memoryText.text = builder.ToString();

                builder.Clear();
                //Audio
                builder.AppendLine(
                    $"Level: <color=white>{GetAudioLevel():F2}</color> dB");
                audioText.text = builder.ToString();
              
                //Graphs
                mainThreadGraph.ShowGraph(_mainThreadTimeRecorder.LastValue * (1e-6f));
            }
        }

        private float GetFPS()
        {
            float fps = 1.0f / Time.deltaTime;
            return fps;
        }

        private double GetRecorderFrameAverage(ProfilerRecorder recorder)
        {
            var samplesCount = recorder.Capacity;
            if (samplesCount == 0)
                return 0;

            double r = 0;
            var samples = new List<ProfilerRecorderSample>(samplesCount);
            recorder.CopyTo(samples);
            for (var i = 0; i < samples.Count; ++i)
                r += samples[i].Value;
            r /= samplesCount;

            return r;
        }

        private float[] samples = new float[512]; // Plus d'échantillons pour plus de précision
        private const float MIN_RMS = 0.00001f; // Valeur minimale pour éviter log(0)
        private const float REFERENCE_RMS = 0.1f; // Niveau de référence ajustable

        private float GetAudioLevel()
        {
            AudioListener.GetOutputData(samples, 0);

            float sum = 0f;
            foreach (float sample in samples)
            {
                sum += sample * sample;
            }
            float rms = Mathf.Sqrt(sum / samples.Length);

            rms = Mathf.Max(rms, MIN_RMS);

            float dB = 20 * Mathf.Log10(rms / REFERENCE_RMS);

            return dB;
        }

        private string ToKilo(long l)
        {
            string formatted = string.Empty;
            if (l >= 1000000)
            {
                formatted = $"{l / 1000000f:F1}M";
            }
            else
            {
                formatted = $"{l / 1000f:F1}K";
            }            
            return formatted;
        }
    }
}

