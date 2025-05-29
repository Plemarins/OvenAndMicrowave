# OvenController

C#で実装されたオーブン制御プログラムです。DS18B20温度センサーを使用してオーブンの温度を監視し、ヒステリシス制御でヒーターをON/OFFします。設定温度はキーボード（↑/↓キー）で調整可能で、コンソールに現在温度と設定温度を表示します。Raspberry Piを主なターゲットプラットフォームとして設計されていますが、Windows PCでのテストも可能です。

## 機能
- DS18B20センサーから温度を500msごとに読み取り
- キーボード（↑/↓キー）で設定温度を1℃単位で調整
- ヒステリシス制御（±2℃）でヒーターを制御
- コンソールに設定温度と現在温度を表示
- Ctrl+Cで安全にプログラム終了

## 要件
### ハードウェア
- **Raspberry Pi**（推奨：Raspberry Pi 4、OS：Raspberry Pi OS）
- **DS18B20温度センサー**（1-Wire、GPIO4に接続、4.7kΩプルアップ抵抗）
- **リレー/SSR**（GPIO17で制御）
- **電源**：センサーとリレーに適切な電源供給

### ソフトウェア
- **.NET 8.0 SDK**（クロスプラットフォーム対応）
- **依存パッケージ**：
  - `System.Device.Gpio`（GPIO制御用）
- **開発環境**：Visual Studio Code、Visual Studio、または任意のC#対応IDE

## セットアップ
1. **.NET 8.0のインストール**（Raspberry Piの場合）：
   ```bash
   sudo apt update
   sudo apt install dotnet-sdk-8.0
