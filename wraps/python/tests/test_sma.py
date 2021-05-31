from .TestBase import TestBase
from SkenderStockIndicators import indicators

class TestSma(TestBase):

    def test_standard(self):
        results = indicators.get_sma(self.history, 20)

        # proper quantities
        # should always be the same number of results as there is history
        self.assertEqual(502, len(results))
        self.assertEqual(483, len(list(filter(lambda x: x.Sma is not None, results))))

        # sample values
        self.assertIsNone(results[18].Sma)
        self.assertEqual(214.5250, round(float(results[19].Sma), 4))
        self.assertEqual(215.0310, round(float(results[24].Sma), 4))
        self.assertEqual(234.9350, round(float(results[149].Sma), 4))
        self.assertEqual(255.5500, round(float(results[249].Sma), 4))
        self.assertEqual(251.8600, round(float(results[501].Sma), 4))

    def test_extended(self):
        results = indicators.get_sma_extended(self.history, 20)

        # proper quantities
        # should always be the same number of results as there is history
        self.assertEqual(502, len(results))
        self.assertEqual(483, len(list(filter(lambda x: x.Sma is not None, results))))

        # sample values
        r = results[501]
        self.assertEqual(251.86, float(r.Sma));
        self.assertEqual(9.45, float(r.Mad));
        self.assertEqual(119.2510, round(float(r.Mse), 4))
        self.assertEqual(0.037637, round(float(r.Mape), 6))

    def test_bad_data(self):
        results = indicators.get_sma_extended(self.history, 15)

        self.assertEqual(502, len(results))

    def test_exceptions(self):
        from System import ArgumentOutOfRangeException
        self.assertRaises(ArgumentOutOfRangeException, indicators.get_sma, self.history, 0)

        from Skender.Stock.Indicators import BadHistoryException
        self.assertRaises(BadHistoryException, indicators.get_sma, self.data_reader.get(9), 10)
