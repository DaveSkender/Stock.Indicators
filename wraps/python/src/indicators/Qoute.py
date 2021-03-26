from Skender.Stock.Indicators import Quote
from cstypes import DateTime, Decimal

class Qoute(Quote):
    def __init__(self, date, open = 0, high = 0, low = 0, close = 0, volume = 0):
        self.Date = DateTime(date)
        self.Open = Decimal(open)
        self.High = Decimal(high)
        self.Low = Decimal(low)
        self.Close = Decimal(close)
        self.Volume = Decimal(volume)
