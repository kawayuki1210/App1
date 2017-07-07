using System;
using System.Drawing;
using System.Windows.Forms;

namespace Reversi
{
    public partial class Form1 : Form
    {
        // ボードを表す配列
        int[,] board;
        // 色の定数
        const int EMPTY = 0;
        const int BLACK = 1;
        const int WHITE = 2;
        // マスの数
        const int MATRIX_NUM = 8;
        // プレイヤーの変数
        int player, rival;
        // 数表示用
        int white_count, black_count;
        // 終了メッセージ
        string message;
        // 思考ルーチン用
        Random rnd = new Random();

        /**
         * コンストラクタ
         */

        public Form1()
        {
            InitializeComponent();
            Text = "Reversi";
            ClientSize = new Size(300, 260);
            BackColor = Color.Green;

            Init();
        }

        /**
         * ゲームの初期化
         */
        private void Init()
        {
            // マスを空にする
            board = new int[MATRIX_NUM, MATRIX_NUM];
            // 最初においてある石
            board[MATRIX_NUM / 2 - 1, MATRIX_NUM / 2 - 1] = WHITE;
            board[MATRIX_NUM / 2 - 1, MATRIX_NUM / 2] = BLACK;
            board[MATRIX_NUM / 2, MATRIX_NUM / 2 - 1] = BLACK;
            board[MATRIX_NUM / 2, MATRIX_NUM / 2] = WHITE;

            // 先手は黒
            player = BLACK;
            rival = WHITE;

            // 初期化
            message = "";

            CountStones();
        }

        /**
         * 描画メソッド
         */
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            for (int i = 0; i <= MATRIX_NUM; i++)
            {
                e.Graphics.DrawLine(Pens.Black, i * 30 + 10, 10, i * 30 + 10, 250);
                e.Graphics.DrawLine(Pens.Black, 10, i * 30 + 10, 250, i * 30 + 10);
            }
            for (int y = 0; y < MATRIX_NUM; y++)
            {
                for (int x = 0; x < MATRIX_NUM; x++)
                {
                    DrawStone(e, board[x, y], x * 30 + 11, y * 30 + 11);
                }

            }
            e.Graphics.DrawRectangle(Pens.Black, 260, 220, 30, 30);
            DrawStone(e, player, 261, 221);

            var sf = new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };
            e.Graphics.DrawString("Turn", Font, Brushes.White, new Rectangle(250, 230, 50, 30), sf);

            DrawStone(e, BLACK, 261, 11);
            e.Graphics.DrawString(black_count.ToString(), Font, Brushes.Black, new Rectangle(250, 40, 50, 30), sf);
            DrawStone(e, WHITE, 261, 71);
            e.Graphics.DrawString(white_count.ToString(), Font, Brushes.White, new Rectangle(250, 100, 50, 30), sf);

