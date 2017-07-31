using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
/// <summary>
/// This is an old script. Please don't look at it for too long :)
/// </summary>
namespace SD.Menu
{
    public class MenuManager : MonoBehaviour
    {
        public GameObject InitialMenu
        {
            get
            {
                return _initialMenu;
            }

            set
            {
                _initialMenu = value;
            }
        }

        public string AnimationName
        {
            get
            {
                return _animationName;
            }
            set
            {
                _animationName = value;
            }
        }

        public List<GameObject> NavigationHistory
        {
            get
            {
                return _navigationHistory;
            }
            set
            {
                _navigationHistory = value;
            }
        }

        [SerializeField]
        private GameObject _initialMenu;

        [SerializeField]
        private string _animationName;

        
        private List<GameObject> _navigationHistory = new List<GameObject>();


        
        [Header("Audio")]
        [Range(0.1f, 1)]
        public float audioVolume = 1f;
        [Range(0.5f, 5)]
        public float fadeAudioSpeed = 0.5f;
        [SerializeField]
        private AudioClip backgroundAudio = null;
        private AudioSource audioSource;

        // Use this for initialization
        private void Awake()
        {
            _navigationHistory = new List<GameObject> { _initialMenu };

            audioSource = GetComponent<AudioSource>();
            if (audioSource != null) {
                audioSource.volume = 0;
                audioSource.loop = true;
            }


            if (backgroundAudio != null)
                audioSource.clip = backgroundAudio;
            
            StartCoroutine(FadeAudio(true));
        }

        private IEnumerator FadeAudio(bool fadeIn)
        {
            if (backgroundAudio == null)
                yield break;

            if (fadeIn)
            {
                audioSource.Play();
                while (audioSource.volume < audioVolume)
                {
                    audioSource.volume += Time.deltaTime * fadeAudioSpeed;
                    yield return new WaitForEndOfFrame();
                }
            }
            else
            {
                while (audioSource.volume > 0)
                {
                    audioSource.volume -= Time.deltaTime * fadeAudioSpeed * 3;
                    yield return new WaitForEndOfFrame();
                }
                audioSource.Stop();
            }
        }
        //Public methods die aanspreekbaar zijn door de buttons

        /// <summary>
        /// Go back to the previous menu from the navigationHistory list.
        /// Only if there is a previous item in the list
        /// </summary>
        public void GoBack()
        {
            if (_navigationHistory.Count > 1)
            {
                //Laad vorige menu uit de List en animeer daar naar toe
                int index = _navigationHistory.Count - 1;
                AnimateMenu(_navigationHistory[index - 1], true);

                //Verwijder het huidige menu
                GameObject currentMenu = _navigationHistory[index];
                _navigationHistory.RemoveAt(index);
                AnimateMenu(currentMenu, false);
            }
        }

        /// <summary>
        /// Method is called by a button set in the inspector.
        /// Goes to the given target menu
        /// </summary>
        /// <param name="target">Menu Target</param>
        public void GoToMenu(GameObject target)
        {
            if (target == null)
                return;

            if (_navigationHistory.Count > 0)
            {
                AnimateMenu(_navigationHistory[_navigationHistory.Count - 1], false);
            }

            _navigationHistory.Add(target);
            AnimateMenu(target, true);
        }

        /// <summary>
        /// Stops the game in the editor and standalone
        /// </summary>
        public void Quit()
        {
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #else
                Application.Quit();
            #endif
        }

        /// <summary>
        /// Goes to the given scene by name
        /// TODO: I will implement the scene loading functionality in the menu manager
        /// </summary>
        /// <param name="scene"></param>
        public static void GoToScene(string scene)
        {
           // StartCoroutine(FadeAudio(false));
         //   SceneLoader.Instance.LoadScene(scene);
            SceneManager.LoadScene(scene);
        }

        /// <summary>
        /// Goes to the given scene by name
        /// TODO: I will implement the scene loading functionality in the menu manager
        /// </summary>
        /// <param name="scene"></param>
        public void GoToSceneRef(string scene) {
            // StartCoroutine(FadeAudio(false));
            
            SceneManager.LoadScene(scene);
        }

        /// <summary>
        /// Animates to the given menu by the target paremeter.
        /// If the direction is set to true, the method will show the menu
        /// else, it will hide it.
        /// </summary>
        /// <param name="target">Menu to target</param>
        /// <param name="direction">Direction the menu will be (true is open, false is closed)</param>
        private void AnimateMenu(GameObject target, bool direction)
        {
            if (target == null)
               return;

            Canvas canvas = target.GetComponent<Canvas>();

            if (canvas != null)
            {
                canvas.overrideSorting = true;
                canvas.sortingOrder = _navigationHistory.Count;
            }
            
            Animator animator = target.GetComponent<Animator>();

            if (animator != null && target.activeSelf)
            {
                animator.SetBool(_animationName, direction);
            }
   
            if (direction)
            {
                target.SetActive(true);
            }
            else
            {
                target.SetActive(false);
            }
        }
    }
}