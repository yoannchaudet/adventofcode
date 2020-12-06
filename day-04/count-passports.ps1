#!/usr/bin/env pwsh
#Requires -Version 7.1

<#
.SYNOPSIS
Count the number of valid passports in a batch.

.PARAMETER Map
Path to the map.

.PARAMETER Part2
Part two: ?
#>

param (
  [Parameter(Mandatory=$true)]
  [string] $Batch,
  [switch] $Part2
)

# Init
Set-StrictMode -version 'Latest'
$ErrorActionPreference = 'Stop'

function Get-Passports() {
  <#
  .SYNOPSIS
  Parse and return the list of passports in the batch (as hashtables).
  #>

  # List of passports
  $passports = @()

  # Current passport
  $currentPassport = $null

  # Get all lines in the batch and iterate (add a blank line)
  $lines = @(Get-Content $Batch) + ''
  foreach ($line in $lines) {
    # Blank line
    if (-Not $line.Trim()) {
      # Add current passport to the list
      if ($currentPassport) {
        $passports += $currentPassport
      }
      $currentPassport = $null
      continue
    }

    # Parse the key:value entries
    foreach ($kvalues in $line.Split(' ')) {
      $kvalue = $kvalues.Split(':')
      if ($kvalue.Count -ne 2) {
        Write-Warning "Unable to parse key/value: $kvalue"
      } else {
        if (-Not $currentPassport) {
          $currentPassport = @{}
        }
        $currentPassport[$kvalue[0]] = $kvalue[1]
      }
    }
  }

  # Return the passports
  $passports
}

# Get the passports
$passports = @(Get-Passports)

# Validate passports
$requiredKeys = @('byr', 'iyr', 'eyr', 'hgt', 'hcl', 'ecl', 'pid')
$validPassports = @($passports | Where-Object {
  $valid = $true
  $passportKeys = @($_.Keys)
  foreach ($requiredKey in $requiredKeys) {
    $valid = $valid -and $passportKeys -contains $requiredKey
  }
  $valid
})

Write-Host @"
      passports: $($passports.Count)
valid passports: $($validPassports.Count)
"@