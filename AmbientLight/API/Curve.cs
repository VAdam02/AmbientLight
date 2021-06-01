namespace AmbientLight.API
{
	class Curve
	{
		public static double[] Linear(double value, double[] x, double[][] y)
		{
			value = value % x[x.Length - 1];
			int i = 0;
			while (x[i] < value) { i++; }

			if (value == x[0]) { i++; }

			value -= x[i - 1];
			double state = value / (x[i] - x[i - 1]);

			double[] data = new double[y[i - 1].Length];
			for (int j = 0; j < y[i - 1].Length; j++)
			{
				data[j] = y[i - 1][j] + (y[i][j] - y[i - 1][j]) * state;
			}

			return data;
		}
	}
}
