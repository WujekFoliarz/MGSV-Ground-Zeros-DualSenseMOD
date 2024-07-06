using Memory;
using System.Diagnostics;

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

        public bool IsGunAutomatic()
        {
            switch (mem.ReadInt(Pointers.IsAutomatic))
            {
                case 1:
                    return true;
                case 0:
                    return false;
                default:
                    return false;
            }
        }
    }

    public enum WeaponType
    {
        None = 0,
        C4 = 12,
        Empty_Magazine = 32,
        Granade = 48,
        Flare_Granade = 52,
        Rifle = 56,
        Pistol = 64,
    }

    public enum FOVZoomedIn
    {
        Default = 0,
        Second_Default = 1,
        Yes = 4,
        Spotted_or_Dying = 1024,
        Spotted = 1025,
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
        public static readonly string IsAutomatic = "MgsGroundZeroes.exe+01E7E3A8,68,B4C";
        public static readonly string EquippedWeaponType = "MgsGroundZeroes.exe+01E7E3A8,28,98,D88,B38";
        public static readonly string Zoom = "MgsGroundZeroes.exe+01E1EB50,2F0,30,120,30,8";
        public static readonly string IDroidTAB = "MgsGroundZeroes.exe+01ED8060,30,100,150,5B8";
        public static readonly string IDroidHover = "MgsGroundZeroes.exe+01ED8060,30,100,20,8,160,3D4";
        public static readonly string ClipSize = "MgsGroundZeroes.exe+01E5E918,430,48,268,3F4";
    }
}
