using Microsoft.Kinect.Toolkit.Controls;
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
using System.Diagnostics;
using System.Threading;

namespace a {
    /// <summary>
    /// game1.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class game2 : Page {
        private KinectSensorChooser sensorChooser;             // KinectSensorChooser 클래스의 인스턴스 선언
        KinectTileButton[] buttons = new KinectTileButton[16]; // 생성한 KinectTileButton를 저장할 인스턴스 배열 생성
        memoryGame currentGame;                    // 게임 클래스 선언
        Random rndGenerate = new Random();         // 카드를 랜덤으로 섞기 위한 난수생성기 선언
        int[] fruitInputList = new int[16];        // 카드에 표시할 이미지의 인덱스를 저장하는 배열
        Stopwatch stopwatch = new Stopwatch();

        public game2(KinectSensorChooser kinectSensor , KinectRegion kinectRegion) {
            InitializeComponent();
            InitializeKienct(kinectSensor, kinectRegion);

            // 게임이 진행중이지 않을 때 (게임 중복 실행 방지)
            if (currentGame == null) {
                var randomArr = Enumerable.Range(1, 16).OrderBy(x => rndGenerate.Next()).ToArray(); // 1~16까지의 숫자를 생성 후 무작위로 섞어 배열에 저장
                for (int i = 0; i < 16; i++) {
                    buttons[i] = canvasPanel.Children[i+3] as KinectTileButton; // canvasPanel의 자식 객체들을 KinectTileButton으로 캐스팅하여 배열에 할당  
                    fruitInputList[i] = (randomArr[i] - 1) % 8 + 1;           // 총 카드는 16개 하지만 카드 2개씩 짝이기 때문에
                }                                                             // 1~16까지의 배열을 가져와서 각각에 숫자를 1~8까지의 숫자로 만듦
                currentGame = new memoryGame(buttons, fruitInputList); // 버튼들과 이미지배열을 사용하여 게임 객체 선언

                stopwatch.Start();
                Thread.Sleep(1000);
            }
        }

        private void InitializeKienct(KinectSensorChooser kinectSensor, KinectRegion kinectRegion) {
            this.sensorChooser = kinectSensor;                  // KinectSensorChooser 인스턴스를 생성
            sensorChooser.KinectChanged += SensorChooserOnKinectChanged;// Kinect 센서가 변경될 때 실행되는 이벤트 핸들러 할당
            sensorChooserUi.KinectSensorChooser = sensorChooser;        // sensorChooserUi에 Kinect 객체 할당 
            this.kinectRegion.KinectSensor = kinectRegion.KinectSensor;
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

        // 다시시작 버튼 클릭 이벤트
        private void resetGame_Click(object sender, RoutedEventArgs e) {
            // 게임이 실행중이면
            if (currentGame != null) {
                for (int i = 0; i < 16; i++) {  // 16개의 모든 버튼 리셋
                    currentGame.Reset(buttons);
                }
                currentGame = null;             // 게임 종료
            }
        }

        // 카드버튼 클릭 이벤트
        private void mtpButtonsOnclick(object sender, RoutedEventArgs e) {
            // 게임이 실행중이면
            if (currentGame != null) {
                String name = (sender as KinectTileButton).Name.ToString();     // 클릭 한 버튼의 이름을 가져옴
                int buttonName = int.Parse(name.Substring(6)) - 1;              // button1이면 1(숫자)를 따서 int형으로 파싱
                currentGame.classOnclick(buttons, buttonName);                  // 버튼과 버튼 이름(버튼 정보)을 game클래스로 전달

                String score = currentGame.getTurnCount().ToString();                 // 카드를 뒤집은 횟수를 출력
                txtCount.Text = "" + score;

                if (currentGame.getGameEnd()) {
                    stopwatch.Stop();
                    TimeSpan ts = stopwatch.Elapsed;
                    String time = ts.Minutes.ToString() + ":" +ts.Seconds.ToString();
                    // 모달 창을 생성
                    ScoreBoard scoreBoardWindow = new ScoreBoard(score, time);

                    // 모달 창을 화면 중앙으로 설정
                    scoreBoardWindow.Width = 800;
                    scoreBoardWindow.Height = 450;

                    // 현재 화면의 가로 및 세로 중앙 계산
                    double screenWidth = SystemParameters.PrimaryScreenWidth;
                    double screenHeight = SystemParameters.PrimaryScreenHeight;
                    double windowWidth = scoreBoardWindow.Width;
                    double windowHeight = scoreBoardWindow.Height;

                    // 모달 창의 위치 설정
                    scoreBoardWindow.Left = (screenWidth - windowWidth) / 2;
                    scoreBoardWindow.Top = (screenHeight - windowHeight) / 2;

                    // 모달 창을 모달로 표시
                    bool? result = scoreBoardWindow.ShowDialog();
                }
            }
        }

        private void homeButtonsOnclick(object sender, RoutedEventArgs e) {
            sensorChooser.Stop();
            NavigationService.Navigate(new main());
        }
    }
}
