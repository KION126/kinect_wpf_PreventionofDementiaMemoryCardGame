using System;
using MySql.Data.MySqlClient;

namespace YourNamespace {
    internal class DB {
        private MySqlConnection conn;

        public DB() {
            // MySQL 연결 문자열 설정
            string connStr = "Server=localhost;Port=3306;Database=kinect;Uid=root;Pwd=kg2001216!";

            // MySqlConnection 인스턴스 생성
            conn = new MySqlConnection(connStr);
        }

        public MySqlConnection Connection {
            get { return conn; }
        }


        public void Open() {
            try {
                // 데이터베이스 연결 열기
                conn.Open();
                Console.WriteLine("MySQL 연결 성공!");
            }
            catch (MySqlException ex) {
                // 연결 실패 시 예외 처리
                Console.WriteLine($"MySQL 연결 실패: {ex.Message}");
            }
        }

        public void Close() {
            try {
                // 데이터베이스 연결 닫기
                conn.Close();
                Console.WriteLine("MySQL 연결 닫힘!");
            }
            catch (MySqlException ex) {
                // 닫기 실패 시 예외 처리
                Console.WriteLine($"MySQL 연결 닫기 실패: {ex.Message}");
            }
        }
    }
}
