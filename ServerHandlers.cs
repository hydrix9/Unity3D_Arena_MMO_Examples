using MEC;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;


/// <summary>
/// contains handlers for receiving data from clients
/// </summary>
public partial class Server : NetworkIO {


    public float delaySec = 0;
    public float packetLossPer = 1;
    [SerializeField]
    public int droppedPacketsCounter = 0;


    //basically the listen process. Mother network interface class that inherits NetworkInterface uses this to continously enter into handling data on a listen loop, hence 'override'. The start point into handling.
    public override void HandleData(int conn, byte[] receiveBytes)
    {

        if (logAll)
            Ext.DebugByte(receiveBytes, "server received: ");

#if debug
        debugEstimatedDataIn += receiveBytes.Length + 30; //count bytes length plus a udp packet header which averages 20-40 bytes
        if (packetLossPer > 0)
        {
            if (UnityEngine.Random.value < packetLossPer / 100)
            {
                droppedPacketsCounter++;
                return;
            }
        }

        if (delaySec > 0) //if delay is enabled
        {
            Timing.CallDelayed(delaySec, () => { ReceiveRaw(conn, receiveBytes, false, true); });
            return;
        }
#endif
        //Debug.Log("handling data");

        // Debug.Log("received");
        // Tools.DebugByte(receiveBytes);

        //main network code code...
        switch (receiveBytes[0])
        {
            case (netcodes.join):
                AddPlayer(conn);
                break;
            case (netcodes.playerQuit):
                RemovePlayer(conn);
                break;
            case (netcodes.inputs):
                HandleInputs(ref conn, ref receiveBytes);
                break;
            case (netcodes.pos3d):
                HandlePos(ref conn, ref receiveBytes);
                break;
            case (netcodes.infoplz):
                HandleAskOfPlayer(ref receiveBytes, ref conn);
                break;
            case (netcodes.yourid): //in this case actually asking for their id
                HandleYourID(ref receiveBytes, ref conn);
                break;
            case (netcodes.ability):
                HandleAbility(ref receiveBytes, ref conn);
                break;
            case (netcodes.aoeAbility):
                HandleAbilityAOE(ref receiveBytes, ref conn);
                break;
            case (netcodes.rayAbility):
                HandleAbilityRay(ref receiveBytes, ref conn);
                break;
            case (netcodes.chat):
                HandleChat(ref receiveBytes, ref conn);
                break;
            case (netcodes.shops):
                HandleShops(ref receiveBytes, ref conn);
                break;
        }
    }



    public void AddPlayer(int conn, bool robot = false)
    {
        if (points.ContainsKey(conn))
        {
            SendPlayer(new byte[] { netcodes.yourid, (byte)points[conn].id, (byte)(points[conn].id >> 8),
                (byte)WorldFunctions.GetEntityID("genericPlayer") //make them spawn LOCAL player prefab...
            }, conn); //inform them of their new id so they can take control of this object

            Debug.LogWarning("already contains IPEndPoint " + conn);
            return;
        }

        Debug.Log("received AddPlayer on server");



        //create player with new Entity ID and add to endpoints reference
        points.Add(
           conn,
                AddEntity(
            GetEmptyID(), //new id
            "genericPlayer"
            ) as Player

                    ); //create player and add references

        points[conn].ServerInit();
        Debug.Log(conn + " initialized entity 'type' " + points[conn].type);

        GameMode.instance.OnAddPlayer(points[conn]);


        players.Add(points[conn]); //add another reference..

        points[conn].robot = robot;
        points[conn].point = conn;

        MMClient.instance.hostedGame.players = (points.Count + ( (player != null) ? 1 : 0) ).ToString(); //update to count + local player (if any)
        MMClient.instance.SerializeHostedGame();
        MMClient.instance.RefreshGameNow(); //push changes to server

        Debug.Log("point set to " + points[conn].point);

        points[conn].name += " ServerRemote";

        SendPlayer(new byte[] { netcodes.yourid, (byte)points[conn].id, (byte)(points[conn].id >> 8),
                (byte)WorldFunctions.GetEntityID("genericPlayer") //make them spawn LOCAL player prefab...
            }, conn); //inform them of their new id so they can take control of this object


        if (!robot)
            onSendData += (ref byte[] data) => { SendPlayer(data, conn); }; //register to send to this address ... beautiful
    }


