﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using facetracking_api.Services;
using facetracking_api.Models;

// 空白頁項目範本已記錄在 https://go.microsoft.com/fwlink/?LinkId=234238

namespace facetracking_api
{
    /// <summary>
    /// 可以在本身使用或巡覽至框架內的空白頁面。
    /// </summary>
    public sealed partial class SettingPage : Page
    {
        private DeviceHelper _deviceHelper;
        private List<CameraModel> _cameraList;
        private Windows.Storage.ApplicationDataContainer _localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;

        public SettingPage()
        {
            this.InitializeComponent();
            _deviceHelper = new DeviceHelper();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {            
            try
            {
                if (_cameraList == null)
                {
                    _cameraList = await _deviceHelper.GetCameraDevicesAsync();
                    foreach (var item in _cameraList)
                    {
                        RadioButton rb = new RadioButton()
                        {
                            Content = item.Name,
                            Tag = item
                        };

                        if (item.Position == CameraPosition.Front && _localSettings.Values["CameraId"] == null)
                        {
                            rb.IsChecked = true;
                            _localSettings.Values["CameraId"] = item.CameraId;
                            _localSettings.Values["CameraPosition"] = 0;
                        }
                        else if (item.CameraId.Equals(_localSettings.Values["CameraId"].ToString()))
                        {
                            rb.IsChecked = true;
                        }

                        rb.Click += CameraRadios_Click;
                        StackCameraRadios.Children.Add(rb);
                    }
                }
            }
            catch (Exception)
            {
                StackCameraRadios.Children.Add(new TextBlock() { Text = "Cannot find any camera." });
            }            
        }        

        public void CameraRadios_Click(object sender, RoutedEventArgs e)
        {
            RadioButton rb = sender as RadioButton;
            var item = rb.Tag as CameraModel;
            _localSettings.Values["CameraId"] = item.CameraId;
            _localSettings.Values["CameraPosition"] = (int)item.Position;
        }
    }        
}
