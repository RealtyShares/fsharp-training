#!/bin/bash
if test "$OS" = "Windows_NT"
then
  # use .Net

  .paket/paket.bootstrapper.exe 5.91.0
  exit_code=$?
  if [ $exit_code -ne 0 ]; then
  	exit $exit_code
  fi

  .paket/paket.exe restore --group build-tools
  exit_code=$?
  if [ $exit_code -ne 0 ]; then
  	exit $exit_code
  fi

  packages/build-tools/FAKE/tools/FAKE.exe $@ --fsiargs build.fsx
else
  # use mono!
  mono .paket/paket.bootstrapper.exe 5.91.0
  exit_code=$?
  if [ $exit_code -ne 0 ]; then
  	exit $exit_code
  fi

  mono .paket/paket.exe restore --group build-tools
  exit_code=$?
  if [ $exit_code -ne 0 ]; then
  	exit $exit_code
  fi
  mono packages/build-tools/FAKE/tools/FAKE.exe $@ --fsiargs -d:MONO build.fsx
fi