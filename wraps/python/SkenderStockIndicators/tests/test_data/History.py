import os
import sys
from datetime import datetime
from decimal import Decimal as PyDecimal
from openpyxl import load_workbook
from SkenderStockIndicators.indicators.common import Quote

dir = os.path.dirname(__file__)
data_path = os.path.join(dir, "../../../../../tests/indicators/test data/History.xlsx")

class HistoryTestData:
    wb = load_workbook(data_path, data_only=True)

    @staticmethod
    def get(days: int = 502):
        rows = list(HistoryTestData.wb['History (primary)'])[1:]

        h = []
        for row in rows:
            h.append(Quote(
                row[3].value,
                row[4].value,
                row[5].value,
                row[6].value,
                row[7].value,
                row[8].value,
            ))

        h.reverse()
        return h[:days]

    @staticmethod
    def get_compare(days: int = 502):
        rows = list(HistoryTestData.wb['Compare'])[1:]

        h = []
        for row in rows:
            h.append(Quote(
                row[3].value,
                row[4].value,
                row[5].value,
                row[6].value,
                row[7].value,
                row[8].value,
            ))

        h.reverse()
        return h[:days]

    @staticmethod
    def get_bad(days: int = 502):
        rows = list(HistoryTestData.wb['Bad'])[1:]

        h = []
        i=1
        for row in rows:
            h.append(Quote(
                # Quoto.data cannot be null.
                row[3].value or datetime.now(),
                # Keep micro values.
                '{:f}'.format(PyDecimal(row[4].value)) if row[4].value is not None else None,
                '{:f}'.format(PyDecimal(row[5].value)) if row[5].value is not None else None,
                '{:f}'.format(PyDecimal(row[6].value)) if row[6].value is not None else None,
                '{:f}'.format(PyDecimal(row[7].value)) if row[7].value is not None else None,
                '{:f}'.format(PyDecimal(row[8].value)) if row[8].value is not None else None,
            ))

        h.reverse()
        return h[:days]

