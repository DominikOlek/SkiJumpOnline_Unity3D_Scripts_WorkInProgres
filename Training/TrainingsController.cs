using Assets.Scripts;
using Assets.Scripts.Competition;
using Assets.Scripts.Competition.Other;
using Assets.Scripts.ImportData;
using Assets.Scripts.Jumping.StaticInfo;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Assets.Scripts.Training
{
    public class TrainingsController : MonoBehaviour
    {
        private const string ATTEMPTTXT = "NUMBER OF ATTEMPT: \n";

        [SerializeReference] List<GameObject> trainings;
        [SerializeField] private GameObject trainList, buttonTrainPref, singleMenu;
        [SerializeField] private Text titleTxt, descriptTxt, attempt;
        [SerializeField] private GameObject swingRunS, swingFlyS, swingDownS, ctrlRunS, ctrlFlyS, ctrlDownS;
        [SerializeField] private Button selectButton;
        [SerializeField] private LoadingScreen loadingScreen;
        DataHolder holder;

        private GameObject selected;

        private void Awake()
        {
            if (SceneManager.GetActiveScene().buildIndex != 0)
                return;
            GameObject tmp = GameObject.FindWithTag("CompetitionController");
            ITrainingController ctrl;
            if (tmp != null && tmp.TryGetComponent<ITrainingController>(out ctrl))
            {
                ctrl.ReceiveExperience();
                Destroy(tmp);
                ITrainingController.instance = null;
            }
            holder = GameObject.FindGameObjectWithTag("DataHolder").GetComponent<DataHolder>();
            if (holder == null)
            {
                throw new UnityException("Not found DataHolder");
            }
        }


        private void OnEnable()
        {
            holder = GameObject.FindGameObjectWithTag("DataHolder").GetComponent<DataHolder>();
            if (holder == null)
            {
                throw new UnityException("Not found DataHolder");
            }
            ShowList();
        }

        

        public void ShowList()
        {
            foreach (Transform child in trainList.transform)
            {
                Destroy(child.gameObject);
            }
            int i = 0;
            foreach (var item in trainings.Select(a => a.GetComponent<ITrainingController>()))
            {
                GameObject tmp = Instantiate(buttonTrainPref);
                tmp.transform.SetParent(trainList.transform, false);
                tmp.transform.localScale = Vector3.one;
                tmp.transform.GetChild(0).GetComponent<Text>().text = item.Name;
                tmp.GetComponent<Button>().onClick.AddListener(() => { ShowInfo(item); });
                item.Refresh();
                if (i == 0)
                    ShowInfo(item);
                i++;
            }
        }

        public void ShowInfo(ITrainingController training)
        {
            selectButton.onClick.RemoveAllListeners();
            selectButton.onClick.AddListener(() => { PlayButton(training); });
            attempt.text = ATTEMPTTXT;
            training.ShowDetails(titleTxt, descriptTxt, attempt, swingRunS, swingFlyS, swingDownS, ctrlRunS, ctrlFlyS, ctrlDownS);
        }

        public void BackButton()
        {
            singleMenu.SetActive(true);
            this.gameObject.SetActive(false);
        }

        public void PlayButton(ITrainingController training)
        {
            StartCoroutine(PlayWithLoading(training));
        }

        IEnumerator PlayWithLoading(ITrainingController training)
        {
            selected = GameObject.Instantiate(training.GameObject);
            holder.isTraining = true;

            SkiJumpInfoGlobal competition = training.Play();

            loadingScreen.Show(competition);

            yield return null;

            AsyncOperation loading = SceneManager.LoadSceneAsync(competition.name, LoadSceneMode.Single);
            loading.allowSceneActivation = false;
            yield return StartCoroutine(LoadSkiJumpAsync(loading));
        }

        System.Collections.IEnumerator LoadSkiJumpAsync(AsyncOperation loading)
        {
            float minLoadTime = 2f;
            float timer = 0f;

            while (!loading.isDone)
            {
                loadingScreen.progressSlider.value = Mathf.Clamp01(loading.progress / 0.9f);
                timer += Time.deltaTime;
                if (loading.progress >= 0.9f && timer >= minLoadTime)
                {
                    loading.allowSceneActivation = true;
                }

                yield return null;
            }
        }

    }
}