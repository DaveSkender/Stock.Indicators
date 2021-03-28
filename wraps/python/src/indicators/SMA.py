from Skender.Stock.Indicators import Indicator
from cstypes import List, to_pydecimal
from .Qoute import Qoute

def get_sma(history, lookbackPeriod: int):
    sma_list = Indicator.GetSma[Qoute](List(Qoute, history), lookbackPeriod)
    sma_list = [ SmaResult(r) for r in sma_list ]

    return sma_list

def get_sma_extended(history, lookbackPeriod: int):
    sma_extended_list = Indicator.GetSmaExtended[Qoute](List(Qoute, history), lookbackPeriod)
    sma_extended_list = [ SmaExtendedResult(r) for r in sma_extended_list ]

    return sma_extended_list

class SmaResult:
    def __init__(self,sma_result):
        self.Date = sma_result.Date
        self.Sma = to_pydecimal(sma_result.Sma)

    def __str__(self):
        return str(self.Date) + ": " + str(self.Sma)

class SmaExtendedResult(SmaResult):
    def __init__(self, sma_extended_result):
        super().__init__(sma_extended_result)
        self.Mad = to_pydecimal(sma_extended_result.Mad)
        self.Mse = to_pydecimal(sma_extended_result.Mse)
        self.Mape = to_pydecimal(sma_extended_result.Mape)

    def __str__(self):
        return str(self.Date) + ": " + str(self.Sma) + "\t"\
             + str(self.Mad) + "\t" + str(self.Mse) + "\t"\
             + str(self.Mape)
    

