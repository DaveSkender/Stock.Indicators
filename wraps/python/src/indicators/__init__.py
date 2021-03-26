import clr
clr.AddReference(r'dll/Skender.Stock.Indicators')

from .Qoute import (Qoute)
from .SMA import (
    get_sma,
    get_sma_extended,
    SmaResult,
    SmaExtendedResult
)
