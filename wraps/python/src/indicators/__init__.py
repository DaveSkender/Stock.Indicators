import clr
clr.AddReference(r'dll/Skender.Stock.Indicators')

from .Quote import (Quote)
from .SMA import (
    get_sma,
    get_sma_extended,
    SmaResult,
    SmaExtendedResult
)
