public class FlatVibrationConfig : VibrationConfig
    {
        private float _leftMotorFrequency = 0f;
        private float _rightMotorFrequency = 0f;
        private float _vibrationTime = 0f;

        public FlatVibrationConfig(float leftMotorFrequency, float rightMotorFrequency, float vibrationTime)
        {
            _leftMotorFrequency = leftMotorFrequency;
            _rightMotorFrequency = rightMotorFrequency;
            _vibrationTime = vibrationTime;
        }
        
        public override void Initialize()
        {
            LeftMotorFrequency = _leftMotorFrequency;
            RightMotorFrequency = _rightMotorFrequency;
            CurrentVibrationTime = _vibrationTime;
            VibrationTime = CurrentVibrationTime;
        }
    }