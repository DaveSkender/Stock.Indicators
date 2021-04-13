from abc import ABC
from .test_data import HistoryTestData

class TestBase(ABC):
    history = HistoryTestData.get()
    history_other = HistoryTestData.get_compare()
    history_bad = HistoryTestData.get_bad()

    converge_quantities = (5, 20, 30, 50, 75, 100, 120, 150, 200, 250, 350, 500, 600, 700, 800, 900, 1000)

