using MGSV_Ground_Zeros_DualSenseMOD;
using Nefarius.ViGEm.Client;
using Nefarius.ViGEm.Client.Targets;
using Nefarius.ViGEm.Client.Targets.Xbox360;
using System.Diagnostics;
using Wujek_Dualsense_API;

Thread.Sleep(8000);

if(Process.GetProcessesByName("MGSV-Ground-Zeroes-DualSenseMOD").Count() > 1)
{
    Environment.Exit(0);
}

bool Running = true;
Dualsense dualsense = null;
int triggerThreshold = 5;

ViGEmClient client = new ViGEmClient();
IXbox360Controller x360Controller = client.CreateXbox360Controller();
bool sendRumble = false;
x360Controller.FeedbackReceived += X360Controller_FeedbackReceived;

x360Controller.Connect();


int ConvertRange(int value, int oldMin, int oldMax, int newMin, int newMax)
{
    if (oldMin == oldMax)
    {
        throw new ArgumentException("Old minimum and maximum cannot be equal.");
    }
    float ratio = (float)(newMax - newMin) / (float)(oldMax - oldMin);
    float scaledValue = (value - oldMin) * ratio + newMin;
    return Math.Clamp((int)scaledValue, newMin, newMax);
}


// Emulate controller
new Thread(() =>
{
    Thread.CurrentThread.IsBackground = true;
    Thread.CurrentThread.Priority = ThreadPriority.Lowest;
    Thread.Sleep(1000);
    while (Running)
    {
        if (dualsense != null && dualsense.Working)
        {
            x360Controller.SetButtonState(Xbox360Button.A, dualsense.ButtonState.cross);
            x360Controller.SetButtonState(Xbox360Button.B, dualsense.ButtonState.circle);
            x360Controller.SetButtonState(Xbox360Button.Y, dualsense.ButtonState.triangle);
            x360Controller.SetButtonState(Xbox360Button.X, dualsense.ButtonState.square);
            x360Controller.SetButtonState(Xbox360Button.Up, dualsense.ButtonState.DpadUp);
            x360Controller.SetButtonState(Xbox360Button.Left, dualsense.ButtonState.DpadLeft);
            x360Controller.SetButtonState(Xbox360Button.Right, dualsense.ButtonState.DpadRight);
            x360Controller.SetButtonState(Xbox360Button.Down, dualsense.ButtonState.DpadDown);

            //-32767 minimum
            //32766 max
            x360Controller.SetAxisValue(Xbox360Axis.LeftThumbX, (short)ConvertRange(dualsense.ButtonState.LX, 0, 255, -32767, 32766));
            x360Controller.SetAxisValue(Xbox360Axis.LeftThumbY, (short)ConvertRange(dualsense.ButtonState.LY, 255, 0, -32767, 32766));
            x360Controller.SetAxisValue(Xbox360Axis.RightThumbX, (short)ConvertRange(dualsense.ButtonState.RX, 0, 255, -32767, 32766));
            x360Controller.SetAxisValue(Xbox360Axis.RightThumbY, (short)ConvertRange(dualsense.ButtonState.RY, 255, 0, -32767, 32766));
            x360Controller.SetButtonState(Xbox360Button.LeftThumb, dualsense.ButtonState.L3);
            x360Controller.SetButtonState(Xbox360Button.RightThumb, dualsense.ButtonState.R3);


            if (triggerThreshold <= (byte)dualsense.ButtonState.L2)
                x360Controller.LeftTrigger = (byte)dualsense.ButtonState.L2;
            else
                x360Controller.LeftTrigger = 0;

            if (triggerThreshold <= (byte)dualsense.ButtonState.R2)
                x360Controller.RightTrigger = (byte)dualsense.ButtonState.R2;
            else
                x360Controller.RightTrigger = 0;

            x360Controller.SetButtonState(Xbox360Button.Start, dualsense.ButtonState.options);
            x360Controller.SetButtonState(Xbox360Button.Back, dualsense.ButtonState.share);
            x360Controller.SetButtonState(Xbox360Button.LeftShoulder, dualsense.ButtonState.L1);
            x360Controller.SetButtonState(Xbox360Button.RightShoulder, dualsense.ButtonState.R1);
            x360Controller.SetButtonState(Xbox360Button.Guide, dualsense.ButtonState.ps);


            Thread.Sleep(1);
        }
    }

}).Start();

