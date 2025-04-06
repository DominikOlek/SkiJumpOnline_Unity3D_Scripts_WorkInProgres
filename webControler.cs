using UnityEngine;
using NativeWebSocket;
using System.Text;
using System.Threading.Tasks;
using SocketIOClient;
using System.Collections.Generic;
using System.Net.Sockets;
using System;
using SocketIOClient.Newtonsoft.Json;
using Newtonsoft.Json;
using UnityEngine.Playables;
using System.Collections.Concurrent;
using Assets.WebDTO;
public class WebControler : MonoBehaviour
{
    SocketIOUnity socket;
    //public GameObject cube;
    //public Vector3 pos = Vector3.zero;
    public Listener listener;
    public StarLevel startLevel;
    private ConcurrentQueue<Action> mainThreadActions = new ConcurrentQueue<Action>();
    private UiController uiController;

    void Start()
    {
        uiController = GameObject.Find("Canvas").GetComponent<UiController>();
        var uri = new Uri("http://127.0.0.1:5000");
        socket = new SocketIOUnity(uri, new SocketIOOptions
        {
            Query = new Dictionary<string, string>
        {
            {"token", "UNITY" }
        }
            ,
            Transport = SocketIOClient.Transport.TransportProtocol.WebSocket
        });
        socket.JsonSerializer = new NewtonsoftJsonSerializer();

        socket.OnConnected += (sender, e) =>
        {
            Debug.Log("Connection open!");      
        };

        socket.OnDisconnected += (sender, e) =>
        {
            Debug.Log("Connection close!");
        };

        socket.OnError += (sender, e) =>
        {
            Debug.Log("Connection error!"+e);
        };

        socket.On("omove", (response) => {
            mainThreadActions.Enqueue(() => {
                InfoFrame frame = response.GetValue<InfoFrame>();
                listener.addFrame(frame);
            });
        });


        socket.On("jump", (response) => {
            mainThreadActions.Enqueue(() =>
            {
                startLevel.SetStart();
                listener.Rec();
            });
        });

        socket.On("show", (response) => {
            mainThreadActions.Enqueue(() =>
            {
                startLevel.Restart();
                listener.Play();
            });
        });

        socket.On("THEEND", (response) => {
            mainThreadActions.Enqueue(() =>
            {
                var results = response.GetValue<List<List<object>>>();
                Dictionary<string,float> res = new Dictionary<string,float>();
                foreach(var el in results)
                {
                    res.Add(el[0].ToString(), Convert.ToSingle(el[1]));
                }
                startLevel.Restart();
                uiController.ShowResults(res);
            });
        });

        socket.On("otherStat", (response) => {
            mainThreadActions.Enqueue(() =>
            {
                PointsStat stat = response.GetValue<PointsStat>();
                uiController.ShowStat(stat);
            });
        });

    }

    async void StartKeepAlive()
    {
        while (socket.Connected)
        {
            await socket.EmitAsync("ping");
            await Task.Delay(25000); // Wysy³anie co 25 sekund
        }
    }

    public async void Connect()
    {
        await socket.ConnectAsync();

        StartKeepAlive();
    }
    // Update is called once per frame
    void Update()
    {
        // Przetwarzanie operacji w w¹tku g³ównym
        while (mainThreadActions.TryDequeue(out var action))
        {
            action?.Invoke();
        }
    }

    async void Send(string name, string message)
    {
        if (socket.Connected)
        {
            await socket.EmitStringAsJSONAsync(name, message);
        }
    }

    private async void OnApplicationQuit()
    {
        await socket.DisconnectAsync();
    }

    public void sendFrame(InfoFrame frame)
    {
        string json = JsonUtility.ToJson(frame);
        Send("move", json);
    }

    public void sendStat(PointsStat stat)
    {
        string json = JsonUtility.ToJson(stat);
        Send("stat", json);
    }

    public async void SendEnd()
    {
        await socket.EmitAsync("end","");
    }
}