﻿using System;

namespace Skender.Stock.Indicators
{
    // QUOTE MODELS

    public interface IQuote
    {
        public DateTime Date { get; }
        public decimal Open { get; }
        public decimal High { get; }
        public decimal Low { get; }
        public decimal Close { get; }
        public decimal Volume { get; }
    }

    [Serializable]
    public class Quote : IQuote
    {
        public DateTime Date { get; set; }
        public decimal Open { get; set; }
        public decimal High { get; set; }
        public decimal Low { get; set; }
        public decimal Close { get; set; }
        public decimal Volume { get; set; }
    }

    [Serializable]
    internal class BasicData
    {
        internal DateTime Date { get; set; }
        internal decimal Value { get; set; }
    }
}
