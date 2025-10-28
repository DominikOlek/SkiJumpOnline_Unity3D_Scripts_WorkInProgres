using Assets.Scripts.ImportData;
using Assets.Scripts.Jumping.StaticInfo;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Assets.Scripts.Competition;
using Assets.Scripts.Competition.Other;
using Assets.Scripts.Competition.Controllers;
using Assets.Scripts.Menu.EventCtrl;
using Assets.Scripts.Menu;


namespace Assets.Scripts.Tournaments
{
    [CreateAssetMenu(fileName = "Tournament", menuName = "Tournaments/Tournament")]
    public class NormalTournament : ScriptableObject
    {
        public string folderName = "Tournaments";
        public Color color;
        [SerializeField] private String description,timestamp;
        public String Name => name;
        [SerializeField] public List<NormalTournament> tournamentsList = new List<NormalTournament>();
        [SerializeField] private List<CompetitionInTournament> competitions;
        public List<CompetitionInTournament> Competitions => competitions;
        [HideInInspector] public List<PlayerScore> ranking;
        [SerializeField] private float[] pointsArray,incomeArray;
        [SerializeField] private int[] dificult;
        [SerializeField] private JumperInfoGlobal player;
        [SerializeField] public bool isLockable = false;
        AIList aiList;
        CountryList countryList;
        DataHolder dataHolder;
        private int playerPosition = 0;
        private int competitionNumber = 0;
        public int getCurCompetitionId => competitionNumber;
        public bool isFinished = false;

        public bool isGroup;
        private int minGroupSize;
        private int maxGroupSize;
        [SerializeField] float divideEarnInCompetition = 100;

        public void ResetInstance(List<CompetitionInTournament> competitions_)
        {
            ranking = new List<PlayerScore>();
            tournamentsList.Clear();
            this.competitions = competitions_.Select(a => (CompetitionInTournament)a.Clone()).ToList();
        }

        public void AwakeTick()
        {
            aiList = GameObject.FindWithTag("ImportController").GetComponent<AIList>();
            countryList = GameObject.FindWithTag("ImportController").GetComponent<CountryList>();
            dataHolder = GameObject.Find("DataHolder").GetComponent<DataHolder>();
            //SceneManager.sceneLoaded += OnSceneLoaded;
            if (!tournamentsList.Contains(this)) { 
                tournamentsList.Add(this);
            }

            GetFromFile();

            if (SceneManager.GetActiveScene().buildIndex != 0 || dataHolder.tournament != this)
                return;

            if (competitionNumber < competitions.Count() && competitions[competitionNumber].competitionInfo.instance != null)
            {
                AddResults(competitions[competitionNumber].competitionInfo.competitionControler.GetResult());
                competitions[competitionNumber].competitionInfo.DisableCompetition();
                competitionNumber++;
            }
            foreach (var tour in tournamentsList)
            {
                if (tour.isFinished || tour.competitionNumber >= tour.competitions.Count())
                {
                    tour.EndTournament();
                }
            }
            dataHolder.tournament = null;
        }

        //private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        //{
        //    if (scene.buildIndex != 0 || dataHolder.tournament != this)
        //        return;


        //    if(competitionNumber < competitions.Count() && competitions[competitionNumber].competitionInfo.instance != null)
        //    {
        //        AddResults(competitions[competitionNumber].competitionInfo.competitionControler.GetResult());
        //        competitions[competitionNumber].competitionInfo.DisableCompetition();
        //        competitionNumber++;
        //    }
        //    foreach(var tour in tournamentsList)
        //    {
        //        if (tour.isFinished || tour.competitionNumber >= tour.competitions.Count())
        //        {
        //            tour.EndTournament();
        //        }
        //    }

        //}

        private void OnValidate()
        {
#if UNITY_EDITOR
            for (int i = 0; i < dificult.Length; i++)
            {
                if (dificult[i]<1)
                    dificult[i] = 1;
                if(dificult[i]>3)
                    dificult[i] = 3;
            }
            string folder = "Assets/Tournaments/"+name;
            int j = 0;
            int k = 0;
            string[] tmp = AssetDatabase.GetAssetPath(this).Split("/");
            if (tmp.Length == 3)
            {
                foreach (var comp in competitions.ToArray())
                {
                    j = 0;
                    //if (comp.competitionInfo.competitionControlerObject.GetComponent<ICompetitionControler>().isGroup != isGroup)
                    //{
                    //    competitions.RemoveAt(k);
                    //    Debug.LogError("Competition " + comp.competitionInfo.name + " dla " + name + " jest b³êdny");
                    //    continue;
                    //}
                    bool isCompGroup = comp.competitionInfo.groupSize > 1;
                    k++;
                    foreach (var extra in comp.extraTournaments.ToArray())
                    {
                        if (extra != this && extra != null && (!AssetDatabase.GetAssetPath(extra).Equals(folder + "/" + extra.name + ".asset") || extra.isGroup != isCompGroup))
                        {
                            Debug.LogError("Tournament " + extra.name + " dla " + name + " jest b³êdny");
                            comp.extraTournaments[j] = null;
                        }
                        j++;
                    }
                }
            }
#endif
        } 

