---
title: Example usage code
permalink: /examples/
layout: default
---

# {{ page.title }}

To help you get started, here is a simple .NET 6.0 C# console application that you can review as a complete minimal working example.
There are two minimalist example projects in this Solution: 1) a basic `ConsoleApp`, and 2) a bit more advanced `Backtest`.  Start with the `ConsoleApp` if you are a newbie.
For more information on how to use this library overall, see the [Guide and Pro Tips]({{site.baseurl}}/guide/#content).

To run the sample app:

1. [Download](Skender.Stock.Indicators-Examples.zip) the ZIP file and extract contents
2. Open `Examples.sln` in [Visual Studio](https://visualstudio.microsoft.com)
3. Review the code in the `Program.cs` file
4. Run the `ConsoleApp` by any one of the following methods:
   - pressing the `CTRL+F5` key
   - clicking the play button

     ![how to execute the code](run.png)

   - execute `dotnet run` CLI command in the `ConsoleApp` folder

## Console output

```console
SMA Results ---------------------------
SMA on 2021-08-13 06:00:10Z was $1.742
SMA on 2021-08-13 06:05:10Z was $1.737
SMA on 2021-08-13 06:10:26Z was $1.733
SMA on 2021-08-13 06:15:26Z was $1.731
SMA on 2021-08-13 06:20:40Z was $1.730
SMA on 2021-08-13 06:25:40Z was $1.729
SMA on 2021-08-13 06:30:41Z was $1.728
SMA on 2021-08-13 06:35:41Z was $1.727
SMA on 2021-08-13 06:40:56Z was $1.726
SMA on 2021-08-13 06:50:11Z was $1.727

SMA on Specific Date ------------------
SMA on 2021-08-12 11:08:17Z was $1.466

SMA Analysis --------------------------
SMA on 2021-08-13 04:40:21Z was $1.681 and Bullishness is True
SMA on 2021-08-13 04:45:21Z was $1.681 and Bullishness is True
SMA on 2021-08-13 04:50:21Z was $1.687 and Bullishness is True
SMA on 2021-08-13 04:55:22Z was $1.691 and Bullishness is True
SMA on 2021-08-13 05:00:25Z was $1.694 and Bullishness is True
SMA on 2021-08-13 05:05:25Z was $1.702 and Bullishness is True
SMA on 2021-08-13 05:10:40Z was $1.711 and Bullishness is True
SMA on 2021-08-13 05:15:40Z was $1.719 and Bullishness is True
SMA on 2021-08-13 05:20:40Z was $1.726 and Bullishness is True
SMA on 2021-08-13 05:25:40Z was $1.732 and Bullishness is True
SMA on 2021-08-13 05:30:40Z was $1.737 and Bullishness is False
SMA on 2021-08-13 05:35:40Z was $1.742 and Bullishness is False
SMA on 2021-08-13 05:40:41Z was $1.742 and Bullishness is False
SMA on 2021-08-13 05:45:55Z was $1.744 and Bullishness is False
SMA on 2021-08-13 05:50:55Z was $1.744 and Bullishness is False
SMA on 2021-08-13 06:00:10Z was $1.742 and Bullishness is False
SMA on 2021-08-13 06:05:10Z was $1.737 and Bullishness is False
SMA on 2021-08-13 06:10:26Z was $1.733 and Bullishness is False
SMA on 2021-08-13 06:15:26Z was $1.731 and Bullishness is False
SMA on 2021-08-13 06:20:40Z was $1.730 and Bullishness is False
SMA on 2021-08-13 06:25:40Z was $1.729 and Bullishness is True
SMA on 2021-08-13 06:30:41Z was $1.728 and Bullishness is False
SMA on 2021-08-13 06:35:41Z was $1.727 and Bullishness is False
SMA on 2021-08-13 06:40:56Z was $1.726 and Bullishness is True
SMA on 2021-08-13 06:50:11Z was $1.727 and Bullishness is True
```

### More info

- Tutorial: [Create a simple C# console app](https://docs.microsoft.com/en-us/visualstudio/get-started/csharp/tutorial-console)
- These files can also be found in the [/docs/examples]({{site.github.repository_url}}/tree/main/docs/examples) GitHub repo folder
