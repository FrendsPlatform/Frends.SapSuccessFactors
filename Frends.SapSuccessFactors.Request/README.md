# Frends.SapSuccessFactors.Request

Frends Task for executing SAP SuccessFactors requests

[![Request_build](https://github.com/FrendsPlatform/Frends.SapSuccessFactors/actions/workflows/Request_test_on_main.yml/badge.svg)](https://github.com/FrendsPlatform/Frends.SapSuccessFactors/actions/workflows/Request_test_on_main.yml)
![Coverage](https://app-github-custom-badges.azurewebsites.net/Badge?key=FrendsPlatform/Frends.SapSuccessFactors/Frends.SapSuccessFactors.Request|main)
[![License: MIT](https://img.shields.io/badge/License-MIT-green.svg)](https://opensource.org/licenses/MIT)

## Installing

You can install the Task via Frends UI Task View.

## Building

### Clone a copy of the repository

`git clone https://github.com/FrendsPlatform/Frends.SapSuccessFactors.git`

### Build the project

`dotnet build`

### Run tests

Run the tests

`dotnet test`

### Create a NuGet package

`dotnet pack --configuration Release`

### StyleCop.Analyzers Version
This project uses StyleCop.Analyzers 1.2.0-beta.556, as recommended by the author, to get the latest fixes and improvements not available in the last stable release.

### Third-party licenses
RestSharp used for HTTP client implementation is licensed under the Apache-2.0 license.
The full license text and source code can be found at https://github.com/restsharp/RestSharp.
