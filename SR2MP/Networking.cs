﻿using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppSteamworks;
using MelonLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SR2MP
{
    public static class Networking
    {
        private delegate void PacketHandler(Packet _packet);
        private static Dictionary<int, PacketHandler> packetHandlers;

        public static void ListenData()
        {
            uint size;
            while (SteamNetworking.IsP2PPacketAvailable(out size, 0))
            {
                Il2CppStructArray<byte> _data = new Il2CppStructArray<byte>(size);
                uint bytesRead;

                CSteamID remoteId;

                if (SteamNetworking.ReadP2PPacket(_data, size, out bytesRead, out remoteId, 0))
                {
                    HandleReceivedData(_data);
                }
            }
        }

        public static void SendTCPData(Packet packet)
        {
            byte[] data = packet.ToArray();
            SteamNetworking.SendP2PPacket(SteamLobby.Instance.Receiver, data, (uint)data.Length, EP2PSend.k_EP2PSendReliable, 0);
        }

        public static void SendUDPData(Packet packet)
        {
            byte[] data = packet.ToArray();
            SteamNetworking.SendP2PPacket(SteamLobby.Instance.Receiver, data, (uint)data.Length, EP2PSend.k_EP2PSendUnreliable, 0);
        }

        private static void HandleReceivedData(byte[] _data)
        {
            using (Packet _packet = new Packet(_data))
            {
                int _packetId = _packet.ReadInt();
                packetHandlers[_packetId].Invoke(_packet);
            }
        }

        public static void InitializePackets()
        {
            packetHandlers = new Dictionary<int, PacketHandler>()
            {
                { (int)Packets.Message, HandleData.HandleMessage },
                { (int)Packets.Connect, HandleData.HandleConnection },
                { (int)Packets.Movement, HandleData.HandleMovement },
                { (int)Packets.Animations, HandleData.HandleAnimations },
                { (int)Packets.CameraAngle, HandleData.HandleCameraAngle },
                { (int)Packets.VacconeState, HandleData.HandleVacconeState },
                { (int)Packets.GameMode, HandleData.HandleGameModeSwitch },
                { (int)Packets.TimeRequest, HandleData.TimeRequested },
                { (int)Packets.Time, HandleData.HandleTime },
                { (int)Packets.SaveRequest, HandleData.SaveRequested },
                { (int)Packets.Save, HandleData.HandleSave },
                { (int)Packets.Slimes, HandleData.HandleSlimes }
            };
        }
    }
}