            if (message != "")
            {
                var r = new Rectangle(20, 120, 220, 20);
                e.Graphics.FillRectangle(Brushes.White, r);
                e.Graphics.DrawRectangle(Pens.Red, r);
                e.Graphics.DrawString(message, Font, Brushes.Black, r, sf);
            }

        }

        /**
         * マウスクリックイベント
         */
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            //GAMEOVERで再開する
            if (message != "")
            {
                Init();
                Refresh();
            }
            else
            {
                int x = (e.X - 10) / 30;
                int y = (e.Y - 10) / 30;

                if (PutStone(x, y) > 0)
                {
                    if (Change() == 1)
                    {
                        do
                        {
                            Refresh();
                            MonteCarloMethodThink();
                        }
                        while (Change() == 2);
                    }
                    Refresh();
                }
            }
        }

        /**
         * 指定したマスがそのプレイヤーの石が置かれているか判定するメソッド
         */
        private bool Check(int x, int y, int player)
        {
            return 0 <= x && x < MATRIX_NUM && 0 <= y && y < MATRIX_NUM && board[x, y] == player;
        }

        /**
         * 石を描画するメソッド
         */
        private void DrawStone(PaintEventArgs e, int player, int x, int y)
        {
            if (player == BLACK)
            {
                e.Graphics.FillEllipse(Brushes.Black, x, y, 28, 28);
            }
            else if (player == WHITE)
            {
                e.Graphics.FillEllipse(Brushes.White, x, y, 28, 28);
            }
        }

        /**
         * 石を数えるメソッド
         */
        private void CountStones()
        {
            black_count = 0;
            white_count = 0;
            for (int y = 0; y < MATRIX_NUM; y++)
            {
                for (int x = 0; x < MATRIX_NUM; x++)
                {
                    if (board[x, y] == BLACK)
                    {
                        black_count++;
                    }
                    else if (board[x, y] == WHITE)
                    {
                        white_count++;
                    }
                }
            }
        }

        /**
         * 今のプレイヤーが打ったときに一方向に裏返すことができる石の数を返すメソッド
         */
        private int CountStone(int x, int y, int dx, int dy)
        {
            int x1 = x + dx;
            int y1 = y + dy;
            int stone = 0;
            while (Check(x1, y1, rival))
            {
                x1 += dx;
                y1 += dy;
                stone++;
            }
            if (Check(x1, y1, player))
            {
                return stone;
            }
            else
            {
                return 0;
            }
        }

        /**
         * 今のプレイヤーが打ったときに全方向に対して裏返すことができる石の数を返すメソッド
         */
        private int CountStone(int x, int y)
        {
            int stone = 0;
            if (Check(x, y, 0))
            {
                stone += CountStone(x, y, 1, 0);   // 右
                stone += CountStone(x, y, -1, 0);  // 左
                stone += CountStone(x, y, 0, 1);   // 下
                stone += CountStone(x, y, 0, -1);  // 上
                stone += CountStone(x, y, 1, 1);   // 右下
                stone += CountStone(x, y, 1, -1);  // 右上
                stone += CountStone(x, y, -1, 1);  // 左下
                stone += CountStone(x, y, -1, -1); // 左上
            }
            return stone;
        }

        /**
         * 今のプレイヤーが石を置くことができるマスが存在するかどうかを返すメソッド
         */
        private bool CanPut()
        {
            for (int y = 0; y < MATRIX_NUM; y++)
            {
                for (int x = 0; x < MATRIX_NUM; x++)
                {
                    if (CountStone(x, y) > 0)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /**
         * 石を置くメソッド(一方向のみ裏返す)
         */
        private int PutStone(int x, int y, int dx, int dy)
        {
            int stone = CountStone(x, y, dx, dy);
            for (int i = 1; i <= stone; i++)
            {
                board[x + dx * i, y + dy * i] = player;
            }
            return stone;
        }

        /**
         * 石を置くメソッド(全方向について裏返す)
         */
        private int PutStone(int x, int y)
        {
            int stone = 0;
            if (Check(x, y, EMPTY))
            {
                stone += PutStone(x, y, 1, 0);   // 右
                stone += PutStone(x, y, -1, 0);  // 左
                stone += PutStone(x, y, 0, 1);   // 下
                stone += PutStone(x, y, 0, -1);  // 上
                stone += PutStone(x, y, 1, 1);   // 右下
                stone += PutStone(x, y, 1, -1);  // 右上
                stone += PutStone(x, y, -1, 1);  // 左下
                stone += PutStone(x, y, -1, -1); // 左上
                if (stone > 0)
                {
                    board[x, y] = player;
                    stone++;
                    CountStones();
                }
            }
            return stone;
        }

        /**
         * プレイヤーの交替メソッド
         */
        private int Change()
        {
            int p = player;
            player = rival;
            rival = p;

            if (CanPut())
            {
                //交替するとき
                return 1;
            }

            rival = player;
            player = p;

            if (CanPut())
            {
                //次の人はパスしなければならない
                return 2;
            }

            //ここからは両方ともおけなかったとき
            if (black_count > white_count)
            {
                message = "Black Wins!";
            }
            else if (black_count < white_count)
            {
                message = "White Wins!";
            }
            else
            {
                message = "Draw!";
            }

            return 3;
        }

        /**
         * 思考ルーチン(ランダム)
         */
        private void RandomThink()
        {
            int x, y;

            do
            {
                x = rnd.Next(MATRIX_NUM);
                y = rnd.Next(MATRIX_NUM);
            }
            while (PutStone(x, y) == 0);
        }

        /**
         * 思考ルーチン(モンテカルロ法)
         */
        private void MonteCarloMethodThink()
        {
            int p = player, r = rival;
            int[,] win = new int[MATRIX_NUM, MATRIX_NUM];
            int[,] bak = new int[MATRIX_NUM, MATRIX_NUM];
            Array.Copy(board, bak, MATRIX_NUM * MATRIX_NUM);
            int max = 0, tx = 0, ty = 0;
            for (int i = 1; i <= 1000; i++)
            {
                int x1 = -1, y1 = -1;
                for (;;)
                {
                    int x2 = rnd.Next(MATRIX_NUM);
                    int y2 = rnd.Next(MATRIX_NUM);
                    if (PutStone(x2, y2) > 0)
                    {
                        if (x1 < 0)
                        {
                            x1 = x2;
                            y1 = y2;
                        }
                        if (Change() == 3)
                        {
                            break;
                        }
                    }
                }
                if ((p == 1 && black_count > white_count) || (p == 2 && black_count < white_count))
                {
                    win[x1, y1]++;
                    if (max < win[x1, y1])
                    {
                        max = win[x1, y1];
                        tx = x1;
                        ty = y1;
                    }
                }
                Array.Copy(bak, board, MATRIX_NUM * MATRIX_NUM);
                player = p;
                rival = r;
            }
            message = "";
            CountStones();
            if (max > 0)
            {
                PutStone(tx, ty);
            }
            else
            {
                RandomThink();
            }
        }

    }
}