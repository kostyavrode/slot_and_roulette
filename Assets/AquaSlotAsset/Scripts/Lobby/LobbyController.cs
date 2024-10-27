using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mkey {
    public class LobbyController : MonoBehaviour {
        public static Action<int> onSceneChanged;

        public void SceneLoad(int scene)
        {
            if (scene == 0 || scene == 1 || scene == 2)
            {
                //SoundMasterController.instance.DisableVolumeWhenWrongScene(1);
                SoundMasterController soundMaster=GameObject.FindObjectOfType<SoundMasterController>();
                soundMaster.DisableVolumeWhenWrongScene(1);
            }
            else
            {
                SoundMasterController soundMaster = GameObject.FindObjectOfType<SoundMasterController>();
                soundMaster.DisableVolumeWhenWrongScene(3);
                //SoundMasterController.instance.DisableVolumeWhenWrongScene(3);
            }
            SceneLoader.Instance.LoadScene(scene);
            //onSceneChanged?.Invoke(scene);

        }

        public void Slider_Click()
        {
            GuiController.Instance.ShowShop();
        }
        public void ExitApp()
        {
            Application.Quit();
        }

    }

}