from System import Decimal as CsDecimal
from typing import TypeVar

def Decimal(decimal):
    """
    Converts Python's numeric type(except for `complex`) into C#'s `System.Decimal` class.
   
    Parameter
    ----------
    decimal : `int` or `float`.
   
    Example
    --------
    Constructing `System.Decimal` from `float` of Python.
   
    >>> cs_decimal = Decimal(2.5)
    >>> cs_decimal
    2.5
    """
    
    if not isinstance(decimal,(int, float)):
        raise TypeError("Only int or float are allowed")

    return CsDecimal(float(decimal))
