<h1 align=center>AsmComp</h1>
<h1 align=center><img src="https://github.com/winscripter/AsmComp/blob/main/AsmComp.png?raw=true" height="64" width="64" /></h1>
When you have two .NET assemblies with the same size and most of the file metadata, it might be difficult to tell whether these two assemblies have differences, and what
these differences are. If you load these assemblies into AsmComp, you can see the differences side-by-side.

For reference, if TestLibX.dll has this code:
```cs
﻿namespace TestLibX;

public enum Level {
    High,
    Medium,
    Low
}
```
and TestLibY.dll has this code:
```cs
namespace TestLibY;

public class Class1;

public enum Level {
    High,
    Medium,
    Low = 4 /*remember this one here*/
}
```
If you then build these two assemblies, you'll end up with two assemblies whose size is 4.5KB. It can be hard to distinguish the difference between the two, if you don't know
about its source code.

Open up AsmComp (`AsmComp.Desktop.Windows.exe`), select two files, click 'Compare Assemblies' and open the 'Comparison' tab. By doing so, you can see the entire hierarchy
of differences between two assemblies.
![Screenshot 2024-08-27 224900](https://github.com/user-attachments/assets/ba4de1df-96a5-455b-8a55-7bf352dd2434)

Remember, AsmComp can do this while being super lightweight - only ~5MB in size.
