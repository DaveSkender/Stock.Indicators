from Skender.Stock.Indicators import Quote as CsQuote
from cstypes import DateTime, Decimal

class Quote(CsQuote):
    def __init__(self, date, open = None, high = None, low = None, close = None, volume = None):
        self.Date = DateTime(date)
        self.Open = Decimal(open)
        self.High = Decimal(high)
        self.Low = Decimal(low)
        self.Close = Decimal(close)
        self.Volume = Decimal(volume)
