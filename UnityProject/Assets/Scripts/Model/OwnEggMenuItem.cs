using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class OwnEggMenuItem : GenericEggMenuItem
{
    public static OwnedEgg eggToSend;

    /*[SerializeField]
    private Button _sendToFriendButton;
    [SerializeField]
    private Button _hatchButton; */
    [SerializeField]
    private GameObject _hatchPanel;
    [SerializeField]
    private GameObject _checkInPanel;

    private void Start()
    {
        refreshView();
    }

    // TODO implement this
    public void sendToFriendButtonHandler()
    {
        eggToSend = _egg;
        MainMenuScript.EggsCanvas.enabled = false;
        MainMenuScript.FriendsCanvas.enabled = true;
    }
        
    override protected IEnumerator updateServer()
    {
		yield return SpatialClient2.single.updateMetadataAfterOwnEggCheckedIn(_egg.KaijuEmbryo.Name);
    }

	override protected void initializeOtherText()
	{
		// TODO uncomment and change this to initialize helpers
		/*_friendNameText.text = "";
            bool firstElement = true;
            foreach (string friendUserID in _egg.Helpers)
            {
                if (firstElement)
                    firstElement = false;
                else
                    _friendNameText.text += COMMA;
                _friendNameText.text += SpatialClient2.single.getNameOfFriend(friendUserID);
            } */
	}

    public void onHatch()
    {
        StartCoroutine(hatchEgg());
    }

    private IEnumerator hatchEgg()
    {
        MessageController.single.displayWaitScreen(MainMenuScript.EggsCanvas);
        MainController.single.selectedEgg = _egg;
		yield return _egg.KaijuEmbryo.initializeSprites (new CoroutineResponse());
        MainMenuScript.addKaijuButton(_egg.KaijuEmbryo);
        MessageController.single.closeWaitScreen(false);
		MessageController.single.closeWaitScreen(false);
		MainController.single.goToPhoneCamera(PhoneImageController.CameraMode.EggHatching);
        Destroy(gameObject);
        yield return SpatialClient2.single.hatchEgg(_egg);
    }

	//Nicky's Code Start
	public void onCheckInButtonPressed()
	{
		onCheckIn();
		MainController.single.selectedEgg = _egg;
		MainController.single.goToPhoneCamera(PhoneImageController.CameraMode.EggCheckin);
	}
	//Nicky's Code End

    override protected void refreshView()
    {
        _hatchPanel.SetActive(_egg.Hatchable);
        _checkInPanel.SetActive(!_egg.Hatchable);
    }
}
