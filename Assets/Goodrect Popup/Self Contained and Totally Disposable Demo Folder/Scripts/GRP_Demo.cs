using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GRP_Demo : MonoBehaviour
{
    private string user;
    private GRP_PromptManager promptManager;
    private GRP_MultichoiceManager multichoiceManager;

    public Transform staticAnchor;
    public Transform worldAnchor;
    public Transform fullscreenAnchor;

    public MouseLook ml;
    public Transform charPos;

    public GameObject mobileControls;

    [Header("World Popups")]
    public GRP_MultichoiceManager worldMC;
    private GRP_MultichoiceManager worldMCClone;
    public Transform worldFollowY;
    public Transform worldFixed;

    [Header("Static & Fullscreen")]
    public GRP_MultichoiceManager static_fs_popup;

    [Header("Ball Cannon")]
    public GRP_PromptManager ballCannonPrompt;
    GRP_MultichoiceManager ballCannonMC;
    public Transform ballCannon;
    public GameObject cubePF;
    public GameObject spherePF;
    public Sprite cubeImage;
    public Sprite sphereImage;

    private bool disableOrbs = false;

    // Start is called before the first frame update
    void Start()
    {
        promptManager = GetComponent<GRP_PromptManager>();
        multichoiceManager = GetComponent<GRP_MultichoiceManager>();

        worldMCClone = Instantiate(worldMC.gameObject).GetComponent<GRP_MultichoiceManager>();

        ToggleControls(false);

        Greet();

        ballCannonPrompt.title = "How many objects would you like to generate?";
        ballCannonPrompt.message = "Number of objects to launch";
        ballCannonPrompt.SetReturn("Choose Shape", ChooseShape);

        ballCannonMC = ballCannonPrompt.gameObject.GetComponent<GRP_MultichoiceManager>();
        ballCannonMC.Match(ballCannonPrompt);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !disableOrbs)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 3, 1<<9))
            {
                OrbTrigger(hit.transform.name);
            }
        }
    }

    private void OrbTrigger(string orbname, bool first = true)
    {
        disableOrbs = true;

        if (orbname == "ballPool")
        {
            BeginCopyPopupTutorial();
            ballCannonMC.Create();
            return;
        }

        static_fs_popup.ResetButtons();

        if (orbname == "fullscreenOrb")
        {
            ToggleControls(false);
            static_fs_popup.title = "This is a Fullscreen Popup";
            static_fs_popup.message = "Fullscreen mode defines the container area as fullscreen minus the padding values.\n\n";

            if(!first)
            {
                static_fs_popup.fullscreenLeft = static_fs_popup.fullscreenRight = Mathf.RoundToInt(Random.Range(0.1f, 0.3f) * Screen.width);
                static_fs_popup.fullscreenTop = static_fs_popup.fullscreenBottom = Mathf.RoundToInt(Random.Range(0.05f, 0.2f) * Screen.height);
            }

            static_fs_popup.message += "This one's paddings are\n" + static_fs_popup.fullscreenTop + "px vertically, "
                + static_fs_popup.fullscreenLeft + "px horizontally.";

            static_fs_popup.placement = Placement.Fullscreen;
        }
        else if (orbname == "staticOrb")
        {
            ToggleControls(false);
            static_fs_popup.title = "This is a Static Popup";
            static_fs_popup.message = "Static mode defines the container area by the width, height and anchor position.\n\n";
            static_fs_popup.placement = Placement.Static;

            if (!first)
            {
                static_fs_popup.staticWidth = Mathf.RoundToInt(Random.Range(0.3f, 0.5f) * Screen.width);
                static_fs_popup.staticHeight = Mathf.RoundToInt(Random.Range(0.3f, 0.5f) * Screen.height);

                int randomAnchor = Random.Range(0, 9);

                if(randomAnchor == 0) {
                    static_fs_popup.staticAnchor = Anchor.bottomLeft;
                    static_fs_popup.staticX = static_fs_popup.staticWidth / 2 + 100;
                    static_fs_popup.staticY = static_fs_popup.staticHeight / 2 + 100;
                }
                else if (randomAnchor == 1)
                {
                    static_fs_popup.staticAnchor = Anchor.bottomCenter;
                    static_fs_popup.staticX = 0;
                    static_fs_popup.staticY = static_fs_popup.staticHeight / 2 + 100;
                }
                else if (randomAnchor == 2)
                {
                    static_fs_popup.staticAnchor = Anchor.bottomRight;
                    static_fs_popup.staticX = -static_fs_popup.staticWidth / 2 - 100;
                    static_fs_popup.staticY = static_fs_popup.staticHeight / 2 + 100;
                }
                else if (randomAnchor == 3)
                {
                    static_fs_popup.staticAnchor = Anchor.middleLeft;
                    static_fs_popup.staticX = static_fs_popup.staticWidth / 2 + 100;
                    static_fs_popup.staticY = 0;
                }
                else if (randomAnchor == 4)
                {
                    static_fs_popup.staticAnchor = Anchor.center;
                    static_fs_popup.staticX = 0;
                    static_fs_popup.staticY = 0;
                }
                else if (randomAnchor == 5)
                {
                    static_fs_popup.staticAnchor = Anchor.middleRight;
                    static_fs_popup.staticX = -static_fs_popup.staticWidth / 2 - 100;
                    static_fs_popup.staticY = 0;
                }
                else if (randomAnchor == 6)
                {
                    static_fs_popup.staticAnchor = Anchor.topLeft;
                    static_fs_popup.staticX = static_fs_popup.staticWidth / 2 + 100;
                    static_fs_popup.staticY = -static_fs_popup.staticHeight / 2 - 100;
                }
                else if (randomAnchor == 7)
                {
                    static_fs_popup.staticAnchor = Anchor.topCenter;
                    static_fs_popup.staticX = 0;
                    static_fs_popup.staticY = -static_fs_popup.staticHeight / 2 - 100;
                }
                else if (randomAnchor == 8)
                {
                    static_fs_popup.staticAnchor = Anchor.topRight;
                    static_fs_popup.staticX = -static_fs_popup.staticWidth / 2 - 100;
                    static_fs_popup.staticY = -static_fs_popup.staticHeight / 2 - 100;
                }
            }

            static_fs_popup.message += "This one's size is " + static_fs_popup.staticWidth + "x" + static_fs_popup.staticHeight;
            static_fs_popup.message += "px and anchored at the ";

                 if (static_fs_popup.staticAnchor == Anchor.bottomLeft  ) static_fs_popup.message += "bottom left corner.";
            else if (static_fs_popup.staticAnchor == Anchor.bottomCenter) static_fs_popup.message += "bottom.";
            else if (static_fs_popup.staticAnchor == Anchor.bottomRight ) static_fs_popup.message += "bottom right corner.";
            else if (static_fs_popup.staticAnchor == Anchor.middleLeft  ) static_fs_popup.message += "middle left.";
            else if (static_fs_popup.staticAnchor == Anchor.center      ) static_fs_popup.message += "center.";
            else if (static_fs_popup.staticAnchor == Anchor.middleRight ) static_fs_popup.message += "middle right.";
            else if (static_fs_popup.staticAnchor == Anchor.topLeft     ) static_fs_popup.message += "top left corner.";
            else if (static_fs_popup.staticAnchor == Anchor.topCenter   ) static_fs_popup.message += "top.";
            else if (static_fs_popup.staticAnchor == Anchor.topRight    ) static_fs_popup.message += "top right corner.";
        }

        static_fs_popup.message += "\n\nGo ahead and create some random variations.";
        static_fs_popup.SetReturn("I've seen enough", () => { disableOrbs = false; ToggleControls(true); });
        static_fs_popup.AddButton("More!", () => { OrbTrigger(orbname, false); });
        static_fs_popup.Create();
    }

    private void ToggleControls(bool state)
    {
        ml.isEnabled = state;
        if (mobileControls != null)
            mobileControls.SetActive(state);

        if(!state && ml.isMobile)
        {
            ml.ManualLook(0, 0);
            ml.MV(0);
            ml.MH(0);
            ml.MJ(0);
        }
    }

    private void BeginCopyPopupTutorial()
    {
        ballCannonMC.ResetButtons();
        ballCannonMC.showReturn = true;
        ballCannonMC.title = "Copying Popups";
        ballCannonMC.message = "You might create a prompt and need a matching multichoice popup, or vice versa.";
        ballCannonMC.message += "\n\nUse \"Match\" function for that. This popup is styled by matching the style of the upcoming number prompt.";
        ballCannonMC.message += "\n\nSpeaking of, prompt types!";
        ballCannonMC.buttonSeparator = 15;
        ballCannonMC.titleSeparator = 90;
        ballCannonMC.SetReturn("Let's party", PartyTime);
    }

    private void PartyTime() {
        StartCoroutine("LookAt", ballCannon.position);
        ballCannonPrompt.Create(true);
    }

    private bool isSphere;
    private int ballCount;

    private void ChooseShape(string count) {
        if (count == "")
            return;

        ballCount = int.Parse(count);

        string bodyText = "Also, check these fancy buttons out.\n\n";
        bodyText += "Buttons can use text and images as a label.";

        ballCannonMC.Initialize("Choose a Shape", bodyText);
        ballCannonMC.ResetButtons();
        ballCannonMC.AddButton(cubeImage,   () => { isSphere = false; LaunchBalls(); });
        ballCannonMC.AddButton(sphereImage, () => { isSphere =  true; LaunchBalls(); });
        ballCannonMC.showReturn = false;
        ballCannonMC.buttonSeparator = 50;
        ballCannonMC.Create();
    }

    private int ballCounter;
    private void LaunchBalls()
    {
        disableOrbs = false;

        ballCounter = 0;
        for(int i = 0; i < ballCount; i++)
        {
            Invoke("InstantiateBall", i * 0.05f);
        }
    }

    private void InstantiateBall()
    {
        GameObject go;
        if (isSphere)
            go = Instantiate(spherePF);
        else
            go = Instantiate(cubePF);

        go.transform.position = Clone(ballCannon.position);
        go.GetComponent<Rigidbody>().velocity = new Vector3(Random.Range(-10, 10), Random.Range(-10, 10), Random.Range(-10, 10));
        go.GetComponent<Renderer>().material.SetColor("_Color", HSB(Random.Range(0f, 1f), 0.4f, 1f));
        go.name = "ball_" + ++ballCounter;
    }

    private void SetName(string user)
    {
        this.user = user;
        Invoke("WelcomeUser", .5f);
    }

    private void Greet()
    {
        promptManager.ResetButtons();
        promptManager.Initialize("Hello!", "What's your name?\n(It doesn't really matter, I'm just showing off.)");
        promptManager.SetReturn("Continue", SetName);

        promptManager.Create(true);
    }

    private void WelcomeUser()
    {
        multichoiceManager.ResetButtons();
        multichoiceManager.Initialize("Hi there, " + user, "You've just used a dynamically created prompt.\nLet's walk around and see some examples.");
        multichoiceManager.returnPosition = ReturnPosition.Last;
        multichoiceManager.buttonDirection = ButtonDirection.Vertical;

        multichoiceManager.buttonSeparator = 50;
        multichoiceManager.SetReturn("Let's go", () => { ToggleControls(true); });
        multichoiceManager.AddButton("I'm not " + user, Greet);
        multichoiceManager.Create();
    }

    private void OnTriggerEnter(Collider other)
    {
        ToggleControls(false);
        PlacementPrompt();
        Destroy(GetComponent<BoxCollider>());
    }

    private void PlacementPrompt() {
        ToggleControls(false);

        int prevWidth = multichoiceManager.flexibleWidth;

        multichoiceManager.ResetButtons();
        string bodyText = "You used a prompt to get information from the user.";
        bodyText += "\nAlso, you used a popup to trigger custom functions.";
        bodyText += "\n\nBoth were placed in \"Flexible\" mode.\n" + "Their width was " + prevWidth + "% of the screen, while this one is at 60%.\n\nLet's see other modes.";
        bodyText += "\n\nP.S. There is a little party going on at the center.";

        multichoiceManager.Initialize("So far...", bodyText);
        multichoiceManager.flexibleWidth = 60;

        multichoiceManager.buttonDirection = ButtonDirection.Horizontal;
        multichoiceManager.buttonSeparator = 20;

        multichoiceManager.showReturn = false;
        multichoiceManager.AddButton("Static", () => { StartCoroutine("LookAt", staticAnchor.position); });
        multichoiceManager.AddButton("World", () => { StartCoroutine("LookAt", worldAnchor.position); });
        multichoiceManager.AddButton("Fullscreen", () => { StartCoroutine("LookAt", fullscreenAnchor.position); });

        multichoiceManager.Create();
    }

    private IEnumerator LookAt(Vector3 target) {
        if (ml.isMobile)
            ml.ResetMobileOffset();

        GameObject hackyGazeFix = new GameObject();
        Transform hft = hackyGazeFix.transform;

        hft.position = Clone(charPos.position);

        Vector3 sourceEA = new Vector3(ml.offset.x, ml.offset.y, 0);
        hft.LookAt(target);
        Vector3 targetEA = new Vector3(hft.eulerAngles.x - ml.baseX, hft.eulerAngles.y - ml.baseY, 0);

        Destroy(hackyGazeFix);

        Vector3 ea;

        for (float i = 0; i < 1.0; i += 0.03f)
        {
            ea = Lerp(sourceEA, targetEA, i);

            float ox = ea.x;
            float oy = ea.y;

            if (ox > 180) ox -= 360;
            if (oy > 180) oy -= 360;

            ml.offset = new Vector2(ox, oy);
            yield return null;
        }

        ToggleControls(true);
    
        yield return null;
    }

    private Vector3 Clone(Vector3 source)
    {
        float x = source.x;
        float y = source.y;
        float z = source.z;

        return new Vector3(x, y, z);
    }

    private Vector3 Lerp(Vector3 source, Vector3 dest, float t)
    {
        float sx = source.x;
        float sy = source.y;
        float sz = source.z;

        float dx = dest.x;
        float dy = dest.y;
        float dz = dest.z;

        if (dx - sx > 180) dx -= 360;
        if (dy - sy > 180) dy -= 360;
        if (dz - sz > 180) dz -= 360;

        float x = sx * (1 - t) + dx * t;
        float y = sy * (1 - t) + dy * t;
        float z = sz * (1 - t) + dz * t;

        return new Vector3(x, y, z);
    }

    //World popups


    public void CreateFollowOnY() {
        StartCoroutine("LookAt", worldFollowY.position);
        worldMCClone.title = "This one follows on Y";
        worldMCClone.message = "Move around, jump around, see the difference.";

        worldMCClone.ResetButtons();
        worldMCClone.showReturn = false;
        worldMCClone.AddButton("Don't follow", CreateDontFollow);
        worldMCClone.AddButton("Follow on all axes", CreateFollowOnAll);

        worldMCClone.worldFollowAxes = new Vector3Bool(0, 1, 0);
        worldMCClone.worldHeight = 600;
        worldMCClone.buttonSeparation = 30;
        worldMCClone.buttonSeparator = 50;
        worldMCClone.container = worldFollowY;
        worldMCClone.buttonDirection = ButtonDirection.Vertical;

        worldMCClone.Create();
    }

    private void CreateDontFollow()
    {
        StartCoroutine("LookAt", worldFixed.position);
        worldMCClone.title = "This one is fixed on all axes";

        worldMCClone.ResetButtons();
        worldMCClone.showReturn = false;
        worldMCClone.AddButton("Follow on Y axis", CreateFollowOnY);
        worldMCClone.AddButton("Follow on all axes", CreateFollowOnAll);

        worldMCClone.worldFollowAxes = new Vector3Bool(0, 0, 0);
        worldMCClone.worldHeight = 600;
        worldMCClone.worldRotation = new Vector3(0,-150,0);
        worldMCClone.buttonSeparation = 30;
        worldMCClone.buttonSeparator = 50;
        worldMCClone.container = worldFixed;
        worldMCClone.buttonDirection = ButtonDirection.Vertical;

        worldMCClone.Create();
    }

    private void CreateFollowOnAll()
    {
        StartCoroutine("LookAt", worldAnchor.position);
        worldMC.Create();
    }

    private static Color HSB(float hue, float saturation, float brightness)
    {
        float r = 0, g = 0, b = 0;
        if (saturation == 0)
        {
            r = g = b = (int)(brightness * 255.0f + 0.5f);
        }
        else
        {
            float h = (hue - Mathf.Floor(hue)) * 6.0f;
            float f = h - Mathf.Floor(h);
            float p = brightness * (1.0f - saturation);
            float q = brightness * (1.0f - saturation * f);
            float t = brightness * (1.0f - (saturation * (1.0f - f)));

            switch ((int)h)
            {
                case 0: r = brightness; g = t; b = p; break;
                case 1: r = q; g = brightness; b = p; break;
                case 2: r = p; g = brightness; b = t; break;
                case 3: r = p; g = q; b = brightness; break;
                case 4: r = t; g = p; b = brightness; break;
                case 5: r = brightness; g = p; b = q; break;
            }
        }
        return new Color(r, g, b);
    }
}

