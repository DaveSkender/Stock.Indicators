from Skender.Stock.Indicators import Quote as CsQuote
from SkenderStockIndicators._cstypes import DateTime, Decimal

class Quote(CsQuote):
    def __init__(self, date, open = None, high = None, low = None, close = None, volume = None):
        self.Date = DateTime(date)
        self.Open = Decimal(open) if open else super().Open
        self.High = Decimal(high) if high else super().High
        self.Low = Decimal(low) if low else super().Low
        self.Close = Decimal(close) if close else super().Close
        self.Volume = Decimal(volume) if volume else super().Volume
