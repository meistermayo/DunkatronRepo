using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

[System.Serializable]
public class Child{
	public string name;
	public System.DateTime lastPlayTime;
	public int plays = 3;
	public int wins, losses, kills, deaths, gamesPlayed;
	public int level;

	public Child(string name)
	{
		this.name = name;
		this.plays = 3;
		this.lastPlayTime = System.DateTime.Now;
		level = 0;
	}
}

public class ChildQueueController : MonoBehaviour {
	[SerializeField] public static Queue<Child> children;
	[SerializeField] public static List<Child> child_profiles;
	public static Child[] player_profiles;

	public Text childname_text, queue_text;


	int pointerPos;
	int queuePos;
	public Vector3 pointerBase;

	public GameObject elements;
	public GameObject pointer, option, info;
	public Text optionText, infoText;

	bool vk_enter, vk_tab, vk_backspace, vk_left, vk_right;

	bool promptOn;

	int state;
	List<int> possibleProfileIndices;
	public List<Child> displayChild;
	public static string[] names = {
		"Bitz",
		"Senor Popo",
		"Froop",
		"Mr Fish",
		"Slicey",
		"Gaze",
		"Pippy",
		"Odore",
		"Moeg",
		"Skulldier",
		"Unalis",
		"Flitch",
		"Rangle",
		"Gug",
	};

	void Awake()
	{
		//LoadChildren ();
		displayChild = new List<Child> ();
	}

	void Update()
	{
		GetInput ();

		if (vk_tab) {					// Turning on the promt <TAB>
			if (Input.GetKey (KeyCode.R) && Input.GetKey (KeyCode.C)) {
				if (!promptOn) {
					elements.SetActive (true);
					pointer.SetActive (true);
					option.SetActive (true);
					info.SetActive (true);
					promptOn = true;
					SetupOptions ();
				}
			}
		}

		if (vk_backspace) {
			// if in select, go back to options.
			// if in options, turn off prompt
			if (state > 0){
				state = 0;
				SetupOptions ();
			}
			else if (state == 0){
				if (promptOn) {/*
					promptOn = false;
					elements.SetActive (false);
					pointer.SetActive (false);
					option.SetActive (false);
					info.SetActive (false);*/
					UnsetPrompt ();
				}

			}
		}

		if (vk_enter) {
			//if in options, go to select1 or 2
			if (state == 0) {
				if (pointerPos == 0)
					SetupEnqueue ();
				else
					SetupRemove ();
			} else {
				if (state == 1) { // Enqueue
					EnqueueChild(displayChild[queuePos]);
					UnsetPrompt ();
				} else { // Remove
					RemoveChild(displayChild[queuePos].name);
					UnsetPrompt ();
				}
			}
		}

		if (promptOn) {
			if (state > 0)
				UpdateQueue ();
			else
				UpdateOptions ();
			CheckForWipeData ();
		}

		CheckForPlayTimes ();
	}

	void GetInput()
	{
		vk_enter = Input.GetKeyDown (KeyCode.Return);
		vk_tab = Input.GetKeyDown (KeyCode.Tab);
		vk_backspace = Input.GetKeyDown (KeyCode.Backspace);
		vk_left = Input.GetKeyDown (KeyCode.LeftArrow);
		vk_right = Input.GetKeyDown (KeyCode.RightArrow);
	}

	void SetupEnqueue()
	{
		state = 1;
		pointer.SetActive (false);
		option.SetActive (false);

		infoText.text = "[";

		displayChild.Clear ();

		for (int i = 0; i < child_profiles.Count; i++) { // LOOP
			//if (!children.Contains (child_profiles [i]))
			{
				if (child_profiles [i].plays > 0)
					infoText.text += "<color=#00ff00ff>";
				else
					infoText.text += "<color=#ff0000ff>";
				
					infoText.text += child_profiles [i].name;
					infoText.text += "[ " + child_profiles [i].plays.ToString ();
					infoText.text += "] " + child_profiles [i].lastPlayTime.TimeOfDay.ToString();
					displayChild.Add (child_profiles [i]);

				infoText.text += "</color>";
			}
			//else
			//Debug.Log ("Already Continaed it");
			if (i == 0)
				infoText.text += "]";
		}

	}

	void SetupRemove()
	{
		state = 2;
		pointer.SetActive (false);
		option.SetActive (false);

		infoText.text = "[";

		displayChild.Clear ();

		//Queue<Child>.Enumerator childEnum = children.GetEnumerator ();

		if (children.Count == 0) {
			SetupOptions ();
			return;
		}
		
		for (int i = 0; i < children.Count; i++) { // LOOP
			Child child = children.Dequeue();
			infoText.text += child.name;
			displayChild.Add (child);
			children.Enqueue(child);

			if (i == 0)
				infoText.text += "]";
		}
	
	}

