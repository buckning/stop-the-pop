using UnityEngine;
using System.Collections;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;

public class GameDataPersistor : MonoBehaviour {

	public static void Save(GameData gameData) {
		BinaryFormatter binaryFormatter = new BinaryFormatter ();
		FileStream fileStream = File.Create (Application.persistentDataPath + "/game.dat");

		String unencryptedGameData = gameData.gameData;

		//encrypt game data here
		String encrypted = StringCipher.encryptData(unencryptedGameData);

		gameData.gameData = encrypted;
		binaryFormatter.Serialize (fileStream, gameData);
		fileStream.Close ();
	}

	public static GameData Load() {
		if(File.Exists(Application.persistentDataPath + "/game.dat")) {
			BinaryFormatter binaryFormatter = new BinaryFormatter ();
			FileStream fileStream = File.Open (Application.persistentDataPath + "/game.dat", FileMode.Open);
			GameData myGameData = (GameData)binaryFormatter.Deserialize(fileStream);

			if(!myGameData.gameData.Contains("###GameData###")) {
				myGameData.gameData = StringCipher.decryptData (myGameData.gameData);
			}

			fileStream.Close ();
			return myGameData;
		}
		else {
			return null;
		}
	}
}
