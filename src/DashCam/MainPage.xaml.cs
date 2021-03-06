﻿using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.Rfcomm;
using Windows.Devices.Enumeration;
using Windows.Media.Capture;
using Windows.Media.MediaProperties;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace DashCam
{
	public sealed partial class MainPage : Page
	{
		MediaCaptureInitializationSettings captureInitSettings;
		DeviceInformation camera;
		MediaEncodingProfile profile;

		public MediaCapture mediaCapture;
		public bool isRecording;
		public string fileName;
		public string message;
	    public bool phoneIsDetected;

		public MainPage()
		{
			this.InitializeComponent();

			InitCaptureSettings();

			InitMediaCapture();

		    IsPhoneDetected("");

        }

		public async Task StartMediaCaptureProcess()
		{
			//message += $"{Environment.NewLine}start process:";
			//txtBlock.Text = message;
			var sw = new Stopwatch();
			isRecording = true;
			while (isRecording)
			{
				sw.Start();
				fileName = GenerateFileName("mp4");
				var storageFile = await Windows.Storage.KnownFolders.VideosLibrary.CreateFileAsync(fileName, Windows.Storage.CreationCollisionOption.GenerateUniqueName);
				//message += $"{Environment.NewLine}start: {fileName}, ";
				//txtBlock.Text = message;
				await mediaCapture.StartRecordToStorageFileAsync(profile, storageFile);
				while (sw.Elapsed.TotalMilliseconds <= 60000)
				{
					//delay between start recording and stop recording
					
				}
                //message += $"{Environment.NewLine}stop: {fileName}, ";
                //txtBlock.Text = message;
			    IsPhoneDetected("");

                await mediaCapture.StopRecordAsync();
				sw.Reset();
			}
		}

		public async Task StopMediaCaptureProcess()
		{
			if (isRecording)
			{
				//message += $"{Environment.NewLine}stop process.";
				//txtBlock.Text = message;
				await mediaCapture.StopRecordAsync();
				isRecording = false;
			}
		}

		public async void InitCaptureSettings()
		{
			captureInitSettings = new MediaCaptureInitializationSettings();
			captureInitSettings.StreamingCaptureMode = StreamingCaptureMode.AudioAndVideo;
			captureInitSettings.PhotoCaptureSource = PhotoCaptureSource.VideoPreview;
			
			var devices = await DeviceInformation.FindAllAsync(DeviceClass.VideoCapture);
			camera = devices.FirstOrDefault();
			captureInitSettings.VideoDeviceId = camera.Id;
		}

		public async void InitMediaCapture()
		{
			mediaCapture = new MediaCapture();
			await mediaCapture.InitializeAsync(captureInitSettings);
			//test
			var properties = mediaCapture.VideoDeviceController;

			// Add video stabilization effect during Live Capture
			//Windows.Media.Effects.VideoEffectDefinition def = new Windows.Media.Effects.VideoEffectDefinition(Windows.Media.VideoEffects.VideoStabilization);
			//await mediaCapture.AddVideoEffectAsync(def, MediaStreamType.VideoRecord);

			profile = MediaEncodingProfile.CreateMp4(VideoEncodingQuality.Auto);
			profile.Video.PixelAspectRatio.Numerator = 16;
			profile.Video.PixelAspectRatio.Denominator = 9;

			// Use MediaEncodingProfile to encode the profile
			//System.Guid MFVideoRotationGuild = new System.Guid("C380465D-2271-428C-9B83-ECEA3B4A85C1");
			//int MFVideoRotation = ConvertVideoRotationToMFRotation(VideoRotation.None);
			//profile.Video.Properties.Add(MFVideoRotationGuild, PropertyValue.CreateInt32(MFVideoRotation));

			//// add the mediaTranscoder 
			//var transcoder = new Windows.Media.Transcoding.MediaTranscoder();
			//transcoder.AddVideoEffect(Windows.Media.VideoEffects.VideoStabilization);

			//// wire to preview XAML element
			//capturePreview.Source = mediaCapture;
			//DisplayInformation.AutoRotationPreferences = DisplayOrientations.None;
		}

        //private int ConvertVideoRotationToMFRotation(VideoRotation rotation)
        //{
        //	int MFVideoRotation = 0;
        //	switch (rotation)
        //	{
        //		case VideoRotation.Clockwise90Degrees:
        //			MFVideoRotation = 90;
        //			break;
        //		case VideoRotation.Clockwise180Degrees:
        //			MFVideoRotation = 180;
        //			break;
        //		case VideoRotation.Clockwise270Degrees:
        //			MFVideoRotation = 270;
        //			break;
        //	}
        //	return MFVideoRotation;
        //}

        public async void startCaptureProcess(object sender, RoutedEventArgs e)
        {
            await StartMediaCaptureProcess();
        }

        public async void stopCaptureProcess(object sender, RoutedEventArgs e)
        {
            await StopMediaCaptureProcess();
        }

        public async void startCaptureProcess()
	    {
	        await StartMediaCaptureProcess();
	    }

	    public async void stopCaptureProcess()
	    {
	        await StopMediaCaptureProcess();
	    }

        public string GenerateFileName(string fileExt)
		{
			var time = DateTime.Now;

			return $"{time.Year}-{time.Month.ToString("d2")}-{time.Day.ToString("d2")}-{time.Hour.ToString("d2")}-{time.Minute.ToString("d2")}-{time.Second.ToString("d2")}.{fileExt}";
		}

	    public async void IsPhoneDetected(string btId)
	    {
	        phoneIsDetected = false;
            var btDevices = await DeviceInformation.FindAllAsync(BluetoothDevice.GetDeviceSelectorFromPairingState(false));
	        foreach (var d in btDevices)
	        {
	            if (d.Id == btId)
	            {
	                phoneIsDetected = true;
                }         
	        }
	        if (phoneIsDetected)
	        {
	            startCaptureProcess();
            }
	        else
	        {
	            stopCaptureProcess();
            }
        }

		//private async void playVideo(object sender, RoutedEventArgs e)
		//{
		//	Windows.Storage.StorageFile storageFile = await Windows.Storage.KnownFolders.VideosLibrary.GetFileAsync(fileName);

		//	using (var stream = await storageFile.OpenAsync(Windows.Storage.FileAccessMode.Read))
		//	{
		//		if (null != stream)
		//		{
		//			media.Visibility = Visibility.Visible;
		//			media.SetSource(stream, storageFile.ContentType);
		//			media.Play();
		//		}
		//	}
		//}
	}
}
