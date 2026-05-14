using System;
using System.IO;
using System.Reflection;
using UnityEngine;
using UnityEngine.Video;

namespace FNAFJumpscare
{
    public static class JumpscarePlayer
    {
        private static readonly string[] VideoResources =
        [
            "FNAFJumpscare.Assets.Videos.freddy.mp4",
            "FNAFJumpscare.Assets.Videos.chica.mp4",
            "FNAFJumpscare.Assets.Videos.foxy.mp4"
        ];

        private static GameObject? CurrentRoot;
        private static string? CurrentTempFile;

        public static void DoIt()
        {
            Cleanup();

            var camera = Camera.main;

            if (!camera)
                return;

            var resourceName = VideoResources[UnityEngine.Random.Range(0, VideoResources.Length)];

            using Stream? resourceStream = Assembly
                .GetExecutingAssembly()
                .GetManifestResourceStream(resourceName);

            if (resourceStream == null)
            {
                Debug.LogError($"no fucking embedded video: {resourceName}");
                return;
            }

            CurrentTempFile = Path.Combine(
                Path.GetTempPath(),
                $"fnafjumpscare_{Guid.NewGuid():N}.mp4"
            );

            using (var fileStream = File.Create(CurrentTempFile))
                resourceStream.CopyTo(fileStream);

            CurrentRoot = new GameObject("Jumpscare");

            var rootTransform = CurrentRoot.transform;
            rootTransform.SetParent(camera.transform, false);

            CreateBackground(rootTransform);
            CreateVideo(rootTransform);
        }

        private static void CreateBackground(Transform parent)
        {
            var background = GameObject.CreatePrimitive(PrimitiveType.Quad);

            UnityEngine.Object.Destroy(background.GetComponent<Collider>());

            background.name = "Background";
            background.transform.SetParent(parent, false);

            background.transform.localPosition = new Vector3(0f, 0f, 0.55f);
            background.transform.localScale = new Vector3(10f, 10f, 1f);

            Material material = new(Shader.Find("Unlit/Color"))
            {
                color = Color.black
            };

            background.GetComponent<Renderer>().material = material;
        }

        private static void CreateVideo(Transform parent)
        {
            var videoObject = GameObject.CreatePrimitive(PrimitiveType.Quad);
            UnityEngine.Object.Destroy(videoObject.GetComponent<Collider>());

            videoObject.name = "Video";
            videoObject.transform.SetParent(parent, false);
            videoObject.transform.localPosition = new Vector3(0f, 0f, 0.5f);
            videoObject.transform.localScale = new Vector3(1.8f, 1f, 1f);

            var renderer = videoObject.GetComponent<Renderer>();
            renderer.material = new Material(Shader.Find("Unlit/Texture"));

            var audioSource = videoObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioSource.spatialBlend = 0f;
            audioSource.volume = 1f;

            var videoPlayer = videoObject.AddComponent<VideoPlayer>();
            videoPlayer.playOnAwake = false;
            videoPlayer.isLooping = false;
            videoPlayer.source = VideoSource.Url;
            videoPlayer.url = CurrentTempFile;
            
            videoPlayer.renderMode = VideoRenderMode.MaterialOverride;
            videoPlayer.targetMaterialRenderer = renderer;
            videoPlayer.targetMaterialProperty = "_MainTex";

            videoPlayer.audioOutputMode = VideoAudioOutputMode.AudioSource;
            videoPlayer.SetTargetAudioSource(0, audioSource);

            videoPlayer.prepareCompleted += player =>
            {
                player.Play();
                audioSource.Play();
            };

            videoPlayer.loopPointReached += _ => Cleanup();

            videoPlayer.Prepare();
        }

        private static void Cleanup()
        {
            if (CurrentRoot)
            {
                UnityEngine.Object.Destroy(CurrentRoot);
                CurrentRoot = null;
            }

            if (string.IsNullOrWhiteSpace(CurrentTempFile) || !File.Exists(CurrentTempFile)) return;
            
            File.Delete(CurrentTempFile);
            CurrentTempFile = null;
        }
    }
}