using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using Windows.Media.Control; //namespace for WinRT API
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Windows.Storage.Streams;

namespace Glass_Audio
{
    class AudioManager
    {
        private GlobalSystemMediaTransportControlsSessionManager? _controlsSessionManager; //all apps that run smth
        private GlobalSystemMediaTransportControlsSession? _controlsCurrentSession; //exact app that run smth

        //events for WPF UI
        public event Action<string>? TrackInfoChanged;
        public event Action<bool>? IsPlayingChanged;
        public event Action<ImageSource?>? ImageChanged;

        public async Task InitializeAsync() //initializing the connection to Windows Media Manager
        {

            _controlsSessionManager = await GlobalSystemMediaTransportControlsSessionManager.RequestAsync();
            _controlsSessionManager.SessionsChanged += SessionsChanged;

            UpdateCurrentSession();
        }

        //event analyzer of media-sessions list changes in Windows
        private void SessionsChanged(GlobalSystemMediaTransportControlsSessionManager sender, SessionsChangedEventArgs args)
        {
            UpdateCurrentSession();
        }

        //method for updating the pointer to current active media-session
        private void UpdateCurrentSession()
        {
            if (_controlsCurrentSession != null) //unfollowing from old session events
            {
                _controlsCurrentSession.MediaPropertiesChanged -= CurrentSession_MediaPropertiesChanged;
                _controlsCurrentSession.PlaybackInfoChanged -= CurrentSession_PlaybackInfoChanged;
                _controlsCurrentSession = null;
            }

            if (_controlsSessionManager == null) { return; }

            /*------------------------------------------------------------------------------- THIS SECTION OF CODE IS FOR AUTOMATIC WINDOWS SESSION SELECTION

                //getting the new session from Windows
                _currentSession = _sessionManager?.GetCurrentSession();

                if (_currentSession != null)

            -------------------------------------------------------------------------------*/

            //getting the new sessions from Windows
            var sessions = _controlsSessionManager.GetSessions(); // we do NOT letting Windows decide which session is priority

            if (sessions.Count > 0)
            {
                _controlsCurrentSession = sessions[0]; // we are selecting first session in list

                // following on events from selected session
                _controlsCurrentSession.MediaPropertiesChanged += CurrentSession_MediaPropertiesChanged;
                _controlsCurrentSession.PlaybackInfoChanged += CurrentSession_PlaybackInfoChanged;

                // hooking data
                _ = UpdateTrackInfoAsync();
                UpdatePlaybackStatus();
            }

            else
            {
                TrackInfoChanged?.Invoke("No active media session");
                IsPlayingChanged?.Invoke(false);
                ImageChanged?.Invoke(null); //clears image, if session doesn't selected
            }
        }

        //analyzer of metadata changes
        private async void CurrentSession_MediaPropertiesChanged(GlobalSystemMediaTransportControlsSession sender, MediaPropertiesChangedEventArgs args)
        {
            await UpdateTrackInfoAsync(); //when song is changed, reading properties again
        }

        //analyzer of playstatus changes
        private void CurrentSession_PlaybackInfoChanged(GlobalSystemMediaTransportControlsSession sender, PlaybackInfoChangedEventArgs args)
        {
            UpdatePlaybackStatus(); //when status is changed, updating it
        }

        //asynced demanding metadata from current session (name and author) and calls the UI update event
        private async Task UpdateTrackInfoAsync()
        {
            if (_controlsCurrentSession == null)
            {
                return;
            }

            try
            {
                var props = await _controlsCurrentSession.TryGetMediaPropertiesAsync();

                if (props != null)
                {
                    //making a string with Artist and Title if we have it both, otherwise just output Title
                    string info = string.IsNullOrEmpty(props.Artist)
                        ? props.Title
                        : $"{props.Artist} ` {props.Title}"; // we do the symbol '`' to not conflict with title if there is '-'

                    TrackInfoChanged?.Invoke(info); //noticing UI that data is updated

                    // processing the preview (cover)
                    if (props.Thumbnail != null)
                    {
                        // opening the read stream from system object of Windows
                        using var randomAccessStream = await props.Thumbnail.OpenReadAsync();
                        using var inputStream = randomAccessStream.GetInputStreamAt(0);
                        using var dataReader = new DataReader(inputStream);

                        // reading all the bytes of image to array
                        var size = (int)randomAccessStream.Size;
                        var bytes = new byte[size];
                        await dataReader.LoadAsync((uint)size);
                        dataReader.ReadBytes(bytes);

                        // converting the array of bytes to BitmapImage
                        var bitmap = new BitmapImage();
                        bitmap.BeginInit();
                        bitmap.CacheOption = BitmapCacheOption.OnLoad; //loading image into memory
                        bitmap.StreamSource = new MemoryStream(bytes);
                        bitmap.EndInit();

                        bitmap.Freeze(); // IT IS IMPORTANT TO USE Freeze(), otherwise there will be an error

                        ImageChanged?.Invoke(bitmap);
                    }
                    else
                    {
                        ImageChanged?.Invoke(null); // if there is no cover at all
                    }
                }
            }
            catch
            {
                //ignoring all the errors
            }
        }

        //demanding current status (play, pause, stop)
        private void UpdatePlaybackStatus()
        {
            if (_controlsCurrentSession == null)
            {
                return;
            }

            var info = _controlsCurrentSession.GetPlaybackInfo();
            
            //checking if "Playing" and providing true/false to UI
            IsPlayingChanged?.Invoke(info?.PlaybackStatus == GlobalSystemMediaTransportControlsSessionPlaybackStatus.Playing);
        }

        ///////////////////////////////////
        /// sending commands to Windows ///
        ///////////////////////////////////
        
        public async Task TogglePlayPauseAsync()
        {
            if (_controlsCurrentSession == null)
            {
                return;
            }

            var info = _controlsCurrentSession.GetPlaybackInfo();

            if (info?.PlaybackStatus == GlobalSystemMediaTransportControlsSessionPlaybackStatus.Playing)
            {
                await _controlsCurrentSession.TryPauseAsync();
            }
            else
            {
                await _controlsCurrentSession.TryPlayAsync();
            }

        }
        //switches to a next track/video
        public async Task NextAsync()
        {
            if (_controlsCurrentSession != null)
            {
                await _controlsCurrentSession.TrySkipNextAsync();
            }
        }

        //switches to a previous track/video
        public async Task PrevAsync()
        {
            if (_controlsCurrentSession != null)
            {
                await _controlsCurrentSession.TrySkipPreviousAsync();
            }
        }
    }
}
