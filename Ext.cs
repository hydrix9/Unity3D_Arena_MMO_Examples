using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using System.Security.Cryptography;
using System.Text;
using System.Linq;
using System.Linq.Expressions;

/// <summary>
/// Extensions. Contains extremely convenient Static methods for animations, byte arrays, uuids, and many things
/// </summary>
public class Ext : MonoBehaviour {

    //Singleton code
    // s_Instance is used to cache the instance found in the scene so we don't have to look it up every time.
    private static Ext s_Instance = null;
    // This defines a static instance property that attempts to find the manager object in the scene and
    // returns it to the caller.
    public static Ext instance {
        get {
            if (s_Instance == null) {
                // This is where the magic happens.
                //  FindObjectOfType(...) returns the first AManager object in the scene.
                s_Instance = FindObjectOfType(typeof(Ext)) as Ext;
            }

            // If it is still null, create a new instance
            if (s_Instance == null) {
                GameObject obj = new GameObject("Ext");
                s_Instance = obj.AddComponent(typeof(Ext)) as Ext;
                Debug.Log("Could not locate an Ext object. Ext was Generated Automaticly.");
            }
            return s_Instance;
        }
    }




    // Use this for initialization
    void Start() {
        QualitySettings.vSyncCount = 0;
    }




    public static byte ToByte(String str)
    {
        return Convert.ToByte(str);
    }

    public static byte ToByte(Char str)
    {
        return Convert.ToByte(str);
    }



    public static string ByteToString(ref byte[] data)
    {
        string s = "";
        for (int i = 0; i < data.Length; i++)
        {
            s += data[i].ToString();
            s += "-";
        }
        return s;
    }
    public static string ByteToString(byte[] data)
    {
        return ByteToString(ref data);
    }

    public static void DebugByte(byte[] data, string msg = "debug: ")
    {
        Debug.Log(msg + ByteToString(ref data));
    }

    /// <summary>
    /// warning: have to start this coroutine. Lerps transform easily with parameters
    /// </summary>
    /// <param name="obj">Object.</param>
    /// <param name="newPos">New position.</param>
    /// <param name="time">time in ms</param>
    /// <param name="speed">Speed multiplier</param>
    public static IEnumerator Lerp(GameObject obj, Vector3 newPos, float time, float speed) {

        //Debug.Log ("lerping " + obj.name + " to " + newPos.ToString ());
        float timer = 0f;
        while (timer < time && Vector3.Distance(obj.transform.position, newPos) > 0.6f) {
            //Debug.Log ("lerping " + (timer/ time));
            yield return Time.deltaTime;
            timer += Time.deltaTime;
            obj.transform.position = Vector3.Lerp(obj.transform.position, newPos, timer / time * speed);
        }

    }


    public static IEnumerator SLerp(Transform obj, Quaternion newPos, float time, float speed) {

        //Debug.Log ("lerping " + obj.name + " to " + newPos.ToString ());
        float timer = 0f;
        while (timer < time && obj.rotation != newPos) {
            //Debug.Log ("lerping " + (timer/ time));
            yield return Time.deltaTime;
            timer += Time.deltaTime;
            obj.rotation = Quaternion.Slerp(obj.rotation, newPos, timer / time * speed);
        }

    }



    /// <summary>
    /// repeats an action without hitting the recursion (call stack) limit in javascript if built to webgl. Usage::  RepeatAction(  () => MyFunction(myparameters)  ); \n
    /// to stop, use StopCoroutine() on the returned value of this function which you should store if you want to do so, but check if it's null first to avoid NullReferenceException
    /// </summary>
    /// <param name="milliseconds">milliseconds between</param>
    /// <param name="funcToRun">Func to run.</param>
    public static Coroutine RepeatAction(float seconds, Action funcToRun, bool runImmediately = false) {

        return Ext.instance.StartCoroutine(Ext.instance.Run(seconds, funcToRun, runImmediately));

    }


    /// <summary>
    /// used internally by this class to start and return the coroutine of RepeatAction() for convience of not having to write RepeatAction() yourself
    /// </summary>
    /// <param name="delay"></param>
    /// <param name="funcToRun"></param>
    /// <returns></returns>
	public IEnumerator Run(float delay, Action funcToRun, bool runImmediately) {
        if (runImmediately)
            funcToRun();
        while (true) {
            yield return new WaitForSeconds(delay);
            funcToRun();
        }




    }



    /// <summary>
    /// Pauses game after 1 frame
    /// </summary>
    public static void DebugFrame() {
        Ext.instance.StartCoroutine(DebugFrameRun());
    }

