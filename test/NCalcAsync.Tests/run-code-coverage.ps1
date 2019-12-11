$config = 'Release'

if (Test-Path coverage) { Remove-Item coverage -Recurse }
if (Test-Path coverage.zip) { Remove-Item coverage.zip }

dotnet tool install dotnet-reportgenerator-globaltool --tool-path bin

$coverage_file = 'coverage.opencover.xml'

dotnet test -c $config --settings "$PWD/coverlet.runsettings" "$PWD/NCalcAsync.Tests.csproj"

$report_args = @('-verbosity:Info', '-reporttypes:Html')
./bin/reportgenerator @report_args "-verbosity:Info" "-reporttypes:Html" "-reports:$PWD/coverage/*/$coverage_file" "-targetdir:$PWD/coverage/"
