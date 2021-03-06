using System;
using UnityEngine;

//Class merged with DotHskDoorSlide.cs
public class Door : ButtonActivated
{
    public Animator[] _animator = new Animator[3];
    public AudioSource _doorSnd = null;
    private AudioSource activationSource = null;
    private AudioClip[] _sounds = new AudioClip[2];
    private bool _sndLoaded = false;
    private int _plaingSnd = -1; // 0-Open, 1-Close 
    private bool isOpen = false;
    public GameObject[] glassVariants = new GameObject[3]; //0 is offline, 1 is standby, 2 is activated
    private bool isPowered = false;

    public void OnPowered(bool status)
    {
        //Debug.Log("Door OnPowered() is being used!");

        if (status)
        {
            isPowered = true;
            DoorColor(1);
            activationSource.clip = onPowerClip;
            activationSource.Play();
        }
        else
        {
            isPowered = false;
            DoorColor(0);
            activationSource.clip = losePowerClip;
            activationSource.Play();
        }
    }

    public void DoorColor(int c) //Where c is the desired color door
    {
        //The end of the door close animation tried to make the glass red
        if (!isPowered && c == 1) 
            return;

        for (int i = 0; i < 3; i++)
        {
            //If the desired color door is being iterated over,
            if (i == c)
            {
                glassVariants[i].SetActive(true);
            }
            //If the unwanted doors are being iterated over,
            else
            {
                glassVariants[i].SetActive(false);
            }
        }
    }

    public void ToggleObj(GameObject playerObject)
    {
        DoorColor(2);

        if (isOpen)
        {
            isOpen = false;
            slide_door(1);
        }            
        else
        {
            isOpen = true;
            slide_door(0);
        }
    }

    public void Activation(GameObject playerObject, bool status)
    {
        //If the door is opened and it's trying to be closed
        if (isOpen && !status)
        {
            DoorColor(0);
            isOpen = false;
            slide_door(1);
        }
        //If the door is closed and it's trying to be opened
        else if (!isOpen && status)
        {
            DoorColor(2);
            isOpen = true;
            slide_door(0);
        }
    }

    //-------

    //From DotHskDoorSlide.cs
    private void Start()
    {
        //Debug.Log("Door.cs used Start()!");

        _sounds[0] = Resources.Load("Open_Sound") as AudioClip;
        _sounds[1] = Resources.Load("Close_Sound") as AudioClip;
        _sndLoaded = (_sounds[0] != null) && (_sounds[1] != null);
        if (!_sndLoaded)
        {
            //Debug.LogWarning("Silence mode:  audioclips \"Open_Sound\" and / or \"Close_Sound\" not found in the \"Resources\" directory");
        }

        activationSource = GetComponent<AudioSource>();

        ButtonActivatedInitialize();
    }

    //From DotHskDoorSlide.cs
    void slide_door(int _id)
    { // 0 - Open, 1 - Close
        string _anim = "Door_" + ((_id == 0) ? "Open" : "Close");

        //Debug.Log(_anim);

        if ((_animator[0] != null))
        {
            AnimatorStateInfo _st = _animator[0].GetCurrentAnimatorStateInfo(0);
            if (!_st.IsName(_anim))
            {
                float _time = _st.normalizedTime;
                _time = (_time < 1.0f && (_st.IsName("Door_Open") || _st.IsName("Door_Close"))) ? 1 - _time : 0.0f;
                if (_sndLoaded)
                {
                    float _timeSnd = 0.0f;
                    if (_doorSnd.isPlaying && (_id > 0) && (_plaingSnd != _id))
                    {
                        _timeSnd = _sounds[_id].length - _doorSnd.time;
                    }
                    _doorSnd.clip = _sounds[_id];
                    _doorSnd.time = _timeSnd;
                    _plaingSnd = _id;
                    _doorSnd.Play();
                }
                for (int i = 0; i < 3; i++)
                {
                    _animator[i].Play(_anim, -1, _time);
                }
            }
        }
    }
}
