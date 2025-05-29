using System;
using System.Device.Gpio;
using System.Threading;
using System.Threading.Tasks;

namespace OvenController
{
    class Program
    {
        // ピンの定義（Raspberry Pi GPIO）
        private const int HEATER_RELAY_PIN = 17; // GPIO17
        private const float HYSTERESIS = 2.0f;   // ヒステリシス（℃）
        private static float targetTemp = 180.0f; // 初期設定温度（℃）
        private static GpioController gpioController;
        private static bool running = true;

        // 仮定のDS18B20センサークラス（実際には1-Wireライブラリが必要）
        private static readonly DummySensor sensor = new DummySensor();

        static async Task Main(string[] args)
        {
            Console.WriteLine("Oven Control Started");
            Thread.Sleep(2000);

            // GPIO初期化（Raspberry Pi用）
            gpioController = new GpioController();
            gpioController.OpenPin(HEATER_RELAY_PIN, PinMode.Output);
            gpioController.Write(HEATER_RELAY_PIN, PinValue.Low); // ヒーターOFF

            // 並行タスク：センサー読み取り、入力処理、制御
            var sensorTask = Task.Run(SensorReadingLoop);
            var inputTask = Task.Run(InputLoop);
            var controlTask = Task.Run(ControlLoop);

            // プログラム終了まで待機
            Console.CancelKeyPress += (s, e) => running = false;
            await Task.WhenAll(sensorTask, inputTask, controlTask);

            // クリーンアップ
            gpioController.Write(HEATER_RELAY_PIN, PinValue.Low);
            gpioController.ClosePin(HEATER_RELAY_PIN);
            gpioController.Dispose();
        }

        // センサー読み取りループ
        static async Task SensorReadingLoop()
        {
            while (running)
            {
                float currentTemp = sensor.ReadTemperature();
                DisplayStatus(currentTemp);
                await Task.Delay(500); // 500ms間隔
            }
        }

        // 入力処理ループ（キーボード）
        static async Task InputLoop()
        {
            while (running)
            {
                if (Console.KeyAvailable)
                {
                    var key = Console.ReadKey(true).Key;
                    if (key == ConsoleKey.UpArrow)
                    {
                        targetTemp += 1.0f;
                    }
                    else if (key == ConsoleKey.DownArrow)
                    {
                        targetTemp -= 1.0f;
                    }
                }
                await Task.Delay(100); // 100ms間隔でキー確認
            }
        }

        // ヒーター制御ループ
        static async Task ControlLoop()
        {
            while (running)
            {
                float currentTemp = sensor.ReadTemperature();
                if (currentTemp < targetTemp - HYSTERESIS)
                {
                    gpioController.Write(HEATER_RELAY_PIN, PinValue.High); // ヒーターON
                }
                else if (currentTemp > targetTemp + HYSTERESIS)
                {
                    gpioController.Write(HEATER_RELAY_PIN, PinValue.Low); // ヒーターOFF
                }
                await Task.Delay(500); // 500ms間隔
            }
        }

        // コンソール表示
        static void DisplayStatus(float currentTemp)
        {
            Console.Clear();
            Console.WriteLine($"Set: {targetTemp:F1} °C");
            Console.WriteLine($"Now: {currentTemp:F1} °C");
        }
    }

    // 仮のDS18B20センサークラス（実際には1-Wire実装が必要）
    class DummySensor
    {
        private float simulatedTemp = 25.0f;
        private Random rand = new Random();

        public float ReadTemperature()
        {
            // シミュレーション：ランダムノイズ付き温度
            simulatedTemp += (float)(rand.NextDouble() - 0.5);
            return simulatedTemp;
        }
    }
}
