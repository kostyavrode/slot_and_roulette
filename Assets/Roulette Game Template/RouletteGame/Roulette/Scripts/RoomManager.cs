using UnityEngine.SceneManagement;
using UnityEngine;
using Mkey;

public class RoomManager : MonoBehaviour
{
    public static void ChangeScene(int SceneID)
    {
        ResultManager.totalBet = 0;
        SceneManager.LoadSceneAsync(SceneID);
    }

    public void GoToScene(int SceneID)
    {
        ResultManager.totalBet = 0;
        SoundMasterController soundMaster = GameObject.FindObjectOfType<SoundMasterController>();
        soundMaster.DisableVolumeWhenWrongScene(1);
        SceneManager.LoadSceneAsync(SceneID);
    }
}
