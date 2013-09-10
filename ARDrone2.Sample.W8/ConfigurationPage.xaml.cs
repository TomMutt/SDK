using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

using ARDrone2Client.Common;
using ARDrone2Client.Common.Configuration;
using ARDrone2Client.Common.Input;
using ARDrone2Client.Common.Navigation;
using ARDrone2Client.Common.ViewModel;
using ARDrone2Client.Common.Helpers;

namespace ARDrone2.Sample
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ConfigurationPage : ARDrone2.Sample.Common.LayoutAwarePage
    {
       public const float RAD_TO_DEG  = 57.295779513f;
       public const float DEG_TO_RAD  = 1.745329252e-02f;

        private DroneClient _droneClient;
        public ConfigurationPage()
        {
            this.InitializeComponent();
            _droneClient = DroneClient.Instance;
            _droneClient.RequestConfiguration();
            this.DataContext = _droneClient;
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            _droneClient.RequestConfiguration();
            CollectionViewSource source = new CollectionViewSource();
            source.Source = _droneClient.ConfigurationSectionsViewModel;
            source.IsSourceGrouped = true;
            source.ItemsPath = new PropertyPath("ConfigItems");
            configItems.SetBinding(GridView.ItemsSourceProperty, new Binding() { Source = source });
            int altitudeMax = _droneClient.Configuration.Control.altitude_max.Value;
            AltitudeMax.Maximum = 100;
            AltitudeMax.Value = altitudeMax / 1000;
            AltitudeMax.StepFrequency = Math.Round(AltitudeMax.Maximum / 100, 1);
            if (Enum.GetName(typeof(ARDrone2Client.Common.Configuration.Native.ARDRONE_VIDEO_CODEC), _droneClient.Configuration.Video.Codec.Value) == null)
            {
               videoCodec.Text = "";
            }
            else
            {
               videoCodec.Text = Enum.GetName(typeof(ARDrone2Client.Common.Configuration.Native.ARDRONE_VIDEO_CODEC), _droneClient.Configuration.Video.Codec.Value);
            }

            this.YawSpeed.Value = (double)_droneClient.Configuration.Control.control_yaw.Value * RAD_TO_DEG;
            this.YawSpeed.ValueChanged += YawSpeed_ValueChanged;

            this.VZMax.Value = (double)_droneClient.Configuration.Control.control_vz_max.Value;
            this.VZMax.ValueChanged += VZMax_ValueChanged;

            this.EulerAngleMax.Value = (double)_droneClient.Configuration.Control.euler_angle_max.Value * RAD_TO_DEG;
            this.EulerAngleMax.ValueChanged += EulerAngleMax_ValueChanged;

            this.FlyModes.ItemsSource = Enum.GetNames(typeof(ARDrone2Client.Common.Navigation.Native.FLYING_STATES));
            this.FlyModes.SelectedItem = Enum.GetName(typeof(ARDrone2Client.Common.Navigation.Native.FLYING_STATES), _droneClient.Configuration.Control.control_level.Value);
           this.FlyModes.SelectionChanged += FlyModes_SelectionChanged;

           this.Outdoor.IsChecked = _droneClient.Configuration.Control.outdoor.Value;
           this.Outdoor.Unchecked += Outdoor_Checked;
           this.Outdoor.Checked += Outdoor_Checked;
        }

        void Outdoor_Checked(object sender, RoutedEventArgs e)
        {
           _droneClient.SetConfiguration(_droneClient.Configuration.Control.outdoor.Set((bool)this.Outdoor.IsChecked).ToCommand());
        }

        void EulerAngleMax_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
           _droneClient.SetConfiguration(_droneClient.Configuration.Control.euler_angle_max.Set((float)this.EulerAngleMax.Value * DEG_TO_RAD).ToCommand());
        }

        void VZMax_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
           _droneClient.SetConfiguration(_droneClient.Configuration.Control.control_vz_max.Set((int)this.VZMax.Value).ToCommand());
        }

        void YawSpeed_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
           _droneClient.SetConfiguration(_droneClient.Configuration.Control.control_yaw.Set((float)this.YawSpeed.Value * DEG_TO_RAD).ToCommand());
        }
        private void AltitudeMax_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            int newValue = (int)AltitudeMax.Value * 1000;
            Debug.WriteLine("newvalue" + newValue.ToString());
            _droneClient.SetConfiguration(_droneClient.Configuration.Control.altitude_max.Set(newValue).ToCommand());
        }

        private void AltitudeMax_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
           int newValue = (int)AltitudeMax.Value * 1000;
           Debug.WriteLine("newvalue" + newValue.ToString());
           _droneClient.SetConfiguration(_droneClient.Configuration.Control.altitude_max.Set(newValue).ToCommand());
        }

        private void FlyModes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
           var newMode = (int)((ARDrone2Client.Common.Navigation.Native.FLYING_STATES)Enum.Parse(typeof(ARDrone2Client.Common.Navigation.Native.FLYING_STATES), (string)this.FlyModes.SelectedItem));
           _droneClient.SetConfiguration(_droneClient.Configuration.Control.control_level.Set(newMode).ToCommand());
        }
    }
}
