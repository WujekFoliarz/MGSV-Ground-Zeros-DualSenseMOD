using Memory;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Media.AppBroadcasting;

namespace MGSV_Ground_Zeros_DualSenseMOD
{
    public class Game
    {
        Mem mem;
        public Game()
        {
            mem = new Mem();
            mem.OpenProcess(Process.GetProcessesByName("MgsGroundZeroes")[1].Id);
        }

        public WeaponType GetEquippedWeapon()
        {

            return (WeaponType)mem.ReadInt(Pointers.EquippedWeaponType);
        }

        public FOVZoomedIn GetZoomStatus()
        {
            return (FOVZoomedIn)mem.ReadInt(Pointers.Zoom);
        }

        public IDroidTab GetIDroidTab()
        {
            return (IDroidTab)mem.ReadInt(Pointers.IDroidTAB);
        }

        public HoveringOverinIDroid GetHoveringOver()
        {
            return (HoveringOverinIDroid)mem.ReadInt(Pointers.IDroidHover);
        }

        public int GetClipSize()
        {
            return mem.ReadInt(Pointers.ClipSize);
        }
    }

    public enum WeaponType
    {
        None = 0,
        C4 = 3,
        Magazine = 8,
        Granade = 12,
        Flare_Granade = 13,
        Rifle = 14,
        Pistol = 16,
    }

    public enum FOVZoomedIn
    {
        No = 0,
        Yes = 4,
    }

    public enum IDroidTab
    {
        Navigation = 1,
        Map = 2,
        Menu = 3,
        Walkman_Log_MissionInfo = 4,
        Helicopter = 7,
    }

    public enum HoveringOverinIDroid
    {
        Nothing = 0,
        Marker = 1,
        HelicoperLZ = 2,
    }

    internal class Pointers
    {
        public static string EquippedWeaponType = "MgsGroundZeroes.exe+01ED8048,18,210,460";
        public static string Zoom = "MgsGroundZeroes.exe+01E1EB50,2F0,30,120,30,8";
        public static string IDroidTAB = "MgsGroundZeroes.exe+01ED8060,30,100,150,5B8";
        public static string IDroidHover = "MgsGroundZeroes.exe+01ED8060,30,100,20,8,160,3D4";
        public static string ClipSize = "MgsGroundZeroes.exe+01E5E918,430,48,268,3F4";
    }
}
