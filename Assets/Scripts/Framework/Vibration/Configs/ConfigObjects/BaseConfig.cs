public class BaseConfig : VibrationConfig{
    public override void Initialize()
    {
        LeftMotorFrequency = 0f;
        RightMotorFrequency = 0f;
        CurrentVibrationTime = 0f;
        VibrationTime = 0f;
    }
}
