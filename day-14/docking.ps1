#!/usr/bin/env pwsh
#Requires -Version 7.1

<#
.SYNOPSIS
Docking the ferry.

.PARAMETER InitProgram
Path to the initialization program file.

.PARAMETER Part2
Part two switch.
#>

param (
  [Parameter(Mandatory=$true)]
  [string] $InitProgram,
  [switch] $Part2
)

# Init
Set-StrictMode -version 'Latest'
$ErrorActionPreference = 'Stop'

function Get-InitializationProgram {
  <#
  .SYNOPSIS
  Return the initialialization program.
  #>

  # Parse and return the instructions
  $currentMask = ""
  foreach ($line in @(Get-Content $InitProgram)) {
    if ($line -match "mask = ([X01]{36})") {
      $currentMask = $Matches[1]
    } elseif ($line -match "mem\[([0-9]+)\] = ([0-9]+)") {
      [PSCustomObject] @{
        Address = [int] $Matches[1]
        Value = [int] $Matches[2]
        Mask = $currentMask
      }
    } else {
      throw "Invalid instruction: $line"
    }
  }
}

function Invoke-InitializationProgram {
  <#
  .SYNOPSIS
  Invoke the initialization program.
  #>
  param ([PSCustomObject[]] $Instructions)

  # Use a hashtable for the memory (so it can expand freely)
  $memory = @{}

  # Go through the instructions
  $Instructions | ForEach-Object {
    $i = $_

    # Get the number in binary (36-padded)
    $value2 = [char[]] [System.Convert]::ToString($i.Value,2).PadLeft(36, '0')

    # Apply the mask
    for ($j = 0; $j -lt $i.Mask.Length; $j++) {
      $m = $i.Mask[$j]
      if ($m -eq '0') {
        $value2[$j] = '0'
      } elseif ($m -eq '1') {
        $value2[$j] = '1'
      }
    }

    # Store the number in memory
    $memory[$i.Address] = [Int64] [System.Convert]::ToInt64(($value2 -Join ""), 2)
  }

  [Int64] $sum = 0
  $memory.Values | ForEach-Object {
    $sum += $_
  }
  $sum
}

# Read the instructions
$instructions = Get-InitializationProgram
$instructions | Format-Table

# Run the program
$sum = Invoke-InitializationProgram $instructions
Write-Host "Result = $sum"