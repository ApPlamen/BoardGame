using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour
{
    public bool isMobile = false;
    Vector2 mouseLook;
    Vector2 smoothV;

    public float sensitivity = 5f;
    public float smoothing = 5f;
    public float speed = 5f;

    public Vector2 offset;
    public float baseX, baseY;
    public bool isEnabled;

    private GameObject character;
    private Rigidbody rb;

    public float mxInput, myInput;
    public float mx, my;

    public float manualVertical, manualHorizontal, manualJump;

    // Start is called before the first frame update
    void Start()
    {
        character = transform.parent.gameObject;
        offset = new Vector2(0, -90);
        rb = character.GetComponent<Rigidbody>();
        mxInput = myInput = 0;

        mx = 0;
        my = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if(isEnabled)
        {
            if(Input.GetMouseButton(1) && !isMobile)
            {
                Vector2 mouseDir = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
                mouseDir = Vector2.Scale(mouseDir, new Vector2(sensitivity * smoothing, sensitivity * smoothing));

                smoothV.x = Mathf.Lerp(smoothV.x, mouseDir.x, 1f / smoothing);
                smoothV.y = Mathf.Lerp(smoothV.y, mouseDir.y, 1f / smoothing);

                mouseLook += smoothV;
            }

            float forward = (manualVertical + Input.GetAxis("Vertical")) * speed;
            float strafe =  (manualHorizontal + Input.GetAxis("Horizontal")) * speed;

            forward *= Time.deltaTime;
            strafe *= Time.deltaTime;

            character.transform.Translate(strafe, 0, forward);

            float jump = Input.GetAxis("Jump") + manualJump;

            if(jump >= 1)
                rb.velocity = new Vector3(0, 5, 0);
        }

        if (isMobile)
        {
            mx += mxInput;
            my += myInput;

            if (mx > 180) mx -= 360;

            if (mx > 60f) mx = 60f;
            if (mx < -60f) mx = -60f;

            transform.localEulerAngles = new Vector3(mx + offset.x, 0, 0);
            character.transform.eulerAngles = new Vector3(0, my + offset.y, 0);
        }
        else
        {
            transform.localRotation = Quaternion.AngleAxis(-mouseLook.y, Vector3.right);

            baseX = transform.localEulerAngles.x;
            float x = baseX + offset.x;
            x = x % 360;
            if (x > 180) x -= 360;

            if (x > 60f) x = 60f;
            if (x < -60f) x = -60f;

            transform.localEulerAngles = new Vector3(x, 0, 0);

            character.transform.localRotation = Quaternion.AngleAxis(mouseLook.x, character.transform.up);

            baseY = character.transform.eulerAngles.y;

            character.transform.eulerAngles = new Vector3(0, baseY + offset.y, 0);
        }

        if(Input.GetMouseButtonUp(0) && isMobile)
        {
            MV(0);
            MH(0);
            MJ(0);
        }
    }


    public void MV(float val) { manualVertical = val; }
    public void MH(float val) { manualHorizontal = val; }
    public void MJ(float val) { manualJump = val; }

    public void ManualLook(float mx, float my)
    {
        mxInput = mx;
        myInput = my;
    }

    public void ResetMobileOffset()
    {
        mx = transform.localEulerAngles.x;
        my = character.transform.eulerAngles.y + 90;
        offset = new Vector2(0, -90);
        baseX = transform.localEulerAngles.x;
        baseY = character.transform.eulerAngles.y + 90;
    }
}