	void SetupOptions()
	{
		state = 0;
		pointerPos = 0;
		pointer.SetActive (true);
		option.SetActive (true);
		info.SetActive (true);

		infoText.text = "";

		for (int i = 0; i < children.Count; i++) {
			Child child = children.Dequeue ();
			infoText.text += child.name;

			if (i < children.Count)
				infoText.text += ", ";
			children.Enqueue (child);
		}
	}

	void UpdateOptions()
	{
		if (vk_left)
			pointerPos--;
		if (vk_right)
			pointerPos++;
		
		pointerPos = Mathf.Clamp (pointerPos, 0, 1);

		pointer.transform.position = Camera.main.WorldToScreenPoint(pointerBase) + pointerPos * Vector3.down * pointer.transform.localScale.z;
	}



	void UpdateQueue()
	{
		if (vk_left)
			queuePos--;
		if (vk_right)
			queuePos++;

		queuePos = Mathf.Clamp (queuePos,0,displayChild.Count-1); // Queuepos update
		/*
		if (state == 1) {
			Debug.Log ("state=1");
			if (Input.GetKeyDown (KeyCode.D)) {
				child_profiles.RemoveAt (queuePos);
				SetupEnqueue ();
				Debug.Log ("Deleted");
				return;
			}
		}
		*/
		if (Input.GetKeyDown (KeyCode.Alpha1)) {
			child_profiles [queuePos].plays++;
			if (child_profiles [queuePos].plays > 3) {
				child_profiles [queuePos].plays = 3;
				SaveChildren ();
			}
		}

		infoText.text = ""; // Update text

		for (int i = 0; i < displayChild.Count; i++) {
			if (child_profiles [i].plays <= 0)
				infoText.text += "<color=#ff0000FF>";
			if (i == queuePos) {
				infoText.text += "<<" + displayChild [i].name;
				infoText.text += " [" + child_profiles [i].plays.ToString ();
				infoText.text += "] ";
				if (child_profiles[i].plays <= 0)
					infoText.text += child_profiles [i].lastPlayTime.AddHours (3.0).TimeOfDay;
				infoText.text += ">>";
			} else {
				infoText.text += displayChild [i].name;
				infoText.text += " [" + child_profiles [i].plays.ToString ();
				infoText.text += "] ";
				if (child_profiles[i].plays <= 0)
					infoText.text += child_profiles [i].lastPlayTime.AddHours(3.0).TimeOfDay;
			}

			if (child_profiles [i].plays <= 0)
				infoText.text += "</color>";
			if (i < displayChild.Count - 1)
				infoText.text += ", ";
		}
	}

	void UnsetPrompt()
	{
		state = 0;
		promptOn = false;
		pointerPos = 0;
		queuePos = 0;
		pointer.SetActive (false);
		option.SetActive (false);
		info.SetActive (false);
		elements.SetActive (false);
		// Set info to queue???
	}

	// save children
	public void SaveChildren() 
	{
		//childname_text.text = "";
		// Serializes data 
		//Debug.Log("Saving Children");
		BinaryFormatter formatter = new BinaryFormatter ();
		// FileStreams
		FileStream file = File.Create(Application.streamingAssetsPath + "/children.bin");

		for (int i = 0; i < child_profiles.Count; i++) { // obsolete??
			Child child = child_profiles [i];//children.Dequeue ();
			//Debug.Log ("Saving: " + child.name);
			//child_profiles.Add (child);
			//children.Enqueue (child);
		}
		formatter.Serialize (file, child_profiles);

		file.Close ();
		//Debug.Log ("Finished");
	}

	// load children
	public void LoadChildren()
	{
		//Debug.Log ("LoadingChildren");
		//childname_text.text = "";
		if (File.Exists (Application.streamingAssetsPath + "/children.bin")) {
			//Debug.Log ("Loading..>");
			BinaryFormatter formatter = new BinaryFormatter ();

			FileStream file = File.Open (Application.streamingAssetsPath + "/children.bin",
				                  FileMode.Open);

			child_profiles = (List<Child>)formatter.Deserialize (file);

			file.Close ();

			//children.Clear ();
			/*
			child_profiles [0].name = "Bitz";
			child_profiles [1].name = "Senor Popo";
			child_profiles [2].name = "Froop";
			child_profiles [3].name = "Mr Fish";
			child_profiles [4].name = "Slicey";
			child_profiles [5].name = "Gaze";
			child_profiles [6].name = "Pippy";
			child_profiles [7].name = "Odore";
			child_profiles [8].name = "Moeg";
			child_profiles [9].name = "Skulldier";
			child_profiles [10].name = "Unalis";
			child_profiles [11].name = "Flitch";
			child_profiles [12].name = "Rangle";
			child_profiles [13].name = "Gug";
			SaveChildren ();
*/
		} else {
			//Debug.Log ("file not found... saving...");
			SaveChildren ();
		}
	}


