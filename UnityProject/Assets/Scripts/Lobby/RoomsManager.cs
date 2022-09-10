using Photon.Pun;
using UnityEngine;
using TMPro;

public class RoomsManager : MonoBehaviourPunCallbacks
{
    #region Fields
    //Create & join room related
    [SerializeField] private TMP_InputField createInput;
    [SerializeField] private TMP_InputField joinInput;

    //Character creation related
    [SerializeField] private GameObject[] characters;
    [SerializeField] private int selectedCharacter = 0;
    [SerializeField] private TMP_Text playerNameText;
    private string playerName;
    #endregion

    #region Create & join rooms
    public void CreateRoom()
    {
        PhotonNetwork.CreateRoom(createInput.text);
    }

    public void JoinRoom()
    {
        PhotonNetwork.JoinRoom(joinInput.text);
    }

    public override void OnJoinedRoom()
    {
        //save player preference
        PlayerPrefs.SetInt("selectedCharacter", selectedCharacter);
        //join a room
        PhotonNetwork.LoadLevel("MainScene");
    }
    #endregion

    #region Character creation

    public void NextCharacter()
    {
        characters[selectedCharacter].SetActive(false);
        selectedCharacter = (selectedCharacter + 1) % characters.Length;
        characters[selectedCharacter].SetActive(true);
    }

    public void PreviousCharacter()
    {
        characters[selectedCharacter].SetActive(false);
        selectedCharacter--;
        if (selectedCharacter < 0)
        {
            selectedCharacter += characters.Length;
        }
        characters[selectedCharacter].SetActive(true);
    }

    public void SetNickname()
    {
        playerName = playerNameText.text;

        PlayerPrefs.SetString("playerName", playerName);
    }
    #endregion
}
