using System;
using System.Threading;

namespace MicrowaveController
{
    // 電子レンジの状態を表すクラス
    public class Microwave
    {
        public bool IsRunning { get; set; }
        public bool DoorClosed { get; set; }
        public uint TimerSeconds { get; set; }
        public byte PowerLevel { get; set; }

        // コンストラクタで初期化
        public Microwave()
        {
            IsRunning = false;
            DoorClosed = true;
            TimerSeconds = 0;
            PowerLevel = 50; // デフォルト50%
        }

        // 電子レンジ開始
        public void Start(uint seconds, byte power)
        {
            if (!CheckSafety())
            {
                Console.WriteLine("Error: Cannot start microwave. Door is open or invalid settings.");
                return;
            }
            if (power > 100) power = 100; // 出力レベル制限
            TimerSeconds = seconds;
            PowerLevel = power;
            IsRunning = true;
            Console.WriteLine($"Microwave started: {seconds} seconds at {power}% power.");
        }

        // 電子レンジ停止
        public void Stop()
        {
            IsRunning = false;
            TimerSeconds = 0;
            Console.WriteLine("Microwave stopped.");
        }

        // タイマー更新（1秒ごとに呼び出しを想定）
        public void UpdateTimer()
        {
            if (IsRunning && TimerSeconds > 0)
            {
                TimerSeconds--;
                DisplayStatus();
                if (TimerSeconds == 0)
                {
                    Stop();
                    Console.WriteLine("Heating complete!");
                }
            }
        }

        // 安全チェック
        private bool CheckSafety()
        {
            return DoorClosed; // ドアが閉じているか確認
        }

        // 状態表示
        public void DisplayStatus()
        {
            Console.WriteLine($"Status: {(IsRunning ? "Running" : "Stopped")}, Time: {TimerSeconds} sec, Power: {PowerLevel}%");
        }
    }

    // メインクラス
    class Program
    {
        static void Main(string[] args)
        {
            Microwave microwave = new Microwave();

            // テストシナリオ
            Console.WriteLine("Test Scenario:");
            microwave.DoorClosed = true; // ドアを閉じる
            microwave.Start(10, 80); // 10秒、80%で開始

            // タイマー更新をシミュレート（1秒ごとに処理）
            for (int i = 0; i < 12; i++)
            {
                microwave.UpdateTimer();
                // ドアが開いた場合のシミュレーション
                if (i == 5)
                {
                    microwave.DoorClosed = false;
                    Console.WriteLine("Door opened!");
                    microwave.Stop();
                }
                Thread.Sleep(1000); // 1秒待機
            }

            // ドアが開いた状態で開始試行
            microwave.Start(5, 50);

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}
