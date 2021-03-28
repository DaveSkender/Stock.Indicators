import clr
clr.AddReference(r'../../../indicators/bin/Release/net461/Skender.Stock.Indicators')

from .Quote import (Quote)
from .SMA import (
    get_sma,
    get_sma_extended,
    SmaResult,
    SmaExtendedResult
)
