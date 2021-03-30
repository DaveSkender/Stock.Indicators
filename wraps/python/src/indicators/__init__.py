import clr
clr.AddReference(r'../../../indicators/bin/Release/net461/Skender.Stock.Indicators')

from .common import (
    Quote,
    ResultBase
)

from .SMA import (
    get_sma,
    get_sma_extended,
    SmaResult,
    SmaExtendedResult
)
