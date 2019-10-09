#!/bin/sh -e

base="$(dirname "$0")"
root=nbrowse

# Retreive latest version from current HEAD
version="$(git --work-tree "$base" tag --points-at HEAD)"

if [ -z "$version" ]; then
	echo >&2 "error: current HEAD doesn't point to a tag"
	exit 1
fi

# Create archive for each target runtime
framework=netcoreapp3.0
source="$(mktemp -d)"

for runtime in debian-x64 osx-x64 ubuntu-x64 win-x64; do
	dotnet publish -c Release -r "$runtime" -v quiet "$base/NBrowse.CLI"

	ln -s "$(realpath "$base/NBrowse.CLI/bin/Release/$framework/$runtime/publish")" "$source/$root"

	archive="$root-v$version-$runtime.zip"

	( cd "$source" && zip -qr "$archive" "$root" )

	mv "$source/$archive" "$base/$archive"
	rm "$source/$root"
done

# Cleanup
rm -r "$source"