    static IEnumerator DebugFrameRun() {
        yield return new WaitForSeconds(Time.deltaTime * 1);
        Debug.Break();
    }


    //usually used for debug so not optimized
    /// <summary>
    /// returns a random string with spaceChance as chance for a space.
    /// </summary>
    /// <param name="size">max length</param>
    /// <param name="spaceChance">chance from 0.0 to 1.0</param>
    /// <returns></returns>
    public static string RandomString(int max, float spaceChance)
    {
        string returns = "";
        for (int i = 0; i < max; i++)
        {
            if (Roll100(spaceChance))
                returns += " ";
            else
                returns += RandomAlpha();

            if (Roll100(max / 100))
                break; //represents a 1/max chance to end early
        }
        return returns;
    }


    /// <summary>
    /// return a number as a string padded with zeros to fit a certain length for networking
    /// </summary>
    /// <param name="number"></param>
    /// <param name="length"></param>
    /// <returns></returns>
    public static string PadInt(int number, int length) {
        string returns = number.ToString();

        while (returns.Length < length)
        {
            returns = "0" + returns;
        }
        return returns;

    }


    static string rlchars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
    /// <summary>
    /// random letter from a-z including caps
    /// </summary>
    /// <returns></returns>
    public static char RandomLetter()
    {
        return rlchars[UnityEngine.Random.Range(0, rlchars.Length - 1)];
    }

    static string rcchars = "$%#@!*abcdefghijklmnopqrstuvwxyz1234567890?;:ABCDEFGHIJKLMNOPQRSTUVWXYZ^&";
    /// <summary>
    /// random char of all keyboard characters - $%#@!*abcdefghijklmnopqrstuvwxyz1234567890?;:ABCDEFGHIJKLMNOPQRSTUVWXYZ^&
    /// </summary>
    /// <returns></returns>
    public static char RandomChar()
    {
        return rcchars[UnityEngine.Random.Range(0, rlchars.Length - 1)];
    }

    static string rachars = "abcdefghijklmnopqrstuvwxyz1234567890ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    /// <summary>
    /// random number or a lowercase/caps letter
    /// </summary>
    /// <returns></returns>
    public static char RandomAlpha()
    {
        return rachars[UnityEngine.Random.Range(0, rlchars.Length - 1)];
    }

    /// <summary>
    /// rolls chance out of a hundred from 0.0 to 1.0
    /// </summary>
    /// <param name="chance">chance out of a hundred from a decimal e.g. 0.175 is a 17.5% chance</param>
    /// <returns></returns>
    public static bool Roll100(float chance)
    {
        if (UnityEngine.Random.value < chance)
            return true;
        else
            return false;
    }


    /// <summary>
    /// randomly twists a number between two values relative to 1 (ex: 0.8, 1.2)
    /// it is exponentially harder to gravitate away from the original because of multiple rolls
    /// </summary>
    /// <param name="number">number to modify</param>
    /// <param name="minMod">minimum to modify</param>
    /// <param name="maxMod">maximum to modify</param>
    public static void ModExponentially10(ref int number, float minMod, float maxMod, int numberRolls)
    {
        //use a random bool to check which mod we're using, then calculate exponential effect of roll to make it easy calculation...
        float random = 1;
        while (numberRolls > 0)
        {
            random *= UnityEngine.Random.Range(minMod, maxMod);
            numberRolls--;
        }

        number = (int)(number * random);
        Debug.Log("rolled " + random + " resulting in " + number);
    }

    /// <summary>
    /// returns the Nth byte of a number. 0 is the first byte, 1 is the second..
    /// </summary>
    /// <param name="number"></param>
    /// <returns></returns>
    public int IntByteN(int target, int number)
    {
        return (target >> (8 * number)) & 0xff;

        
    }


    public static bool randomBool { get
        {
            return UnityEngine.Random.value > 0.5;
        }
    }




    /// <summary>
    /// returns -1 when to the left, 1 to the right, and 0 for forward/backward
    /// </summary>
    /// <param name="fwd">transform.forward</param>
    /// <param name="targetDir">from </param>
    /// <param name="up">transform.up</param>
    /// <returns></returns>
    public static int AngleDirLR(Vector3 fwd, Vector3 targetDir, Vector3 up)
    {
        float dir = Vector3.Dot(
            Vector3.Cross(fwd, targetDir),
            up);

        if (dir > 0.0f)
        {
            return 1;
        }
        else if (dir < 0.0f)
        {
            return -1;
        }
        else
        {
            return 0;
        }
    }

