#!/bin/sh -e

current="$(dirname "$0")"
root=nbrowse
source="$(mktemp -d)"

# Retreive latest version from current HEAD
version="$(git --work-tree "$current" tag --points-at HEAD)"

if [ -z "$version" ]; then
	echo >&2 "error: current HEAD doesn't point to a tag"
	exit 1
fi

# Prepare archive directory structure
mkdir "$source/$root"

for runtime in centos-x64 debian-x64 osx-x64 ubuntu-x64 win-x64; do
	dotnet publish -c Release -r "$runtime" -v quiet "$current/NBrowse.CLI"

	ln -s "$(realpath "$current/NBrowse.CLI/bin/Release/netcoreapp2.1/$runtime/publish")" "$source/$root/$runtime"
done

# Create archive file
archive="$root-v$version.zip"

( cd "$source" && zip -qr "$archive" "$root" )

mv "$source/$archive" "$current/$archive"

# Cleanup
rm -r "$source"
