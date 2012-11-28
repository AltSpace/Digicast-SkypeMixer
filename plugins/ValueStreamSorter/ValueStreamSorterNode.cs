#region usings
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;

using VVVV.PluginInterfaces.V1;
using VVVV.PluginInterfaces.V2;
using VVVV.Utils.VColor;
using VVVV.Utils.VMath;

using VVVV.Core.Logging;
#endregion usings

namespace VVVV.Nodes
{
	#region PluginInfo
	[PluginInfo(Name = "StreamSorter", Category = "Value", Help = "Basic template with one value in/out", Tags = "")]
	#endregion PluginInfo
	public class ValueStreamSorterNode : IPluginEvaluate
	{
		#region fields & pins
		[Input("Enabled Stream")]
		IDiffSpread<bool> FEnabledStreamIn;
		
		[Input ("Element Width")]
		ISpread<double> FElementWidthIn;

		[Output("FStreamX")]
		ISpread<double> FStreamXOut;
		
		[Output("FElementVisible")]
		ISpread<bool> FStreamVisibleOut;

		[Import()]
		ILogger FLogger;
		#endregion fields & pins
		
		List<double> FStackID = new List<double>();

		//called when data for any output pin is requested
		public void Evaluate(int SpreadMax)
		{
			FStreamXOut.SliceCount = SpreadMax;
			FStreamVisibleOut.SliceCount = SpreadMax;
			
			if(FEnabledStreamIn.IsChanged)
			{
				//Indexes;
				for (int i = 0; i < SpreadMax; i++)
				{
					if(FEnabledStreamIn[i] && FStackID.IndexOf(i) < 0)
					{
						FStackID.Add(i);
					}
					else if(!FEnabledStreamIn[i] && FStackID.IndexOf(i) >= 0)
					{
						FStackID.RemoveAt(FStackID.IndexOf(i));
					}
				}
				
				//Sorting output
				for (int i = 0; i < SpreadMax; i++)
				{
					int index = FStackID.IndexOf(i);
					
					if(index < 0)
					{
						FStreamVisibleOut[i] = false;
						FStreamXOut[i] = 0;
						continue;
					}
					
					FStreamVisibleOut[i] = true;
					double res = (FStackID.Count * FElementWidthIn[0]) / 2;
					FLogger.Log(LogType.Debug, res.ToString());
					FLogger.Log(LogType.Debug, "!!!!!");
					FStreamXOut[i] = index * FElementWidthIn[0] - (((FStackID.Count * FElementWidthIn[0]) / 2) - FElementWidthIn[0] / 2);
				}
				
			}
		}
	}
}
