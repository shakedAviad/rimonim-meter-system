namespace MeterSystem.Shared.Models;

public sealed record MeterData(long MeterNumber, IReadOnlyDictionary<DateTime, double> Readings);
