using Microsoft.Kinect.Toolkit;
using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Kinect.Toolkit.Controls;

namespace a {
    /// <summary>
    /// main.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class main : Page {
        private KinectSensorChooser sensorChooser;             // KinectSensorChooser 클래스의 인스턴스 선언

        public main() {
            InitializeComponent();
            sensorChooser = new KinectSensorChooser();                  // KinectSensorChooser 인스턴스를 생성
            sensorChooser.KinectChanged += SensorChooserOnKinectChanged;// Kinect 센서가 변경될 때 실행되는 이벤트 핸들러 할당
            sensorChooserUi.KinectSensorChooser = sensorChooser;        // sensorChooserUi에 Kinect 객체 할당 
            sensorChooser.Start();                                      // sensorChooserUi는 감지된 키넥트 상태를 화면에 보여준다    
        }

        // Kinect 센서 변경 시 호출되는 이벤트 핸들러
        private void SensorChooserOnKinectChanged(object sender, KinectChangedEventArgs args) {
            bool error = false; // 오류 발생 여부를 나타내는 변수 선언

            // 이전 Kinect 센서가 있었을 경우
            if (args.OldSensor != null) {
                try {
                    args.OldSensor.DepthStream.Range = DepthRange.Default; // Depth 범위를 기본 범위로 설정
                    args.OldSensor.SkeletonStream.EnableTrackingInNearRange = false; // 근거리 트래킹 비활성화

                    // 이전 Kinect 센서의 DepthStream과 SkeletonStream을 비활성화
                    args.OldSensor.DepthStream.Disable();
                    args.OldSensor.SkeletonStream.Disable();

                }
                catch (InvalidOperationException) {
                    // Kinect 센서가 스트림 또는 스트림 기능을 활성화/비활성화하는 동안에러 발생 시
                    error = true;
                }
            }

            //새로운 Kinect 센서가 있을 경우
            if (args.NewSensor != null) {
                try {
                    // 새로운 Kinect 센서의 깊이 스트림 및 스켈레톤 스트림을 활성화
                    args.NewSensor.DepthStream.Enable(DepthImageFormat.Resolution640x480Fps30);     //깊이 스트림
                    args.NewSensor.SkeletonStream.Enable();                                         //스켈레톤 스트림

                    try {
                        args.NewSensor.DepthStream.Range = DepthRange.Near; // Depth 범위를 가깝게 설정
                        args.NewSensor.SkeletonStream.EnableTrackingInNearRange = true; // 근거리 트래킹 활성화
                        args.NewSensor.SkeletonStream.TrackingMode = SkeletonTrackingMode.Seated; // 트래킹 모드를 앉아있을 때로 설정 (하체 움직임은 무시한다)
                    }
                    // Kinect for Windows가 아닌 장치는 Near 모드를 지원하지 않으므로 기본 모드로 재설정
                    catch (InvalidOperationException) {
                        args.NewSensor.DepthStream.Range = DepthRange.Default; // Depth 범위를 기본 범위로 설정
                        args.NewSensor.SkeletonStream.EnableTrackingInNearRange = false; // 근거리 트래킹 비활성화
                        error = true;
                    }
                }
                // Kinect 센서가 스트림 또는 스트림 기능을 활성화/비활성화하는 동안에러 발생 시
                catch (InvalidOperationException) {
                    error = true;
                }
            }
            if (!error) {
                kinectRegion.KinectSensor = args.NewSensor; // 오류가 없으면 Kinect 리전에 현재 Kinect 센서를 할당
            }
        }

        // 게임시작 버튼 클릭 이벤트
        private void startGame_Click(object sender, RoutedEventArgs e) {
            //sensorChooser.Stop();
            //NavigationService.Navigate(new Uri("/menu.xaml", UriKind.Relative));
            NavigationService.Navigate(new menu(sensorChooser, kinectRegion));
        }
    }
}