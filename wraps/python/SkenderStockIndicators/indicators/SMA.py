from Skender.Stock.Indicators import Indicator
from SkenderStockIndicators._cstypes import List, to_pydecimal, to_pydatetime
from .common import Quote, ResultBase

def get_sma(history, lookbackPeriod: int):
    sma_list = Indicator.GetSma[Quote](List(Quote, history), lookbackPeriod)
    sma_list = [ SmaResult(r) for r in sma_list ]

    return sma_list

def get_sma_extended(history, lookbackPeriod: int):
    sma_extended_list = Indicator.GetSmaExtended[Quote](List(Quote, history), lookbackPeriod)
    sma_extended_list = [ SmaExtendedResult(r) for r in sma_extended_list ]

    return sma_extended_list

class SmaResult(ResultBase):
    def __init__(self, sma_result):
        super().__init__(sma_result)
        self.Sma = to_pydecimal(sma_result.Sma)

class SmaExtendedResult(SmaResult):
    def __init__(self, sma_extended_result):
        super().__init__(sma_extended_result)
        self.Mad = to_pydecimal(sma_extended_result.Mad)
        self.Mse = to_pydecimal(sma_extended_result.Mse)
        self.Mape = to_pydecimal(sma_extended_result.Mape)


