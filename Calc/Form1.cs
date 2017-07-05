using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Calc
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        string Input_str = "";
        double Result = 0;
        string Operator = null;

        private void button13_Click(object sender, EventArgs e)
        {
            // senderの詳しい情報を取り扱えるようにする
            Button btn = (Button)sender;

            // 押されたボタンの数字（または小数点の記号）
            string text = btn.Text;

            // [入力された数字]に連結する
            Input_str += text;
            // 画面上に数字を出す
            textBox1.Text = Input_str;

        }

        private void button3_Click(object sender, EventArgs e)
        {
            double num1 = Result;
            double num2;

            // 入力された文字が空欄なら計算をスキップする
            if (Input_str != "")
            {
                // 入力した文字を数字に変換
                num2 = double.Parse(Input_str);
                
                // 四則演算
                if (Operator == "+")
                    Result = num1 + num2;
                if (Operator == "-")
                    Result = num1 - num2;
                if (Operator == "*")
                    Result = num1 * num2;
                if (Operator == "/")
                    Result = num1 / num2;

                // 演算子を押されていなかった場合、入力されている文字をそのまま結果扱いにする
                if (Operator == null)
                    Result = num2;
            }

            // 画面に計算結果を表示する
            textBox1.Text = Result.ToString();

            // 今入力されている数字をリセットする
            Input_str = "";

            // 演算子をOperator変数に入れる
            Button btn = (Button)sender;
            Operator = btn.Text;

            if (Operator == "=")
                Operator = "";
        }
    }
}
