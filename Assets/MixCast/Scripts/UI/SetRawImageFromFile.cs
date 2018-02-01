using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace BlueprintReality.MixCast
{
    [RequireComponent(typeof(RawImage))]
    public class SetRawImageFromFile : MonoBehaviour
    {
        public string filepath;
        public bool setImageOnStart;
        public UnityEvent OnImageLoaded;

        void Start()
        {
            if (setImageOnStart)
            {
                LoadImage();
            }
        }

        public void LoadImage()
        {
            StartCoroutine(LoadImageAsync(filepath));
        }

        private IEnumerator LoadImageAsync(string filepath)
        {
            if (string.IsNullOrEmpty(filepath))
            {
                Debug.LogError("No filepath set for raw texture!");

                yield break;
            }

            WWW www = new WWW("file://" + filepath);

            yield return www;

            if (!string.IsNullOrEmpty(www.error) || www.texture == null)
            {
                gameObject.SetActive(false);
            }
            else
            {
                GetComponent<RawImage>().texture = www.texture;

                OnImageLoaded.Invoke();

                gameObject.SetActive(true);
            }
        }
    }
}