void X360Controller_FeedbackReceived(object sender, Xbox360FeedbackReceivedEventArgs e)
{
    if (sendRumble)
    {
        dualsense.SetVibrationType(Vibrations.VibrationType.Standard_Rumble);
        dualsense.SetStandardRumble(e.LargeMotor, e.SmallMotor);
    }
}

new Thread(() => LookForControllers()).Start();
Thread.Sleep(1000);

Game game = new Game();
try
{
    dualsense.SetPlayerLED(LED.PlayerLED.PLAYER_1);
    bool idroidOpenFirstTime = true;
    bool helicopterTabFirstTime = true;
    bool lzconfirmed = false;
    bool markerRemoved = false;
    Stopwatch sw = Stopwatch.StartNew();

    while (Running)
    {
        if (Process.GetProcessesByName("MgsGroundZeroes").Count() == 0)
        {
            Running = false;
            dualsense.Dispose();
            Thread.Sleep(1000);
            Environment.Exit(0);
        }

        dualsense.SetVibrationType(Vibrations.VibrationType.Haptic_Feedback);

        WeaponType currentWeapon = game.GetEquippedWeapon();
        FOVZoomedIn Zoom = game.GetZoomStatus();
        IDroidTab IDroidTAB = game.GetIDroidTab();
        HoveringOverinIDroid HoveringOver = game.GetHoveringOver();
        int clipSize = game.GetClipSize();
        bool isAutomatic = game.IsGunAutomatic();
        
        switch (Zoom)
        {
            case FOVZoomedIn.Yes:
                sendRumble = false;
                dualsense.SetVibrationType(Vibrations.VibrationType.Haptic_Feedback);
                if (dualsense.ButtonState.options)
                {                  
                    triggerThreshold = 255;
                    dualsense.SetLeftTrigger(TriggerType.TriggerModes.Rigid_A, 0, 250, 255, 0, 0, 0, 0);
                    dualsense.SetRightTrigger(TriggerType.TriggerModes.Rigid_A, 0, 250, 255, 0, 0, 0, 0);

                    if (idroidOpenFirstTime)
                    {
                        idroidOpenFirstTime = false;
                        dualsense.SetLightbarTransition(255, 255, 255, 5, 5);
                        dualsense.PlayHaptics(Haptics.TerminalON, 1, 1, 1, true);
                    }
                }

                if (IDroidTAB == IDroidTab.Map && dualsense.ButtonState.cross && sw.ElapsedMilliseconds > 500)
                {
                    if (!markerRemoved)
                    {
                        dualsense.PlayHaptics(Haptics.MarkerPlaced, 0.5f, 0, 0, true);
                        markerRemoved = true;
                    }
                    else
                    {
                        dualsense.PlayHaptics(Haptics.MarkerRemoved, 0.5f, 0, 0, true);
                        markerRemoved = false;
                    }
                    sw.Restart();
                }

                if (IDroidTAB == IDroidTab.Helicopter && helicopterTabFirstTime && sw.ElapsedMilliseconds > 300)
                {
                    sw.Restart();
                    helicopterTabFirstTime = false;
                    dualsense.PlayHaptics(Haptics.PleaseSelectLZ, 0.5f, 0, 0, true);
                }
                else if (IDroidTAB == IDroidTab.Helicopter && HoveringOver == HoveringOverinIDroid.HelicoperLZ && dualsense.ButtonState.cross && sw.ElapsedMilliseconds > 500)
                {
                    dualsense.PlayHaptics(Haptics.LZConfirmed, 0.5f, 0, 0, true);
                    lzconfirmed = true;
                }
                else if (IDroidTAB != IDroidTab.Helicopter && !helicopterTabFirstTime)
                {
                    helicopterTabFirstTime = true;
                }

                break;
            case FOVZoomedIn.Default:
                sendRumble = true;
                if (clipSize > 0 && x360Controller.LeftTrigger > 120)
                {
                    switch (isAutomatic)
                    {
                        case true:
                            dualsense.SetLeftTrigger(TriggerType.TriggerModes.Rigid, 0, 0, 0, 0, 0, 0, 0);
                            dualsense.SetRightTrigger(TriggerType.TriggerModes.Pulse_B, 10, 255, 50, 0, 0, 0, 0);
                            triggerThreshold = 120;
                            break;
                        case false:
                            dualsense.SetLeftTrigger(TriggerType.TriggerModes.Rigid, 0, 0, 0, 0, 0, 0, 0);
                            dualsense.SetRightTrigger(TriggerType.TriggerModes.Rigid_AB, 93, 184, 255, 143, 71, 0, 0);
                            triggerThreshold = 120;
                            break;
                    }
                }
                else
                {
                    dualsense.SetRightTrigger(TriggerType.TriggerModes.Rigid_B, 0, 0, 0, 0, 0, 0, 0);
                }

                if (!idroidOpenFirstTime && sw.ElapsedMilliseconds > 2000)
                {
                    idroidOpenFirstTime = true;
                    lzconfirmed = false;
                    dualsense.PlayHaptics(Haptics.TerminalOFF, 1, 1, 1, false);
                    dualsense.SetLightbarTransition(0, 0, 0, 5, 10);
                }
                break;
            case FOVZoomedIn.Second_Default:
                sendRumble = true;
                if (clipSize > 0 && x360Controller.LeftTrigger > 120)
                {
                    switch (isAutomatic)
                    {
                        case true:
                            dualsense.SetLeftTrigger(TriggerType.TriggerModes.Rigid, 0, 0, 0, 0, 0, 0, 0);
                            dualsense.SetRightTrigger(TriggerType.TriggerModes.Pulse_B, 10, 255, 50, 0, 0, 0, 0);
                            triggerThreshold = 120;
                            break;
                        case false:
                            dualsense.SetLeftTrigger(TriggerType.TriggerModes.Rigid, 0, 0, 0, 0, 0, 0, 0);
                            dualsense.SetRightTrigger(TriggerType.TriggerModes.Rigid_AB, 93, 184, 255, 143, 71, 0, 0);
                            triggerThreshold = 120;
                            break;
                    }
                }
                else
                {
                    dualsense.SetRightTrigger(TriggerType.TriggerModes.Rigid_B, 0, 0, 0, 0, 0, 0, 0);
                }

                if (!idroidOpenFirstTime && sw.ElapsedMilliseconds > 2000)
                {
                    idroidOpenFirstTime = true;
                    lzconfirmed = false;
                    dualsense.PlayHaptics(Haptics.TerminalOFF, 1, 1, 1, false);
                    dualsense.SetLightbarTransition(0, 0, 0, 5, 10);
                }
                break;
            case FOVZoomedIn.Spotted:
                sendRumble = true;
                dualsense.SetLightbarTransition(0, 255, 255, 5, 5);
                break;
            case FOVZoomedIn.Spotted_or_Dying:
                sendRumble = true;
                dualsense.SetLightbarTransition(255, 0, 0, 5, 5);
                break;
        }

        Thread.Sleep(25);
    }
}
catch (Exception e) { Console.WriteLine(e); }

void LookForControllers()
{
    while (Running)
    {
        if (dualsense == null || !dualsense.Working)
        {
            try
            {
                dualsense = new Dualsense(0);
                dualsense.Start();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                continue;
            }
        }
        Thread.Sleep(1000);
    }
}