        public bool IsAvailable()
        {
            if(!isLockable) return true;
            return player.isAvailableLevel(dificult.Max());
        }

        public void addTournament(NormalTournament tournament)
        {
            tournamentsList.Add(tournament);
            competitions.AddRange(tournament.Competitions);
        }

        public void StartTournament()
        {
            minGroupSize = Math.Max(competitions.Min(a => a.competitionInfo.groupSize),2);
            maxGroupSize = Math.Max(competitions.Max(a => a.competitionInfo.groupSize),2);
            isFinished = false;

            if(aiList == null)
                aiList = GameObject.FindWithTag("ImportController").GetComponent<AIList>();
            if (countryList == null)
                countryList = GameObject.FindWithTag("ImportController").GetComponent<CountryList>();

            var tmp = aiList.AIJumpers.Select(a => a.Value).Where(a => dificult.Contains(a.Level));
            foreach (var tour in tournamentsList)
            {
                tour.ranking.Clear();
                if (!tour.isGroup)
                {
                    foreach (var comp in tmp)
                    {
                        tour.ranking.Add(new PlayerScore(0,comp.Number,null,1));
                    }
                    tour.ranking.Add(new PlayerScore(0,0,null,1));
                    tour.playerPosition = tour.ranking.Count();
                }
                else
                {
                    int i = 0;
                    foreach(var comp in countryList.countries.Keys)
                    {
                        if (aiList.JumpersByCountry.ContainsKey(countryList.getCountry(comp).id) && aiList.JumpersByCountry[countryList.getCountry(comp).id].Count() >= minGroupSize)
                        {
                            i++;
                            tour.ranking.Add(new PlayerScore(
                                0,
                                countryList.getCountry(comp).id,
                                null
                            ));

                            if (comp == player.Country3dig)
                            {
                                tour.playerPosition = i;
                            }
                        }else if (player.Country3dig == comp)
                        {
                            Debug.Log("NIE DA SIE");
                        }
                    }
                }
                tour.ranking.Sort();
                tour.competitionNumber = 0;
                tour.CreateReference(true);
            }
            //playerPosition = ranking.FindIndex(a => a.competitor.Equals(player)) + 1;
            competitionNumber = 0;
            SaveToFile();
        }

        //void RunNext()
        //{
        //    List<Competitor> tmp = ranking.Select(a => { return a.GetCompetitor(); }).ToList();
        //    competitions[competitionNumber].competitionInfo.instance.GetComponent<ICompetitionControler>().SetStartList(tmp,dificult.Max());
        //    SceneManager.LoadScene("Scenes/" + competitions[competitionNumber].Name);
        //}

        public void AddResults(Dictionary<Competitor, CompetitorResult> fastReferenceResult)
        {
            foreach (var tour in tournamentsList)
            {
                if (tour != this && !competitions[competitionNumber].extraTournaments.Contains(tour))
                    continue;

                if (tour.isGroup != (competitions[competitionNumber].competitionInfo.groupSize > 1))
                    continue;

                foreach (var competitor in tour.ranking)
                {
                    if (competitor.GetCompetitor() == null)
                        continue;
                    if (fastReferenceResult.ContainsKey(competitor.GetCompetitor()))
                    {
                        if (tour.pointsArray == null || tour.pointsArray.Length == 0)
                        {
                            competitor.points += fastReferenceResult[competitor.GetCompetitor()].points;
                        }
                        else
                        {
                            try
                            {
                                competitor.points += tour.pointsArray.ElementAt(fastReferenceResult[competitor.GetCompetitor()].rank - 1);
                            }
                            catch (ArgumentOutOfRangeException)
                            {
                                competitor.points += 0;
                            }
                        }
                    }
                }
                tour.ranking.Sort();
            }
            playerPosition = ranking.FindIndex(a => !a.GetCompetitor().isAI())+1; //for UI
            int competitonplayerPosition = 0;
            if (competitions[competitionNumber].competitionInfo.groupSize > 1 != this.isGroup)
            {
                NormalTournament tmp = competitions[competitionNumber].extraTournaments[0];
                var p = tmp.ranking.Find(a => !a.GetCompetitor().isAI());
                competitonplayerPosition = fastReferenceResult[p.GetCompetitor()].rank;
                player.money += competitonplayerPosition - 1 < incomeArray.Length ? incomeArray[competitonplayerPosition - 1] / divideEarnInCompetition : 0;
            }
            else
            {
                competitonplayerPosition = fastReferenceResult[player].rank;
                player.money += competitonplayerPosition - 1 < incomeArray.Length ? incomeArray[competitonplayerPosition - 1] / divideEarnInCompetition : 0;
            }
            competitionNumber++;
            SaveToFile();
            competitionNumber--;
            ShowInfo($"You took {competitonplayerPosition} place in the last competition. \r\nAS A RESULT YOU EARNED {(competitonplayerPosition - 1 < incomeArray.Length ? incomeArray[competitonplayerPosition - 1] / divideEarnInCompetition : 0)}.");
        }

