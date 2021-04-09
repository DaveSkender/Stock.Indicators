from SkenderStockIndicators._cstypes import to_pydatetime

class ResultBase:
    def __init__(self, base_result):
        super().__init__()
        self.Date = to_pydatetime(base_result.Date)