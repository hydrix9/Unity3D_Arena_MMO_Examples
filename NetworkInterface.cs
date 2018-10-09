using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// classes inherit from this to make receiving on different interfaces uniform with common functions like SendPlayer and ReceiveData
/// </summary>
public abstract class NetworkInterface : MonoBehaviour {

    /// <summary>
    /// MM lists this if we need to have a GUID so server can be found in some signaling server 
    /// </summary>
    private static string _serverGUID;
    //public static string serverGUID { get { if(_serverGUID == null) _serverGUID = System.Guid.NewGuid().ToString(); return _serverGUID; } } //merely init if not done so...
    public static string serverGUID { get { if (_serverGUID == null) _serverGUID = Ext.GUIDX(16); return _serverGUID; } } //merely init if not done so...

    public bool logAll;

    public virtual void Start() {
    }

    public virtual void Init() { }

    public virtual void HandleData(int conn, byte[] data) { }

    public virtual void SendNow(ref byte[] data) { }

    public virtual void SendNow(byte[] data) { }


    public virtual void SendPlayer(ref byte[] data, ref int conn) { }
    public virtual void SendPlayer(ref byte[] data, int conn) { }

    //connect to signaling if any... also Server class overides this
    public virtual void StartServer() {
    }

    public virtual void StopServer() { }

    public virtual void ConnectGame(listing game) { }

    public delegate void OnNewConnectionDelegate(int id);
    public event OnNewConnectionDelegate onNewConnection;

    public virtual void OnNewConnection(int id) {
        if (onNewConnection != null)
            onNewConnection(id);
    }
    public virtual void OnDisconnect(int id) { }

    public delegate void OnConnectionFailedDelegate(int id);
    public OnConnectionFailedDelegate onConnectionFailed;

    public virtual void OnConnectionFailed(int id) {
        if (onConnectionFailed != null)
            onConnectionFailed(id);
    }

    public virtual void FixedUpdate()
    {

    }

    public virtual void Reset() { }
}
