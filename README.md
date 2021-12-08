# Trading Algorithms
Using computers to make investment decisions.
## Getting Started
Go to the iex website and obtain your token [Link](https://iexcloud.io/)
### Run the project
`NOTE: This project was built and compiled using [.NET 6]`[.NET](https://dotnet.microsoft.com/download)

open up your terminal or coommand propmt and run the following commands
```
cd Desktop && git clone https://github.com/aniekanBane/trading-algorithms.git
cd trading-algorithms 
```
Create a .env file to store your token
```
vim tk.env
```
Press i to insert text into the editor and type
```
IEX_CLOUD_TOKEN="Your_token_here"
```
Press `shift + :` and then `w` then `Enter` to save and exit

Once all of that is done we are ready to build and run the program. Let's start by building and restoring all dependencies
```
dotnet build
```
We can run either of the projects:
- Equal-weight S&P500 Index Fund
```
dotnet run --project EqualWeightIndexFund/EqualWeightIndexFund.csproj
```
- Quantitative Momentum Investing
```
dotnet run --project MomentumInvesting/MomentumInvesting.csproj
```
- Quantitative Value Investing
```
dotnet run --project ValueInvesting/ValueInvesting.csproj
```
