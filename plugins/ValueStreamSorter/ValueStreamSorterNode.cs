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
		
		[Input ("Space", IsSingle = true)]
		ISpread<double> FSpaceIn;

		[Output("StreamX")]
		ISpread<double> FStreamXOut;
		
		[Output("FElementVisible")]
		ISpread<bool> FStreamVisibleOut;
		
		[Output("CenterX")]
		ISpread<double> FCenterXOut;

		[Import()]
		ILogger FLogger;
		#endregion fields & pins
		
		List<int> FStackID = new List<int>();

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
			}
			
			//Checking visibility
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
				//FStreamXOut[i] = index * FElementWidthIn[0] - (((FStackID.Count * FElementWidthIn[0]) / 2) - FElementWidthIn[0] / 2);
			}
			
			//Sorting
			double prevX = 0;
			for (int i = 0; i < FStackID.Count; i++)
			{
				int slice = FStackID[i];

				double space = FSpaceIn[0];
				if(i==0) space = 0;
				
				double move = space + FElementWidthIn[slice] / 2;
				FStreamXOut[slice] = prevX + move;
				
				prevX += space + FElementWidthIn[slice];
			}
			
			//Center
			FCenterXOut[0] = -prevX / 2;
		}
	}
}
