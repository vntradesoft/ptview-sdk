using System.Globalization;

namespace PTView.SDK.Models;

/// <summary>
/// Platform-independent 32-bit RGBA color representation for indicator plots and styling.
/// </summary>
public readonly record struct ColorRgba(byte R, byte G, byte B, byte A = 255)
{
    // Standard basic colors
    public static readonly ColorRgba Blue = new(41, 98, 255);
    public static readonly ColorRgba Red = new(242, 54, 69);
    public static readonly ColorRgba Green = new(8, 153, 129);
    public static readonly ColorRgba Orange = new(255, 152, 0);
    public static readonly ColorRgba Purple = new(156, 39, 176);
    public static readonly ColorRgba White = new(255, 255, 255);
    public static readonly ColorRgba Gray = new(128, 128, 128);
    public static readonly ColorRgba Black = new(0, 0, 0);
    public static readonly ColorRgba Yellow = new(255, 235, 59);
    public static readonly ColorRgba Cyan = new(0, 229, 255);
    public static readonly ColorRgba Transparent = new(0, 0, 0, 0);

    // PtView standard indicator palette
    public static readonly ColorRgba PtBlue = new(41, 98, 255);       // #2962FF - Primary line, MACD fast, Stoch %K, Upper/Lower BB
    public static readonly ColorRgba PtOrange = new(255, 109, 0);     // #FF6D00 - Signal lines, Stoch %D, BB middle
    public static readonly ColorRgba PtAmber = new(255, 152, 0);      // #FF9800 - Secondary lines, VWAP, Volume MA
    public static readonly ColorRgba PtPurple = new(126, 87, 194);    // #7E57C2 - RSI, ATR, Envelope
    public static readonly ColorRgba PtGreen = new(8, 153, 129);      // #089981 - SuperTrend up, strong bullish trend
    public static readonly ColorRgba PtRed = new(242, 54, 69);        // #F23645 - SuperTrend down, strong bearish trend
    public static readonly ColorRgba PtTeal = new(38, 166, 154);      // #26A69A - Bullish histogram/volume, AO/AC growing
    public static readonly ColorRgba PtSalmon = new(239, 83, 80);     // #EF5350 - Bearish histogram/volume, AO/AC falling
    public static readonly ColorRgba PtYellow = new(255, 235, 59);    // #FFEB3B - RSI-based MA, spike highlights
    public static readonly ColorRgba PtMuted = new(120, 123, 134);    // #787B86 - Guide levels, bounds (RSI 30/70, Stoch 20/80)
    public static readonly ColorRgba PtCyan = new(0, 188, 212);       // #00BCD4 - Keltner Channels, BandWidth, Momentum
    public static readonly ColorRgba PtBurgundy = new(178, 40, 51);   // #B22833 - Kijun-sen, deep trend resistance

    /// <summary>
    /// Creates a ColorRgba from R, G, B channels with full opacity (A = 255).
    /// </summary>
    public static ColorRgba FromRgb(byte r, byte g, byte b) => new(r, g, b, 255);

    /// <summary>
    /// Creates a ColorRgba from A, R, G, B channels (matching WPF Color.FromArgb order).
    /// </summary>
    public static ColorRgba FromArgb(byte a, byte r, byte g, byte b) => new(r, g, b, a);

    /// <summary>
    /// Returns a copy of this color with the specified alpha channel.
    /// </summary>
    public ColorRgba WithAlpha(byte a) => new(R, G, B, a);

    /// <summary>
    /// Returns a copy of this color with the specified opacity factor (0.0 to 1.0).
    /// </summary>
    public ColorRgba WithOpacity(double opacity) =>
        new(R, G, B, (byte)Math.Clamp(Math.Round(opacity * 255.0), 0, 255));

    private static readonly Dictionary<string, ColorRgba> NamedColors = new(StringComparer.OrdinalIgnoreCase)
    {
        ["blue"] = Blue,
        ["red"] = Red,
        ["green"] = Green,
        ["orange"] = Orange,
        ["purple"] = Purple,
        ["white"] = White,
        ["gray"] = Gray,
        ["grey"] = Gray,
        ["black"] = Black,
        ["yellow"] = Yellow,
        ["cyan"] = Cyan,
        ["magenta"] = new ColorRgba(255, 0, 255),
        ["pink"] = new ColorRgba(233, 30, 99),
        ["lime"] = new ColorRgba(0, 230, 118),
        ["gold"] = new ColorRgba(255, 215, 0),
        ["silver"] = new ColorRgba(192, 192, 192),
        ["teal"] = new ColorRgba(0, 150, 136),
        ["navy"] = new ColorRgba(0, 0, 128),
        ["maroon"] = new ColorRgba(128, 0, 0),
        ["olive"] = new ColorRgba(128, 128, 0),
        ["aqua"] = new ColorRgba(0, 255, 255),
        ["coral"] = new ColorRgba(255, 127, 80),
        ["transparent"] = Transparent
    };

    public string ToRgbaString() => $"rgba({R}, {G}, {B}, {A / 255.0:F2})";

    public string ToHexString() => A == 255 ? $"#{R:X2}{G:X2}{B:X2}" : $"#{R:X2}{G:X2}{B:X2}{A:X2}";

    public static ColorRgba FromHex(string hex)
    {
        if (TryParse(hex, out var color))
        {
            return color;
        }

        throw new FormatException($"Invalid color string: {hex}");
    }

    /// <summary>
    /// Parses a color string supporting standard color names ('red', 'blue', etc.),
    /// hex formats ('#RGB', '#RGBA', '#RRGGBB', '#RRGGBBAA'), and CSS functions ('rgb(...)', 'rgba(...)').
    /// </summary>
    public static bool TryParse(string? input, out ColorRgba color)
    {
        color = default;
        if (string.IsNullOrWhiteSpace(input))
            return false;

        string s = input.Trim();

        // 1. Named color check
        if (NamedColors.TryGetValue(s, out color))
            return true;

        // 2. Strip optional '#' or '0x' prefix
        if (s.StartsWith('#'))
            s = s[1..];
        else if (s.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
            s = s[2..];

        // 3. Hex formats
        if (s.Length == 3 &&
            byte.TryParse(new string(s[0], 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out byte r3) &&
            byte.TryParse(new string(s[1], 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out byte g3) &&
            byte.TryParse(new string(s[2], 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out byte b3))
        {
            color = new ColorRgba(r3, g3, b3, 255);
            return true;
        }

        if (s.Length == 4 &&
            byte.TryParse(new string(s[0], 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out byte r4) &&
            byte.TryParse(new string(s[1], 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out byte g4) &&
            byte.TryParse(new string(s[2], 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out byte b4) &&
            byte.TryParse(new string(s[3], 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out byte a4))
        {
            color = new ColorRgba(r4, g4, b4, a4);
            return true;
        }

        if (s.Length == 6 &&
            byte.TryParse(s[0..2], NumberStyles.HexNumber, CultureInfo.InvariantCulture, out byte r6) &&
            byte.TryParse(s[2..4], NumberStyles.HexNumber, CultureInfo.InvariantCulture, out byte g6) &&
            byte.TryParse(s[4..6], NumberStyles.HexNumber, CultureInfo.InvariantCulture, out byte b6))
        {
            color = new ColorRgba(r6, g6, b6, 255);
            return true;
        }

        if (s.Length == 8 &&
            byte.TryParse(s[0..2], NumberStyles.HexNumber, CultureInfo.InvariantCulture, out byte r8) &&
            byte.TryParse(s[2..4], NumberStyles.HexNumber, CultureInfo.InvariantCulture, out byte g8) &&
            byte.TryParse(s[4..6], NumberStyles.HexNumber, CultureInfo.InvariantCulture, out byte b8) &&
            byte.TryParse(s[6..8], NumberStyles.HexNumber, CultureInfo.InvariantCulture, out byte a8))
        {
            color = new ColorRgba(r8, g8, b8, a8);
            return true;
        }

        // 4. CSS rgb(...) or rgba(...) functions
        if (s.StartsWith("rgb(", StringComparison.OrdinalIgnoreCase) && s.EndsWith(')'))
        {
            string body = s[4..^1];
            var parts = body.Split(',', StringSplitOptions.TrimEntries);
            if (parts.Length == 3 &&
                byte.TryParse(parts[0], CultureInfo.InvariantCulture, out byte r) &&
                byte.TryParse(parts[1], CultureInfo.InvariantCulture, out byte g) &&
                byte.TryParse(parts[2], CultureInfo.InvariantCulture, out byte b))
            {
                color = new ColorRgba(r, g, b, 255);
                return true;
            }
        }
        else if (s.StartsWith("rgba(", StringComparison.OrdinalIgnoreCase) && s.EndsWith(')'))
        {
            string body = s[5..^1];
            var parts = body.Split(',', StringSplitOptions.TrimEntries);
            if (parts.Length == 4 &&
                byte.TryParse(parts[0], CultureInfo.InvariantCulture, out byte r) &&
                byte.TryParse(parts[1], CultureInfo.InvariantCulture, out byte g) &&
                byte.TryParse(parts[2], CultureInfo.InvariantCulture, out byte b))
            {
                byte a = 255;
                if (double.TryParse(parts[3], NumberStyles.Float, CultureInfo.InvariantCulture, out double alphaDouble))
                {
                    a = (byte)Math.Clamp(Math.Round(alphaDouble <= 1.0 ? alphaDouble * 255.0 : alphaDouble), 0, 255);
                }
                color = new ColorRgba(r, g, b, a);
                return true;
            }
        }

        return false;
    }
}
