using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PolyNodeHandler {

	public static void sendPlayerLogin(PolyNetPlayer player) {
		//player.playerId is working now so you send that to node
		//this will send to node
		receivePlayerLogin (player.playerId);
	}

	public static void receivePlayerLogin(int playerId) {
		//this is where node gets back to us with the data
		PolyNetPlayer player = PolyServer.getPlayerPId(playerId);
		//eventually any player data that node wants to set can get thrown in here
		player.setData (new Vector3 (0, 0, 0));
		PolyServer.onLoginData(PolyServer.getPlayerPId(playerId));
	}
}
