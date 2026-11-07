# PTView.SDK — Bộ Công Cụ Phát Triển Chỉ Báo & Plugin

[![.NET 10](https://img.shields.io/badge/.NET-10.0-512BD4)](https://dotnet.microsoft.com/)
[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](https://opensource.org/licenses/MIT)

`PTView.SDK` là thư viện hợp đồng (contract library) chính thức, **hoàn toàn không phụ thuộc bên ngoài (zero-dependency)**, dùng để phát triển các chỉ báo phân tích kỹ thuật hiệu năng cao và plugin chạy độc lập ngoài tiến trình (out-of-process) cho **PTView Trading Workstation**.

> [!NOTE]
> [English version of this document is available at README.md](README.md).

---

## Liên Kết Hệ Sinh Thái
- **Bản phát hành Workstation (Cài đặt & Portable)**: [github.com/vntradesoft/ptview-releases](https://github.com/vntradesoft/ptview-releases)
- **Repository SDK & Dự án Mẫu**: [github.com/vntradesoft/ptview-sdk](https://github.com/vntradesoft/ptview-sdk)
- **Cổng Tài Liệu Trực Tuyến**: [vntradesoft.github.io/ptview-docs](https://vntradesoft.github.io/ptview-docs)

---

## Tính Năng Cốt Lõi
- **Zero Phụ Thuộc (Zero External Dependencies)**: Các hợp đồng C# thuần túy (`IIndicator`, `IndicatorBase`, `IIndicatorBuffer`, `Candle`, `TimeFrame`, `IPTViewPlugin`) nhắm tới nền tảng `.NET 10`.
- **Tính toán Từng Nến (Incremental Bar-by-Bar)**: Thiết kế tối ưu cho luồng dữ liệu thời gian thực không cấp phát bộ nhớ (zero-allocation) qua phương thức `Calculate(IndicatorInput input)` với cờ `input.IsIncremental`.
- **Tùy biến Đồ họa Toàn diện**: Hỗ trợ vẽ đè lên đồ thị chính (Overlay) hoặc phân vùng độc lập bên dưới (SeparatePane), bảng màu RGB tùy biến, dynamic color indexes và tô vùng (Fills - ví dụ: mây Ichimoku, dải Bollinger Bands).
- **Cô lập Tiến trình An toàn (Sandboxing)**: Plugin chạy trong tiến trình worker độc lập qua IPC (`PTView.PluginWorker`), bảo vệ an toàn tuyệt đối cho ứng dụng chính khỏi sự cố crash từ mã nguồn bên thứ ba.

---

## Hướng Dẫn Nhanh: Tạo Custom Indicator

### 1. Tạo Dự Án Class Library .NET 10
```powershell
dotnet new classlib -n MyCustomIndicators -f net10.0
cd MyCustomIndicators
dotnet add package PTView.SDK
```

### 2. Triển khai Lớp Kế thừa `IndicatorBase`

```csharp
using PTView.SDK.Indicators;
using PTView.SDK.Models;

namespace MyCustomIndicators;

public sealed class CustomSimpleMovingAverage : IndicatorBase
{
    public const string Id = "MyCustom.SMA";
    public const string PlotSma = "sma";
    public const string ParamPeriod = "Period";

    private int _period = 14;
    private IIndicatorBuffer? _smaBuffer;

    public override IndicatorMetadata Metadata { get; } = IndicatorMetadata.Create(
        id: Id,
        name: "Custom Simple Moving Average",
        shortName: "CSMA",
        description: "Tính toán đường trung bình động số học của giá đóng cửa qua N chu kỳ.",
        type: IndicatorType.Overlay,
        parameters:
        [
            new IndicatorParameter(ParamPeriod, "Period", typeof(int), 14, "Số nến chu kỳ tính toán.", minValue: 1, maxValue: 500)
        ],
        plots:
        [
            new IndicatorPlot(PlotSma, "Đường SMA", PlotStyle.Line, ColorRgba.TvBlue, lineWidth: 2)
        ],
        category: "Trend",
        iconKey: "Icon.LineChart");

    protected override void OnInit()
    {
        _period = GetParam(ParamPeriod, 14);
        _smaBuffer = GetPlotBuffer(PlotSma);
    }

    public override void Calculate(IndicatorInput input)
    {
        if (_smaBuffer == null)
            return;

        int index = input.CurrentIndex;
        if (index < _period - 1)
        {
            _smaBuffer.SetValue(index, null);
            return;
        }

        double sum = 0.0;
        for (int i = index - _period + 1; i <= index; i++)
        {
            sum += (double)input.Candles[i].Close;
        }

        _smaBuffer.SetValue(index, sum / _period);
    }
}
```

---

## Đóng Gói Plugin

Tạo file manifest `plugin.json` đặt cùng thư mục với file `.dll` sau khi biên dịch:

```json
{
  "id": "com.mycompany.indicators",
  "name": "Custom Trading Indicators",
  "version": "1.0.0",
  "minSdkVersion": "0.1.0",
  "description": "Bộ chỉ báo kỹ thuật mở rộng cho PTView Workstation",
  "author": "Trader Dev",
  "entryPointDll": "MyCustomIndicators.dll"
}
```

Triển khai interface `IPTViewPlugin` làm điểm khởi chạy (entry point):

```csharp
using PTView.SDK.Plugins;

namespace MyCustomIndicators;

public sealed class MyIndicatorsPlugin : IPTViewPlugin
{
    public PluginMetadata Metadata { get; } = new(
        Id: "com.mycompany.indicators",
        Name: "Custom Trading Indicators",
        Version: new Version(1, 0, 0),
        Description: "Bộ chỉ báo mở rộng",
        Author: "Trader Dev",
        IsBuiltIn: false);

    public Task InitializeAsync(IPluginContext context, CancellationToken ct = default)
    {
        context.RegisterIndicator<CustomSimpleMovingAverage>();
        context.LogInfo("Đã đăng ký chỉ báo CustomSimpleMovingAverage thành công.");
        return Task.CompletedTask;
    }

    public Task StartAsync(CancellationToken ct = default) => Task.CompletedTask;
    public Task StopAsync(CancellationToken ct = default) => Task.CompletedTask;
    public void Dispose() { }
}
```

Sao chép toàn bộ thư mục chứa file biên dịch và `plugin.json` vào:
`%LocalAppData%\PTView\plugins\<PluginId>\`

Ứng dụng PTView Workstation sẽ tự động quét, phát hiện và nạp plugin vào danh sách chỉ báo kỹ thuật!

