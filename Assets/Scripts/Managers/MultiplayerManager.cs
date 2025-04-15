using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiplayerManager : Singleton<MultiplayerManager>
{

    public int _PlayerNumber = 1;

    //TODO: WHILE offline keep the player number at 1 but when online assign it correctly


    /*Overview of online:
    -each player will have an individual player manager 
    -the online will send all the data in the form of packets containing data on each movement and other important dat such as the current objective time etc
    -the units sent will be registared as enemy units and will be handled by an enemy spawner
    -the owner of the lobby will generate the map and send the data to the other players who will recreate the map
        -a sync will need to be implemented to make sure all units are correctly configured
        -moves might take a handshake approach to make sure no sync issues occur
        -Creating logic for players up to 4 players
        - on init each player number will be set up and assigned by the lobby host
    */
}
