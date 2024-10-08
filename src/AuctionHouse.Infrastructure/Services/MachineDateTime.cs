﻿namespace AuctionHouse.Infrastructure.Services;

using AuctionHouse.Application.Common.Interfaces;
using System;

// TODO: Replace with .NET 8 TimeProvider
public class MachineDateTime : IDateTime
{
    /// <inheritdoc/>
    public DateTime Now => DateTime.UtcNow;

    /// <inheritdoc/>
    public int CurrentYear => DateTime.UtcNow.Year;

    /// <inheritdoc/>
    public int CurrentMonth => DateTime.UtcNow.Month;

    /// <inheritdoc/>
    public int CurrentDay => DateTime.UtcNow.Day;
}