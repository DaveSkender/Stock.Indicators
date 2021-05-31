"""
Skender.Stock.Indicators
~~~~~~~~~~~~~~~~~~~~~~~~

This module loads `Skender.Stock.Indicators.dll`, which is a compiled library package
 from <https://github.com/DaveSkender/Stock.Indicators>, written in C#.
"""

import os
import sys
import clr

dir = os.path.dirname(__file__)
path = os.path.join(dir, "lib/Skender.Stock.Indicators.dll")
clr.AddReference(path)
clr.AddReference('System.Collections')