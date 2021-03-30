from System import DateTime as CsDateTime
from System.Globalization import CultureInfo
from datetime import datetime as PyDateTime

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
    
    if not isinstance(datetime, PyDateTime):
        raise TypeError("Only datetime.datetime is allowed")
        
    return CsDateTime.Parse(datetime.isoformat())

def to_pydatetime(cs_datetime):
    """
    Converts C#'s `System.DateTime` struct to a native Python datetime object.

    Parameter
    ----------
    cs_datetime : `System.DateTime` of C#.
   
    """

    if not isinstance(cs_datetime, CsDateTime):
        raise TypeError("Only System.DateTime is allowed")
    
    return PyDateTime.fromisoformat(cs_datetime.ToString("o", CultureInfo.InvariantCulture)[:-1])

    
     
