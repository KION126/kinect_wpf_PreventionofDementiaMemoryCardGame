using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static a.MainWindow;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using System.Windows;
using Microsoft.Kinect.Toolkit.Controls;
using System.Reflection;

namespace a {
    // 게임의 로직 구현 클래스
    internal class memoryGameHard {
        private KinectTileButton[] xBox; // 버튼 배열 선언

        int[] testarray;  // 버튼들의 인덱스를 저장할 배열
        int conditionCounter = 0;       // 현재까지 짝이 맞춰진 횟수 저장 (종료여부를 알기 위함)
        int buttonCount;
        int counter = 0;                // 현재까지 클릭된 카드의 개수 저장 (두 개의 버튼이 눌렸는지, 매치가 되는지 판별을 위함) 
        int turnCount = 0;              // 현재까지의 턴 수 저장

        int[] pressed = new int[2];     // 클릭된 카드의 인덱스 저장
        bool[] opened;   // 각 카드들이 뒤집혀있는지 여부를 저장

        bool gameEnd;                // 게임이 끝났는지 

        public memoryGameHard(KinectTileButton[] box, int[] TArray) {
            xBox = box;                     // 카드 버튼들을 전달 받아 저장
            buttonCount = TArray.Length;
            testarray = TArray;             // 각 카드에 이미지를 넣기 위한 랜덤 배열 저장
            opened = new bool[buttonCount];


            var timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(3) };

            for (int i = 0; i < buttonCount; i++) {  // 16개의 버튼들의 Content를 초기화
                xBox[i].Content = setPic((matchIconFruit)testarray[i]);
                timer.Start();
                
            }
            timer.Tick += (sender, args) => {
                timer.Stop();               // 타이머가 멈추면 두 개의 카드의 이미지를 가림
                for (int i = 0; i < buttonCount; i++) {
                    xBox[i].Content = "";
                }
            };
        }

        // 카드에 번호를 매겨 정의(랜덤으로 섞은 숫자들과 매치 시킴)
        public enum matchIconFruit {
            apple = 1,
            banana = 2,
            grape = 3,
            melon = 4,
            orientalMelon = 5,
            peach = 6,
            persimmon = 7,
            bomb = 8
        }

        //  카드 이미지 생성
        public StackPanel setPic(matchIconFruit test) { // 카드에 매치되는 이미지 번호 받기
            Image img = new Image();                    // 이미지 객체 생성
            StackPanel stackPnl = new StackPanel();     // 이미지를 담을 패널 생성
            img.Source = new BitmapImage(new Uri        // 카드들의 이름으로 이미지 경로를 지정
                ("image/" + test.ToString() + ".png", UriKind.Relative));
            stackPnl.Children.Add(img);                 // 이미지를 패널안에 넣음
            return stackPnl;                            // 완성된 이미지 패널을 반환
        }

        // 카드 버튼이 클릭되었을때 실행
        public void classOnclick(KinectTileButton[] box, int index) {   // 버튼 배열과 클릭된 버튼의 인덱스 받기
            // 1. 이미 두개의 카드가 뒤집힘
            // 2. 현재 클릭된 카드가 이미 뒤집힘
            // 3. 첫번째로 누른 카드를 또 누름
            if (counter == 2 || opened[index] || (counter == 1 && pressed[0] == index))
                return;

            // 클릭된 버튼에 해당 버튼에 매치되는 이미지패널을 버튼에 삽입
            xBox[index].Content = setPic((matchIconFruit)testarray[index]);

            if (((matchIconFruit)testarray[index]).ToString().Equals("bomb")) {
                Console.WriteLine(((matchIconFruit)testarray[index]).ToString());
                MessageBox.Show("펑! 게임오버!");                                                                                                                                                                                                                                    
            }
            
            pressed[counter] = index;   // 현재 클릭 되어있는 카드를 저장하는 변수에 현재 카드 인덱스 저장
            counter++;                  // 현재까지 클릭된 카드의 개수 +1

            if (counter == 2) {         // 현재까지 두개의 카드가 뒤집혔을때 (매치가 되는 지 확인)
                turnCount++;                // 턴 수 +1
                buttonCompare(box, pressed[0], pressed[1]); // 매치를 확인하는 메서드로
                                                            // 첫번째로 클릭한 버튼과, 두번째로 클릭한 버튼의 인덱스 전달
            }
        }

        // 턴 수를 MainWindow로 넘겨주는 메서드
        public int getTurnCount() {
            return turnCount;
        }

        // 게임 종료 여부를 넘겨줌
        public bool getGameEnd() {
            return gameEnd;
        }


        // 클릭된 두 개의 카드의 매치 검사 메서드
        public void buttonCompare(KinectTileButton[] box, int check1, int check2) { // 두 개의 버튼의 인덱스를 받음

            // 두 개의 버튼의 인덱스가 같다면(정답이라면)
            if (testarray[check1] == testarray[check2]) {   
                conditionCounter++;         // 짝이 맞춰진 횟수 +1
                opened[check1] = true;      // 두 개의 버튼이 열려있다고 저장
                opened[check2] = true;

                if (conditionCounter == buttonCount/2) {    // 짝이 8번 맞춰졌을때 (전부 맞췄을 때)
                    gameEnd = true;
                }

                counter = 0;                    // 현재 뒤집힌 카드의 수 초기화
            }

            // 두 개의 버튼이 매치가 되지 않을 때
            else {
                // 0.5초동안 이미지를 보여주는 타이머 시작
                var timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(0.5) };
                timer.Start();

                timer.Tick += (sender, args) =>
                {
                    timer.Stop();               // 타이머가 멈추면 두 개의 카드의 이미지를 가림
                    xBox[check1].Content = "";
                    xBox[check2].Content = "";
                    counter = 0;                // 현재 뒤집힌 카드의 수 초기화
                };
            }
        }


        // 다시시작 버튼을 누를 때 실행
        public void Reset(KinectTileButton[] box) {
            for (int i = 0; i < buttonCount; i++) {       // 16개의 모든 버튼들의 이미지를 가림
                xBox[i].Content = "";
            }
        }

    }
}