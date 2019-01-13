﻿using System;
using System.Drawing;
using System.IO;
using System.Net;
using System.Text;
using System.Windows.Forms;

namespace Dimiwords_Client_CS
{
    public partial class Login : Form
    {
        //dll이 존재하는지 확인하기 위한 변수
        private bool IsDLL = false;

        public Login()
        {
            //창 생성
            InitializeComponent();
        }

        //창이 처음 생성되면 실행
        private void Login_Load(object sender, EventArgs e)
        {
            //dll이 존재하면 true이므로 if문을 동작 시키기 위해 false일때 true로 변환
            if (!Discord.LibCheck())
            {
                //메세지박스를 띄워 사용자에게 dll이 없음을 알림
                MessageBox.Show(this, "discord-rpc-w32.dll이 존재하지 않습니다. dll이 제대로 존재하는지 확인해주세요.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //dll이 없으면 정상적으로 작동하지 않음으로 종료
                Close();
            }
            //dll이 존재하므로 true로 변경
            IsDLL = true;
            //디스코드에 연결
            Discord.Start();
        }

        //창을 닫으면 실행
        private void Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            //dll이 존재하면
            if (IsDLL)
            {
                //디스코드에 연결을 끊는다
                Discord.Shutdown();
            }
        }

        //로그인 버튼 클릭시 실행
        private void button1_Click(object sender, EventArgs e)
        {
            //이메일, 비밀번호가 워터마크 텍스트인지 확인
            if (textBox1.Text == "이메일" || textBox2.Text == "비밀번호" || textBox1.Text.Contains(" ") || textBox2.Text.Contains(" "))
            {
                MessageBox.Show(this, "로그인 정보가 맞는지 확인해주세요.", "Fail", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                //아래 코드를 실행하지 않음
                return;
            }
            //결과값 변수를 비어져 있는 string자료형으로 선언
            var result = string.Empty;
            //json형태로 Byte[]자료형 선언
            var Data = Encoding.UTF8.GetBytes($"{{\"email\":\"{textBox1.Text}\",\"password\":\"{textBox2.Text}\"}}");
            //로그인 서버
            var req = (HttpWebRequest)WebRequest.Create("https://dimiwords.tk:5000/api/auth/login");
            //Post 형태로
            req.Method = "POST";
            //json 보낸다
            req.ContentType = "application/json";
            //길이는 요만큼
            req.ContentLength = Data.Length;
            //using = 용량이 큰 자료형에 존재하는 함수인 Dispose를 자동으로 실행
            //보낼 준비를 할께!
            using (var reqStream = req.GetRequestStream())
            {
                //보낸다!
                reqStream.Write(Data, 0, Data.Length);
                //다 보냈으니 나머지는 정리할께
                reqStream.Close();
            }
            //이제 받을 준비를 할께!
            using (var res = (HttpWebResponse)req.GetResponse())
            {
                //너의 상태가 괜찮아 보이는군!
                if (res.StatusCode == HttpStatusCode.OK)
                {
                    //받을 준비를 할께!
                    using (var resStream = res.GetResponseStream())
                    {
                        //받았다!
                        result = new StreamReader(resStream).ReadToEnd();
                        //다 받았으니 나머지는 정리할께
                        resStream.Close();
                    }
                }
                //이것도 정리
                res.Close();
            }
            //json 읽기
            var success = result.Split(new string[] { "\"success\":" }, StringSplitOptions.None)[1].Split(',')[0];
            //로그인에 성공했다면
            if (Convert.ToBoolean(success))
            {
                //나머지 json 읽기
                var name = result.Split(new string[] { "\"name\":\"" }, StringSplitOptions.None)[1].Split(new string[] { "\"," }, StringSplitOptions.None)[0];
                var intro = result.Split(new string[] { "\"intro\":\"" }, StringSplitOptions.None)[1].Split(new string[] { "\"," }, StringSplitOptions.None)[0];
                var email = result.Split(new string[] { "\"email\":\"" }, StringSplitOptions.None)[1].Split(new string[] { "\"," }, StringSplitOptions.None)[0];
                var department = result.Split(new string[] { "\"department\":" }, StringSplitOptions.None)[1].Split(',')[0];
                var points = result.Split(new string[] { "\"points\":" }, StringSplitOptions.None)[1].Split(',')[0];
                var submit = result.Split(new string[] { "\"submit\":" }, StringSplitOptions.None)[1].Split(',')[0];
                var accept = result.Split(new string[] { "\"accept\":" }, StringSplitOptions.None)[1].Split(',')[0];
                var token = result.Split(new string[] { "\"token\":\"" }, StringSplitOptions.None)[1].Split(new string[] { "\"," }, StringSplitOptions.None)[0];
                //로그인 창을 종료하고 메인 창을 띄운다 User 정보와 함께
                new Main(new User(name, intro, email, department, points, submit, accept, token), this).Show();
                Hide();
            }
            //로그인에 실패했다면
            else
            {
                //어째서 실패했는지 가져온다
                var error = result.Split(new string[] { "\"error\":\"" }, StringSplitOptions.None)[1].Split(new string[] { "\"}" }, StringSplitOptions.None)[0];
                //실패한 이유를 사용자에게 알려준다
                MessageBox.Show(this, $"{(error == "No User" ? "로그인 정보가 맞는지 확인해주세요." : "로그인 도중 에러가 발생했습니다.")}", "Fail", MessageBoxButtons.OK, MessageBoxIcon.Hand);
            }
        }
        
        //이 아래 4개 함수(메서드)는 워터마크 텍스트를 위함
        
        private void textBox1_Enter(object sender, EventArgs e)
        {
            if (textBox1.Text == "이메일")
            {
                textBox1.Text = "";
                textBox1.ForeColor = SystemColors.WindowText;
            }
        }

        private void textBox1_Leave(object sender, EventArgs e)
        {
            if (textBox1.Text.Length == 0)
            {
                textBox1.Text = "이메일";
                textBox1.ForeColor = SystemColors.GrayText;
            }
        }

        private void textBox2_Enter(object sender, EventArgs e)
        {
            if (textBox2.Text == "비밀번호")
            {
                textBox2.Text = "";
                textBox2.ForeColor = SystemColors.WindowText;
                textBox2.UseSystemPasswordChar = true;
            }
        }

        private void textBox2_Leave(object sender, EventArgs e)
        {
            if (textBox2.Text.Length == 0)
            {
                textBox2.Text = "비밀번호";
                textBox2.ForeColor = SystemColors.GrayText;
                textBox2.UseSystemPasswordChar = false;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            new Signup().Show();
        }
    }
}