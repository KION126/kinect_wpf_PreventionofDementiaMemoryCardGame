using MySql.Data.MySqlClient;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.Collections;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using YourNamespace;

namespace a {
    public partial class rank : Page {
        DB db;
      

        public rank() {
            InitializeComponent();
            db = new DB();
            userSelect(); // 페이지가 로드될 때 데이터를 가져와 ListBox에 추가
        }

        public void userSelect() {
            try {
                // 데이터베이스 연결 열기
                db.Open();

                // 쿼리 작성
                string selectQuery = "SELECT userName, score, date, tim, " +
                     "RANK() OVER (ORDER BY score DESC, tim ASC) AS ranking " +
                     "FROM kinect ORDER BY score DESC, tim ASC;";

                // MySqlCommand 인스턴스 생성
                using (MySqlCommand cmd = new MySqlCommand(selectQuery, db.Connection)) {
                    using (MySqlDataReader reader = cmd.ExecuteReader()) {
                        List<UserInfo> scoreList = new List<UserInfo>();
                        while (reader.Read()) {
                            String ra = reader["ranking"].ToString();
                            String name = reader["userName"].ToString();
                            String sco = reader["score"].ToString();  
                            String da = reader["date"].ToString();
                            String ti = reader["tim"].ToString();

                            scoreList.Add(new UserInfo { rank = ra, userName = name, score = sco, date = da, time = ti});
                        }
                        // ListBox에 데이터 바인딩
                        scoreListBox.ItemsSource = scoreList;
                    }
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
        public class UserInfo {
            public String rank { set; get; }
            public String userName { set; get; }
            public String score { set; get; }
            public String date { set; get; }
            public String time { set; get; }
        }
}