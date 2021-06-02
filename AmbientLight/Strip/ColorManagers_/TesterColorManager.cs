using AmbientLight.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmbientLight.Strip.ColorManagers_
{
	class TesterColorManager : ColorManager
	{
		public TesterColorManager()
		{

		}

		protected override Color GetColor(long deltatime)
		{
			double[] data = Curve.Linear(deltatime,
				new double[] { 0, 10000, 20000, 30000, 40000, 50000, 60000 },
				new double[][] { new double[] { 255, 0, 0 },
								 new double[] { 255, 255, 0 },
								 new double[] { 0, 255, 0 },
								 new double[] { 0, 255, 255 },
								 new double[] { 0, 0, 255 },
								 new double[] { 255, 0, 255 },
								 new double[] { 255, 0, 0 } });
			return new Color() { r = (byte)data[0], g = (byte)data[1], b = (byte)data[2] };
		}
	}
}
