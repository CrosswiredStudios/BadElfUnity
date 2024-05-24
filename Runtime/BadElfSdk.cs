using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace CrosswiredStudios.BadElf {
    public class BadElfSdk : MonoBehaviour
    {
        #if UNITY_IOS
        [DllImport("__Internal")]
        private static extern IntPtr BadElfSdk_GetConnectedDevices(ref int deviceCount);

        [DllImport("__Internal")]
        private static extern void BadElfSdk_FreeAccessoryInfo(IntPtr infoArray, int count);

        [DllImport("__Internal")]
        private static extern void BadElfSdk_SetupControllerForAccessory(int index, string protocolString);

        [DllImport("__Internal")]
        private static extern bool BadElfSdk_OpenSession();

        [DllImport("__Internal")]
        private static extern void BadElfSdk_CloseSession();

        [DllImport("__Internal")]
        private static extern void BadElfSdk_WriteData(string data);

        [DllImport("__Internal")]
        private static extern uint BadElfSdk_ReadBytesAvailable();

        [DllImport("__Internal")]
        private static extern IntPtr BadElfSdk_ReadData(uint bytesToRead);

        [DllImport("__Internal")]
        private static extern void BadElfSdk_FreePointer(IntPtr ptr);
        #endif

        [StructLayout(LayoutKind.Sequential)]
        public struct BadElfAccessoryInfo
        {
            public string name;
            public string modelNumber;
            public string serialNumber;
            public string hardwareRevision;
            public string firmwareRevision;
            public string protocolString;
        }

        public void Start()
        {
            // Example usage
            int deviceCount = 0;
            BadElfAccessoryInfo[] devices = GetConnectedDevices(ref deviceCount);
            if (deviceCount > 0)
            {
                Debug.Log("Devices found:");
                for (int i = 0; i < deviceCount; i++)
                {
                    Debug.Log($"Device {i}: {devices[i].name}, Model: {devices[i].modelNumber}, Serial: {devices[i].serialNumber}, Hardware: {devices[i].hardwareRevision}, Firmware: {devices[i].firmwareRevision}, Protocol: {devices[i].protocolString}");
                }
                // Select the first device and open a session
                SetupControllerForAccessory(0, devices[0].protocolString);
                if (OpenSession())
                {
                    Debug.Log("Session opened successfully");
                    InvokeRepeating("PollForData", 1.0f, 1.0f);
                }
            }
            else
            {
                Debug.Log("No devices found");
            }
        }

        public BadElfAccessoryInfo[] GetConnectedDevices(ref int deviceCount)
        {
            #if UNITY_IOS
            IntPtr infoPtr = BadElfSdk_GetConnectedDevices(ref deviceCount);
            if (infoPtr == IntPtr.Zero)
            {
                Debug.Log("No connected devices found.");
                return new BadElfAccessoryInfo[0];
            }

            BadElfAccessoryInfo[] devices = new BadElfAccessoryInfo[deviceCount];
            IntPtr currentPtr = infoPtr;
            for (int i = 0; i < deviceCount; i++)
            {
                devices[i] = Marshal.PtrToStructure<BadElfAccessoryInfo>(currentPtr);
                currentPtr = (IntPtr)(currentPtr.ToInt64() + Marshal.SizeOf<BadElfAccessoryInfo>());
            }
            BadElfSdk_FreeAccessoryInfo(infoPtr, deviceCount);
            return devices;
            #else
            Debug.Log("Not on iOS");
            return new BadElfAccessoryInfo[0];
            #endif
        }

        public void SetupControllerForAccessory(int index, string protocolString)
        {
            #if UNITY_IOS
            BadElfSdk_SetupControllerForAccessory(index, protocolString);
            #else
            Debug.Log("Not on iOS");
            #endif
        }

        public bool OpenSession()
        {
            #if UNITY_IOS
            return BadElfSdk_OpenSession();
            #else
            Debug.Log("Not on iOS");
            return false;
            #endif
        }

        public void CloseSession()
        {
            #if UNITY_IOS
            BadElfSdk_CloseSession();
            #else
            Debug.Log("Not on iOS");
            #endif
        }

        public void WriteData(string data)
        {
            #if UNITY_IOS
            BadElfSdk_WriteData(data);
            #else
            Debug.Log("Not on iOS");
            #endif
        }

        public uint ReadBytesAvailable()
        {
            #if UNITY_IOS
            return BadElfSdk_ReadBytesAvailable();
            #else
            Debug.Log("Not on iOS");
            return 0;
            #endif
        }

        public string ReadData(uint bytesToRead)
        {
            #if UNITY_IOS
            IntPtr dataPtr = BadElfSdk_ReadData(bytesToRead);
            string data = Marshal.PtrToStringAnsi(dataPtr);
            BadElfSdk_FreePointer(dataPtr);
            return data;
            #else
            Debug.Log("Not on iOS");
            return null;
            #endif
        }

        private void PollForData()
        {
            uint availableBytes = ReadBytesAvailable();
            if (availableBytes > 0)
            {
                string data = ReadData(availableBytes);
                Debug.Log("Data received: " + data);
            }
            else
            {
                Debug.Log("No data available");
            }
        }
    }
}