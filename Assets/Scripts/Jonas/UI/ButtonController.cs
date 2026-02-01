using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

// Ermöglicht die Nutzung von UI-Events

namespace Jonas.UI
{
    public class ButtonController : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler, ISelectHandler, ISubmitHandler
    {
        [Header("Setup")]
        private AudioSource _audioSource;
        private AudioSource AudioSource
        {
            get
            {
                if (_audioSource == null)
                {
                    _audioSource = GameObject.Find("UISounds").GetComponent<AudioSource>();
                }
                return _audioSource;
            }
        }

        [Header("Audio Clips")]
        public AudioClip hoverClip;
        public AudioClip clickClip;
        
        [Header("Animation Settings")]
        [SerializeField] private float scaleFactor = 1.2f; // Zielgröße (1.2 = 120%)
        [SerializeField] private float duration = 0.1f;    // Dauer pro Phase in Sekunden
        
        private Vector3 _originalScale;
        private Coroutine _activeCoroutine;
        
        void Awake()
        {
            _originalScale = transform.localScale;
        }
        
        public void OnSelect(BaseEventData eventData)
        {
            if (eventData is AxisEventData axisEvent && axisEvent.moveVector.magnitude > 0)
            {
                if(!AudioSource.isPlaying) AudioSource.PlayOneShot(hoverClip);
                if (_activeCoroutine != null) StopCoroutine(_activeCoroutine);
                _activeCoroutine = StartCoroutine(PulseRoutine());
            }
        }
        
        public void OnPointerEnter(PointerEventData eventData)
        {
            if(!AudioSource.isPlaying) AudioSource.PlayOneShot(hoverClip);
            if (_activeCoroutine != null) StopCoroutine(_activeCoroutine);
            _activeCoroutine = StartCoroutine(PulseRoutine());
        }
        
        

        public void OnPointerClick(PointerEventData eventData)
        {
            if(!AudioSource.isPlaying) AudioSource.PlayOneShot(clickClip);
        }

        public void OnSubmit(BaseEventData eventData)
        {
            if(!AudioSource.isPlaying) AudioSource.PlayOneShot(clickClip);
        }
        
        private IEnumerator PulseRoutine()
        {
            Vector3 targetScale = _originalScale * scaleFactor;

            yield return MoveScale(_originalScale, targetScale);
        
            yield return MoveScale(targetScale, _originalScale);
        
            _activeCoroutine = null;
        }

        private IEnumerator MoveScale(Vector3 from, Vector3 to)
        {
            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                // Berechnet den Fortschritt normiert auf [0, 1]
                float progress = elapsed / duration;
                transform.localScale = Vector3.Lerp(from, to, progress);
                yield return null;
            }
            transform.localScale = to;
        }
    }
}