NBrowse: .NET assembly query utility
====================================

[![Build Status](https://img.shields.io/github/actions/workflow/status/r3c/nbrowse/validate.yml?branch=master)](https://github.com/r3c/nbrowse/actions)
[![License](https://img.shields.io/github/license/r3c/nbrowse.svg)](https://opensource.org/licenses/MIT)

Overview
--------

NBrowse is a command-line utility to browse and execute search queries in .NET
compiled assemblies. It exposes loaded assemblies through a standard set of
traversable entities (e.g. "assembly", "project", "type", "method", etc.) and
running C# statements to query anything you want to retrieve from them.

You can think of it as a stripped down [NDepend](https://www.ndepend.com/)
equivalent with no graphical interface.

This example will search in assembly `NBrowse.dll` for every type that
implements interface `IPrinter` and print them to standard output:

    $ dotnet NBrowse.CLI.dll '
        project // From current project
            .Assemblies // ...pick all loaded assemblies
            .SelectMany(a => a.Types) // ...find their declared types
            .Where(t => // ...keep those implementing interface `IPrinter`
                t.Interfaces.Any(i => i.Name == "IPrinter"))' NBrowse.dll

Usage
-----

Download latest [release](https://github.com/r3c/nbrowse/releases) from GitHub
or checkout code and build using your preferred
[.NET](https://dotnet.microsoft.com/download) SDK.

Once you have a NBrowse.CLI executable file, run it with `-h` command line
argument to display help.

    $ dotnet NBrowse.CLI -h

Licence
-------

This project is open-source, released under MIT licence. See `LICENSE.md` file
for details.

Author
------

[Rémi Caput](http://remi.caput.fr/) (github.com+nbrowse [at] mirari [dot] fr)
