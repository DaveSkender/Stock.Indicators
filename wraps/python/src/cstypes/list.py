from System.Collections.Generic import List as CsList

def List(generic, sequence):
    """
    Converts Python's iterator type into C#'s `System.Collections.Generic.List` class.
   
    Parameters
    ----------
    generic : generic type for `System.Collections.Generic.List`.

    sequence : iterator types. (e.g. `list`, `tuple`, `range`)

    See Also
    ---------
    [Iterator Types](https://docs.python.org/3/library/stdtypes.html#iterator-types)
   
    Examples
    --------
    Constructing `System.Collections.Generic.List` from `list` of Python.
   
    >>> py_list = [1, 2, 3]
    >>> cs_list = List(py_list)
    >>> cs_list
    System.Collections.Generic.List`1[System.Int32]

    Notice that It can be iterated like other iterable types in Python.

    >>> cs_list = List([1, 2, 3])
    >>> for i in cs_list:
    >>>     print(i, end='')
    123
    """

    clist = CsList[generic]()
    for i in sequence: clist.Add(i)
    
    return clist