    public void RemovePlayer(int conn, bool robot = false)
    {
        Debug.Log("received RemovePlayer on server");

        if (!points.ContainsKey(conn))
        {
            Debug.LogError("couldn't remove nonexistant player ");
            return;
        }

        if (!robot)
            onSendData -= (ref byte[] data) => { SendPlayer(data, conn); }; //deregister



        if (EntityExists(points[conn].id))
            RemoveEntity(points[conn].id);
        if (players.Contains(points[conn]))
            players.Remove(points[conn]);
        points.Remove(conn);

    }



    public void HandleAskOfPlayer(ref byte[] data, ref int conn)
    {
        //SendPlayer()
    }

    public void HandleInputs(ref int conn, ref byte[] data)
    {
        //Debug.Log("received movement " + Ext.ByteToString(data));

        points[conn].lastInputNum = data[1]; //set last request num for pinging back later
        points[conn].nm.HandleCode(data[2]); //apply inputs, mostly for syncing animations
        points[conn].nm.HandleRotation(data[3], data[4]); //apply rotation
        points[conn].nm.HandleInput(Vector3DeserializeNormalized(ref data, 5));


        //TODO: add something to drop the player if the point doesn't exist?

    }
    const int rotPos = 2;
    public void HandlePos(ref int conn, ref byte[] data)
    {

        if (!clientSidePos) //possibly a hack, because client side pos is disabled and have received it anyway
            return;
        points[conn].nm.HandleCode(data[1]);
        points[conn].nm.HandlePos(ref data, 4, false);
        points[conn].nm.HandleRotation(ref data, rotPos);

    }

    //used when player asks for their id
    void HandleYourID(ref byte[] data, ref int conn)
    {
        SendPlayer(new byte[] { netcodes.yourid, (byte)points[conn].id, (byte)(points[conn].id >> 8) }, conn); //tell them their id
    }


    bool CheckAbilitySanity(ref int casted)
    {
        if (!Spell.SpellExists(casted))
        {
            Debug.LogWarning("player tried to cast nonexistant spell");
            return false;
        }

        return true;
    }


    void HandleAbility(ref byte[] data, ref int conn)
    {
        int target = data[1] | data[2] << 8; //deserialize
        int casted = data[3] | data[4] << 8; //deserialize

        if (data.Length != 7)
        {
            Debug.LogWarning("deformed packet in HandleAbility from " + points[conn].id);
            return;
        }

        Ext.DebugByte(data, "received ability: ");

        if (target != 0 && !EntityExists(target))
        {
            Debug.LogWarning("player tried to target nonexistant entity " + target);
            return;
        }

        if (target != 0 && !(entities[target] is Entity))
        {
            Debug.LogWarning("player tried to cast at a SyncObject not an Entity");
            return;
        }

        if (!CheckAbilitySanity(ref casted))
            return;

        if (!Spell.GetSpell(casted).targetedTT)
        {
            Debug.LogWarning(points[conn].id + " tried casting a non-ray ability as a ray");
            return;
        }

        Spell.GetSpell(casted).TryStartCast(target != 0 ? entities[target] as Entity : null, points[conn]);


        //Also check if they used AoE or something?...


        //points[conn].Cast(entities[target].e, WorldFunctions.instance.allSpells[casted], new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, data[5], data[6] * 4)); //try cast normal ability

    }


    
    void HandleAbilityAOE(ref byte[] data, ref int conn)
    {
        if (data.Length != 17)
        {
            Debug.LogWarning("deformed packet in HandleAbility from " + points[conn].id);
            return;
        }

        int casted = data[1] | data[2] << 8;

        if (!CheckAbilitySanity(ref casted))
            return;

        if (!Spell.GetSpell(casted).oneVectorTT)
        {
            Debug.LogWarning(points[conn].id + " tried casting a non-aoe ability as a ray");
            return;
        }

        Spell.GetSpell(casted).TryStartCast(Vector3Deserialize(ref data, data[3]), points[conn]);

    }

