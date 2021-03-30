from System import Decimal as CsDecimal
from decimal import Decimal as PyDecimal
import math

def Decimal(decimal):
    """
    Converts a number into C#'s `System.Decimal` class.
   
    Parameter
    ----------
    decimal : `int`, `float` or any `object` that can be represented as a number.
   
    Example
    --------
    Constructing `System.Decimal` from `float` of Python.
   
    >>> cs_decimal = Decimal(2.5)
    >>> cs_decimal
    2.5
    """
    
    return CsDecimal.Parse(str(decimal))

def to_pydecimal(cs_decimal):
    """
    Converts an object to a native Python decimal object.

    Parameter
    ----------
    cs_decimal : `System.Decimal` of C# or any `object` that can be represented as a number.

    """

    if cs_decimal is not None: 
        return PyDecimal(str(cs_decimal))
