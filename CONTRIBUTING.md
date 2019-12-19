# Contributing to NCalcAsync

As with any open source projects, contributions are most welcome!

# Pull requests

Generally pull requests should go to the `master` branch where all new development and bugfixes are integrated.  An exception are bug fixes that only apply to a specific version, which can be targeted to the relevant `release/vX.Y` branch.


# Testing

Run the unit tests in `NCalcAsync.Tests` to test the project.  New features should always have corresponding unit tests that exercise them.


# Code coverage

[Coverlet](https://github.com/tonerdo/coverlet/) is used to collect code coverage when running the unit tests, using the VSTest integration.

The script `run-code-coverage.ps1` will run the tests with code coverage enabled and then produce an HTML report.


# Building and publishing packages

An Azure Pipeline builds NuGet packages automatically when new commits are pushed to either `master` or a `release/vX.Y` branch.  


## Versioning

[Nerdbank.GitVersioning](https://github.com/AArnott/Nerdbank.GitVersioning) is used to assign version numbers to the packages using a standard `major.minor.patch[-alpha]` scheme.  Patch versions may only contain bugfixes.  New backward compatible features require a new minor version, and any breaking changes require a new major version.

The [`nbgv`](https://github.com/AArnott/Nerdbank.GitVersioning/blob/master/doc/nbgv-cli.md) tool is used to manage the git branches and tags and to update `version.json`.

`master` is used to integrates new features and bugfixes.  Any builds here will be labelled `X.Y.Z-alpha`.

`release/vX.Y` branches contain the commits that are published as non-alpha versions, and any builds in them will get a `X.Y.Z` version number.


## Creating a new major or minor versions

New backward-compatible features should always be published in a new minor release.  This should be created from `master` with `nbgv`:

    git checkout master
    nbgv prepare-release

`nbgv` will create a new `release/vX.Y` branch and update `version.json` in both that branch and in `master`.  Check that it looks correct and then push both `master` and the new branch to this repository to trigger builds.


## Creating a new patch release

Just push commits to the relevant `release/vX.Y` branch to create a new patch release on that version.


## Publishing to NuGet

Publishing to NuGet is done by Azure Pipelines, but triggered manually:

* Under `Pipelines` in the left column, select `Releases`
* Click `Create new release` in the top right corner
* Under `Artifacts` select the build to be published in the version dropdown
* Click `Create` to start the publishing process


## Tagging the new version

After publishing a new non-alpha package use `nbgv` to tag the release:

    nbgv tag release/vX.Y

Push the tag to this repository (a command for this will be part of the output).


