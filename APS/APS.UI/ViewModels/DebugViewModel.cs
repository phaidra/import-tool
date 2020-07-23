using System;
using System.Collections.Generic;
using System.Text;

namespace APS.UI
{
    public class DebugViewModel : VMBase
    {
		private string _debugText;

		public string DebugText
		{
			get { return _debugText; }
			set { _debugText = value; }
		}
	}
}