        public void SaveToFile()
        {
            foreach (var tour in tournamentsList) {
                TournamentInfo info = new TournamentInfo(tour.ranking, tour.competitionNumber,tour.isGroup ? tour.minGroupSize:1,tour.pointsArray,tour.incomeArray);
                info.SaveToFile(Path.Combine(Application.streamingAssetsPath, folderName, name, tour.Name) + ".json");
            }
        }

        public void GetFromFile()
        {
            minGroupSize = Math.Max(competitions.Min(a => a.competitionInfo.groupSize), 2);
            foreach (var tour in tournamentsList)
            {
                string dir = Path.Combine(Application.streamingAssetsPath, folderName, name);
                if (!Directory.Exists(dir))
                {
                    tour.StartTournament();
                    continue;
                }
                string path = Path.Combine(dir,tour.Name) + ".json";
                if (!File.Exists(path))
                {
                    tour.StartTournament();
                    continue;
                }
                string data = File.ReadAllText(path);
                TournamentInfo info = JsonConvert.DeserializeObject<TournamentInfo>(data);
                tour.ranking = new List<PlayerScore>();
                foreach (var c in info.competitors)
                {
                    tour.ranking.Add(new PlayerScore(c.points, c.competitorID, null,info.groupSize));
                }
                tour.competitionNumber = info.nextCompetition;
                if (info.incomeArray != null && info.incomeArray.Length > 0)
                    tour.incomeArray = info.incomeArray;
                if (info.pointsArray != null && info.incomeArray.Length > 0)
                    tour.pointsArray = info.pointsArray;
            }

            CreateReference(true);
            playerPosition = ranking.FindIndex(a => !a.GetCompetitor().isAI()) + 1;

        }

        public void ShowDetails(Text title,Text desc,Text time,Slider dif,Slider progress,GameObject prizeList,GameObject prizePref)
        {
            foreach (Transform child in prizeList.transform) { 
                Destroy(child.gameObject);
            }
            title.text = name;
            title.color = new Color(99/256f,70/256f,209 / 256f);
            if (!IsAvailable())
            {
                title.text += " (LOCK)";
                title.color = new Color(230 / 256f, 18 / 256f, 0 / 256f);
                title.GetComponent<HoverInfo>().enabled = true;
            }
            else
            {
                title.GetComponent<HoverInfo>().enabled = false;
            }
            desc.text = description;
            time.text = $"IS REQUIRED: {(isLockable ? "YES": "NO")} \n\n"+ "TIME STAMP: \n" +timestamp;
            dif.value = dificult.Max();
            progress.maxValue = competitions.Count();
            progress.value = competitionNumber;
            for (int i=0;i<incomeArray.Length;i++) {
                GameObject tmp = GameObject.Instantiate(prizePref);
                tmp.transform.SetParent(prizeList.transform,false);
                tmp.transform.localScale = Vector3.one;
                tmp.GetComponent<Text>().text = i+1 + " - " + incomeArray[i];
            }
        }

        public void ShowPage(Text nextCompetition, GameObject position, GameObject progress)
        {
            if (isFinished)
            {
                nextCompetition.text = "\r\nTOURNAMENT FINISED \r\n\r\n Press Play to Restart";
            }
            else
            {
                nextCompetition.text = "NEXT COMPETITION:\r" +
                    $"\n{competitions[competitionNumber].Name}\r\n\r\nRECORD: {competitions[competitionNumber].Record}\n HS: {competitions[competitionNumber].HS}\r\n\r" +
                    "\nEXTRA INFO:\r\n" + competitions[competitionNumber].competitionInfo.shortInfo + "\r\n\r\nTOURNAMENTS:\r\n" + $"<color=#{UnityEngine.ColorUtility.ToHtmlStringRGB(color)}>" + Name + "</color> " + "\r\n";
                foreach (var item in competitions[competitionNumber].extraTournaments)
                {
                    nextCompetition.text += $"<color=#{UnityEngine.ColorUtility.ToHtmlStringRGB(item.color)}>" + item.Name + "</color> " + "\r\n";
                }
            }

            Slider posSlid = position.GetComponentInChildren<Slider>();
            Slider progSlid = progress.GetComponentInChildren<Slider>();
            posSlid.maxValue = ranking.Count();
            posSlid.value = playerPosition;
            progSlid.maxValue = competitions.Count();
            progSlid.value = competitionNumber;
            position.transform.Find("CUR").GetComponent<Text>().text = playerPosition.ToString();
            position.transform.Find("MAX").GetComponent<Text>().text = ranking.Count().ToString();
            progress.transform.Find("CUR").GetComponent<Text>().text = competitionNumber.ToString();
            progress.transform.Find("MAX").GetComponent<Text>().text = competitions.Count() + "";
        }