    /// <summary>
    ///  returns -1 when to the back, 1 to the front
    /// </summary>
    /// <param name="fwd"></param>
    /// <param name="toTarget"> target.position - transform.position </param>
    /// <returns></returns>
    public static bool AngleDirFB(Vector3 fwd, Vector3 toTarget)
    {

        if (Vector3.Dot(toTarget.normalized, fwd) > 0)
        {
            return true; //is in front
        }
        else
        {
            return false; //not
        }
    }


    /// <summary>
    /// returns 1 if left, 2 if forward, 3 if right, 4 if back
    /// </summary>
    /// <param name="self"></param>
    /// <param name="target"></param>
    /// <returns></returns>
    public static int AngleDir4(Transform self, Vector3 target)
    {
        float lr = AngleDirLR(self.forward, target - self.position, self.up);
        if (lr == -1)
            return 1; //left
        if (lr == 1)
            return 3; //right

        //neither, must be FB

        if (AngleDirFB(self.forward, target - self.position))
            return 2; //forward
        else
            return 4; //back
    }

    /// <summary>
    /// endian safe GUID
    /// </summary>
    /// <returns></returns>
    public static string RandomGUID()
    {
        //byte[] bytes = Convert.FromBase64String(RandomString(8, 0));
        /*
        Array.Reverse(bytes, 0, 4);
        Array.Reverse(bytes, 4, 2);
        Array.Reverse(bytes, 6, 2);*/
        return new Guid().ToString();
    }



    /// <summary>
    /// X byte GUID
    /// </summary>
    /// <returns></returns>
    public static string GUIDX(int maxSize)
    {
        char[] chars = new char[62];
        string a;
        a = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
        chars = a.ToCharArray();
        int size = maxSize;
        byte[] data = new byte[1];
        RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider();
        crypto.GetNonZeroBytes(data);
        size = maxSize;
        data = new byte[size];
        crypto.GetNonZeroBytes(data);
        StringBuilder result = new StringBuilder(size);
        foreach (byte b in data)
        { result.Append(chars[b % (chars.Length - 1)]); }
        return result.ToString();
    }


    public static void TimeCode(string description, Action action)
    {
        var watch = System.Diagnostics.Stopwatch.StartNew();
        action.Invoke();
        watch.Stop();
        Debug.Log(description + " took " + watch.ElapsedMilliseconds);
    }

    public static void WriteInt(ref byte[] array, ref int value, int index)
    {
        array[index] = (byte)value;
        array[index + 1] = (byte)(value >> 8);
    }
    public static void WriteInt(ref byte[] array, int value, int index)
    {
        array[index] = (byte)value;
        array[index + 1] = (byte)(value >> 8);
    }
    public static void ReadInt(ref byte[] array, ref int changed, int index)
    {
        changed = (array[index] | array[index + 1] << 8);
    }

    public static void WriteInts(ref byte[] array, int startIndex, params int[] values)
    {
        for(int i = 0; i < values.Length; i+= 2)
        {
            array[startIndex + i] = (byte)values[i];
            array[startIndex + i + 1] = (byte)(values[i + 1] >> 8);
        }
    }

    static Vector3 tempRot;
    public static void LookAtOnlyY(Transform target, Transform recipient)
    {
        recipient.LookAt(target);
        tempRot = recipient.eulerAngles;
        tempRot.x = 0;
        tempRot.z = 0;
        recipient.eulerAngles = tempRot;

    }

    public static int ExpNeededJRPG(int currentLevel)
    {
        return 500 * (currentLevel * currentLevel) - (500 * currentLevel);
    }





    /// <summary>
    /// gets all classes that inherit from type
    /// </summary>
    public static List<Type> GetAllTypesInherited(Type type)
    {
        var types = AppDomain.CurrentDomain.GetAssemblies() //linq expression to get all spells
            .SelectMany(s => s.GetTypes())
            .Where((p) => {

                if (type.IsAssignableFrom(p) && !p.IsInterface && p.Name != type.Name)
                { //if inherit ISpell interface and not ISpell and not Spell, this is a spell we created
                    Debug.Log("loading singleton: " + p.Name);

                    if (p.GetConstructor(Type.EmptyTypes) == null)
                    {
                        Debug.Log(p.Name + " does not have a parameterless constructor. Likely it is a intermediate class, not an instance of " + type.Name + ". Skipping singleton creation");
                        return false; //skip
                    }

                    Activator.CreateInstance(p); //create new instance, which should add itself to the singleton list Spell.spells

                    return true;
                }
                else
                    return false;
            });
        return types.ToList();
    }


} //end class Ext
