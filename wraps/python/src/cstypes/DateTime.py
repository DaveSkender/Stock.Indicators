from System import DateTime as CsDateTime
from datetime import datetime as dt

def DateTime(datetime):
    """
    Converts Python's `datetime.datetime` class into C#'s `System.DateTime` struct.
   
    Parameter
    ----------
    datetime : `datetime.datetime`.
   
    Example
    --------
    Constructing `System.DateTime` from `datetime.datetime` of Python.
   
    >>> now = datetime.now()
    >>> cs_now = DateTime(now)
    >>> cs_now
    3/26/2021 10:02:22 PM
    """
    
    if not isinstance(datetime, dt):
        raise TypeError("Only datetime.datetime is allowed")
        
    return CsDateTime.Parse(datetime.isoformat())
