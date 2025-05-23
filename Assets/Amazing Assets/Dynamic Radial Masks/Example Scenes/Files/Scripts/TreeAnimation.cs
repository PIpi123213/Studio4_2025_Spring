// Dynamic Radial Masks <https://u3d.as/1w0H>
// Copyright (c) Amazing Assets <https://amazingassets.world>
 
using UnityEngine;


namespace AmazingAssets.DynamicRadialMasks.Examples
{
    public class TreeAnimation : MonoBehaviour
    {
        public DRMController drmController;

        Animation anim;
        bool animationHasPlayed;


        void Start()
        {
            anim = GetComponent<Animation>();
        }

        void Update()
        {
            float maskValue = drmController.GetMaskValue(transform.position);


            if (maskValue > 0.4 && maskValue < 0.6f)
            {
                if (anim.isPlaying == false && animationHasPlayed == false)
                {
                    anim.Play();

                    animationHasPlayed = true;
                }
            }
            else
            {
                animationHasPlayed = false;
            }
        }
    }
}
