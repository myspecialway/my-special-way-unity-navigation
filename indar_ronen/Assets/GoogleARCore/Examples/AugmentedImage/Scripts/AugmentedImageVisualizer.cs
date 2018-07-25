//-----------------------------------------------------------------------
// <copyright file="AugmentedImageVisualizer.cs" company="Google">
//
// Copyright 2018 Google Inc. All Rights Reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
// </copyright>
//-----------------------------------------------------------------------

namespace GoogleARCore.Examples.AugmentedImage
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using GoogleARCore;
    using GoogleARCoreInternal;
    using UnityEngine;

    /// <summary>
    public enum DestinationType
    {
        AtoB,
        AtoC,
        AtoD,
        AtoE
    }

    /// Uses 4 frame corner objects to visualize an AugmentedImage.
    /// </summary>
    public class AugmentedImageVisualizer : MonoBehaviour
    {
        public static DestinationType CurrentDestination = DestinationType.AtoE;

        [SerializeField] private GameObject _aToB;
        [SerializeField] private GameObject _bToC;
        [SerializeField] private GameObject _bToD;
        [SerializeField] private GameObject _dToE;
        
        [SerializeField] private GameObject _aLetter;
        [SerializeField] private GameObject _bLetter;
        [SerializeField] private GameObject _cLetter;
        [SerializeField] private GameObject _dLetter;
        [SerializeField] private GameObject _eLetter;
        
        /// <summary>
        /// The AugmentedImage to visualize.
        /// </summary>
        public AugmentedImage Image;

        /// <summary>
        /// A model for the lower left corner of the frame to place when an image is detected.
        /// </summary>
        public GameObject FrameLowerLeft;

        /// <summary>
        /// A model for the lower right corner of the frame to place when an image is detected.
        /// </summary>
        public GameObject FrameLowerRight;

        /// <summary>
        /// A model for the upper left corner of the frame to place when an image is detected.
        /// </summary>
        public GameObject FrameUpperLeft;

        /// <summary>
        /// A model for the upper right corner of the frame to place when an image is detected.
        /// </summary>
        public GameObject FrameUpperRight;

        public  GameObject plane5;
        public  GameObject plane6;
        public  GameObject plane7;
        public  GameObject plane8;
        public  GameObject plane9;
        public  GameObject plane10;
        public  GameObject plane11;
        public  GameObject plane12;
        public  GameObject Track;
        private int        Dist = 24;

        /// <summary>
        /// The Unity Update method.
        /// </summary>
        public void Update()
        {
            if (Image == null || Image.TrackingState != TrackingState.Tracking)
            {
                FrameLowerLeft.SetActive(false);
                FrameLowerRight.SetActive(false);
                FrameUpperLeft.SetActive(false);
                FrameUpperRight.SetActive(false);
                plane5.SetActive(false);
                plane6.SetActive(false);
                plane7.SetActive(false);
                plane8.SetActive(false);
                plane9.SetActive(false);
                plane10.SetActive(false);
                plane11.SetActive(false);
                plane12.SetActive(false);
                return;
            }

            //float halfWidth = Image.ExtentX / 2;
            //float halfHeight = Image.ExtentZ / 2;
            FrameLowerLeft.transform.localPosition  = (12 * Vector3.left)  + (12 * Vector3.back);
            FrameLowerRight.transform.localPosition = (12 * Vector3.right) + (12 * Vector3.back);
            FrameUpperLeft.transform.localPosition  = (12 * Vector3.left)  + (12 * Vector3.forward);
            FrameUpperRight.transform.localPosition = (12 * Vector3.right) + (12 * Vector3.forward);

            ActivatePathsAccordingToCurrentDestination();
//            plane5.transform.localPosition  = (Dist       * Vector3.forward);
//            plane6.transform.localPosition  = ((Dist * 2) * Vector3.forward);
//            plane7.transform.localPosition  = ((Dist * 3) * Vector3.forward);
//            plane8.transform.localPosition  = ((Dist * 4) * Vector3.forward);
//            plane9.transform.localPosition  = ((Dist * 5) * Vector3.forward);
//            plane10.transform.localPosition = ((Dist * 6) * Vector3.forward);
//            plane11.transform.localPosition = ((Dist * 7) * Vector3.forward);
//            plane12.transform.localPosition = ((Dist * 8) * Vector3.forward);

            FrameLowerLeft.SetActive(true);
            FrameLowerRight.SetActive(true);
            FrameUpperLeft.SetActive(true);
            FrameUpperRight.SetActive(true);
            //plane5.SetActive (true);
            //plane6.SetActive (true);
            //plane7.SetActive (true);
            //plane8.SetActive (true);
            //plane9.SetActive (true);
            //plane10.SetActive (true);
            //plane11.SetActive (true);
            //plane12.SetActive (true);
        }

        private void ActivatePathsAccordingToCurrentDestination()
        {
            switch (CurrentDestination)
            {
                case DestinationType.AtoB:
                    _aToB.SetActive(true);
                    _bToC.SetActive(false);
                    _bToD.SetActive(false);
                    _dToE.SetActive(false);
                    
                    _aLetter.SetActive(true);
                    _bLetter.SetActive(true);
                    _cLetter.SetActive(false);
                    _dLetter.SetActive(false);
                    _eLetter.SetActive(false);
                    break;
                case DestinationType.AtoC:
                    _aToB.SetActive(true);
                    _bToC.SetActive(true);
                    _bToD.SetActive(false);
                    _dToE.SetActive(false);
                    
                    _aLetter.SetActive(true);
                    _bLetter.SetActive(true);
                    _cLetter.SetActive(true);
                    _dLetter.SetActive(false);
                    _eLetter.SetActive(false);
                    break;
                case DestinationType.AtoD:
                    _aToB.SetActive(true);
                    _bToC.SetActive(false);
                    _bToD.SetActive(true);
                    _dToE.SetActive(false);
                    
                    _aLetter.SetActive(true);
                    _bLetter.SetActive(true);
                    _cLetter.SetActive(false);
                    _dLetter.SetActive(true);
                    _eLetter.SetActive(false);
                    break;
                case DestinationType.AtoE:
                    _aToB.SetActive(true);
                    _bToC.SetActive(false);
                    _bToD.SetActive(true);
                    _dToE.SetActive(true);
                    
                    _aLetter.SetActive(true);
                    _bLetter.SetActive(true);
                    _cLetter.SetActive(false);
                    _dLetter.SetActive(true);
                    _eLetter.SetActive(true);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}