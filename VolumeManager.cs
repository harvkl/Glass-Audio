using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NAudio.CoreAudioApi;

namespace Glass_Audio
{
    public class VolumeManager
    {
        // enum of audio devices in Windows
        private readonly MMDeviceEnumerator _deviceEnumenator;

        // current default audio device
        private MMDevice? _defaultDevice;

        // events for UI
        public event Action<double>? VolumeChanged;
        public event Action<bool>? MuteChanged;

        public VolumeManager()
        {
            _deviceEnumenator = new MMDeviceEnumerator();

            // getting default device
            _defaultDevice = _deviceEnumenator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);

            if (_defaultDevice != null )
            {
                // following the volume changed event
                _defaultDevice.AudioEndpointVolume.OnVolumeNotification += AudioEndpointVolume_OnVolumeNotification;
            }
        }

        private void AudioEndpointVolume_OnVolumeNotification(AudioVolumeNotificationData data)
        {
            double volumePercent = data.MasterVolume * 100;

            // calling event with volume in percent
            VolumeChanged?.Invoke(volumePercent);

            // calling event that notifies about Mute status
            MuteChanged?.Invoke(data.Muted);
        }

        public double GetVolume()
        {
            return _defaultDevice?.AudioEndpointVolume.MasterVolumeLevelScalar * 100 ?? 0;
        }

        public void SetVolume(double volume)
        {
            try
            {
                if (_defaultDevice != null)
                {
                    _defaultDevice.AudioEndpointVolume.MasterVolumeLevelScalar = (float)(volume / 100);
                }
            }
            catch 
            { 
                // ingoring all errors
            }
        }

        public bool IsMuted()
        {
            return _defaultDevice?.AudioEndpointVolume.Mute ?? false;
        }

        public void ToogleMute()
        {
            try
            {
                if (_defaultDevice != null)
                {
                    _defaultDevice.AudioEndpointVolume.Mute = !_defaultDevice.AudioEndpointVolume.Mute;
                }
            }
            catch
            {
                // ingoring all errors
            }
        }
    }
}