    void HandleAbilityRay(ref byte[] data, ref int conn)
    {
        if (data.Length != 29)
        {
            Debug.LogWarning("deformed packet in HandleAbility from " + points[conn].id);
            return;
        }

        int casted = data[1] | data[2] << 8;


        if (!CheckAbilitySanity(ref casted))
            return;


        if(!Spell.GetSpell(casted).twoVectorTT)
        {
            Debug.LogWarning(points[conn].id + " tried casting a non-ray ability as a ray");
            return;
        }


        Spell.GetSpell(casted).TryStartCast(Vector3Deserialize(ref data, data[3]), Vector3Deserialize(ref data, data[15]), points[conn]);
    }


    void HandleShops(ref byte[] data, ref int conn)
    {
        int id = data[1] | (data[2] << 8);

        if (!EntityExists(id) || entities[id].e == null)
        { //if doesn't exist or isn't an entity
            Debug.LogWarning("player " + points[conn].id + " asked for shop from entity id " + id + " but that entity does not exist");
            return;
        }

        SendPlayer(Shop.SerializeViewableShops(entities[id].e, points[conn]), conn);


    }

    void HandleShopTab(ref byte[] data, ref int conn)
    {
        int id = data[1] | (data[2] << 8);
        int shopTab = data[3] | data[4] << 8;

        if (!EntityExists(id) || entities[id].e == null)
        { //if doesn't exist or isn't an entity
            Debug.LogWarning("player " + points[conn].id + " asked for shop from entity id " + id + " but that entity does not exist");
            return;
        }

        if(!entities[id].e.shopEntries.ContainsKey(shopTab))
        {
            Debug.LogWarning("player " + points[conn].id + " asked for shopTab " + shopTab + " from entity " + id + " when it doesn't have it");
            return;
        }

        if(Shop.GetSpendableShopTabsInt(entities[id].e, points[conn]).Contains(shopTab))
        {
            Debug.LogWarning("player " + points[conn].id + " asked for shopTab " + shopTab + " from entity " + id + " when they don't have permission to spend on it");
            return;
        }

        byte[] sends = new byte[1 + 4 + (8 * entities[id].e.shopEntries[shopTab].Count * 8)]; //1 byte for netcode, 2 bytes for id, 2 bytes for length if items, plus 8 bytes per item
        sends[0] = netcodes.singleShopTab;
        int index = 1; //where to start serializing

        ShopTab.SerializeTo(shopTab, entities[id].e, points[conn], ref sends, ref index); //serialize but start after netcode

        SendPlayer(sends, conn);


    }



    //list of list of players where index is room and int is conn
    List<int>[] subbedChatChannels;

    void HandleChat(ref byte[] data, ref int conn)
    {
        if (data.Length < 3)
        {
            Debug.LogWarning("received chat msg too short");
        }
        int channel = data[1] | (data[2] << 8);

        if (channel <= 0 || channel >= subbedChatChannels.Length)
        { //if oob
            Debug.LogWarning("player " + points[conn].displayName + " (" + points[conn].name + ") id " + points[conn].id + " tried to send to unknown chat channel " + channel);
            return;
        }

        SendChat(channel, ref data);
    }

    public void SendChat(int channel, ref byte[] msg)
    {
        for (int i = 0; i < subbedChatChannels[channel].Count; i++)
        {
            SendPlayer(msg, subbedChatChannels[channel][i]); //forward msg on channel
        }
    }
    public void SendChat(int channel, byte[] msg)
    {
        SendChat(channel, ref msg);
    }

} //end class