	void EnqueueChild(Child child) // is child in the queue? Can child play? put child in queu
	{
	//	if (childname_text.text.Length == 0)
	//		return;
		if (!children.Contains (child)) {
			if (System.DateTime.Now.Subtract( child.lastPlayTime ).TotalDays >= 1) { // refresh child plays
				child.plays = 3;
			}

			if (child.plays == 3)
				child.lastPlayTime = System.DateTime.Now; // not used one yet? set playtime
			else if (child.plays == 0) // not enough plays
				return;

			children.Enqueue (child);
		}
	}


	void RemoveChild(string child_name) // ejects a child early.
	{
		if (child_name.Length == 0)
			return;
		Queue<Child> temp = new Queue<Child> ();

		while (children.Count > 0) { // loop thru child queue
			Child child = children.Dequeue ();
			bool dequeue = true;
			//Debug.Log (child.name);
			for (int i = 0; i < child_name.Length-1 && i < child.name.Length-1; i++) {
				//Debug.Log (child_name [i] + " vs " + child.name [i]);
				if (child_name [i] != child.name [i]) {
					//Debug.Log (child.name + "b");
					dequeue = false; break;
				}

			}

			if (dequeue) { // pop a child and check name
				
				while (children.Count > 0)
					temp.Enqueue (children.Dequeue ()); // if we found it, just enqueue the rest of children.
			} else {
				temp.Enqueue (child); // else, put the child into temp.
			}

		}
		children = temp; // reset queue after
	}

	void CheckForWipeData()
	{
		if (Input.GetKeyDown (KeyCode.G)) {
			for (int i = 0; i < child_profiles.Count; i++) {
				child_profiles [i].deaths = 0;
				child_profiles [i].kills = 0;
				child_profiles [i].losses = 0;
				child_profiles [i].wins = 0;
				child_profiles [i].gamesPlayed = 0;
			}
			SaveChildren ();
		}

	}
	/*
	void Update()
	{
		childname_text.text += Input.inputString; // childname and dequeue
		if (Input.GetKeyDown (KeyCode.Backspace))
			if (Input.GetKey (KeyCode.LeftShift)) {
				RemoveChild (childname_text.text);
				childname_text.text = "";
			} else
				childname_text.text.Remove(childname_text.text.Length-2);

		if (Input.GetKeyDown (KeyCode.Return)) { // enqueuing
			EnqueueChild (new Child (childname_text.text));
			childname_text.text = "";
		}

		if (Input.GetKeyDown (KeyCode.Tab)) { // requeueing
			RequeueChild ();
		}


		string queue_string = "Queue: "; 					// crete queue string
		for (int i = 0; i < children.Count; i++) {
			Child child = children.Dequeue ();
			queue_string += child.name + ", ";
			children.Enqueue (child);
		}

		queue_text.text = queue_string;


		// load/save
		if (Input.GetKey (KeyCode.LeftShift)) {
			if (Input.GetKeyDown (KeyCode.W))
				LoadChildren ();
			if (Input.GetKeyDown (KeyCode.S))
				SaveChildren ();
		}
		
	}

	void RequeueChild() // after child plays, requeue them into children.
	{
		if (children.Count == 0)
			return;
		
		Child child = children.Dequeue ();
		if (child.plays > 0) {
			child.plays--;
			children.Enqueue (child);
		}
	}

*/
	void CheckForPlayTimes()
	{
		for (int i = 0; i < ChildQueueController.child_profiles.Count; i++) {
			Child child = ChildQueueController.child_profiles [i];

			bool reset = false;
			if (child.lastPlayTime.Day.CompareTo (System.DateTime.Now.Day) != 0)
				reset = true;
			else if (child.lastPlayTime.Hour - System.DateTime.Now.Hour <= -3)
				reset = true;

			//Debug.Log("child: " + child.lastPlayTime.Hour + "\nSystem: " + System.DateTime.Now.Hour + "\nDiff: " + (child.lastPlayTime.Hour - System.DateTime.Now.Hour).ToString());
			if (reset) {
				child.plays = 3;
			}
		}
	}

}
