using System;
using System.Linq;
using System.Threading.Tasks;
using Unity.Cinemachine;
using UnityEngine;

[Serializable]
public class InfoFrame
{
    public int ID;
    public Vector3 Position;
    public Quaternion Rotation;
    public float axisInRun;
    public float axisFly;
    public float axisFlySide;
    public float axisDown;
    public int animation;
    public int state;
}

public class Listener : MonoBehaviour
{
    public bool startShow = false;
    public WebControler webControler;

    private InfoFrame[] story = new InfoFrame[900];
    private float timer = 0.01f;
    [SerializeField] GameObject jumper,jumperAnim;
    private Animator animator;
    private Fly fly;
    private FlyAnim flyAnim;
    private InRunAnim inRunAnim;
    private LandingAnim landingAnim;
    private CameraCtrl camCtrl;
    private CinemachineSplineCart cart;

    int i = 0,j=0;
    public bool rep = true;

    float axisFly;
    float axisFlySide;
    float axisInRun;
    float axisDown;
    private void Start()
    {
        animator = jumperAnim.GetComponent<Animator>();
        fly = jumper.GetComponent<Fly>();
        flyAnim = jumperAnim.GetComponent<FlyAnim>();
        inRunAnim = jumperAnim.GetComponent<InRunAnim>();
        landingAnim = jumperAnim.GetComponent <LandingAnim>();
        camCtrl = gameObject.GetComponent<CameraCtrl>();
        cart = jumper.GetComponent<CinemachineSplineCart>();
    }

    void Update()
    {
        if (!rep)
        {
            if (fly.state > 0 && fly.speed > 0)
            {
                if (timer <= 0)
                {
                    story[i] = new InfoFrame()
                    {
                        ID = i,
                        state = fly.state,
                        Position = jumper.transform.position,
                        Rotation = jumper.transform.rotation,
                        animation = animator.GetCurrentAnimatorStateInfo(0).shortNameHash,
                        axisFly = flyAnim.getAxisSpeed(),
                        axisFlySide = flyAnim.getAxisSide(),
                        axisInRun = inRunAnim.getAxis(),
                        axisDown = landingAnim.getAxis()
                    };
                    timer = 0.01f;
                    webControler.sendFrame(story[i]);
                    i++;
                }
                else
                {
                    timer -= Time.deltaTime;
                }
            }else if(fly.state > 0 && fly.speed==0 && i> 50)
            {
                Task.Delay(3000);
                webControler.SendEnd();
                rep = true;
                startShow = false;
            }
        }
        else if(j<i-1 && startShow)
        {
            if (timer <= 0)
            {
                if (j-1>= 0 && story[j - 1].animation != story[j].animation || j==0)
                {
                    animator.CrossFade(story[j].animation, 0.001f);
                }
                if (j == 0)
                {
                    jumper.transform.rotation = story[j].Rotation;
                    jumper.transform.position = story[j].Position;
                }
                timer = 0.01f;
                j++;
            }
            else
            {
                timer -= Time.deltaTime;
            }
            jumper.transform.position = Vector3.Lerp(jumper.transform.position, story[j].Position,150*Time.deltaTime);
            jumper.transform.rotation = Quaternion.Lerp(jumper.transform.rotation, story[j].Rotation, 150* Time.deltaTime);
            axisFly = Mathf.Lerp(axisFly, story[j].axisFly,150*Time.deltaTime);
            axisFlySide = Mathf.Lerp(axisFlySide, story[j].axisFlySide, 150 * Time.deltaTime);
            axisInRun = Mathf.Lerp(axisInRun, story[j].axisInRun,150*Time.deltaTime);
            axisDown = Mathf.Lerp(axisDown, story[j].axisDown, 150 * Time.deltaTime);
        }
        else if (j==i-1 && startShow)
        {
            webControler.SendEnd();
            startShow = false;
        }
    }

    private void LateUpdate()
    {
        if (rep && startShow)
            if (story[j].state == 2)
                flyAnim.setAnim(axisFlySide, axisFly);
            else if (story[j].state == 1)
                inRunAnim.setAnim(axisInRun);
            else if (story[j].state == 3)
                landingAnim.setAnim(axisDown);
            if (j>35 && story[j-34].state == 2 && story[j-35].state == 1)
                camCtrl.RunCamTV2();
    }

    public void Play() {
        rep = true;
        startShow = false;
        fly.enabled = false;
        j = 0;
        i = 0;
        camCtrl.RunCamTV1();
        cart.enabled = false;
    }

    public void Rec()
    {
        rep = false;
        startShow = false;
        i = 0;
        j = 0;
        camCtrl.RunCamPov();
        cart.enabled = true;
    }

    public void addFrame(InfoFrame frame)
    {
        story[frame.ID] = frame;
        i++;
        if (i > 200)
        {
            startShow = true;
        }
    }

    public void Restart()
    {
        flyAnim.setAnim(0, 0);
        inRunAnim.setAnim(0);
        landingAnim.setAnim(0);
        Array.Clear(story, 0, i);
        i = 0;
        j=0;
        rep = false;
        camCtrl.RunCamPov();
    }
}
