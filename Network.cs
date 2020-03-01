using Photon.Pun;
using Photon.Realtime;
using Timer = UnityEngine.Time;
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
/// <summary>
/// Plugin For PhotonPun 0.6 in testing
/// </summary>
public class Network
{
    private static Network myClass;
    private static Converters ConvertersInstance;
    private float timer;
    private static List<object> ToSeend;
    private static readonly string[] RegionName = new string[12]
{
    //Region     |     Token
    "Asia",//           asia
    "Australia",//      au
    "Canada East",//    cae
    "Europe",//         eu
    "India",//          ind
    "Japan",//          jp
    "Russia",//         ru
    "Russia East",//    rue
    "South America",//  sa
    "South Korea",//    kr
    "USA East",//       us
    "USA West"//        usw
};
    /// <summary>
    /// Instance of Converters class
    /// </summary>
    public static Converters Convert
    {
        get { return ConvertersInstance ?? (ConvertersInstance = new Converters()); }
    }
    /// <summary>
    /// Instance of Network class
    /// </summary>
    public static Network Get
    {
        get { return myClass ?? (myClass = new Network()); }
    }
    #region Destroy
    /// <summary>
    /// Destroy object in Network or offline mode
    /// </summary>
    /// <param name="Object"> GameObject requires PhotonView </param>
    /// <param name="Offline">Set this true, if code must run in offline</param>
    public static void Destroy(GameObject Object, bool Offline)
    {
        if (Object != null)
        {
            if (!IsOffline())
            {
                if (Object.GetComponent<PhotonView>().IsMine)
                {
                    PhotonNetwork.Destroy(Object);
                }
                else
                {
                    Object.SetActive(false);
                    UnityEngine.Object.Destroy(Object,1f);
                }
            }
            else
            if (Offline)
            {
                {
                    UnityEngine.Object.Destroy(Object);
                }
            }
        }
    }
    /// <summary>
    /// Destroy Timer Use StartCoroutine(Network.Get.Destroy());
    /// </summary>
    /// <param name="Object">GameObject requires PhotonView </param>
    /// <param name="Offline">Set this true, if code must run in offline</param>
    /// <param name="time"></param>
    /// <returns></returns>
    public IEnumerator Destroy(GameObject Object, bool Offline, float time)
    {
        yield return new WaitForSeconds(time);
        if (Object != null)
        {
            if (Object.activeInHierarchy)
            {
                Destroy(Object, Offline);
            }
        }
    }
    #endregion
    #region Instantiate
    /// <summary>
    /// Instantiate in network or offline mode
    /// </summary>
    /// <param name="IsSceneObject">Set this to true if is not a Player and not spawned by Player</param>
    /// <param name="Object">The prefab must be in a folder called Resources. more information in doc photon pun</param>
    /// <param name="Position"></param>
    /// <param name="Rotation"></param>
    /// <returns></returns>
    public static GameObject Instantiate(bool IsSceneObject, GameObject Object, Vector3 Position, Quaternion Rotation)
    {
        if (!IsOffline())
        {
            if (IsSceneObject)
            {
                return PhotonNetwork.InstantiateSceneObject(Object.name, Position, Rotation);
            }
            else
            {
                return PhotonNetwork.Instantiate(Object.name, Position, Rotation);
            }
        }
        else
        {
            return UnityEngine.Object.Instantiate(Object, Position, Rotation);
        }
    }
    /// <summary>
    /// Instantiate in network or offline mode
    /// </summary>
    /// <param name="IsSceneObject">Set this to true if is not a Player and not spawned by Player</param>
    /// <param name="Object">The prefab must be in a folder called Resources. more information in doc photon pun</param>
    /// <param name="Position"></param>
    /// <param name="Rotation"></param>
    /// <param name="group"></param>
    /// <returns></returns>
    public static GameObject Instantiate(bool IsSceneObject, GameObject Object, Vector3 Position, Quaternion Rotation, byte group)
    {
        if (!IsOffline() && PhotonNetwork.InRoom)
        {
            if (IsSceneObject)
            {
                return PhotonNetwork.InstantiateSceneObject(Object.name, Position, Rotation, group);
            }
            else
            {
                return PhotonNetwork.Instantiate(Object.name, Position, Rotation, group);
            }
        }
        else
        {
            return UnityEngine.Object.Instantiate(Object, Position, Rotation);
        }
    }
    /// <summary>
    /// Instantiate in network or offline mode
    /// </summary>
    /// <param name="IsSceneObject">Set this to true if is not a Player and not spawned by Player</param>
    /// <param name="Object">The prefab must be in a folder called Resources. More information in doc photon pun</param>
    /// <param name="Position"></param>
    /// <param name="Rotation"></param>
    /// <param name="group"></param>
    /// <param name="data"></param>
    /// <returns></returns>
    public static GameObject Instantiate(bool IsSceneObject, GameObject Object, Vector3 Position, Quaternion Rotation, byte group, object[] data)
    {
        if (!IsOffline())
        {
            if (IsSceneObject)
            {
                return PhotonNetwork.InstantiateSceneObject(Object.name, Position, Rotation, group, data);
            }
            else
            {
                return PhotonNetwork.Instantiate(Object.name, Position, Rotation, group, data);
            }
        }
        else
        {
            return UnityEngine.Object.Instantiate(Object, Position, Rotation);
        }
    }
    #endregion
    #region Network Stuff And Region Stuff
    /// <summary>
    ///  return true if Is Connected,Ready and IsMine
    /// </summary>
    /// <param name="photonView"></param>
    /// <param name="Offline">Set this true, if code must run in offline</param>
    /// <returns></returns>
    public bool IsConnectedAndIsMine(PhotonView photonView, bool Offline)
    {
        if (!IsOffline())
        {
            if (photonView != null)
            {
                if (photonView.IsMine)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                Debug.LogWarning("PhotonView == null Return false");
                return false;
            }
        }
        else if (Offline)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    public static bool IsOffline()
    {
        return !PhotonNetwork.IsConnectedAndReady;
    }
    /// <summary>
    /// if is offlone return false
    /// </summary>
    public static bool IsMaster()
    {
        if (!IsOffline())
        {
            return PhotonNetwork.IsMasterClient;
        }
        else
        {
            return false;
        }
    }
    /// <summary>
    /// use PhotonNetwork.ConnectToRegion, returns fullname of Region
    /// </summary>
    /// <param name="region"></param>
    public static string ConnectToRegion(Region region)
    {
        if (!PhotonNetwork.IsConnected)
        {
            if (region != Region.ind)
            {
                PhotonNetwork.ConnectToRegion(region.ToString());
            }
            else
            {
                PhotonNetwork.ConnectToRegion("in");
            }
            if (PhotonNetwork.IsConnectedAndReady)
            {
                Debug.Log(":D");
                Photon.Pun.PhotonNetwork.JoinLobby();
            }
        }
        return RegionName[(int)region];
    }
    private static bool IsConnected;
    private static Region Reg;
    /// <summary>
    /// 
    /// </summary>
    /// <param name="region"></param>
    public static void ChangeRegion(Region region)
    {
        if (Reg != region)
        {
            Debug.Log("Change of region from " + GetRegionFullName((int)Reg) + " to " + GetRegionFullName((int)region));
            IsConnected = false;
            Reg = region;
        }
        // string RegInd;
        if (PhotonNetwork.IsConnectedAndReady && !IsConnected)
        {

            IsConnected = true;
            PhotonNetwork.Disconnect();
        }
        else
        {
            if (region.ToString() == "ind")
            {
                if (PhotonNetwork.CloudRegion != "in")
                {
                    ConnectToRegion(region);
                }
            }
            else
            {
                if (PhotonNetwork.CloudRegion != region.ToString())
                {
                    ConnectToRegion(region);

                }
            }
        }
    }
    public static string GetRegionFullName(int Index)
    {
        return RegionName[Index];
    }
    public enum Region
    {
        asia,
        au,
        cae,
        eu,
        ind,
        jp,
        ru,
        rue,
        sa,
        kr,
        us,
        usw,
    }
    #endregion
    #region Experimental
    public void SynkFloat(PhotonStream stream,float Value)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(Value);
        }
        else if(stream.IsReading)
        {
            Value = (float)stream.ReceiveNext();
        }
    }
    public  void SynkInt(PhotonStream stream, int Value)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(Value);
        }
        else if (stream.IsReading)
        {
            Value = (int)stream.ReceiveNext();
        }
    }
    public  void SynkColor(PhotonStream stream, Color Value)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(Convert.ColorToHex(Value));
        }
        else if (stream.IsReading)
        {
            Value = Convert.HexToColor((string)stream.ReceiveNext());
        }
    }
    public static void Synk(object obj)
    {
        if (ToSeend == null)
        {
            ToSeend = new List<object>(0);
            Debug.Log("CreateNewList");
        }
        //Debug.Log(obj.GetType().Name);
        if (obj.GetType().Name == "Single")
        {
            ToSeend.Add(obj);
        }
        else
        {
            Debug.LogWarning("Type" + obj.GetType() + " Is Not Allow");
        }
    }
    #endregion Experimental

}
public class Converters
{
    /// <summary>
    /// Convert value to byte mini 0 max 255
    /// </summary>
    /// <param name="Valie"></param>
    /// <returns></returns>
    public  byte IntToByte(int Value)
    {

        int Val = Mathf.Clamp(Value, 0, 255);
        Convert.ToByte(Val);
        return Convert.ToByte(Val);
    }
    public  int ByteToInt(byte Value)
    {
        return (int)Value;
    }
    /// <summary>
    /// converting ColorRGB to string
    /// </summary>
    /// <param name="color"></param>
    /// <returns></returns>
    public  string ColorToHex(Color color)
    {
        byte[] RGB = new byte[] { Convert.ToByte(color.r * 255), Convert.ToByte(color.g * 255), Convert.ToByte(color.b * 255) };
        return string.Format("{0:X2}{1:X2}{2:X2}", RGB[0], RGB[1], RGB[2]);
    }
    /// <summary>
    /// converting string to ColorRGB
    /// </summary>
    /// <param name="hex"></param>
    /// <returns></returns>
    public  Color HexToColor(string hex)
    {
        Color CalColor = new Color(int.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber),
             int.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber),
             int.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber));
        return new Color(CalColor.r / 255, CalColor.g / 255, CalColor.b / 255);
    }
    /// <summary>
    /// Converting bool[7] to single byte
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    /// 
    public  byte ConvertBoolToByte(bool[] bools)
    {
        if (bools != null)
        {
            if ((bools?.Length ?? 0) > 8) throw new ArgumentException("We can only fit 8 flags in a byte.", nameof(bools));
            byte result = byte.MinValue;
            for (int i = 0; i < (bools?.Length ?? 0); i++)
            {
                if (bools[i]) result += (byte)(1 << i);
            }
            return result;
        }
        else
        {
            return 0xFF;
        }
    }
    public  bool[] ConvertByteToBool(byte b)
    {
        int[] bl = new int[8];
        bool[] bools = new bool[8];
        for (int i = 0; i < bl.Length; i++)
        {
            bools[i] = ((b & (1 << i)) != 0);
        }
        return bools;
    }
    public  object ConvertArrayToObject(float[] array)
    {
        return array;
    }
    public object ConvertArrayToObject(int[] array)
    {
        return array;
    }
    public object ConvertArrayToObject(byte[] array)
    {
        return array;
    }
}
