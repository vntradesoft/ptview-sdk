# PTView.SDK — Indicator & Plugin Development Kit

[![.NET 10](https://img.shields.io/badge/.NET-10.0-512BD4)](https://dotnet.microsoft.com/)
[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](https://opensource.org/licenses/MIT)

`PTView.SDK` is the official, **zero-dependency** contract library for developing high-performance technical indicators and out-of-process plugins for **PTView Trading Workstation**.

> [!NOTE]
> [Bản dịch Tiếng Việt của tài liệu này có sẵn tại README.vi.md](README.vi.md).

---

## Ecosystem Links
- **Workstation Releases (Setup & Portable)**: [github.com/vntradesoft/ptview-releases](https://github.com/vntradesoft/ptview-releases)
- **Public SDK Repository & Samples**: [github.com/vntradesoft/ptview-sdk](https://github.com/vntradesoft/ptview-sdk)
- **Official Documentation Portal**: [vntradesoft.github.io/ptview-docs](https://vntradesoft.github.io/ptview-docs)

---

## Key Capabilities
- **Zero External Dependencies**: Pure C# contracts (`IIndicator`, `IndicatorBase`, `IIndicatorBuffer`, `Candle`, `TimeFrame`, `IPTViewPlugin`) targeting `.NET 10`.
- **Incremental Bar-by-Bar Calculation**: Designed for zero-allocation hot paths via `Calculate(IndicatorInput input)` with `input.IsIncremental`.
- **Full Plot & Fill Customization**: Support for overlay or sub-chart plots, custom RGB palettes, dynamic color indexes, and filled areas (e.g., Kumo clouds, Bollinger channels).
- **Out-of-Process Sandboxing**: Plugins run safely isolated via `PTView.PluginWorker` IPC, preventing third-party code from crashing the main workstation.

---

## Quickstart: Creating a Custom Indicator

### 1. Create a .NET 10 Class Library
```powershell
dotnet new classlib -n MyCustomIndicators -f net10.0
cd MyCustomIndicators
dotnet add package PTView.SDK
```

### 2. Implement `IndicatorBase`

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
        description: "Calculates the arithmetic moving average of close prices over N periods.",
        type: IndicatorType.Overlay,
        parameters:
        [
            new IndicatorParameter(ParamPeriod, "Period", typeof(int), 14, "Lookback period in bars.", minValue: 1, maxValue: 500)
        ],
        plots:
        [
            new IndicatorPlot(PlotSma, "SMA Line", PlotStyle.Line, ColorRgba.TvBlue, lineWidth: 2)
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

## Packaging a Plugin

Create a `plugin.json` manifest alongside your compiled `.dll`:

```json
{
  "id": "com.mycompany.indicators",
  "name": "Custom Trading Indicators",
  "version": "1.0.0",
  "minSdkVersion": "0.1.0",
  "description": "Custom indicator collection for PTView Workstation",
  "author": "Trader Dev",
  "entryPointDll": "MyCustomIndicators.dll"
}
```

Implement `IPTViewPlugin` as your entry point:

```csharp
using PTView.SDK.Plugins;

namespace MyCustomIndicators;

public sealed class MyIndicatorsPlugin : IPTViewPlugin
{
    public PluginMetadata Metadata { get; } = new(
        Id: "com.mycompany.indicators",
        Name: "Custom Trading Indicators",
        Version: new Version(1, 0, 0),
        Description: "Custom indicator collection",
        Author: "Trader Dev",
        IsBuiltIn: false);

    public Task InitializeAsync(IPluginContext context, CancellationToken ct = default)
    {
        context.RegisterIndicator<CustomSimpleMovingAverage>();
        context.LogInfo("Registered CustomSimpleMovingAverage indicator.");
        return Task.CompletedTask;
    }

    public Task StartAsync(CancellationToken ct = default) => Task.CompletedTask;
    public Task StopAsync(CancellationToken ct = default) => Task.CompletedTask;
    public void Dispose() { }
}
```

Deploy the directory containing your compiled assembly and `plugin.json` to:
`%LocalAppData%\PTView\plugins\<PluginId>\`

The PTView Workstation will automatically detect and load your plugin!
