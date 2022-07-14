using Photon.Pun;
using Photon.Realtime;
using Il2CppSystem.Collections.Generic;
using ExitGames.Client.Photon;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System;
using System.Runtime.InteropServices;

namespace PendulumClient.Anti
{
    static class Serialization
    {
        public static byte[] ToByteArray(Il2CppSystem.Object obj)
        {
            if (obj == null) return null;
            var size = Marshal.SizeOf(obj);
            var bytes = new byte[size];
            var ptr = Marshal.AllocHGlobal(size);
            // Copy object byte-to-byte to unmanaged memory.
            Marshal.StructureToPtr(obj, ptr, false);
            // Copy data from unmanaged memory to managed buffer.
            Marshal.Copy(ptr, bytes, 0, size);
            // Release unmanaged memory.
            Marshal.FreeHGlobal(ptr);
            /*var bf = new Il2CppSystem.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            var ms = new Il2CppSystem.IO.MemoryStream();
            bf.Serialize(ms, obj);*/
            return bytes;
        }

        public static byte[] ToByteArray(object obj)
        {
            if (obj == null) return null;
            /*var bf = new BinaryFormatter();
            var ms = new MemoryStream();
            bf.Serialize(ms, obj);*/

            var size = Marshal.SizeOf(obj);
            // Both managed and unmanaged buffers required.
            var bytes = new byte[size];
            var ptr = Marshal.AllocHGlobal(size);
            // Copy object byte-to-byte to unmanaged memory.
            Marshal.StructureToPtr(obj, ptr, false);
            // Copy data from unmanaged memory to managed buffer.
            Marshal.Copy(ptr, bytes, 0, size);
            // Release unmanaged memory.
            Marshal.FreeHGlobal(ptr);

            return bytes;//ms.ToArray();
        }
        public static T FromByteArray<T>(byte[] data)
        {
            if (data == null) return default;
            /*BinaryFormatter bf = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream(data))
            {
                object obj = bf.Deserialize(ms);
                return (T)obj;
            }*/
            var size = Marshal.SizeOf(data);
            var bytes = new byte[size];
            var ptr = Marshal.AllocHGlobal(size);
            Marshal.Copy(bytes, 0, ptr, size);
            var your_object = (T)Marshal.PtrToStructure(ptr, typeof(T));
            Marshal.FreeHGlobal(ptr);
            return your_object;
        }

        public static T IL2CPPFromByteArray<T>(byte[] data)
        {
            if (data == null) return default(T);
            /*var bf = new Il2CppSystem.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            var ms = new Il2CppSystem.IO.MemoryStream(data);
            object obj = bf.Deserialize(ms);
            return (T)obj;*/
            var size = Marshal.SizeOf(data);
            var bytes = new byte[size];
            var ptr = Marshal.AllocHGlobal(size);
            Marshal.Copy(bytes, 0, ptr, size);
            var your_object = (T)Marshal.PtrToStructure(ptr, typeof(T));
            Marshal.FreeHGlobal(ptr);
            return your_object;
        }

        public static T FromIL2CPPToManaged<T>(Il2CppSystem.Object obj)
        {
            return FromByteArray<T>(ToByteArray(obj));
        }

        public static T FromManagedToIL2CPP<T>(object obj)
        {
            return IL2CPPFromByteArray<T>(ToByteArray(obj));
        }
    }
    static class PhotonExtensions
    {
        public static Photon.Realtime.RaiseEventOptions UnreliableEventOptions = new Photon.Realtime.RaiseEventOptions
        {
            field_Public_ReceiverGroup_0 = Photon.Realtime.ReceiverGroup.Others,
            field_Public_EventCaching_0 = Photon.Realtime.EventCaching.DoNotCache,
            //field_Public_ArrayOf_Int32_0 = new UnhollowerBaseLib.Il2CppStructArray<int>(1)
        };

        public static ExitGames.Client.Photon.SendOptions UnreliableOptions = new ExitGames.Client.Photon.SendOptions
        {
            DeliveryMode = ExitGames.Client.Photon.DeliveryMode.Unreliable
        };
        public static void OpRaiseEvent(byte code, object customObject, RaiseEventOptions RaiseEventOptions, SendOptions sendOptions)
        {
            Il2CppSystem.Object Object = Serialization.FromManagedToIL2CPP<Il2CppSystem.Object>(customObject);
            OpRaiseEvent(code, Object, RaiseEventOptions, sendOptions);
        }
        public static Il2CppSystem.Collections.Generic.Dictionary<int, Player> GetAllPhotonPlayers()
        {
            return VRC.Player.prop_Player_0.prop_Player_1.prop_Room_0.prop_Dictionary_2_Int32_Player_0;
        }


        public static void OpRaiseEvent(byte code, Il2CppSystem.Object customObject, RaiseEventOptions RaiseEventOptions, SendOptions sendOptions)
           => PhotonNetwork.Method_Private_Static_Boolean_Byte_Object_RaiseEventOptions_SendOptions_0
            (code,
             customObject,
             RaiseEventOptions,
             sendOptions);
    }
}
