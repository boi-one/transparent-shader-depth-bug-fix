namespace Vibration.Configs.ConfigObjects
{
    public class PullVibrationConfig : VibrationConfig
    {
        public override void Initialize()
        {
            LeftMotorFrequency = 0.5f;
            RightMotorFrequency = 0.5f;
            CurrentVibrationTime = 0.35f;
            VibrationTime = CurrentVibrationTime;
        }
    }
}