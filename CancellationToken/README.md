
# CancellationToken Example

[Example originally found here](https://docs.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken?view=netcore-2.1).

# Lessons

* Be cautious when refactoring away local vars; sometimes they are used to [capture lamdba vars](https://medium.com/@Coder_HarryLee/how-to-capture-a-variable-in-c-and-not-to-shoot-yourself-in-the-foot-d169aa161aa6)
* Adding `<GenerateProgramFile>false</GenerateProgramFile>` to csproj helped tests and console app main live in the same project
* Moq has a bizarre limitation where it cannot handle pure functions, see [StackOverflow thread](https://stackoverflow.com/questions/12580015/how-to-mock-static-methods-in-c-sharp-using-moq-framework) and [here](https://stackoverflow.com/questions/537308/how-to-verify-that-method-was-not-called-in-moq)
* [The MS Docs](https://docs.microsoft.com/en-us/dotnet/api/system.random?redirectedfrom=MSDN&view=netframework-4.7.2#ThreadSafety) about locking `System.Random` calls across threads is a candidate for another refactoring exercise, although very similar to this one