        void EndTournament()
        {
            isFinished = true;
            for (int i = 0; i < Mathf.Min(ranking.Count, incomeArray.Length); i++) {
                //if (ranking[i].GetCompetitor().Equals(player))
                if (!ranking[i].GetCompetitor().isAI())
                {
                    if (!isGroup)
                    {
                        player.money += i<incomeArray.Length ? incomeArray[i] : 0;
                        ShowInfo($"The {name} tournament has just ended. You took {playerPosition} place. \r\nAS A RESULT YOU EARNED {(i < incomeArray.Length ? incomeArray[i] : 0)}.");
                    }
                    else
                    {
                        player.money += i < incomeArray.Length ? incomeArray[i] / maxGroupSize:0;
                        ShowInfo($"The {name} tournament has just ended. Your team took {playerPosition} place. \r\nAS A RESULT YOU EARNED {(i < incomeArray.Length ? incomeArray[i] / maxGroupSize : 0)}.");
                    }

                    if (isLockable && i + 1 <= 3)
                    {
                        player.winLevel = dificult.Max();
                        ShowInfo($"You was in top 3 of last tournament, so now you can take part in tournaments with difficulty equals {player.winLevel}.");
                    }
                    break;
                }
            }
        }

        public CompetitionInTournament NextCompetition()
        {
            CompetitionSettings competition = competitions[competitionNumber].competitionInfo;
            CreateReference();
            competition.EnableCompetition();
            if (competition.groupSize > 1 != isGroup)
            {
                NormalTournament main = competitions[competitionNumber].extraTournaments.First();
                competition.competitionControler.SetStartList(main.ranking.Where(a => a.GroupSize == competition.groupSize).Select(a => a.GetCompetitor()).Reverse().ToList(), dificult.Max());
            }
            else
            {
                competition.competitionControler.SetStartList(ranking.Where(a => a.GroupSize == competition.groupSize).Select(a => a.GetCompetitor()).Reverse().ToList(), dificult.Max());
            }
            dataHolder.tournament = this;
            return competitions[competitionNumber];
        }

        public List<PlayerScore> GetRanking(int index)
        {
            if (index == 0)
            {
                return ranking;
            }
            else
            {
                return tournamentsList[index-1].ranking;
            }
        }
        public string GetTournamentName(int index)
        {
            if (index == 0)
            {
                return name;
            }
            else
            {
                return tournamentsList[index - 1].name;
            }
        }

        public List<CompetitionInTournament> GetTournamentCompetitions(int index)
        {
            if (index == 0)
            {
                return competitions.ToList();
            }
            else
            {
                return tournamentsList[index - 1].competitions.ToList();
            }
        }

        public void CreateReference(bool isInitial = false)
        {
            foreach (var tour in tournamentsList)
            {
                foreach (var jumper in tour.ranking)
                {
                    if(!isInitial)
                        jumper.createReference(aiList, player, countryList, dificult, tour.isGroup ? Mathf.Max(competitions[Mathf.Min(competitionNumber, competitions.Count() - 1)].competitionInfo.groupSize,minGroupSize) : 1);
                    else if(tour.isGroup) 
                        jumper.createReference(aiList, player, countryList, dificult, minGroupSize);
                    else
                        jumper.createReference(aiList, player, countryList, dificult, 1);
                }   
            }
        }

        public void ResetData(AIList aIList, Text nextCompetition, GameObject position, GameObject progress)
        {
            StartTournament();
            ShowPage(nextCompetition,position,progress);
        }

        private void ShowInfo(string text)
        {
            GameObject.FindGameObjectWithTag("UIPopUp").GetComponent<InfoPopUp>().Show(text);
        }
    }


    [Serializable]
    public class CompetitionInTournament : ICloneable
    {
        public SkiJumpInfoGlobal skiJump;
        public CompetitionSettings competitionInfo;
        public List<NormalTournament> extraTournaments = new List<NormalTournament>();
        public string Name => skiJump.name;
        public float Record => skiJump.record;
        public float HS => skiJump.HS;
        public float K => skiJump.K;

        public string Country => skiJump.country3Dig;

        public object Clone()
        {
            return new CompetitionInTournament
            {
                skiJump = this.skiJump,
                competitionInfo = this.competitionInfo,
                extraTournaments = new List<NormalTournament>()
            };
        }
    }

}
