#! /usr/bin/env bash
./a.out < input.txt | diff tests.txt - && echo "OK"

