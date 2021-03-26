from System import Decimal as CsDecimal

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

def to_pyfloat(cs_decimal):
    if cs_decimal == None:
        cs_decimal = "0"
    
    return float(str(cs_decimal))