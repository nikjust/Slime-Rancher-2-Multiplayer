﻿using Il2CppMonomiPark.SlimeRancher.Player.CharacterController;
using MelonLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SR2MP
{
    public class ReadData : MonoBehaviour
    {
        public ReadData(IntPtr ptr) : base(ptr) { }

        private SRCharacterController _Player;
        private Animator _PlayerAnimator;
        private SRCameraController _SRCameraController;
        private Animator _VacconeAnimator;

        //Movement
        private Vector3 _Position;
        private float _Rotation;

        //Animations
        private float _HM;
        private float _FM;
        private float _Yaw;
        private int _AS;
        private bool _Moving;
        private float _HS;
        private float _FS;

        //Vacpack
        private float _CameraAngle;
        private bool _VacMode;
        private bool _CachedVacMode;

        void Start()
        {
            _Player = Main.Instance.Player;
            _PlayerAnimator = Main.Instance.PlayerAnimator;
            _SRCameraController = _Player.cameraController;
            _VacconeAnimator = GameObject.Find("PlayerCameraKCC/First Person Objects/vac shape/Vaccone Prefab").GetComponent<Animator>();
        }

        void Update()
        {
            ReadVacconeState();
            if (_CachedVacMode != _VacMode)
            {
                SendData.SendVacconeState(_VacMode);
                _CachedVacMode = _VacMode;
            }
        }

        void FixedUpdate()
        {
            ReadMovement();
            SendData.SendMovement(_Position, _Rotation);

            ReadAnimations();
            SendData.SendAnimations(_HM, _FM, _Yaw, _AS, _Moving, _HS, _FS);

            ReadCameraAngle();
            SendData.SendCameraAngle(_CameraAngle);

            if (Main.Instance.SendActors)
            {
                SendData.SendSlimes();
            }
        }

        private void ReadMovement()
        {
            _Position = _Player.transform.position;
            _Rotation = _Player.transform.rotation.eulerAngles.y;
        }

        private void ReadAnimations()
        {
            _HM = _PlayerAnimator.GetFloat("HorizontalMovement");
            _FM = _PlayerAnimator.GetFloat("ForwardMovement");
            _Yaw = _PlayerAnimator.GetFloat("Yaw");
            _AS = _PlayerAnimator.GetInteger("AirborneState");
            _Moving = _PlayerAnimator.GetBool("Moving");
            _HS = _PlayerAnimator.GetFloat("HorizontalSpeed");
            _FS = _PlayerAnimator.GetFloat("ForwardSpeed");
        }

        private void ReadCameraAngle()
        {
            _CameraAngle = _SRCameraController.targetVerticalAngle;
        }

        private void ReadVacconeState()
        {
            _VacMode = _VacconeAnimator.GetBool("vacMode");
        }
    }
}
