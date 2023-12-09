using Microsoft.Kinect.Toolkit.Controls;
using MySql.Data.MySqlClient;
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
using YourNamespace;

namespace a {
    public partial class ScoreBoard : Window {
        int score;
        String time;
        DB db;
        public ScoreBoard(String score, String time) {
            InitializeComponent();
            db = new DB();
            this.score = int.Parse(score);
            this.time = time;

            txtScore.Text = score + "점";
        }

        private void rank(object sender, RoutedEventArgs e) {
            String name = nameInput.Text;

            if (name == null || name.Equals("")) {
                MessageBox.Show("이름을 입력해주세요!");
            }
            else {
                userInsert(name, score);
                rank page = new rank(); 
                page.Title = "rank"; 
                this.Content = page;
            }
        }

        public void userInsert(String name, int score) {
            try {
                // 데이터베이스 연결 열기
                db.Open();

                // 삽입 쿼리 작성
                string insertQuery = "INSERT INTO kinect (userName, score, date, tim) VALUES (@Value1, @Value2, CURDATE(), @Value3)";

                // MySqlCommand 인스턴스 생성
                using (MySqlCommand cmd = new MySqlCommand(insertQuery, db.Connection)) {
                    // 매개변수 설정
                    cmd.Parameters.AddWithValue("@Value1", name);   // 이름
                    cmd.Parameters.AddWithValue("@Value2", score);  // 점수
                    cmd.Parameters.AddWithValue("@Value3", time);  // 점수

                    // 쿼리 실행
                    cmd.ExecuteNonQuery();
                }
            }
            catch (MySql.Data.MySqlClient.MySqlException ex) {
                if (ex.Number == 1062) {
                    db.Close();
                    // Primary Key 관련 오류 (오류 코드 1062) 처리
                    userUpdate(name, score);        // 이미 있는 유저라면 기록 업데이트
                }
                else {
                    // 다른 MySQL 예외 처리
                    MessageBox.Show($"오류 발생: {ex.Message}");
                }
            }
            finally {
                // 데이터베이스 연결 닫기
                db.Close();
            }
        }

        public void userUpdate(String name, int score) {
            try {
                // 데이터베이스 연결 열기
                db.Open();

                // 삽입 쿼리 작성
                string insertQuery = "UPDATE kinect set score = @Value1, date = CURDATE(), tim = @Value2 WHERE userName = @Value3";

                // MySqlCommand 인스턴스 생성
                using (MySqlCommand cmd = new MySqlCommand(insertQuery, db.Connection)) {
                    // 매개변수 설정
                    cmd.Parameters.AddWithValue("@Value1", score);   // 점수
                    cmd.Parameters.AddWithValue("@Value2", time);    // 시간
                    cmd.Parameters.AddWithValue("@Value3", name);    // 이름

                    // 쿼리 실행
                    cmd.ExecuteNonQuery();
                }
            }
            catch (MySqlException ex) {
                // 연결 또는 쿼리 실행 중 오류가 발생한 경우 예외 처리
                MessageBox.Show($"오류 발생: {ex.Message}");
            }
            finally {
                // 데이터베이스 연결 닫기
                db.Close();
            }
        }
    }
}
