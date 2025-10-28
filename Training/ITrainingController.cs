using Assets.Scripts;
using Assets.Scripts.Competition;
using Assets.Scripts.Competition.Other;
using Assets.Scripts.ImportData;
using Assets.Scripts.Jumping.Controllers;
using Assets.Scripts.Jumping.StaticInfo;
using Assets.Scripts.Menu;
using Assets.Scripts.WebDTO;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Assets.Scripts.Training
{
    public abstract class ITrainingController : MonoBehaviour, IToBeat, ISetResult, IGetDifficulty
    {
        public const string SWINGDOWNTXT = "SWING LANDING";
        public const string SWINGFLYTXT = "SWING FLY";
        public const string SWINGRUNTXT = "SWING INRUN";
        public const string CTRLDOWNTXT = "CONTROL LANDING";
        public const string CTRLFLYTXT = "CONTROL FLY";
        public const string CTRLRUNTXT = "CONTROL INRUN";

        public int attempts;
        public TrainingExperience trainingExperience;
        [SerializeReference] protected JumperInfoGlobal jumper;
        [SerializeField] protected string description;
        protected SceneObjects sceneObjects;
        [SerializeReference] protected SkiJumpInfoGlobal skiJumpInfo;
        protected DataHolder dataHolder;
        protected int attemptEnded = 0;
        protected bool isEnd = false;
        protected float result;

        private SkiJumpInfoGlobal[] skiJumpInfos;

        public string Name => name;
        public GameObject GameObject => gameObject;

        public abstract (float, bool) DiffToBeat();

        public abstract void SetResult(PointsStat stats, bool isShowResult = true);

        /// <summary>
        /// Restart class to show it in menu list
        /// </summary>
        public abstract void Refresh();
        public static ITrainingController instance;

        protected virtual void Awake()
        {
            if (instance != null && instance != this) { 
                Destroy(gameObject);
                return;
            }

            DontDestroyOnLoad(gameObject);
            instance = this;
            loadJump();

            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void loadJump()
        {
            //string folder = "Assets/Resources/SkiJump";
            skiJumpInfos = Resources.LoadAll<SkiJumpInfoGlobal>("SkiJump");
        }

        protected virtual void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        protected virtual void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene.buildIndex == 0)
                return;
            GameObject tmp = GameObject.FindGameObjectsWithTag("SceneData").FirstOrDefault();
            if (tmp == null)
            {
                Debug.LogError("Not Found SceneData");
                return;
            }
            sceneObjects = tmp.GetComponent<SceneObjects>();
            skiJumpInfo = sceneObjects.skijumpInfo;

            dataHolder = GameObject.Find("DataHolder").GetComponent<DataHolder>();
            if (dataHolder == null)
            {
                Debug.LogError("Not Found DataHolder");
                return;
            }

            setData();
        }

        protected virtual void setData()
        {
            sceneObjects.pointsScript.enabled = false;
            sceneObjects.player.SetActive(false);
            sceneObjects.jumpResult.HideStat();
            sceneObjects.jumperOverlay.Show(jumper, attemptEnded + 1);
            StartCoroutine(HideInfo());
            sceneObjects.suits.clothe(dataHolder.playerSuit);
            sceneObjects.player.SetActive(true);
            sceneObjects.aIPlayer.SetActive(false);
            sceneObjects.flyPlayer.enabled = true;
            sceneObjects.cineCamera.Follow = sceneObjects.player.transform;
            sceneObjects.cineCamera.LookAt = sceneObjects.player.transform;
            sceneObjects.pointsScript.jumper = sceneObjects.player;
        }

        /// <summary>
        /// </summary>
        /// <returns>Return result as float beetwen 0 .. 1</returns>
        public float getResult()
        {
            return result;
        }

        protected void RandomHill()
        {
            if (skiJumpInfos == null || skiJumpInfos.Length == 0)
                loadJump();
            skiJumpInfo = skiJumpInfos[Random.Range(0, skiJumpInfos.Length)];
        }

        /// <summary>
        /// Set variable to start new game
        /// </summary>
        /// <returns>Name of Ski Jump scene</returns>
        public virtual SkiJumpInfoGlobal Play()
        {
            attemptEnded = 0;
            return skiJumpInfo;
        }


        /// <summary>
        /// Do receive on trainingExperience class and other operation
        /// </summary>
        public virtual void ReceiveExperience()
        {
            trainingExperience.Receive(jumper, result);
            result = 0;
        }

        protected void ShowInfo(string text)
        {
            GameObject.FindGameObjectWithTag("UIPopUp").GetComponent<InfoPopUp>().Show(text);
        }

        /// <summary>
        /// Set UI menu item
        /// </summary>
        public virtual void ShowDetails(Text title, Text desc, Text attemp, GameObject SR, GameObject SF, GameObject SD, GameObject CR, GameObject CF, GameObject CD)
        {
            title.text = name;
            desc.text = description;
            attemp.text += attempts;

            SR.GetComponentInChildren<Text>().text = SWINGRUNTXT + " " + jumper.swingsFactorRun + " +" + trainingExperience.swingRUN;
            SR.GetComponentInChildren<Slider>().value = jumper.swingsFactorRun + trainingExperience.swingRUN;
            SF.GetComponentInChildren<Text>().text = SWINGFLYTXT + " " + jumper.swingsFactorFly + " +" + trainingExperience.swingFLY;
            SF.GetComponentInChildren<Slider>().value = jumper.swingsFactorFly + trainingExperience.swingFLY;
            SD.GetComponentInChildren<Text>().text = SWINGDOWNTXT + " " + jumper.swingsFactorDown + " +" + trainingExperience.swingDown;
            SD.GetComponentInChildren<Slider>().value = jumper.swingsFactorDown + trainingExperience.swingDown;
            CR.GetComponentInChildren<Text>().text = CTRLRUNTXT + " " + jumper.controlFactorRun + " +" + trainingExperience.ctrlRUN;
            CR.GetComponentInChildren<Slider>().value = jumper.controlFactorRun + trainingExperience.ctrlRUN;
            CF.GetComponentInChildren<Text>().text = CTRLFLYTXT + " " + jumper.controlFactorFly + " +" + trainingExperience.ctrlFLY;
            CF.GetComponentInChildren<Slider>().value = jumper.controlFactorFly + trainingExperience.ctrlFLY;
            CD.GetComponentInChildren<Text>().text = CTRLDOWNTXT + " " + jumper.controlFactorDown + " +" + trainingExperience.ctrlDown;
            CD.GetComponentInChildren<Slider>().value = jumper.controlFactorDown + trainingExperience.ctrlDown;
        }

        protected IEnumerator StartNext()
        {
            yield return new WaitForSeconds(3);
            if (isEnd || attemptEnded >= attempts)
            {
                //Refresh();
                isEnd = false;
                SceneManager.LoadScene(0);
            }
            else
                setData();
        }

        protected IEnumerator HideInfo()
        {
            yield return new WaitForSeconds(3);
            sceneObjects.jumperOverlay.Hide();
        }

        public int getDifLevel()
        {
            return jumper.getLevel();
        }

    }

    [System.Serializable]
    public class TrainingExperience
    {
        public float swingRUN, swingFLY, swingDown, ctrlRUN, ctrlFLY, ctrlDown;

        public void Receive(JumperInfoGlobal jumperInfo, float factor)
        {
            jumperInfo.setCtrlDown(jumperInfo.controlFactorDown + factor * ctrlDown);
            jumperInfo.setCtrlFly(jumperInfo.controlFactorFly + factor * ctrlFLY);
            jumperInfo.setCtrlRun(jumperInfo.controlFactorRun + factor * ctrlRUN);
            jumperInfo.setSwingDown(jumperInfo.swingsFactorDown + factor * swingDown);
            jumperInfo.setSwingFly(jumperInfo.swingsFactorFly + factor * swingFLY);
            jumperInfo.setSwingRun(jumperInfo.swingsFactorRun + factor * swingRUN);
        }
    }
}