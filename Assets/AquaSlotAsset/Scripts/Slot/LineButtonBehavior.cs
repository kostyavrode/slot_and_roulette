﻿using System;
using UnityEngine;

namespace Mkey
{
    public class LineButtonBehavior : MonoBehaviour, ICustomMessageTarget
    {
        public Font normalFont;
        public Font pressedFont;
        public Material normalMaterial;
        public Material pressedMaterial;
        public int number;
        public LineBehavior line;

        private bool pressed = false;

        public bool Pressed
        {
            get { return pressed; }
        }
        public Action PressButtonDelegate;
        public Action UnPressButtonDelegate;
        private LineButtonBehavior[] lbbs;
        private TextMesh textMesh;
        private MeshRenderer meshRenderer;


        void Start()
        {
            //1) cache all line buttons
            lbbs = FindObjectsOfType<LineButtonBehavior>();
            PressButtonDelegate += () => {if(line) line.Select(false, 0); };
            UnPressButtonDelegate += () => {if(line) line.DeSelect(); };
            textMesh = GetComponentInChildren<TextMesh>();
            meshRenderer = GetComponentInChildren<MeshRenderer>();
        }

        #region touch callbacks
        public void PointerUp(TouchPadEventArgs tpea)
        {

        }
        public void PointerDown(TouchPadEventArgs tpea)
        {
            pressed = !pressed;
            if (textMesh)
                textMesh.font = (pressed) ? pressedFont : normalFont;
            if(meshRenderer)
                meshRenderer.material = (pressed) ? pressedMaterial : normalMaterial;
            if (pressed)
            {
                if (PressButtonDelegate != null) PressButtonDelegate();
            }
            else
            {
                if (UnPressButtonDelegate != null) UnPressButtonDelegate();
            }

            // handle other lines
            if (tpea != null)
            {
                SoundMasterController.Instance.SoundPlayCheck(0, null);
                foreach (var lbb in lbbs)
                {
                    if (Pressed && lbb.number < number && lbb.Pressed == false)
                    {
                        lbb.PointerDown(null);
                    }
                    if (Pressed && lbb.number > number && lbb.Pressed == true)
                    {
                        lbb.PointerDown(null);
                    }
                    if (!Pressed && lbb.number > number && lbb.Pressed == true)
                    {
                        lbb.PointerDown(null);
                    }

                }
            }
        }
        public void DragBegin(TouchPadEventArgs tpea) { }
        public void DragEnter(TouchPadEventArgs tpea) { }
        public void DragExit(TouchPadEventArgs tpea) { }
        public void DragDrop(TouchPadEventArgs tpea) { }
        public void Drag(TouchPadEventArgs tpea) { }
        public GameObject GetDataIcon()
        {
            return null;
        }
        public GameObject GetGameObject()
        {
            return gameObject;
        }
        #endregion touch callbacks
    }
}