using System.Numerics;

namespace Skender.Stock.Indicators;

public static partial class Sma
{
    internal static double[] CalcSma(this double[] prices, int period)
    {
        int count = prices.Length - period + 1;
        double[] sma = new double[count];

        int simdWidth = Vector<double>.Count;
        for (int i = 0; i < count; i++)
        {
            Vector<double> sumVector = Vector<double>.Zero;

            int j;
            for (j = 0; j <= period - simdWidth; j += simdWidth)
            {
                Vector<double> priceVector = new(prices, i + j);
                sumVector += priceVector;
            }

            double sum = 0;
            for (; j < period; j++) // remainder loop
            {
                sum += prices[i + j];
            }
            sum += Vector.Dot(sumVector, Vector<double>.One);

            sma[i] = sum / period;
        }

        return sma;
    }
}
